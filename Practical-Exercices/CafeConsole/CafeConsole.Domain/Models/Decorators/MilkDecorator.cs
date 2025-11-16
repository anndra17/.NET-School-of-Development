using CafeConsole.Domain.Abstractions;

namespace CafeConsole.Domain.Models.Decorators;

public sealed class MilkDecorator : BeverageDecorator
{
    private const decimal _price = 0.40m;

    public MilkDecorator(IBeverage baseBeverage) : base(baseBeverage) { }

    protected override string AddedDecorator => "Milk";
    protected override decimal DecoratorPrice => _price;
}
