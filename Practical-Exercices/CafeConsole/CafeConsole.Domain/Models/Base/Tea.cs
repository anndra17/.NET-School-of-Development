using CafeConsole.Domain.Abstractions;

namespace CafeConsole.Domain.Models.Base;

public class Tea : IBeverage
{
    private const decimal _price = 2m;

    public string Name => "Tea";

    public decimal Cost() => _price;

    public string Describe() => Name;
}
