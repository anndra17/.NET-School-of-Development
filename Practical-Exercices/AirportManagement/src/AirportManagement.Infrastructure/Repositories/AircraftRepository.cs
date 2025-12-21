using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;

namespace AirportManagement.Infrastructure.Repositories;

public class AircraftRepository : IAircraftRepository
{   
    public AircraftRepository(AirportManagementDbContext context)
    {
    }

    public Task DeleteAsync(object Id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Aircraft>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Aircraft?> GetByIdAsync(object Id)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(Aircraft entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Aircraft entity)
    {
        throw new NotImplementedException();
    }
}
