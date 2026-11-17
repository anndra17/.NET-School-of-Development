using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Application.Abstractions.Services;
using AirportManagement.Application.Common.Converters;
using AirportManagement.Application.Common.Generators;
using AirportManagement.Application.Common.Results;
using AirportManagement.Application.Dtos.Booking;
using AirportManagement.Application.Enums;
using AirportManagement.Domain.Enums;
using AirportManagement.Domain.Models;

namespace AirportManagement.Application.Services;

public sealed class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookingService(IUnitOfWork uow)
    {
        _unitOfWork = uow;
    }

    public async Task<Result<CreateBookingResponseDto>> CreateAsync(CreateBookingRequest request, CancellationToken ct)
    {
        if (request.Passengers is null || request.Passengers.Count == 0)
            return Result<CreateBookingResponseDto>.Fail(ErrorType.Validation, "Passengers list must contain at least one passenger.");

        var quantity = request.Passengers.Count;

        if (!FareClassConverter.TryParse(request.FareClass, out var fareClass))
            return Result<CreateBookingResponseDto>.Fail(ErrorType.Validation, "FareClass must be one of: Y, M, J, F.");

        var schedule = await _unitOfWork.FlightSchedulesRepository.GetByIdAsync(request.FlightScheduleId, ct);
        if (schedule is null)
            return Result<CreateBookingResponseDto>.Fail(ErrorType.NotFound, $"Schedule {request.FlightScheduleId} not found.");

        var fareOffer = await _unitOfWork.FareOffersRepository.GetByScheduleAndClassAsync(
            request.FlightScheduleId, fareClass, ct);
        if (fareOffer is null)
            return Result<CreateBookingResponseDto>.Fail(
                ErrorType.NotFound,
                $"No {request.FareClass} fare offer exists for schedule {request.FlightScheduleId}.");

        var alreadyTaken = await _unitOfWork.TicketsRepository.CountByScheduleAsync(request.FlightScheduleId, ct);

        var capacityResult = await GetCapacityAsync(schedule, ct);
        if (!capacityResult.Success)
            return Result<CreateBookingResponseDto>.Fail(capacityResult.ErrorType, capacityResult.ErrorMessage!);

        var capacity = capacityResult.Value;
        if (alreadyTaken + quantity > capacity)
            return Result<CreateBookingResponseDto>.Fail(ErrorType.Conflict, "Not enough seats available.");

        string code;
        do { code = ConfirmationCodeGenerator.Generate(6); }
        while (await _unitOfWork.BookingsRepository.ExistsByCodeAsync(code, ct));

        var unitPrice = fareOffer.BasePrice + fareOffer.Taxes;
        var total = unitPrice * quantity;
        var seatsReserved = false;

        await _unitOfWork.ExecuteInTransactionAsync(async txCt =>
        {
            seatsReserved = await _unitOfWork.FareOffersRepository.TryReserveSeatsAsync(
                fareOffer.Id, quantity, txCt);
            if (!seatsReserved)
                return;

            var booking = new Booking
            {
                UserId = 1,
                ConfirmationCode = code,
                Quantity = quantity,
                Status = BookingStatus.Active,
                CreatedUtc = DateTime.UtcNow
            };

            await _unitOfWork.BookingsRepository.InsertAsync(booking, txCt);
            await _unitOfWork.SaveChangesAsync(txCt);

            var saved = await _unitOfWork.BookingsRepository.GetByCodeAsync(code, txCt);
            if (saved is null)
                throw new InvalidOperationException("Failed to create booking.");

            await _unitOfWork.TicketsRepository.CreateTicketsForBookingAsync(
                bookingId: saved.Id,
                fareOfferId: fareOffer.Id,
                totalPrice: unitPrice,
                currency: fareOffer.Currency,
                isRefundable: fareOffer.IsRefundable,
                passengers: request.Passengers,
                ct: txCt);

            await _unitOfWork.SaveChangesAsync(txCt);
        }, ct);

        if (!seatsReserved)
            return Result<CreateBookingResponseDto>.Fail(ErrorType.Conflict, "Not enough seats available for this fare offer.");

        return Result<CreateBookingResponseDto>.Ok(new CreateBookingResponseDto
        {
            ConfirmationCode = code,
            Status = BookingStatus.Active.ToString(),
            TotalAmount = total,
            Currency = fareOffer.Currency
        });
    }

    public async Task<BookingResponseDto?> GetByCodeAsync(string code, CancellationToken ct)
    {
        var booking = await _unitOfWork.BookingsRepository.GetByCodeAsync(code, ct);
        if (booking is null) return null;

        return new BookingResponseDto
        {
            ConfirmationCode = booking.ConfirmationCode,
            Status = booking.Status.ToString(),
            Quantity = booking.Quantity,
            CreatedUtc = booking.CreatedUtc
        };
    }

    public async Task<Result> CancelAsync(string code, CancellationToken ct)
    {
        var booking = await _unitOfWork.BookingsRepository.GetByCodeAsync(code, ct);
        if (booking is null)
            return Result.Fail(ErrorType.NotFound, $"Booking '{code}' not found.");

        if (booking.Status == BookingStatus.Cancelled)
            return Result.Fail(ErrorType.Conflict, $"Booking '{code}' is already cancelled.");

        await _unitOfWork.ExecuteInTransactionAsync(async txCt =>
        {
            var tickets = await _unitOfWork.TicketsRepository.GetByBookingAsync(booking.Id, txCt);
            await _unitOfWork.BookingsRepository.CancelByCodeAsync(code, txCt);

            foreach (var group in tickets.GroupBy(ticket => ticket.FareOfferId))
                await _unitOfWork.FareOffersRepository.RestoreSeatsAsync(group.Key, group.Count(), txCt);
        }, ct);

        return Result.Ok();
    }

    private async Task<Result<int>> GetCapacityAsync(FlightSchedule schedule, CancellationToken ct)
    {
        int? aircraftId = schedule.AssignedAircraftId;

        if (aircraftId is null)
        {
            var flight = await _unitOfWork.FlightsRepository.GetByIdAsync(schedule.FlightId, ct);
            if (flight?.DefaultAircraftId is not null)
                aircraftId = flight.DefaultAircraftId;
        }

        if (aircraftId is null)
            return Result<int>.Fail(ErrorType.Validation,
                "Capacity unknown: schedule has no AssignedAircraftId and flight has no DefaultAircraftId.");

        var aircraft = await _unitOfWork.AircraftsRepository.GetByIdAsync(aircraftId.Value, ct);
        if (aircraft is null)
            return Result<int>.Fail(ErrorType.NotFound, $"Aircraft {aircraftId.Value} not found.");

        return Result<int>.Ok(aircraft.SeatCapacity);
    }
}
