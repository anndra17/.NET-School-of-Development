namespace AirportManagement.Infrastructure.Persistence.Entities;

public partial class AirlineEntity
{
    public int Id { get; set; }

    public string IATACode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<FlightEntity> Flights { get; set; } = new List<FlightEntity>();
}
