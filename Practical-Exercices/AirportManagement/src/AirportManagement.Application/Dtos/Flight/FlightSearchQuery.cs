namespace AirportManagement.Application.Dtos.Flight;

public sealed class FlightSearchQuery
{
    public int? AirlineId { get; set; }
    public int? OriginAirportId { get; set; }
    public int? DestinationAirportId { get; set; }
    public string? FlightNumber { get; set; }
    public bool? IsActive { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}