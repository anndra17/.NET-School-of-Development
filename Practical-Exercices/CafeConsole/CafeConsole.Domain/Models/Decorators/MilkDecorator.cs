using CafeConsole.Domain.Abstractions;

namespace CafeConsole.Domain.Models.Decorators;

public class MilkDecorator : IBeverage
{
    private const decimal _price = 0.40m;
    public string Name => "Milk";

    public decimal Cost() => _price;

    public string Describe() => $"{Name}. Price {_price.ToString("C")}";
}
