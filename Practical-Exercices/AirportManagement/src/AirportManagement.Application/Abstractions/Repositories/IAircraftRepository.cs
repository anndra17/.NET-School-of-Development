using AirportManagement.Domain.Models;

namespace AirportManagement.Application.Abstractions.Repositories;

public interface IAircraftRepository : IRepository<Aircraft, int>
{
    Task<Aircraft?> GetByTailNumberAsync(string tailNumber, CancellationToken ct = default);
}
