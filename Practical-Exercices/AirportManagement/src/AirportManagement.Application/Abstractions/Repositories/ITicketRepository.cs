using AirportManagement.Application.Dtos.Booking;
using AirportManagement.Application.Dtos.Ticket;
using AirportManagement.Domain.Enums;
using AirportManagement.Domain.Models;

namespace AirportManagement.Application.Abstractions.Repositories;

public interface ITicketRepository : IRepository<Ticket, long>
{
    Task<IReadOnlyList<Ticket>> GetByBookingAsync(int bookingId, CancellationToken ct = default);

    Task<int> CountByFareOfferAsync(int fareOfferId, CancellationToken ct = default);

    Task CreateTicketsForBookingAsync(
       int bookingId,
        int fareOfferId,
        decimal totalPrice,
        string currency,
        bool isRefundable,
        IReadOnlyList<PassengerDto> passengers,
        CancellationToken ct = default);
}
