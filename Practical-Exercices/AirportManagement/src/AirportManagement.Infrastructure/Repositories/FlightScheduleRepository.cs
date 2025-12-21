using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;

namespace AirportManagement.Infrastructure.Repositories;

public class FlightScheduleRepository : IFlightScheduleRepository
{
    public FlightScheduleRepository(AirportManagementDbContext context)
    {
    }

    public Task DeleteAsync(object Id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<FlightSchedule>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<FlightSchedule?> GetByIdAsync(object Id)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(FlightSchedule entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(FlightSchedule entity)
    {
        throw new NotImplementedException();
    }
}
