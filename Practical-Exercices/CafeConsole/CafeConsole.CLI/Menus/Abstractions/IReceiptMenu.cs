using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Pricing;

namespace CafeConsole.CLI.Menus.Abstractions;

public interface IReceiptMenu
{
    void Print(IBeverage beverage, PricingPolicy policy);
}