namespace EFCore.Domain.Models;

public class League : BaseDomainModel
{
    public string Name { get; set; }

    public virtual  ICollection<Team> Teams { get; set; } = new List<Team>();
}
