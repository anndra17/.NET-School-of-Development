using CafeConsole.Domain.Abstractions;

namespace CafeConsole.App.Abstractions;

public interface IBeverageFactory
{
    IBeverage Create(string type);
}
