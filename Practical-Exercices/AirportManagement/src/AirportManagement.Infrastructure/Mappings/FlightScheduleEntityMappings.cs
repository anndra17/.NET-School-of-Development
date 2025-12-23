using AirportManagement.Domain;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence.Entities;

namespace AirportManagement.Infrastructure.Mappings;

internal static class FlightScheduleEntityMappings
{
    public static FlightSchedule ToDomain(this FlightScheduleEntity entity)
        => new FlightSchedule
        {
            Id = entity.Id,
            FlightId = entity.FlightId,
            ScheduledDepartureUtc = entity.ScheduledDepartureUtc,
            ScheduledArrivalUtc = entity.ScheduledArrivalUtc,
            GateId = entity.GateId,
            AssignedAircraftId = entity.AssignedAircraftId,
            Status = (FlightScheduleStatus)entity.Status
        };
}