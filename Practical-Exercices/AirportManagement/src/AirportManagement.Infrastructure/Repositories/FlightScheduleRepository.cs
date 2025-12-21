using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;

namespace AirportManagement.Infrastructure.Repositories;

public class FlightScheduleRepository : IFlightScheduleRepository
{
    public FlightScheduleRepository(AirportManagementDbContext context)
    {
    }
}
