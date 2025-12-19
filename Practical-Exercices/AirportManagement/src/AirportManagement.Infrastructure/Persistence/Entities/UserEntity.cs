namespace AirportManagement.Infrastructure.Persistence.Entities;

public partial class UserEntity
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<BookingEntity> Bookings { get; set; } = new List<BookingEntity>();
}
