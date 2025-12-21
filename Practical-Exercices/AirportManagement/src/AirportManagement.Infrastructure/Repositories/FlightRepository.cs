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
}
