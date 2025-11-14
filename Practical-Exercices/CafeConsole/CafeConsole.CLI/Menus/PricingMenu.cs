using CafeConsole.CLI.Menus.Abstractions;
using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Pricing;

namespace CafeConsole.CLI.Menus;

public class PricingMenu : IPricingMenu
{
    public IPricingStrategy ChoosePricing()
    {
        while (true) 
        {
            Console.WriteLine();
            Console.WriteLine("Pricing policy: 1) Regular 2) Happy Hour (-20%)");
            Console.WriteLine();
            
            Console.WriteLine("Select: ");
            var input = Console.ReadLine().Trim();
            if (input == "1") return new RegularPricing();
            if (input == "2") return new HappyHourPricing();
            
            Console.WriteLine("Invalid choice. Try again.");
        }
    }
}