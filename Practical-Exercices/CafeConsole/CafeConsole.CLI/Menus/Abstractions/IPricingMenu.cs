using CafeConsole.Domain.Abstractions;

namespace CafeConsole.CLI.Menus.Abstractions;

public interface IPricingMenu
{
    IPricingStrategy ChoosePricing();
}