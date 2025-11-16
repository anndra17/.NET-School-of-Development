using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Pricing;

namespace CafeConsole.CLI.Menus.Abstractions;

public interface IPricingMenu
{
    PricingPolicy ChoosePricing();
}