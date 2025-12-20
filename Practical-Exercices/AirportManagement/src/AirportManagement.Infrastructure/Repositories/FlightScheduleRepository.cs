using AirportManagement.Application.Abstractions;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;

namespace AirportManagement.Infrastructure.Repositories;

public class FlightScheduleRepository : GenericRepository<FlightSchedule>, IFlightScheduleRepository
{
    public FlightScheduleRepository(AirportManagementDbContext context) : base(context)
    {
    }
}
