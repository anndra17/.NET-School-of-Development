using AirportManagement.Application.Dtos.Schedule;
using AirportManagement.Domain.Models;

namespace AirportManagement.Application.Mappings;

internal static class ScheduleMappings
{
    public static ScheduleResponseDto MapToScheduleResponse(this FlightSchedule s)
       => new()
       {
           Id = s.Id,
           FlightId = s.FlightId,
           ScheduledDepartureUtc = s.ScheduledDepartureUtc,
           ScheduledArrivalUtc = s.ScheduledArrivalUtc,
           GateId = s.GateId,
           AssignedAircraftId = s.AssignedAircraftId,
           Status = s.Status.ToString(),
       };
}
