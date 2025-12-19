namespace AirportManagement.Infrastructure.Persistence.Entities;

public partial class BookingEntity
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string ConfirmationCode { get; set; } = null!;

    public int Quantity { get; set; }

    public byte Status { get; set; }

    public DateTime? CreatedUtc { get; set; }

    public virtual ICollection<TicketEntity> Tickets { get; set; } = new List<TicketEntity>();

    public virtual UserEntity User { get; set; } = null!;
}
