namespace AirportManagement.Infrastructure.Persistence.Entities;

public partial class TicketEntity
{
    public long Id { get; set; }

    public int FlightScheduleId { get; set; }

    public string FareClass { get; set; } = null!;

    public decimal BasePrice { get; set; }

    public decimal Taxes { get; set; }

    public decimal? TotalPrice { get; set; }

    public string Currency { get; set; } = null!;

    public bool IsRefundable { get; set; }

    public int SeatInventory { get; set; }

    public int BookingId { get; set; }

    public string PassengerFullName { get; set; } = null!;

    public string PassengerEmail { get; set; } = null!;

    public string PassengerPhoneNumber { get; set; } = null!;

    public virtual BookingEntity Booking { get; set; } = null!;

    public virtual FlightScheduleEntity FlightSchedule { get; set; } = null!;
}
