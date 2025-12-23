namespace AirportManagement.Application.Dtos.Booking;

public sealed class PassengerDto
{
    public string FullName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string PhoneNumber { get; init; } = null!;
}