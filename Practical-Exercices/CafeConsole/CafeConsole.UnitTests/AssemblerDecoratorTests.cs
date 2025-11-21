using CafeConsole.App.Dtos;
using CafeConsole.Domain.Models.Base;
using CafeConsole.Infrastructure.Composition;

namespace CafeConsole.Tests;

public class AssemblerDecoratorTests
{
    [Fact]
    public void Assembler_Adds_Milk_And_ExtraShot_Correctly()
    {
        var assembler = new BeverageAssembler();
        var baseBeverage = new Espresso();

        var addOns = new List<AddOnDto>
        {
            new("milk"),
            new("extra shot")
        };

        var composed = assembler.Assemble(baseBeverage, addOns);
        var cost = composed.Cost();
        var description = composed.Describe().ToLowerInvariant();

        Assert.Equal(3.70m, cost);
        Assert.Contains("milk", description);
        Assert.Contains("extra shot", description);
    }
}
