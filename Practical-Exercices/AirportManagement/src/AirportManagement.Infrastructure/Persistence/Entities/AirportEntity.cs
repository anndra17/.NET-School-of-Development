namespace AirportManagement.Infrastructure.Persistence.Entities;

public partial class AirportEntity
{
    public int Id { get; set; }

    public string IATACode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? City { get; set; }

    public string? Country { get; set; }

    public string TimeZone { get; set; } = null!;

    public virtual ICollection<FlightEntity> FlightDestinationAirports { get; set; } = new List<FlightEntity>();

    public virtual ICollection<FlightEntity> FlightOriginAirports { get; set; } = new List<FlightEntity>();

    public virtual ICollection<GateEntity> Gates { get; set; } = new List<GateEntity>();
}
