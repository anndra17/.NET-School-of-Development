using System.Text.Json.Serialization;

namespace AirportManagement.Application.Dtos.Schedule;

public sealed class ImportScheduleRowDto
{
    [JsonPropertyName("flightId")]
    public int FlightId { get; init; }

    [JsonPropertyName("scheduledDepartureUtc")]
    public DateTime ScheduledDepartureUtc { get; init; }

    [JsonPropertyName("scheduledArrivalUtc")]
    public DateTime ScheduledArrivalUtc { get; init; }

    [JsonPropertyName("gateCode")]
    public string? GateCode { get; init; }

    [JsonPropertyName("assignedAircraftTail")]
    public string? AssignedAircraftTail { get; init; }

    [JsonPropertyName("status")]
    public byte Status { get; init; }
}