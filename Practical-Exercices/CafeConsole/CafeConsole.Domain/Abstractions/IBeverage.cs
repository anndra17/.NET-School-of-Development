namespace CafeConsole.Domain.Abstractions;

public interface IBeverage
{
    public string Name { get; }
    public decimal Cost();
    public string Describe();
}
