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
        if (!await _unitOfWork.FlightSchedules.ExistsAsync(request.FlightScheduleId, ct))
            return Result<TicketResponseDto>.Fail(ErrorType.NotFound, $"Schedule {request.FlightScheduleId} not found.");

        if (!await _unitOfWork.Bookings.ExistsAsync(request.BookingId, ct))
            return Result<TicketResponseDto>.Fail(ErrorType.NotFound, $"Booking {request.BookingId} not found.");

        if (!FareClassConverter.TryParse(request.FareClass, out var fareClass))
            return Result<TicketResponseDto>.Fail(ErrorType.Validation, "FareClass must be one of: Y, M, J, F.");

        if (request.BasePrice < 0 || request.Taxes < 0)
            return Result<TicketResponseDto>.Fail(ErrorType.Validation, "BasePrice/Taxes must be >= 0.");

        if (request.SeatInventory < 0)
            return Result<TicketResponseDto>.Fail(ErrorType.Validation, "SeatInventory must be >= 0.");

        var currency = request.Currency?.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            return Result<TicketResponseDto>.Fail(ErrorType.Validation, "Currency must be a 3-letter code (e.g. EUR).");

        var ticket = new Ticket
        {
            FlightScheduleId = request.FlightScheduleId,
            FareClass = fareClass,
            BasePrice = request.BasePrice,
            Taxes = request.Taxes,
            Currency = currency!,
            IsRefundable = request.IsRefundable,
            SeatInventory = request.SeatInventory,
            BookingId = request.BookingId,
            PassengerFullName = request.PassengerFullName,
            PassengerEmail = request.PassengerEmail,
            PassengerPhoneNumber = request.PassengerPhoneNumber
        };

        await _unitOfWork.Tickets.InsertAsync(ticket, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var created = await _unitOfWork.Tickets.GetByIdAsync(ticket.Id, ct);
        return Result<TicketResponseDto>.Ok((created ?? ticket).MapToTicketResponse());
    }

    public async Task<Result> DeleteAsync(long id, CancellationToken ct)
    {
        var exists = await _unitOfWork.Tickets.ExistsAsync(id, ct);
        if (!exists)
            return Result.Fail(ErrorType.NotFound, $"Ticket {id} not found.");

        await _unitOfWork.Tickets.DeleteAsync(id, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Ok();
    }

    public async Task<Result<TicketAvailabilityResponseDto>> GetAvailabilityAsync(int flightScheduleId, CancellationToken ct)
    {
        var schedule = await _unitOfWork.FlightSchedules.GetByIdAsync(flightScheduleId, ct);
        if (schedule is null)
            return Result<TicketAvailabilityResponseDto>.Fail(ErrorType.NotFound, $"Schedule {flightScheduleId} not found.");
        
        int? aircraftId = schedule.AssignedAircraftId;
        if (aircraftId is null)
        {
            var flight = await _unitOfWork.Flights.GetByIdAsync(schedule.FlightId, ct);
            if (flight?.DefaultAircraftId is not null)
                aircraftId = flight.DefaultAircraftId;
        }

        if (aircraftId is null)
            return Result<TicketAvailabilityResponseDto>.Fail(
                ErrorType.Validation,
                "Capacity unknown: schedule has no AssignedAircraftId and flight has no DefaultAircraftId."
            );

        var aircraft = await _unitOfWork.Aircrafts.GetByIdAsync(aircraftId.Value, ct);
        if (aircraft is null)
            return Result<TicketAvailabilityResponseDto>.Fail(
                ErrorType.NotFound,
                $"Aircraft {aircraftId.Value} not found."
            );

        var capacity = aircraft.SeatCapacity;

        var ticketsCount = await _unitOfWork.Tickets.CountByScheduleAsync(flightScheduleId, ct);
        var remaining = Math.Max(0, capacity - ticketsCount);

        var prices = await _unitOfWork.Tickets.GetMinPricesByFareClassAsync(flightScheduleId, ct);

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
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(id, ct);

        return ticket?.MapToTicketResponse();
    }

    public async Task<IReadOnlyList<TicketResponseDto>> GetByScheduleAsync(int flightScheduleId, CancellationToken ct)
    {
        var tickets = await _unitOfWork.Tickets.GetByScheduleAsync(flightScheduleId, ct);

        return tickets.Select(t => t.MapToTicketResponse()).ToList();
    }

    public async Task<Result<TicketResponseDto>> UpdateInventoryAsync(long id, UpdateTicketInventoryRequest request, CancellationToken ct)
    {
        if (request.SeatInventory < 0)
            return Result<TicketResponseDto>.Fail(ErrorType.Validation, "SeatInventory must be >= 0.");

        var ticket = await _unitOfWork.Tickets.GetByIdAsync(id, ct);
        if (ticket is null)
            return Result<TicketResponseDto>.Fail(ErrorType.NotFound, $"Ticket {id} not found.");

        await _unitOfWork.Tickets.UpdateSeatInventoryAsync(id, request.SeatInventory, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var updated = await _unitOfWork.Tickets.GetByIdAsync(id, ct);
        return Result<TicketResponseDto>.Ok((updated ?? ticket).MapToTicketResponse());
    }
}
