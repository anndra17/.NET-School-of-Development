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

    public static Flight MapToDomain(this CreateFlightRequest request)
    {
        return new Flight
        {
            AirlineId = request.AirlineId,
            FlightNumber = request.FlightNumber,
            OriginAirportId = request.OriginAirportId,
            DestinationAirportId = request.DestinationAirportId,
            DefaultAircraftId = request.DefaultAircraftId,
            IsActive = request.IsActive ?? true
        };
    }

    public static void ApplyToDomain(this UpdateFlightRequest request, Flight flight)
    {
        flight.AirlineId = request.AirlineId;
        flight.FlightNumber = request.FlightNumber;
        flight.OriginAirportId = request.OriginAirportId;
        flight.DestinationAirportId = request.DestinationAirportId;
        flight.DefaultAircraftId = request.DefaultAircraftId;
        flight.IsActive = request.IsActive;
    }
}
