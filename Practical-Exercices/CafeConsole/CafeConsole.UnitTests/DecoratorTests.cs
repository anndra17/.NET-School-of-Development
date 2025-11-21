using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Base;
using CafeConsole.Domain.Models.Decorators;

namespace CafeConsole.UnitTests;

public class DecoratorTests
{
    [Fact]
    public void Espresso_With_Milk_And_ExtraShot_Has_Correct_Cost_And_Description()
    {
        IBeverage beverage = new Espresso();
        beverage = new MilkDecorator(beverage);
        beverage = new ExtraShotDecorator(beverage);

        var cost = beverage.Cost();
        var description = beverage.Describe();

        Assert.Equal(3.70m, cost);
        Assert.Contains("milk", description.ToLowerInvariant());
        Assert.Contains("extra shot", description.ToLowerInvariant());
    }
}