using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Application.Dtos.Schedule;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Mappings;
using AirportManagement.Infrastructure.Persistence;
using AirportManagement.Infrastructure.Persistence.Entities;
using AirportManagement.Infrastructure.Projections;
using Microsoft.EntityFrameworkCore;

namespace AirportManagement.Infrastructure.Repositories;

public class FlightScheduleRepository : IFlightScheduleRepository
{
    private readonly AirportManagementDbContext _context;

    public FlightScheduleRepository(AirportManagementDbContext context)
    {
        _context = context;
    }

    public async Task<FlightSchedule?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _context.Set<FlightScheduleEntity>()
             .AsNoTracking()
             .FirstOrDefaultAsync(fs => fs.Id == id, ct);

        return entity?.ToDomain();
    }

    public async Task<FlightSchedule?> GetByFlightIdAndDepartureAsync(int flightId, DateTime departureUtc, CancellationToken ct = default)
    {
        var entity = await  _context.Set<FlightScheduleEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(fs => fs.FlightId == flightId && fs.ScheduledDepartureUtc == departureUtc, ct);

        return entity?.ToDomain();
    }


    public Task DeleteAsync(int Id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<FlightSchedule>> GetAllAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task InsertAsync(FlightSchedule entity, CancellationToken ct = default)
    {
        var schedule = entity.ToNewEntity();

        await _context.Set<FlightScheduleEntity>().AddAsync(schedule, ct);
    }

    public Task UpdateAsync(FlightSchedule entity, CancellationToken ct = default)
    {
        return UpdateInternalAsync(entity, ct);
    }

    private async Task UpdateInternalAsync(FlightSchedule domain, CancellationToken ct = default)
    {
        var tracked = await _context.Set<FlightScheduleEntity>()
            .FirstOrDefaultAsync(s => s.Id == domain.Id, ct);

        if (tracked is null)
            return;

        domain.ApplyToEntity(tracked);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return await _context.FlightSchedules.AnyAsync(f => f.Id == id, ct);
    }

    public Task<bool> ExistsGateOverlapAsync(int gateId, DateTime departureUtc, DateTime arrivalUtc, int? excludeScheduleId, CancellationToken ct = default)
    {
        var q = _context.Set<FlightScheduleEntity>().AsNoTracking().AsQueryable();

        q = q.Where(fs => fs.GateId == gateId);

        if (excludeScheduleId is not null)
            q = q.Where(s => s.Id != excludeScheduleId.Value);

        return q.AnyAsync(s =>
            s.ScheduledDepartureUtc < arrivalUtc &&
            departureUtc < s.ScheduledArrivalUtc, ct);
    }

    public async Task<(IReadOnlyList<ScheduleListItemResponse> Items, int TotalCount)> SearchAsync(
        string? originIata, 
        string? destinationIata, 
        DateOnly? date,
        int page,
        int pageSize, 
        CancellationToken ct = default)
    {
        var query = _context.Set<FlightScheduleEntity>()
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(originIata))
        {
            var o = originIata.Trim().ToUpperInvariant();
            query = query.Where(s => s.Flight != null &&
                                     s.Flight.OriginAirport != null &&
                                     s.Flight.OriginAirport.IATACode == o);
        }

        if (!string.IsNullOrWhiteSpace(destinationIata))
        {
            var d = destinationIata.Trim().ToUpperInvariant();
            query = query.Where(s => s.Flight != null &&
                                     s.Flight.DestinationAirport != null &&
                                     s.Flight.DestinationAirport.IATACode == d);
        }

        if (date is not null)
        {
            var dayStartUtc = date.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var dayEndUtc = dayStartUtc.AddDays(1);

            query = query.Where(s => s.ScheduledDepartureUtc >= dayStartUtc &&
                                     s.ScheduledDepartureUtc < dayEndUtc);
        }

        var total = await query.CountAsync(ct);

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var skip = (page - 1) * pageSize;

        var items = await query
            .OrderBy(s => s.ScheduledDepartureUtc)
            .Skip(skip)
            .Take(pageSize)
            .Select(ScheduleProjections.ToListItem)
            .ToListAsync(ct);

        return (items, total);
    }
}
