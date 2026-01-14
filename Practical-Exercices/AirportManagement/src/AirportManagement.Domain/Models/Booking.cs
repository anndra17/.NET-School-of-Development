using AirportManagement.Domain.Enums;

namespace AirportManagement.Domain.Models;

public class Booking
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string ConfirmationCode { get; set; } = null!;

    public int Quantity { get; set; }

    public BookingStatus Status { get; set; }

    public DateTime? CreatedUtc { get; set; }
}