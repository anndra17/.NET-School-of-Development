using AirportManagement.Application.Dtos.Ticket;
using AirportManagement.Domain.Models;

namespace AirportManagement.Application.Mappings;

internal static class TicketMappings
{
    public static TicketResponseDto MapToTicketResponse(this Ticket t)
        => new()
        {
            Id = t.Id,
            FlightScheduleId = t.FareOffer?.FlightScheduleId ?? 0,
            FareClass = t.FareOffer?.FareClass.ToString() ?? string.Empty,
            BasePrice = t.FareOffer?.BasePrice ?? 0,
            Taxes = t.FareOffer?.Taxes ?? 0,
            TotalPrice = t.TotalPrice,
            Currency = t.Currency,
            IsRefundable = t.IsRefundable,
            SeatInventory = t.FareOffer?.SeatsAvailable ?? 0,
            BookingId = t.BookingId,
            PassengerFullName = t.PassengerFullName,
            PassengerEmail = t.PassengerEmail,
            PassengerPhoneNumber = t.PassengerPhoneNumber
        };
}
