using System;
using System.Collections.Generic;

namespace AirportManagement.Infrastructure.Persistence.Entities;

public partial class FlightEntity
{
    public int Id { get; set; }

    public int AirlineId { get; set; }

    public string FlightNumber { get; set; } = null!;

    public int OriginAirportId { get; set; }

    public int DestinationAirportId { get; set; }

    public int? DefaultAircraftId { get; set; }

    public bool IsActive { get; set; }

    public virtual AirlineEntity Airline { get; set; } = null!;

    public virtual AircraftEntity? DefaultAircraft { get; set; }

    public virtual AirportEntity DestinationAirport { get; set; } = null!;

    public virtual AirportEntity OriginAirport { get; set; } = null!;

    public virtual ICollection<FlightScheduleEntity> FlightSchedules { get; set; } = new List<FlightScheduleEntity>();
}
