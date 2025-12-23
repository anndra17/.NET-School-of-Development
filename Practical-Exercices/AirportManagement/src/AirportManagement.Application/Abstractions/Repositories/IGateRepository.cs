using AirportManagement.Domain.Models;

namespace AirportManagement.Application.Abstractions.Repositories;

public interface IGateRepository : IRepository<Gate>
{
    Task<Gate?> GetByAirportAndCodeAsync(int airportId, string code, CancellationToken cancellationToken);
}
