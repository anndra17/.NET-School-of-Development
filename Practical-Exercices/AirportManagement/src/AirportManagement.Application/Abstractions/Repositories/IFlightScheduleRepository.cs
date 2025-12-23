using AirportManagement.Domain.Models;

namespace AirportManagement.Application.Abstractions.Repositories;

public interface IFlightScheduleRepository : IRepository<FlightSchedule>
{
    Task<FlightSchedule?> GetByFlightIdAndDepartureAsync(int flightId, DateTime departureUtc, CancellationToken ct = default);

    Task<bool> ExistsGateOverlapAsync(int gateId, DateTime depUtc, DateTime arrUtc, int? excludeScheduleId, CancellationToken ct = default);
}
