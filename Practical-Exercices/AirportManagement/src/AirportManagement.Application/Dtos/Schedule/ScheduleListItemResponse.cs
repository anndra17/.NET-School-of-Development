namespace AirportManagement.Application.Dtos.Schedule;

public sealed class ScheduleListItemResponse
{
    public int Id { get; init; }
    public int FlightId { get; init; }

    public DateTime ScheduledDepartureUtc { get; init; }
    public DateTime ScheduledArrivalUtc { get; init; }

    public string Status { get; init; } = null!;

    public string? FlightNumber { get; init; }
    public string? AirlineIataCode { get; init; }
    public string? OriginIataCode { get; init; }
    public string? DestinationIataCode { get; init; }

    public string? GateCode { get; init; }
    public string? AssignedAircraftTailNumber { get; init; }
}