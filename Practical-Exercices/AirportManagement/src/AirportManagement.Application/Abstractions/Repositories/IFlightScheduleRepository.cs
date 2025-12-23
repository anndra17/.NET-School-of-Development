using AirportManagement.Application.Dtos.Schedule;
using AirportManagement.Domain.Models;

namespace AirportManagement.Application.Abstractions.Repositories;

public interface IFlightScheduleRepository : IRepository<FlightSchedule>
{
    Task<FlightSchedule?> GetByFlightIdAndDepartureAsync(int flightId, DateTime departureUtc, CancellationToken ct = default);

    Task<bool> ExistsGateOverlapAsync(int gateId, DateTime depUtc, DateTime arrUtc, int? excludeScheduleId, CancellationToken ct = default);
    Task<(IReadOnlyList<ScheduleListItemResponse> Items, int TotalCount)> SearchAsync(
        string? originIata,
        string? destinationIata,
        DateOnly? date,
        int page,
        int pageSize,
        CancellationToken ct = default);
    Task<bool> HasTicketsAsync(int scheduleId, CancellationToken ct = default);
}
