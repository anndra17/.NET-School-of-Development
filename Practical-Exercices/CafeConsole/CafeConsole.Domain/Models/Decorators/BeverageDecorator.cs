using CafeConsole.Domain.Abstractions;

namespace CafeConsole.Domain.Models.Decorators;

public abstract class BeverageDecorator : IBeverage
{
    protected readonly IBeverage _baseBeverage;

    protected BeverageDecorator(IBeverage baseBeverage) => _baseBeverage = baseBeverage;

    protected abstract string AddedDecorator { get; }   

    protected abstract decimal DecoratorPrice { get; }


    public string Name => $"{_baseBeverage.Name} + {AddedDecorator}";

    public decimal Cost() => _baseBeverage.Cost() + DecoratorPrice;

    public string Describe() => $"{_baseBeverage.Describe()} + {AddedDecorator}";
}
