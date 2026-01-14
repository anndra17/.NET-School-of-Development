using System;
using System.Collections.Generic;

namespace AirportManagement.Infrastructure.Persistence.Entities;

public partial class TicketEntity
{
    public long Id { get; set; }

    public int BookingId { get; set; }

    public int FareOfferId { get; set; }

    public decimal TotalPrice { get; set; }

    public string Currency { get; set; } = null!;

    public bool IsRefundable { get; set; }

    public string PassengerFullName { get; set; } = null!;

    public string PassengerEmail { get; set; } = null!;

    public string PassengerPhoneNumber { get; set; } = null!;

    public virtual BookingEntity Booking { get; set; } = null!;

    public virtual FareOfferEntity FareOffer { get; set; } = null!;
}
