using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;

namespace AirportManagement.Infrastructure.Repositories;

public class AirportRepository : IAirportRepository
{
    public AirportRepository(AirportManagementDbContext context) 
    {
    }

    public Task DeleteAsync(object Id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Airport>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Airport?> GetByIdAsync(object Id)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(Airport entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Airport entity)
    {
        throw new NotImplementedException();
    }
}
