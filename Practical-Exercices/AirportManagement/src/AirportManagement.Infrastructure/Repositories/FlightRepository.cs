using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Application.Dtos.Flight;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Mappings;
using AirportManagement.Infrastructure.Persistence;
using AirportManagement.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AirportManagement.Infrastructure.Repositories;

public class FlightRepository : IFlightRepository
{
    private readonly AirportManagementDbContext _context;

    public FlightRepository(AirportManagementDbContext context) 
    {
        _context = context;
    }

    public async Task<Flight?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var flightEntity = await _context.Set<FlightEntity>()
           .AsNoTracking()
           .FirstOrDefaultAsync(f => f.Id == id, ct);

        return flightEntity?.ToDomain();
    }

    public async Task<FlightResponseWithRelatedData?> GetByIdWithRelatedDataAsync(int id, CancellationToken ct = default)
    {
        return await _context.Set<FlightEntity>()
            .AsNoTracking()
            .Where(f => f.Id == id)
            .Select(FlightProjections.ToResponseWithRelatedData)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<Flight>> GetAllAsync(CancellationToken ct = default)
    {
        var flightsEntities = await _context.Flights
            .AsNoTracking()
            .ToListAsync(ct);

        var flights = flightsEntities
            .Select(fe => fe.ToDomain())
            .ToList();

        return flights;
    }

    public async Task<(IReadOnlyList<FlightListItemResponse> Items, int TotalCount)> SearchAsync(
        int? airlineId,
        int? originAirportId,
        int? destinationAirportId,
        string? flightNumber,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _context.Set<FlightEntity>().AsNoTracking().AsQueryable();

        if (airlineId is not null)
            query = query.Where(f => f.AirlineId == airlineId.Value);

        if (originAirportId is not null)
            query = query.Where(f => f.OriginAirportId == originAirportId.Value);

        if (destinationAirportId is not null)
            query = query.Where(f => f.DestinationAirportId == destinationAirportId.Value);

        if (!string.IsNullOrWhiteSpace(flightNumber))
            query = query.Where(f => f.FlightNumber == flightNumber);

        if (isActive is not null)
            query = query.Where(f => f.IsActive == isActive.Value);

        var total = await query.CountAsync(ct);

        // Paging
        var skip = (page - 1) * pageSize;
        var items = await query
            .OrderBy(f => f.Id)
            .Skip(skip)
            .Take(pageSize)
            .Select(FlightProjections.ToListItem)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task InsertAsync(Flight entity, CancellationToken ct = default)
    {
        var flightEntity = entity.ToNewEntity();

        await _context.Set<FlightEntity>().AddAsync(flightEntity, ct);
    }

    public Task UpdateAsync(Flight entity, CancellationToken ct = default)
    {
        return UpdateInternalAsync(entity, ct);
    }

    private async Task UpdateInternalAsync(Flight domain, CancellationToken ct)
    {
        var tracked = await _context.Set<FlightEntity>()
            .FirstOrDefaultAsync(f => f.Id == domain.Id, ct);

        if (tracked is null)
            return;

        domain.ApplyToEntity(tracked);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var tracked = await _context.Set<FlightEntity>()
            .FirstOrDefaultAsync(f => f.Id == id, ct);

        if (tracked is not null)
            _context.Set<FlightEntity>().Remove(tracked);
    }

    public Task<bool> ExistsByAirlineAndNumberAsync(int airlineId, string flightNumber, CancellationToken ct = default)
    {
        return _context.Set<FlightEntity>()
            .AsNoTracking()
            .AnyAsync(f => 
                f.AirlineId == airlineId && 
                f.FlightNumber == flightNumber, 
                ct
            );
    }

    public Task<bool> ExistsByAirlineAndNumberExceptAsync(int airlineId, string flightNumber, int excludeFlightId, CancellationToken ct = default)
    {
        return _context.Set<FlightEntity>()
            .AsNoTracking()
            .AnyAsync(f =>
                f.AirlineId == airlineId &&
                f.FlightNumber == flightNumber &&
                f.Id != excludeFlightId,
                ct
            );
    }
}
