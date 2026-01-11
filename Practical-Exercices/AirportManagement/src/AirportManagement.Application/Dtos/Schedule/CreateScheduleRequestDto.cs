using System.ComponentModel.DataAnnotations;

namespace AirportManagement.Application.Dtos.Schedule;

public class CreateScheduleRequestDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int FlightId { get; set; }

    [Required]
    public DateTime ScheduledDepartureUtc { get; set; }

    [Required]
    public DateTime ScheduledArrivalUtc { get; set; }

    public string? GateCode { get; set; }

    public string? AssignedAircraftTail { get; set; }

    public int? Status { get; set; }
}