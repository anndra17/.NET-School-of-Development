using CafeConsole.Domain.Abstractions;

namespace CafeConsole.Domain.Models.Decorators;

public class ExtraShotDecorator : IBeverage
{
    private const decimal _price = 0.80m;

    public string Name => "Extra Shot";

    public decimal Cost() => _price;

    public string Describe() => $"{Name}. Price {_price.ToString("C")}";
}
