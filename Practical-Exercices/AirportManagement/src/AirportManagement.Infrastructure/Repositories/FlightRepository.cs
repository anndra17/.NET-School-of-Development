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

    public async Task InsertAsync(Flight entity, CancellationToken ct = default)
    {
        var flightEntity = entity.ToNewEntity();

        await _context.Set<FlightEntity>().AddAsync(flightEntity, ct);
    }

    public async Task<Flight?> GetByAirlineAndNumberAsync(int airlineId, string flightNumber, CancellationToken ct = default)
    {
        var flightEntity = await _context.Set<FlightEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.AirlineId == airlineId && f.FlightNumber == flightNumber, ct);

        return flightEntity?.ToDomain();
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

    public Task<bool> HasSchedulesAsync(int flightId, CancellationToken ct = default)
    {
        return _context.Set<FlightScheduleEntity>()
            .AsNoTracking()
            .AnyAsync(fs => fs.FlightId == flightId, ct);
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

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return await _context.Flights.AnyAsync(f => f.Id == id, ct);
    }
}
