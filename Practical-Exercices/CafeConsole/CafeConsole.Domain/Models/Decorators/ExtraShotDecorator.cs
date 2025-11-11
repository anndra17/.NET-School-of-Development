using CafeConsole.Domain.Abstractions;

namespace CafeConsole.Domain.Models.Decorators;

public class ExtraShotDecorator : BeverageDecorator
{
    private const decimal _price = 0.80m;

    public ExtraShotDecorator(IBeverage baseBeverage) : base(baseBeverage) { }

    protected override string AddedDecorator => "Extra Shot";
    protected override decimal DecoratorPrice => _price;
}
