namespace EFCore.Domain.Models;

public class Match
{
    public int HomeTeamId { get; set; }
    public int AwayTeamId { get; set; }
    public decimal TicketPrice { get; set; }
    public DateTime Date { get; set; }
}
