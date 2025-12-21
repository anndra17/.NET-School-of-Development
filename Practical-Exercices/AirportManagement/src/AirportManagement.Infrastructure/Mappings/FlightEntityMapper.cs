using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence.Entities;

namespace AirportManagement.Infrastructure.Mappings;

internal static class FlightEntityMapper
{
    public static Flight ToDomain(this FlightEntity e)
        => new Flight
        {
            Id = e.Id,
            AirlineId = e.AirlineId,
            FlightNumber = e.FlightNumber,
            OriginAirportId = e.OriginAirportId,
            DestinationAirportId = e.DestinationAirportId,
            DefaultAircraftId = e.DefaultAircraftId,
            IsActive = e.IsActive
        };

    public static void ApplyToEntity(this Flight domain, FlightEntity entity)
    {
        entity.AirlineId = domain.AirlineId;
        entity.FlightNumber = domain.FlightNumber;
        entity.OriginAirportId = domain.OriginAirportId;
        entity.DestinationAirportId = domain.DestinationAirportId;
        entity.DefaultAircraftId = domain.DefaultAircraftId;
        entity.IsActive = domain.IsActive;
    }

    public static FlightEntity ToNewEntity(this Flight domain)
    {
        var e = new FlightEntity();
        domain.ApplyToEntity(e);
        return e;
    }
}
