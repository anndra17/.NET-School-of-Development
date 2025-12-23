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

    public static void ApplyToEntity(this FlightSchedule domain, FlightScheduleEntity entity)
    {
        entity.FlightId = domain.FlightId;
        entity.ScheduledDepartureUtc = domain.ScheduledDepartureUtc;
        entity.ScheduledArrivalUtc = domain.ScheduledArrivalUtc;
        entity.GateId = domain.GateId;
        entity.AssignedAircraftId = domain.AssignedAircraftId;
        entity.Status = (byte)domain.Status;
    }

    public static FlightScheduleEntity ToNewEntity(this FlightSchedule domain)
    {
        var e = new FlightScheduleEntity();
        domain.ApplyToEntity(e);
        return e;
    }
}