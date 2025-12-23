using AirportManagement.Application.Dtos.Flight;
using AirportManagement.Application.Dtos.Schedule;

namespace AirportManagement.Application.Abstractions.Services;

public interface IFlightScheduleService
{
    Task<ScheduleResponseDto> GetByIdAsync(int id, CancellationToken ct);
}
