using CafeConsole.App.Abstractions;
using CafeConsole.CLI.Menus.Abstractions;
using CafeConsole.Domain.Abstractions;

namespace CafeConsole.CLI.Menus;

internal class BeverageMenu : IBeverageMenu
{
    private readonly IBeverageFactory _factory;

    public BeverageMenu(IBeverageFactory factory)
    {
        _factory = factory;
    }

    public IBeverage ChooseBase()
    {
        while (true)
        {
            Console.WriteLine("Choose base beverage: ");
            Console.WriteLine("1) Espresso ($2.50)");
            Console.WriteLine("2) Tea ($2.00)");
            Console.WriteLine("3) Hot Chocolate ($3.00)");
            Console.WriteLine();

            Console.WriteLine("Select: ");
            var input = Console.ReadLine().Trim();
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