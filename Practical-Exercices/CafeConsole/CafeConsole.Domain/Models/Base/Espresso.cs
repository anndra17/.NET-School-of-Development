using CafeConsole.Domain.Abstractions;

namespace CafeConsole.Domain.Models.Base;

public sealed class Espresso : IBeverage
{
    private const decimal _price = 2.50m;

    public string Name => "Espresso";

    public decimal Cost() => _price;

    public string Describe() => Name;
}
