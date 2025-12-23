namespace AirportManagement.Application.Dtos.Schedule;

public sealed class ScheduleResponseDto
{
    public int Id { get; init; }
    public int FlightId { get; init; }
    public DateTime ScheduledDepartureUtc { get; init; }
    public DateTime ScheduledArrivalUtc { get; init; }
    public int? GateId { get; init; }
    public int? AssignedAircraftId { get; init; }
    public string Status { get; init; } = null!;
}
