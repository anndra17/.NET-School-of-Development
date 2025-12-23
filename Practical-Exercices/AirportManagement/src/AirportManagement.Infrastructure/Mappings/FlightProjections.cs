using AirportManagement.Application.Dtos.Flight;
using AirportManagement.Infrastructure.Persistence.Entities;
using System.Linq.Expressions;

namespace AirportManagement.Infrastructure.Mappings;

internal static class FlightProjections
{

    public static readonly Expression<Func<FlightEntity, FlightResponseWithRelatedData>> 
        ToResponseWithRelatedData = e => new FlightResponseWithRelatedData
       {
           Id = e.Id,
           AirlineId = e.AirlineId,
           FlightNumber = e.FlightNumber,
           OriginAirportId = e.OriginAirportId,
           DestinationAirportId = e.DestinationAirportId,
           DefaultAircraftId = e.DefaultAircraftId,
           IsActive = e.IsActive,

           AirlineIataCode = e.Airline != null ? e.Airline.IATACode : null,
           AirlineName = e.Airline != null ? e.Airline.Name : null,
           OriginIataCode = e.OriginAirport != null ? e.OriginAirport.IATACode : null,
           DestinationIataCode = e.DestinationAirport != null ? e.DestinationAirport.IATACode : null,
           DefaultAircraftTailNumber = e.DefaultAircraft != null ? e.DefaultAircraft.TailNumber : null
       };
}
