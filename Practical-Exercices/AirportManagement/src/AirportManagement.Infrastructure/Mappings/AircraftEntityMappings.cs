using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence.Entities;

namespace AirportManagement.Infrastructure.Mappings;

internal static class AircraftEntityMappings
{
    public static Domain.Models.Aircraft ToDomain(this Persistence.Entities.AircraftEntity entity)
        => new Domain.Models.Aircraft
        {
            Id = entity.Id,
            TailNumber = entity.TailNumber,
            Model = entity.Model,
            SeatCapacity = entity.SeatCapacity
        };
}
