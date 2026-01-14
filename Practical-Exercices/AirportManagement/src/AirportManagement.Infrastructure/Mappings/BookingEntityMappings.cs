using AirportManagement.Domain.Enums;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence.Entities;

namespace AirportManagement.Infrastructure.Mappings;

internal static class BookingEntityMappings
{
    public static Booking ToDomain(this BookingEntity e)
        => new Booking
        {
            Id = e.Id,
            UserId = e.UserId,
            ConfirmationCode = e.ConfirmationCode,
            Quantity = e.Quantity,
            Status = (BookingStatus)e.Status,
            CreatedUtc = e.CreatedUtc
        };
}
