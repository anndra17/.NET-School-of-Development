using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;

namespace AirportManagement.Infrastructure.Repositories;

public class AircraftRepository : GenericRepository<Aircraft>, IAircraftRepository
{
    public AircraftRepository(AirportManagementDbContext context) : base(context)
    {
    }
}
