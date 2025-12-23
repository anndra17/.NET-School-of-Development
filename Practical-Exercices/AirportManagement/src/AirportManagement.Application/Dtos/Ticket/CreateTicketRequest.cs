namespace AirportManagement.Application.Dtos.Ticket;

public sealed class CreateTicketRequest
{
    public int FlightScheduleId { get; init; }

    public string FareClass { get; init; } = null!;
    public decimal BasePrice { get; init; }
    public decimal Taxes { get; init; }
    public string Currency { get; init; } = "EUR";

    public bool IsRefundable { get; init; }
    public int SeatInventory { get; init; }

    public int BookingId { get; init; }
    public string PassengerFullName { get; init; } = null!;
    public string PassengerEmail { get; init; } = null!;
    public string PassengerPhoneNumber { get; init; } = null!;
}