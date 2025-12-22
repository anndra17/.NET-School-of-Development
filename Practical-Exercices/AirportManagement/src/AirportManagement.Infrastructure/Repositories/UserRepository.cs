using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AirportManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AirportManagementDbContext _context;

    public UserRepository(AirportManagementDbContext context)
    {
        _context = context;
    }

    public Task DeleteAsync(int Id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(User entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(User entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return await _context.Users.AnyAsync(f => f.Id == id, ct);
    }
}
