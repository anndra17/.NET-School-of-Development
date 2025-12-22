namespace AirportManagement.Application.Dtos.Flight;

public sealed class FlightListItemResponse
{
    public int Id { get; set; }
    public string FlightNumber { get; set; } = null!;
    public string? AirlineIataCode { get; set; }
    public string? OriginIataCode { get; set; }
    public string? DestinationIataCode { get; set; }
    public bool IsActive { get; set; }
}
