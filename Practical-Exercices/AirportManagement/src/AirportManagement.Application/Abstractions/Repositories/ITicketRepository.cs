using AirportManagement.Application.Dtos.Ticket;
using AirportManagement.Domain.Models;

namespace AirportManagement.Application.Abstractions.Repositories;

public interface ITicketRepository : IRepository<Ticket, long>
{
    Task<int> CountByScheduleAsync(int flightScheduleId, CancellationToken ct = default);
    Task<IReadOnlyList<FareClassPriceDto>> GetMinPricesByFareClassAsync(int flightScheduleId, CancellationToken ct = default);
    Task<IReadOnlyList<Ticket>> GetByScheduleAsync(int flightScheduleId, CancellationToken ct = default);
    Task UpdateSeatInventoryAsync(long id, int seatInventory, CancellationToken ct = default);
}
