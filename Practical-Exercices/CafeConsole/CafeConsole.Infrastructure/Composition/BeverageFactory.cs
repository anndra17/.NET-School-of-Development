using CafeConsole.App.Abstractions;
using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Base;

namespace CafeConsole.Infrastructure.Factories;

public sealed class BeverageFactory : IBeverageFactory
{
    private readonly Dictionary<string, Func<IBeverage>> _creators = new Dictionary<string, Func<IBeverage>>();

    public BeverageFactory() 
    {
        _creators["espresso"] = () => new Espresso();
        _creators["tea"] = () => new Tea();
        _creators["hot chocolate"] = () => new HotChocolate();
    }

    public IBeverage Create(string type)
    {
        Func<IBeverage> creator;

        _creators.TryGetValue(type, out creator);

        if (creator == null)
        {
            throw new ArgumentException($"Unknown beverage: {type}", nameof(type));
        }

        IBeverage beverage= creator();

        return beverage;
    }
}
