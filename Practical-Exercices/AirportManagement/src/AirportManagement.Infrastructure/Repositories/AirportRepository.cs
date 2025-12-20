using AirportManagement.Application.Abstractions;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;

namespace AirportManagement.Infrastructure.Repositories;

public class AirportRepository : GenericRepository<Airport>, IAirportRepository
{
    public AirportRepository(AirportManagementDbContext context) : base(context)
    {
    }
}
