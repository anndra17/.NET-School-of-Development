namespace AirportManagement.Application.Dtos.Booking;

public sealed class CreateBookingResponseDto
{
    public string ConfirmationCode { get; init; } = null!;
    public string Status { get; init; } = null!;
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; } = null!;
}
