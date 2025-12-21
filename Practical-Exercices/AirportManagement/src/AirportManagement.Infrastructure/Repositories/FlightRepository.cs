using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;
using AirportManagement.Infrastructure.Persistence.Entities;

namespace AirportManagement.Infrastructure.Repositories;

public class FlightRepository : IFlightRepository
{
    public FlightRepository(AirportManagementDbContext context) 
    {
    }

    public Task DeleteAsync(object Id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Flight>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Flight?> GetByIdAsync(object Id)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(Flight entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Flight entity)
    {
        throw new NotImplementedException();
    }
}
