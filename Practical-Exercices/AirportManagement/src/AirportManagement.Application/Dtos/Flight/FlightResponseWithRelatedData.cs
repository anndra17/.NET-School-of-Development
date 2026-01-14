namespace AirportManagement.Application.Dtos.Flight;

public sealed class FlightResponseWithRelatedData : FlightResponseDto
{
    public string? AirlineIataCode { get; set; }
    public string? AirlineName { get; set; }
    public string? OriginIataCode { get; set; }
    public string? DestinationIataCode { get; set; }
    public string? DefaultAircraftTailNumber { get; set; }
}
