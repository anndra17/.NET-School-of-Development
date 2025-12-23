using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Mappings;
using AirportManagement.Infrastructure.Persistence;
using AirportManagement.Infrastructure.Persistence.Entities;
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
             .FirstOrDefaultAsync(x => x.Id == id, ct);

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

    public Task InsertAsync(FlightSchedule entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(FlightSchedule entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return await _context.FlightSchedules.AnyAsync(f => f.Id == id, ct);
    }
}
