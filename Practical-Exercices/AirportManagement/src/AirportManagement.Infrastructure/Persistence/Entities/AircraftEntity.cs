namespace AirportManagement.Infrastructure.Persistence.Entities;

public partial class AircraftEntity
{
    public int Id { get; set; }

    public string TailNumber { get; set; } = null!;

    public string Model { get; set; } = null!;

    public int SeatCapacity { get; set; }

    public virtual ICollection<FlightScheduleEntity> FlightSchedules { get; set; } = new List<FlightScheduleEntity>();

    public virtual ICollection<FlightEntity> Flights { get; set; } = new List<FlightEntity>();
}
