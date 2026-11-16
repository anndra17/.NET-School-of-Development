using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AirportManagement.Infrastructure.Repositories;

public class AirlineRepository : IAirlineRepository
{
    private readonly AirportManagementDbContext _context;

    public AirlineRepository(AirportManagementDbContext context) 
    {
        _context = context;
    }

    public Task DeleteAsync(int Id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Airline>> GetAllAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Airline?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(Airline entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Airline entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return await _context.Airlines.AnyAsync(f => f.Id == id, ct);
    }
}
