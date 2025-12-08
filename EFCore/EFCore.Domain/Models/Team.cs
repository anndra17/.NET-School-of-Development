namespace EFCore.Domain.Models;

public class Team : BaseDomainModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

