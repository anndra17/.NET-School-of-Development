using AirportManagement.Application.Abstractions;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;

namespace AirportManagement.Infrastructure.Repositories;

public class AirlineRepository : GenericRepository<Airline>, IAirlineRepository
{
    public AirlineRepository(AirportManagementDbContext context) : base(context)
    {
    }
}
