using AirportManagement.Application.Dtos.Ticket;
using AirportManagement.Domain.Models;

namespace AirportManagement.Application.Mappings;

internal static class TicketMappings
{
    public static TicketResponseDto MapToTicketResponse(this Ticket t)
        => new()
        {
            Id = t.Id,
            FlightScheduleId = t.FlightScheduleId,
            FareClass = t.FareClass.ToString(),
            BasePrice = t.BasePrice,
            Taxes = t.Taxes,
            TotalPrice = t.TotalPrice,
            Currency = t.Currency,
            IsRefundable = t.IsRefundable,
            SeatInventory = t.SeatInventory,
            BookingId = t.BookingId,
            PassengerFullName = t.PassengerFullName,
            PassengerEmail = t.PassengerEmail,
            PassengerPhoneNumber = t.PassengerPhoneNumber
        };
}