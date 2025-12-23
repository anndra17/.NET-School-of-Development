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
        // Passengers
        if (request.Passengers is null || request.Passengers.Count == 0)
            return Result<CreateBookingResponseDto>.Fail(ErrorType.Validation, "Passengers list is required.");

        var quantity = request.Passengers.Count;

        foreach (var p in request.Passengers)
        {
            if (string.IsNullOrWhiteSpace(p.FullName))
                return Result<CreateBookingResponseDto>.Fail(ErrorType.Validation, "Passenger full name is required.");
            if (string.IsNullOrWhiteSpace(p.Email))
                return Result<CreateBookingResponseDto>.Fail(ErrorType.Validation, "Passenger email is required.");
            if (string.IsNullOrWhiteSpace(p.PhoneNumber))
                return Result<CreateBookingResponseDto>.Fail(ErrorType.Validation, "Passenger phone number is required.");
        }

        // Tarif validations
        if (!FareClassConverter.TryParse(request.FareClass, out var fareClass))
            return Result<CreateBookingResponseDto>.Fail(ErrorType.Validation, "FareClass must be one of: Y, M, J, F.");

        if (request.BasePrice < 0 || request.Taxes < 0)
            return Result<CreateBookingResponseDto>.Fail(ErrorType.Validation, "BasePrice/Taxes must be >= 0.");

        var currency = request.Currency?.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            return Result<CreateBookingResponseDto>.Fail(ErrorType.Validation, "Currency must be a 3-letter code (e.g. EUR).");

        // Schedule exists?
        var schedule = await _unitOfWork.FlightSchedules.GetByIdAsync(request.FlightScheduleId, ct);
        if (schedule is null)
            return Result<CreateBookingResponseDto>.Fail(ErrorType.NotFound, $"Schedule {request.FlightScheduleId} not found.");

        // Capacity rule
        var alreadyTaken = await _unitOfWork.Tickets.CountByScheduleAsync(request.FlightScheduleId, ct);

        var capacityResult = await GetCapacityAsync(schedule, ct);
        if (!capacityResult.Success)
            return Result<CreateBookingResponseDto>.Fail(capacityResult.ErrorType, capacityResult.ErrorMessage!);

        var capacity = capacityResult.Value;
        if (alreadyTaken + quantity > capacity)
            return Result<CreateBookingResponseDto>.Fail(ErrorType.Conflict, "Not enough seats available.");

        // confirmation code
        string code;
        do { code = ConfirmationCodeGenerator.Generate(6); }
        while (await _unitOfWork.Bookings.ExistsByCodeAsync(code, ct));

        var unitPrice = request.BasePrice + request.Taxes;
        var total = unitPrice * quantity;

        await _unitOfWork.ExecuteInTransactionAsync(async txCt =>
        {
            var booking = new Booking
            {
                UserId = 1,
                ConfirmationCode = code,
                Quantity = quantity,
                Status = BookingStatus.Active,
                CreatedUtc = DateTime.UtcNow
            };

            await _unitOfWork.Bookings.InsertAsync(booking, txCt);
            await _unitOfWork.SaveChangesAsync(txCt);

            var saved = await _unitOfWork.Bookings.GetByCodeAsync(code, txCt);
            if (saved is null)
                throw new InvalidOperationException("Failed to create booking.");

            await _unitOfWork.Tickets.CreateTicketsForBookingAsync(
                bookingId: saved.Id,
                flightScheduleId: request.FlightScheduleId,
                fareClass: fareClass,
                basePrice: request.BasePrice,
                taxes: request.Taxes,
                currency: currency!,
                isRefundable: request.IsRefundable,
                passengers: request.Passengers,
                ct: txCt);

            await _unitOfWork.SaveChangesAsync(txCt);
        }, ct);

        return Result<CreateBookingResponseDto>.Ok(new CreateBookingResponseDto
        {
            ConfirmationCode = code,
            Status = BookingStatus.Active.ToString(),
            TotalAmount = total,
            Currency = currency!
        });
    }

    public async Task<BookingResponseDto?> GetByCodeAsync(string code, CancellationToken ct)
    {
        var booking = await _unitOfWork.Bookings.GetByCodeAsync(code, ct);
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
        var booking = await _unitOfWork.Bookings.GetByCodeAsync(code, ct);
        if (booking is null)
            return Result.Fail(ErrorType.NotFound, $"Booking '{code}' not found.");

        if (booking.Status == BookingStatus.Cancelled)
            return Result.Ok(); 

        await _unitOfWork.Bookings.CancelByCodeAsync(code, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Ok();
    }

    private async Task<Result<int>> GetCapacityAsync(FlightSchedule schedule, CancellationToken ct)
    {
        int? aircraftId = schedule.AssignedAircraftId;

        if (aircraftId is null)
        {
            var flight = await _unitOfWork.Flights.GetByIdAsync(schedule.FlightId, ct);
            if (flight?.DefaultAircraftId is not null)
                aircraftId = flight.DefaultAircraftId;
        }

        if (aircraftId is null)
            return Result<int>.Fail(ErrorType.Validation,
                "Capacity unknown: schedule has no AssignedAircraftId and flight has no DefaultAircraftId.");

        var aircraft = await _unitOfWork.Aircrafts.GetByIdAsync(aircraftId.Value, ct);
        if (aircraft is null)
            return Result<int>.Fail(ErrorType.NotFound, $"Aircraft {aircraftId.Value} not found.");

        return Result<int>.Ok(aircraft.SeatCapacity);
    }
}
