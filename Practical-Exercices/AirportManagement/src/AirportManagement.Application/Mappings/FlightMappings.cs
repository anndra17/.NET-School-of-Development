using AirportManagement.Application.Dtos.Flight;
using AirportManagement.Domain.Models;

namespace AirportManagement.Application.Mappings;

internal static class FlightMappings
{
    public static FlightResponseDto MapToFlightResponse(this Flight e)
       => new()
       {
           Id = e.Id,
           AirlineId = e.AirlineId,
           FlightNumber = e.FlightNumber,
           OriginAirportId = e.OriginAirportId,
           DestinationAirportId = e.DestinationAirportId,
           DefaultAircraftId = e.DefaultAircraftId,
           IsActive = e.IsActive,
       };
}
