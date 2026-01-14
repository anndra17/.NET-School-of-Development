namespace AirportManagement.Application.Dtos.Booking;

public sealed class BookingResponseDto
{
    public string ConfirmationCode { get; init; } = null!;
    public string Status { get; init; } = null!;
    public int Quantity { get; init; }
    public DateTime? CreatedUtc { get; init; }
}
