namespace AirportManagement.Application.Dtos.Ticket;

public sealed class TicketResponseDto
{
    public long Id { get; init; }
    public int FlightScheduleId { get; init; }

    public string FareClass { get; init; } = null!;
    public decimal BasePrice { get; init; }
    public decimal Taxes { get; init; }
    public decimal? TotalPrice { get; init; }

    public string Currency { get; init; } = null!;
    public bool IsRefundable { get; init; }

    public int SeatInventory { get; init; }

    public int BookingId { get; init; }
    public string PassengerFullName { get; init; } = null!;
    public string PassengerEmail { get; init; } = null!;
    public string PassengerPhoneNumber { get; init; } = null!;
}