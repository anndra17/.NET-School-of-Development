using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence.Entities;


namespace AirportManagement.Infrastructure.Mappings;

internal static class TicketEntityMappings
{
    public static Ticket ToDomain(this TicketEntity e)
        => new Ticket
        {
            Id = e.Id,
            FlightScheduleId = e.FlightScheduleId,
            FareClass = FareClassMappings.ToDomain(e.FareClass),
            BasePrice = e.BasePrice,
            Taxes = e.Taxes,
            TotalPrice = e.TotalPrice,
            Currency = e.Currency,
            IsRefundable = e.IsRefundable,
            SeatInventory = e.SeatInventory,
            BookingId = e.BookingId,
            PassengerFullName = e.PassengerFullName,
            PassengerEmail = e.PassengerEmail,
            PassengerPhoneNumber = e.PassengerPhoneNumber
        };

    public static TicketEntity ToNewEntity(this Ticket d)
        => new TicketEntity
        {
            FlightScheduleId = d.FlightScheduleId,
            FareClass = FareClassMappings.ToEntity(d.FareClass),
            BasePrice = d.BasePrice,
            Taxes = d.Taxes,
            Currency = d.Currency,
            IsRefundable = d.IsRefundable,
            SeatInventory = d.SeatInventory,
            BookingId = d.BookingId,
            PassengerFullName = d.PassengerFullName,
            PassengerEmail = d.PassengerEmail,
            PassengerPhoneNumber = d.PassengerPhoneNumber
        };
}