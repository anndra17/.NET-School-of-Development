using CafeConsole.Domain.Abstractions;

namespace CafeConsole.Domain.Models.Decorators;

public sealed class SyrupDecorator : BeverageDecorator
{
    private const decimal _price = 0.50m;
    private string _flavor;

    public SyrupDecorator(IBeverage baseBeverage, string flavor) : base(baseBeverage) => _flavor = flavor;

    protected override string AddedDecorator => $"{_flavor} Syrup";
    protected override decimal DecoratorPrice => _price;
}
