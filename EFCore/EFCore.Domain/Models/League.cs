namespace EFCore.Domain.Models;

public class League : BaseDomainModel
{
    public string Name { get; set; }

    public ICollection<Team>? Teams { get; set; }
}
