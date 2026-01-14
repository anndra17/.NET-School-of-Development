using System;
using System.Collections.Generic;

namespace AirportManagement.Infrastructure.Persistence.Entities;

public partial class FareOffer
{
    public int Id { get; set; }

    public int FlightScheduleId { get; set; }

    public string FareClass { get; set; } = null!;

    public decimal BasePrice { get; set; }

    public decimal Taxes { get; set; }

    public decimal? TotalPrice { get; set; }

    public string Currency { get; set; } = null!;

    public bool IsRefundable { get; set; }

    public int SeatsAvailable { get; set; }

    public virtual FlightSchedule FlightSchedule { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
