using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;

namespace AirportManagement.Infrastructure.Repositories;

public class AirportRepository : IAirportRepository
{
    public AirportRepository(AirportManagementDbContext context) 
    {
    }
}
