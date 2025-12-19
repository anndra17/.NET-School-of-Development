namespace AirportManagement.Infrastructure.Persistence.Entities;

public partial class GateEntity
{
    public int Id { get; set; }

    public int AirportId { get; set; }

    public string Code { get; set; } = null!;

    public virtual AirportEntity Airport { get; set; } = null!;

    public virtual ICollection<FlightScheduleEntity> FlightSchedules { get; set; } = new List<FlightScheduleEntity>();
}
