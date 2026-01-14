using System;
using System.Collections.Generic;

namespace AirportManagement.Infrastructure.Persistence.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
