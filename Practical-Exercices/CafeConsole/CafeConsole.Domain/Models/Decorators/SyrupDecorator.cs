using CafeConsole.Domain.Abstractions;

namespace CafeConsole.Domain.Models.Decorators;

public class SyrupDecorator : IBeverage
{
    private const decimal _price = 0.50m;
    public string flavor = string.Empty;
    public string Name => $"{flavor} syrup";

    public decimal Cost() => _price;

    public string Describe() => $"{Name}. Price {_price.ToString("C")}";
}
