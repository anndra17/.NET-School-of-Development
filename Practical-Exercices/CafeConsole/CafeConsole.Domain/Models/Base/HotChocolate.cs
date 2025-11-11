using CafeConsole.Domain.Abstractions;

namespace CafeConsole.Domain.Models.Base;

public class HotChocolate : IBeverage
{
    private const decimal _price = 3m;

    public string Name => "Hot Chocolate";

    public decimal Cost() => _price;

    public string Describe() => Name;
}
