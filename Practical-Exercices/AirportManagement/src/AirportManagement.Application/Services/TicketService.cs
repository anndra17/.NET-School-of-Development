using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Application.Abstractions.Services;
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
        if (!await _unitOfWork.BookingsRepository.ExistsAsync(request.BookingId, ct))
            return Result<TicketResponseDto>.Fail(ErrorType.NotFound, $"Booking {request.BookingId} not found.");

        var offer = await _unitOfWork.FareOffersRepository.GetByIdAsync(request.FareOfferId, ct);
        if (offer is null)
            return Result<TicketResponseDto>.Fail(ErrorType.NotFound, $"FareOffer {request.FareOfferId} not found.");

        var reserved = await _unitOfWork.FareOffersRepository.TryReserveSeatsAsync(request.FareOfferId, 1, ct);
        if (!reserved)
            return Result<TicketResponseDto>.Fail(ErrorType.Conflict, "No seats available for this fare offer.");

        var ticket = new Ticket
        {
            BookingId = request.BookingId,
            FareOfferId = request.FareOfferId,
            TotalPrice = offer.BasePrice + offer.Taxes,
            Currency = offer.Currency,
            IsRefundable = offer.IsRefundable,
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
        var offers = await _unitOfWork.FareOffersRepository.GetByScheduleAsync(flightScheduleId, ct);
        var remaining = offers.Sum(o => o.SeatsAvailable);

        var prices = offers
               .OrderBy(o => o.FareClass)
               .Select(o => new FareClassPriceDto
               {
                   FareClass = o.FareClass.ToString(), // "Y/M/J/F"
                   MinTotalPrice = o.BasePrice + o.Taxes,
                   Currency = o.Currency
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
}
