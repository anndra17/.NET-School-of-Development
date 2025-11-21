using CafeConsole.App.Dtos;
using CafeConsole.Domain.Abstractions;

namespace CafeConsole.App.Abstractions;

public interface IBeverageAssembler
{
    IBeverage Assemble(IBeverage baseType, IEnumerable<AddOnDto> addOns);
}
