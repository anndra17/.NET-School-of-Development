using CafeConsole.Domain.Abstractions;

namespace CafeConsole.App.Abstractions;

public interface IBeverageAssembler
{
    IBeverage Assemble(IBeverage baseType, IEnumerable<(string Name, string? flavor)> addOns);
}
