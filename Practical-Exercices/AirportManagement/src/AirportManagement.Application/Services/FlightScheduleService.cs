using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Application.Abstractions.Services;
using AirportManagement.Application.Dtos.Schedule;
using AirportManagement.Application.Mappings;

namespace AirportManagement.Application.Services;

public class FlightScheduleService : IFlightScheduleService
{
    private readonly IUnitOfWork _unitOfWork;

    public FlightScheduleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<ScheduleResponseDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var schedule = await _unitOfWork.FlightSchedules.GetByIdAsync(id, ct);

        if (schedule is null)
        {
            return null;
        }

        return schedule.MapToScheduleResponse();
    }
}
