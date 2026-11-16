using AirportManagement.Application.Dtos.Flight;
using AirportManagement.Domain.Models;

namespace AirportManagement.Application.Abstractions.Repositories;

public interface IFlightRepository : IRepository<Flight, int>
{
    Task<FlightResponseWithRelatedData?> GetByIdWithRelatedDataAsync(int id, CancellationToken ct = default);

    Task<bool> ExistsByAirlineAndNumberAsync(int airlineId, string flightNumber, CancellationToken ct = default);

    Task<bool> ExistsByAirlineAndNumberExceptAsync(int airlineId, string flightNumber, int excludeFlightId, CancellationToken ct = default);

    Task<bool> HasSchedulesAsync(int flightId, CancellationToken ct = default);

    Task<Flight?> GetByAirlineAndNumberAsync(int airlineId, string flightNumber, CancellationToken ct = default);
}
