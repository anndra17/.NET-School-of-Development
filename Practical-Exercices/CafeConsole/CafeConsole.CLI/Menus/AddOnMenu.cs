using CafeConsole.App.Abstractions;
using CafeConsole.CLI.Menus.Abstractions;
using CafeConsole.Domain.Abstractions;
using CafeConsole.Infrastructure.Observers;

namespace CafeConsole.CLI.Menus;

public class AddOnMenu : IAddOnMenu
{
    private readonly IBeverageAssembler _assembler;
    private readonly CurrencyOptions _currency;

    public AddOnMenu(IBeverageAssembler assembler, CurrencyOptions currency)
    {
        _assembler = assembler;
        _currency = currency;
    }

    public IBeverage ChooseAddOns(IBeverage baseBeverage)
    {
        var currentBeverage = baseBeverage;

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine($"""
                Add-ons: 
                1) Milk (+ {_currency.Symbol}0.40) 
                2) Syrup (+ {_currency.Symbol}0.50) 
                3) Extra shot (+ {_currency.Symbol}0.80) 
                0) Done
                """);
            Console.WriteLine();

            Console.WriteLine("Select: ");
            var input = Console.ReadLine().Trim();

            switch (input) 
            {
                case "1": 
                    currentBeverage = _assembler.Assemble(currentBeverage, new[] { ("milk", (string?) null) });
                    break;
                case "2":
                    Console.WriteLine("Insert flavor: ");
                    string flavor = Console.ReadLine().Trim();
                    currentBeverage = _assembler.Assemble(currentBeverage,new[] { ("syrup", flavor) });
                    break;
                case "3":
                    currentBeverage = _assembler.Assemble(currentBeverage, new[] { ("extra shot", (string?)null) });
                    break;
                case "0":
                    return currentBeverage;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }
}