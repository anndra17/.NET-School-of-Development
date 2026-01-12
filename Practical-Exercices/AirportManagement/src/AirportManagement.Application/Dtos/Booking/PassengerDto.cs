using System.ComponentModel.DataAnnotations;

namespace AirportManagement.Application.Dtos.Booking;

public sealed class PassengerDto
{
    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string FullName { get; init; } = null!;

    [Required]
    [EmailAddress(ErrorMessage = "Passenger email is invalid.")]
    public string Email { get; init; } = null!;

    [Required]
    [Phone(ErrorMessage = "Passenger phone number is invalid.")]
    [StringLength(30)]
    public string PhoneNumber { get; init; } = null!;
}