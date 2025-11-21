using CafeConsole.Domain.Models.Base;
using CafeConsole.Infrastructure.Factories;

namespace CafeConsole.Tests;

public class BeverageFactoryTests
{
    [Theory]
    [InlineData("espresso", typeof(Espresso))]
    [InlineData("tea", typeof(Tea))]
    [InlineData("hot chocolate", typeof(HotChocolate))]
    public void Create_Should_Return_Correct_Type_For_Valid_Keys(string input, Type expectedType)
    {
        var factory = new BeverageFactory();

        var beverage = factory.Create(input);

        Assert.IsType(expectedType, beverage);
    }

    [Fact]
    public void Create_Should_Throw_ArgumentException_For_Invalid_Key()
    {
        var factory = new BeverageFactory();

        Assert.Throws<ArgumentException>(() => factory.Create("cola"));
    }
}