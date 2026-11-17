using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Application.Abstractions.Services;
using AirportManagement.Application.Common.Converters;
using AirportManagement.Application.Common.Results;
using AirportManagement.Application.Dtos.Ticket;
using AirportManagement.Application.Enums;
using AirportManagement.Application.Mappings;
using AirportManagement.Domain.Models;

namespace AirportManagement.Application.Services;

public class TicketService : ITicketService
{
    private readonly IUnitOfWork _unitOfWork;

    public TicketService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TicketResponseDto>> CreateAsync(CreateTicketRequest request, CancellationToken ct)
    {
        if (!await _unitOfWork.FlightSchedulesRepository.ExistsAsync(request.FlightScheduleId, ct))
            return Result<TicketResponseDto>.Fail(ErrorType.NotFound, $"Schedule {request.FlightScheduleId} not found.");

        if (!await _unitOfWork.BookingsRepository.ExistsAsync(request.BookingId, ct))
            return Result<TicketResponseDto>.Fail(ErrorType.NotFound, $"Booking {request.BookingId} not found.");

        if (!FareClassConverter.TryParse(request.FareClass, out var fareClass))
            return Result<TicketResponseDto>.Fail(ErrorType.Validation, "FareClass must be one of: Y, M, J, F.");

        var offer = await _unitOfWork.FareOffersRepository.GetByScheduleAndClassAsync(
            request.FlightScheduleId, fareClass, ct);
        if (offer is null)
            return Result<TicketResponseDto>.Fail(ErrorType.NotFound,
                $"No {request.FareClass} fare offer exists for schedule {request.FlightScheduleId}.");

        var seatsReserved = false;
        await _unitOfWork.ExecuteInTransactionAsync(async txCt =>
        {
            seatsReserved = await _unitOfWork.FareOffersRepository.TryReserveSeatsAsync(offer.Id, 1, txCt);
            if (!seatsReserved)
                return;

            var ticket = new Ticket
            {
                FareOfferId = offer.Id,
                FareOffer = offer,
                TotalPrice = offer.BasePrice + offer.Taxes,
                Currency = offer.Currency,
                IsRefundable = offer.IsRefundable,
                BookingId = request.BookingId,
                PassengerFullName = request.PassengerFullName.Trim(),
                PassengerEmail = request.PassengerEmail.Trim(),
                PassengerPhoneNumber = request.PassengerPhoneNumber.Trim()
            };

            await _unitOfWork.TicketsRepository.InsertAsync(ticket, txCt);
        }, ct);

        if (!seatsReserved)
            return Result<TicketResponseDto>.Fail(ErrorType.Conflict, "No seats available for this fare offer.");

        var created = (await _unitOfWork.TicketsRepository.GetByBookingAsync(request.BookingId, ct))
            .Where(t => t.FareOfferId == offer.Id &&
                        t.PassengerEmail.Equals(request.PassengerEmail.Trim(), StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(t => t.Id)
            .FirstOrDefault();

        return created is null
            ? Result<TicketResponseDto>.Fail(ErrorType.Conflict, "Ticket was saved but could not be reloaded.")
            : Result<TicketResponseDto>.Ok(created.MapToTicketResponse());
    }

    public async Task<Result> DeleteAsync(long id, CancellationToken ct)
    {
        var exists = await _unitOfWork.TicketsRepository.ExistsAsync(id, ct);
        if (!exists)
            return Result.Fail(ErrorType.NotFound, $"Ticket {id} not found.");

        var ticket = await _unitOfWork.TicketsRepository.GetByIdAsync(id, ct);
        if (ticket is null)
            return Result.Fail(ErrorType.NotFound, $"Ticket {id} not found.");

        await _unitOfWork.ExecuteInTransactionAsync(async txCt =>
        {
            await _unitOfWork.TicketsRepository.DeleteAsync(id, txCt);
            await _unitOfWork.FareOffersRepository.RestoreSeatsAsync(ticket.FareOfferId, 1, txCt);
        }, ct);

        return Result.Ok();
    }

    public async Task<Result<TicketAvailabilityResponseDto>> GetAvailabilityAsync(int flightScheduleId, CancellationToken ct)
    {
        var schedule = await _unitOfWork.FlightSchedulesRepository.GetByIdAsync(flightScheduleId, ct);
        if (schedule is null)
            return Result<TicketAvailabilityResponseDto>.Fail(ErrorType.NotFound, $"Schedule {flightScheduleId} not found.");
        
        int? aircraftId = schedule.AssignedAircraftId;
        if (aircraftId is null)
        {
            var flight = await _unitOfWork.FlightsRepository.GetByIdAsync(schedule.FlightId, ct);
            if (flight?.DefaultAircraftId is not null)
                aircraftId = flight.DefaultAircraftId;
        }

        if (aircraftId is null)
            return Result<TicketAvailabilityResponseDto>.Fail(
                ErrorType.Validation,
                "Capacity unknown: schedule has no AssignedAircraftId and flight has no DefaultAircraftId."
            );

        var aircraft = await _unitOfWork.AircraftsRepository.GetByIdAsync(aircraftId.Value, ct);
        if (aircraft is null)
            return Result<TicketAvailabilityResponseDto>.Fail(
                ErrorType.NotFound,
                $"Aircraft {aircraftId.Value} not found."
            );

        var capacity = aircraft.SeatCapacity;

        var ticketsCount = await _unitOfWork.TicketsRepository.CountByScheduleAsync(flightScheduleId, ct);
        var offers = await _unitOfWork.FareOffersRepository.GetByScheduleAsync(flightScheduleId, ct);
        var remaining = Math.Min(Math.Max(0, capacity - ticketsCount),
            offers.Sum(offer => offer.SeatsAvailable));
        var prices = offers
            .OrderBy(offer => offer.FareClass)
            .Select(offer => new FareClassPriceDto
            {
                FareClass = offer.FareClass.ToString(),
                MinTotalPrice = offer.BasePrice + offer.Taxes,
                Currency = offer.Currency
            })
            .ToList();

        return Result<TicketAvailabilityResponseDto>.Ok(new TicketAvailabilityResponseDto
        {
            FlightScheduleId = flightScheduleId,
            Capacity = capacity,
            TicketsCount = ticketsCount,
            SeatsRemaining = remaining,
            Prices = prices
        });
    }

    public async Task<TicketResponseDto?> GetByIdAsync(long id, CancellationToken ct)
    {
        var ticket = await _unitOfWork.TicketsRepository.GetByIdAsync(id, ct);

        return ticket?.MapToTicketResponse();
    }

    public async Task<IReadOnlyList<TicketResponseDto>> GetByScheduleAsync(int flightScheduleId, CancellationToken ct)
    {
        var tickets = await _unitOfWork.TicketsRepository.GetByScheduleAsync(flightScheduleId, ct);

        return tickets.Select(t => t.MapToTicketResponse()).ToList();
    }

    public async Task<Result<TicketResponseDto>> UpdateInventoryAsync(long id, UpdateTicketInventoryRequest request, CancellationToken ct)
    {
        var ticket = await _unitOfWork.TicketsRepository.GetByIdAsync(id, ct);
        if (ticket is null)
            return Result<TicketResponseDto>.Fail(ErrorType.NotFound, $"Ticket {id} not found.");

        await _unitOfWork.FareOffersRepository.SetSeatsAvailableAsync(
            ticket.FareOfferId, request.SeatInventory, ct);

        var updated = await _unitOfWork.TicketsRepository.GetByIdAsync(id, ct);
        return Result<TicketResponseDto>.Ok((updated ?? ticket).MapToTicketResponse());
    }
}
