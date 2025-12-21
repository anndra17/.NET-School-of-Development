using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;

namespace AirportManagement.Infrastructure.Repositories;

public class FlightRepository : GenericRepository<Flight>, IFlightRepository
{
    public FlightRepository(AirportManagementDbContext context) : base(context)
    {
    }
}
