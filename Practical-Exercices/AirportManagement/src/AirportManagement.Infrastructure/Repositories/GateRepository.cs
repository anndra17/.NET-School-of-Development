using AirportManagement.Application.Abstractions;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;

namespace AirportManagement.Infrastructure.Repositories;

public class GateRepository : GenericRepository<Gate>, IGateRepository
{
    public GateRepository(AirportManagementDbContext context) : base(context)
    {
    }
}
