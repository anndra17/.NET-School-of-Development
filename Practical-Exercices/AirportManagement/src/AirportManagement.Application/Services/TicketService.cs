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

        var currency = request.Currency?.Trim().ToUpperInvariant();

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

        await _unitOfWork.TicketsRepository.InsertAsync(ticket, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var created = await _unitOfWork.TicketsRepository.GetByIdAsync(ticket.Id, ct);
        return Result<TicketResponseDto>.Ok((created ?? ticket).MapToTicketResponse());
    }

    public async Task<Result> DeleteAsync(long id, CancellationToken ct)
    {
        var exists = await _unitOfWork.TicketsRepository.ExistsAsync(id, ct);
        if (!exists)
            return Result.Fail(ErrorType.NotFound, $"Ticket {id} not found.");

        await _unitOfWork.TicketsRepository.DeleteAsync(id, ct);
        await _unitOfWork.SaveChangesAsync(ct);

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
        var remaining = Math.Max(0, capacity - ticketsCount);

        var prices = await _unitOfWork.TicketsRepository.GetMinPricesByFareClassAsync(flightScheduleId, ct);

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

        await _unitOfWork.TicketsRepository.UpdateSeatInventoryAsync(id, request.SeatInventory, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var updated = await _unitOfWork.TicketsRepository.GetByIdAsync(id, ct);
        return Result<TicketResponseDto>.Ok((updated ?? ticket).MapToTicketResponse());
    }
}
