using System;
using System.Collections.Generic;

namespace AirportManagement.Infrastructure.Persistence.Entities;

public partial class Airport
{
    public int Id { get; set; }

    public string IATACode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? City { get; set; }

    public string? Country { get; set; }

    public string TimeZone { get; set; } = null!;

    public virtual ICollection<Flight> FlightDestinationAirports { get; set; } = new List<Flight>();

    public virtual ICollection<Flight> FlightOriginAirports { get; set; } = new List<Flight>();

    public virtual ICollection<Gate> Gates { get; set; } = new List<Gate>();
}
