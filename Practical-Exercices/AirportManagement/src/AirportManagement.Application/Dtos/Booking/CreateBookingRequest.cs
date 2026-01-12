using System.ComponentModel.DataAnnotations;

namespace AirportManagement.Application.Dtos.Booking;


public sealed class CreateBookingRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "FlightScheduleId must be a positive integer.")]
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
    public string Currency { get; init; } = null!;

    public bool IsRefundable { get; init; }

    [Required]
    [MinLength(1, ErrorMessage = "Passengers list must contain at least one passenger.")]
    public List<PassengerDto> Passengers { get; init; } = new();
}
