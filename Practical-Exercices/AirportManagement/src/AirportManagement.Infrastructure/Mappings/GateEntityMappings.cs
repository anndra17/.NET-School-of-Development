using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence.Entities;

namespace AirportManagement.Infrastructure.Mappings;

internal static class GateEntityMappings
{
    public static Gate ToDomain(this GateEntity entity)
        => new Gate
        {
            Id = entity.Id,
            AirportId = entity.AirportId,
            Code = entity.Code
        };
}
