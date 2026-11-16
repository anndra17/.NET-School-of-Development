using AirportManagement.Domain.Enums;

namespace AirportManagement.Domain.Models;

public class FareOffer
{
    public int Id { get; set; }

    public int FlightScheduleId { get; set; }

    public FareClass FareClass { get; set; }

    public decimal BasePrice { get; set; }

    public decimal Taxes { get; set; }

    public string Currency { get; set; } = null!;

    public bool IsRefundable { get; set; }

    public int SeatsAvailable { get; set; }
}
