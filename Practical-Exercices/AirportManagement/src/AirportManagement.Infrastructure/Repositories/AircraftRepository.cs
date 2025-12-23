using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;
using AirportManagement.Infrastructure.Mappings;
using AirportManagement.Infrastructure.Persistence.Entities;
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

    public async Task<Aircraft?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _context.Set<AircraftEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, ct);

        return entity?.ToDomain();
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

    public async Task<Aircraft?> GetByTailNumberAsync(string tailNumber, CancellationToken ct = default)
    {
        var entity = await _context.Set<AircraftEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.TailNumber == tailNumber, ct);

        return entity?.ToDomain();
    }
}
