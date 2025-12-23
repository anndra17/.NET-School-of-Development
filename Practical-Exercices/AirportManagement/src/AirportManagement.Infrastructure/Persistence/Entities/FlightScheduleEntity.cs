namespace AirportManagement.Infrastructure.Persistence.Entities;

public partial class FlightScheduleEntity
{
    public int Id { get; set; }

    public int FlightId { get; set; }

    public DateTime ScheduledDepartureUtc { get; set; }

    public DateTime ScheduledArrivalUtc { get; set; }

    public int? GateId { get; set; }

    public int? AssignedAircraftId { get; set; }

    public byte Status { get; set; }

    public virtual AircraftEntity? AssignedAircraft { get; set; }

    public virtual FlightEntity Flight { get; set; } = null!;

    public virtual GateEntity? Gate { get; set; }

    public virtual ICollection<TicketEntity> Tickets { get; set; } = new List<TicketEntity>();
}
