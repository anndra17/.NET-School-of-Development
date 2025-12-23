namespace AirportManagement.Application.Dtos.Booking;


public sealed class CreateBookingRequest
{
    public int FlightScheduleId { get; init; }

    public string FareClass { get; init; } = null!;   // "Y","M","J","F"
    public decimal BasePrice { get; init; }
    public decimal Taxes { get; init; }
    public string Currency { get; init; } = null!;
    public bool IsRefundable { get; init; }

    public List<PassengerDto> Passengers { get; init; } = new();
}
