namespace AirportManagement.Domain.Models;

public class Ticket
{
    public long Id { get; set; }

    public int BookingId { get; set; }

    public int FareOfferId { get; set; }

    public decimal TotalPrice { get; set; }

    public string Currency { get; set; } = null!;

    public bool IsRefundable { get; set; }

    public string PassengerFullName { get; set; } = null!;

    public string PassengerEmail { get; set; } = null!;

    public string PassengerPhoneNumber { get; set; } = null!;
}
