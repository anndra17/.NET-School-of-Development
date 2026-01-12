using System.ComponentModel.DataAnnotations;

namespace AirportManagement.Application.Dtos.Flight;
public sealed class UpdateFlightRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "AirlineId must be a positive integer.")]
    public int AirlineId { get; set; }

    [Required]
    [RegularExpression(
            "^[A-Z]{2}[0-9]{4}$",
            ErrorMessage = "FlightNumber must be 2 uppercase letters followed by 4 digits (e.g. RO1234).")]
    public string FlightNumber { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "OriginAirportId must be a positive integer.")]
    public int OriginAirportId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "DestinationAirportId must be a positive integer.")]
    public int DestinationAirportId { get; set; }

    public int? DefaultAircraftId { get; set; }

    public bool IsActive { get; set; }
}
