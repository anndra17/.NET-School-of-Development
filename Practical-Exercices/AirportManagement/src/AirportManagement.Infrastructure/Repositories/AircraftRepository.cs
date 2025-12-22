using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AirportManagement.Infrastructure.Repositories;

public class AircraftRepository : IAircraftRepository
{
    private readonly AirportManagementDbContext _context;

    public AircraftRepository(AirportManagementDbContext context)
    {
        _context = context;
    }

    public Task DeleteAsync(int Id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Aircraft>> GetAllAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Aircraft?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(Aircraft entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Aircraft entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return await _context.Aircrafts.AnyAsync(f => f.Id == id, ct);
    }
}
