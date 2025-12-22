using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AirportManagement.Infrastructure.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly AirportManagementDbContext _context;

    public TicketRepository(AirportManagementDbContext context) 
    {
        _context = context;
    }

    public Task DeleteAsync(int Id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Ticket>> GetAllAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Ticket?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(Ticket entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Ticket entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return await _context.Tickets.AnyAsync(f => f.Id == id, ct);
    }
}
