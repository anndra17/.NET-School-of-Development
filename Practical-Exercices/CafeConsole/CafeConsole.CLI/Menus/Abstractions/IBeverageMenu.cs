using CafeConsole.Domain.Abstractions;

namespace CafeConsole.CLI.Menus.Abstractions;

public interface IBeverageMenu
{
    IBeverage ChooseBase();
}