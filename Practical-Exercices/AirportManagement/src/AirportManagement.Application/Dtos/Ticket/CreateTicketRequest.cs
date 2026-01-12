using System.ComponentModel.DataAnnotations;

namespace AirportManagement.Application.Dtos.Ticket;

public sealed class CreateTicketRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "FlightScheduleId must be a positive integer.")]]
    public int FlightScheduleId { get; init; }

    [Required]
    [RegularExpression("^(Y|M|J|F)$", ErrorMessage = "FareClass must be one of: Y, M, J, F.")]
    public string FareClass { get; init; } = null!;

    [Range(0, double.MaxValue, ErrorMessage = "BasePrice must be >= 0.")]
    public decimal BasePrice { get; init; }

    [Range(0, double.MaxValue, ErrorMessage = "Taxes must be >= 0.")]
    public decimal Taxes { get; init; }

    [Required]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a 3-letter code (e.g. EUR).")]
    public string Currency { get; init; } = "EUR";

    public bool IsRefundable { get; init; }

    [Range(0, int.MaxValue, ErrorMessage = "SeatInventory must be >= 0.")]
    public int SeatInventory { get; init; }

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