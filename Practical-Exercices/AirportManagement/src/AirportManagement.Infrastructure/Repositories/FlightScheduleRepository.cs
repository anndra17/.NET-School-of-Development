using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AirportManagement.Infrastructure.Repositories;

public class FlightScheduleRepository : IFlightScheduleRepository
{
    private readonly AirportManagementDbContext _context;

    public FlightScheduleRepository(AirportManagementDbContext context)
    {
        _context = context;
    }

    public Task DeleteAsync(int Id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<FlightSchedule>> GetAllAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<FlightSchedule?> GetByIdAsync(int id, CancellationToken ct = default)
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
