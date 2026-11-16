using System.ComponentModel.DataAnnotations;

namespace AirportManagement.Application.Dtos.Ticket;

public sealed class CreateTicketRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "FareOfferId must be a positive integer.")]
    public int FareOfferId { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "BookingId must be a positive integer.")]
    public int BookingId { get; init; }

    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string PassengerFullName { get; init; } = null!;

    [Required]
    [EmailAddress(ErrorMessage = "PassengerEmail is invalid.")]
    public string PassengerEmail { get; init; } = null!;

    [Required]
    [Phone(ErrorMessage = "PassengerPhoneNumber is invalid.")]
    [StringLength(30)]
    public string PassengerPhoneNumber { get; init; } = null!;
}
