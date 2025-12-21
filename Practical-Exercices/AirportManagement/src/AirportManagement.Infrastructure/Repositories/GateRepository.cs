using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;

namespace AirportManagement.Infrastructure.Repositories;

public class GateRepository :  IGateRepository
{
    public GateRepository(AirportManagementDbContext context) 
    {
    }

    public Task DeleteAsync(object Id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Gate>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Gate?> GetByIdAsync(object Id)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(Gate entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Gate entity)
    {
        throw new NotImplementedException();
    }
}
