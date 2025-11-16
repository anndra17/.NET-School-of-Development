using CafeConsole.App.Abstractions;
using CafeConsole.CLI.Menus.Abstractions;
using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Pricing;

namespace CafeConsole.CLI.Menus;

public class PricingMenu : IPricingMenu
{
    public PricingPolicy ChoosePricing()
    {
        while (true) 
        {
            Console.WriteLine();
            Console.WriteLine("Pricing policy:\n 1) Regular\n 2) Happy Hour (-20%)");
            Console.WriteLine("Select: ");

            var input = Console.ReadLine().Trim() ?? string.Empty;

            PricingPolicy policy;
            if (input == "1")
                policy = PricingPolicy.Regular;
            else if (input == "2")
                policy = PricingPolicy.HappyHour;
            else
            {
                Console.WriteLine("Invalid choice. Try again.");
                continue;
            }

            return policy;
        }
    }
}