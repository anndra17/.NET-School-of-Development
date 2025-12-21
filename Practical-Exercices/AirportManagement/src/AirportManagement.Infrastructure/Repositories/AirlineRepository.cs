using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;

namespace AirportManagement.Infrastructure.Repositories;

public class AirlineRepository : IAirlineRepository
{
    public AirlineRepository(AirportManagementDbContext context) 
    {
    }

    public Task DeleteAsync(object Id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Airline>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Airline?> GetByIdAsync(object Id)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(Airline entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Airline entity)
    {
        throw new NotImplementedException();
    }
}
