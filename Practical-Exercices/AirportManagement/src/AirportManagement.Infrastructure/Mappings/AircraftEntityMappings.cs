using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence.Entities;

namespace AirportManagement.Infrastructure.Mappings;

internal static class AircraftEntityMappings
{
    public static Aircraft ToDomain(this AircraftEntity entity)
        => new Aircraft
        {
            Id = entity.Id,
            TailNumber = entity.TailNumber,
            Model = entity.Model,
            SeatCapacity = entity.SeatCapacity
        };
}
