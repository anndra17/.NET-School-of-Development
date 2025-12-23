using AirportManagement.Application.Common.Results;
using AirportManagement.Application.Dtos.Schedule;

namespace AirportManagement.Application.Abstractions.Services;

public interface IFlightScheduleService
{
    Task<ScheduleResponseDto> GetByIdAsync(int id, CancellationToken ct);
    Task<Result<ImportSchedulesResponseDto>> ImportAsync(Stream jsonStream, CancellationToken ct);
}
