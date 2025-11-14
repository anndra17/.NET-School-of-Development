using CafeConsole.Domain.Abstractions;

namespace CafeConsole.CLI.Menus.Abstractions;

public interface IAddOnMenu
{
    IBeverage ChooseAddOns(IBeverage baseBeverage);
}