namespace EFCore.Domain.Models;

public class Coach : BaseDomainModel
{
    public string Name { get; set; }
    public int? TeamId { get; set; }
}

