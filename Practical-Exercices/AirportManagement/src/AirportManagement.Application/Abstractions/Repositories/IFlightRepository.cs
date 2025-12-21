using AirportManagement.Domain.Models;

namespace AirportManagement.Application.Abstractions.Repositories;

public interface IFlightRepository : IRepository<Flight>
{
    Task<bool> ExistsByAirlineAndNumberAsync(int airlineId, string flightNumber, CancellationToken ct = default);

    Task<bool> ExistsByAirlineAndNumberExceptAsync(int airlineId, string flightNumber, int excludeFlightId, CancellationToken ct = default);

    // Pentru listare cu filtre + paging
    Task<(IReadOnlyList<Flight> Items, int TotalCount)> SearchAsync(
        int? airlineId,
        int? originAirportId,
        int? destinationAirportId,
        string? flightNumber,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken ct = default);
}
