using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Application.Dtos.Ticket;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Mappings;
using AirportManagement.Infrastructure.Persistence;
using AirportManagement.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AirportManagement.Infrastructure.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly AirportManagementDbContext _context;

    public TicketRepository(AirportManagementDbContext context) 
    {
        _context = context;
    }

    public async Task<Ticket?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var entity = await _context.Set<TicketEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        return entity?.ToDomain();
    }

    public async Task InsertAsync(Ticket entity, CancellationToken ct = default)
    {
        var e = entity.ToNewEntity();

        await _context.Set<TicketEntity>().AddAsync(e, ct);
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var tracked = await _context.Set<TicketEntity>()
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (tracked is not null)
            _context.Set<TicketEntity>().Remove(tracked);
    }

    public async Task<bool> ExistsAsync(long id, CancellationToken ct = default)
    {
        return await _context.Tickets.AnyAsync(f => f.Id == id, ct);
    }

    public Task<IEnumerable<Ticket>> GetAllAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Ticket entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<int> CountByScheduleAsync(int flightScheduleId, CancellationToken ct = default)
    {
        return await _context.Set<TicketEntity>()
       .AsNoTracking()
       .Where(t => t.FlightScheduleId == flightScheduleId &&
                   t.Booking.Status == 0) 
       .CountAsync(ct);
    }

    public async Task<IReadOnlyList<FareClassPriceDto>> GetMinPricesByFareClassAsync(int flightScheduleId, CancellationToken ct = default)
    {
        var rows = await _context.Set<TicketEntity>()
            .AsNoTracking()
            .Where(t => t.FlightScheduleId == flightScheduleId &&
                        t.Booking.Status == 0)
            .GroupBy(t => t.FareClass)
            .Select(g => new FareClassPriceDto
            {
                FareClass = g.Key,
                MinTotalPrice = g.Min(x => x.TotalPrice),
                Currency = g.Select(x => x.Currency).FirstOrDefault()
            })
            .ToListAsync(ct);

        return rows;
    }

    public async Task<IReadOnlyList<Ticket>> GetByScheduleAsync(int flightScheduleId, CancellationToken ct = default)
    {
        var entities = await _context.Set<TicketEntity>()
            .AsNoTracking()
            .Where(t => t.FlightScheduleId == flightScheduleId)
            .ToListAsync(ct);

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task UpdateSeatInventoryAsync(long id, int seatInventory, CancellationToken ct = default)
    {
        var tracked = await _context.Set<TicketEntity>()
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (tracked is null)
            return;

        tracked.SeatInventory = seatInventory;
    }
}
