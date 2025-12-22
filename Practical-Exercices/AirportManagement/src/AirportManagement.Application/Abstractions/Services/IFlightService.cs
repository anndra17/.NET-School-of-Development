using AirportManagement.Application.Dtos.Flight;

namespace AirportManagement.Application.Abstractions.Services;

public interface IFlightService
{
    Task<FlightResponseDto> GetByIdAsync(int id, CancellationToken ct);
}
