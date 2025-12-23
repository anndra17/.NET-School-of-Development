using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Mappings;
using AirportManagement.Infrastructure.Persistence;
using AirportManagement.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AirportManagement.Infrastructure.Repositories;

public class GateRepository : IGateRepository
{
    private readonly AirportManagementDbContext _context;

    public GateRepository(AirportManagementDbContext context) 
    {
        _context = context;
    }

    public Task DeleteAsync(int Id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Gate>> GetAllAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Gate?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(Gate entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Gate entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return await _context.Gates.AnyAsync(f => f.Id == id, ct);
    }

    public async Task<Gate?> GetByAirportAndCodeAsync(int airportId, string code, CancellationToken ct = default)
    {
        var entity = await _context.Set<GateEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.AirportId == airportId && g.Code == code, ct);

        return entity?.ToDomain();
    }
}
