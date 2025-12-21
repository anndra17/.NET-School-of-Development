using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;

namespace AirportManagement.Infrastructure.Repositories;

public class TicketRepository : ITicketRepository
{
    public TicketRepository(AirportManagementDbContext context) 
    {
    }

    public Task DeleteAsync(object Id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Ticket>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Ticket?> GetByIdAsync(object Id)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(Ticket entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Ticket entity)
    {
        throw new NotImplementedException();
    }
}
