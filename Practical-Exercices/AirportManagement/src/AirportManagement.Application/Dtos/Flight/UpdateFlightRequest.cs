namespace AirportManagement.Application.Dtos.Flight;
public sealed class UpdateFlightRequest
{
    public int AirlineId { get; set; }
    public string FlightNumber { get; set; } = null!;
    public int OriginAirportId { get; set; }
    public int DestinationAirportId { get; set; }
    public int? DefaultAircraftId { get; set; }
    public bool IsActive { get; set; }
}
