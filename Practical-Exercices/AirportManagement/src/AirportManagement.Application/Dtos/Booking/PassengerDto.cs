using System.ComponentModel.DataAnnotations;

namespace AirportManagement.Application.Dtos.Booking;

public sealed class PassengerDto
{
    [Required]
    public string FullName { get; init; } = null!;

    [Required]
    [EmailAddress(ErrorMessage = "Passenger email is invalid.")]
    public string Email { get; init; } = null!;

    [Required]
    public string PhoneNumber { get; init; } = null!;
}