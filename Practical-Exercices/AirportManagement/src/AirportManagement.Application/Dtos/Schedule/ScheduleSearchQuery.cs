namespace AirportManagement.Application.Dtos.Schedule;

public sealed class ScheduleSearchQuery
{
    public string? Origin { get; init; }
    public string? Destination { get; init; }

    public DateOnly? Date { get; init; }

    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
