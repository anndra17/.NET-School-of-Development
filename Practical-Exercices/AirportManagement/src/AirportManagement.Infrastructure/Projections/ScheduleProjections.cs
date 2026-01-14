using AirportManagement.Application.Dtos.Schedule;
using AirportManagement.Infrastructure.Persistence.Entities;
using System.Linq.Expressions;

namespace AirportManagement.Infrastructure.Projections;

internal static class ScheduleProjections
{
    public static readonly Expression<Func<FlightScheduleEntity, ScheduleListItemResponse>> ToListItem =
        s => new ScheduleListItemResponse
        {
            Id = s.Id,
            FlightId = s.FlightId,
            ScheduledDepartureUtc = s.ScheduledDepartureUtc,
            ScheduledArrivalUtc = s.ScheduledArrivalUtc,
            Status = s.Status.ToString(),

            FlightNumber = s.Flight != null ? s.Flight.FlightNumber : null,
            AirlineIataCode = s.Flight != null && s.Flight.Airline != null ? s.Flight.Airline.IATACode : null,

            OriginIataCode = s.Flight != null && s.Flight.OriginAirport != null ? s.Flight.OriginAirport.IATACode : null,
            DestinationIataCode = s.Flight != null && s.Flight.DestinationAirport != null ? s.Flight.DestinationAirport.IATACode : null,

            GateCode = s.Gate != null ? s.Gate.Code : null,
            AssignedAircraftTailNumber = s.AssignedAircraft != null ? s.AssignedAircraft.TailNumber : null
        };
}
