using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence.Entities;


namespace AirportManagement.Infrastructure.Mappings;

internal static class TicketEntityMappings
{
    public static Ticket ToDomain(this TicketEntity e)
        => new Ticket
        {
            Id = e.Id,
            BookingId = e.BookingId,
            FareOfferId = e.FareOfferId,
            TotalPrice = e.TotalPrice,
            Currency = e.Currency,
            IsRefundable = e.IsRefundable,
            PassengerFullName = e.PassengerFullName,
            PassengerEmail = e.PassengerEmail,
            PassengerPhoneNumber = e.PassengerPhoneNumber
        };

    public static TicketEntity ToNewEntity(this Ticket d)
        => new TicketEntity
        {
            BookingId = d.BookingId,
            FareOfferId = d.FareOfferId,
            TotalPrice = d.TotalPrice,
            Currency = d.Currency,
            IsRefundable = d.IsRefundable,
            PassengerFullName = d.PassengerFullName,
            PassengerEmail = d.PassengerEmail,
            PassengerPhoneNumber = d.PassengerPhoneNumber
        };
}