using System.ComponentModel.DataAnnotations;

namespace AirportManagement.Application.Dtos.Schedule;

public class CreateScheduleRequestDto : IValidatableObject
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "FlightId must be a positive integer.")]
    public int FlightId { get; set; }

    [Required]
    public DateTime ScheduledDepartureUtc { get; set; }

    [Required]
    public DateTime ScheduledArrivalUtc { get; set; }

    [StringLength(10, ErrorMessage = "GateCode is too long.")]
    public string? GateCode { get; set; }

    [StringLength(20, ErrorMessage = "AssignedAircraftTail is too long.")]
    public string? AssignedAircraftTail { get; set; }

    [Range(0, 4, ErrorMessage = "Status must be between 0 and 4.")]
    public int? Status { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ScheduledDepartureUtc >= ScheduledArrivalUtc)
        {
            yield return new ValidationResult(
                "Departure must be earlier than arrival.",
                new[] { nameof(ScheduledDepartureUtc), nameof(ScheduledArrivalUtc) });
        }
    }
}