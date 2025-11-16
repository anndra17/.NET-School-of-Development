using CafeConsole.App.Abstractions;
using CafeConsole.CLI.Menus.Abstractions;
using CafeConsole.Domain.Abstractions;
using CafeConsole.Infrastructure.Observers;

namespace CafeConsole.CLI.Menus;

internal class BeverageMenu : IBeverageMenu
{
    private readonly IBeverageFactory _factory;
    private readonly CurrencyOptions _currency;

    public BeverageMenu(IBeverageFactory factory, CurrencyOptions currency)
    {
        _factory = factory;
        _currency = currency;
    }

    public IBeverage ChooseBase()
    {
        while (true)
        {
            Console.WriteLine("Choose base beverage: ");
            Console.WriteLine($" 1) Espresso ({_currency.Symbol}2.50)");
            Console.WriteLine($" 2) Tea ({_currency.Symbol}2.00)");
            Console.WriteLine($" 3) Hot Chocolate ({_currency.Symbol}3.00)");
            Console.WriteLine();

            Console.WriteLine("Select: ");
            var input = Console.ReadLine().Trim() ?? string.Empty;
            switch (input)
            {
                case "1": input = "espresso"; break;
                case "2": input = "tea"; break;
                case "3": input = "hot chocolate"; break;
                default: break;
            }

            try
            {
                return _factory.Create(input);
            }
            catch
            {
                Console.WriteLine("Invalid choice. Please try again.");
            }
        }
    }
}