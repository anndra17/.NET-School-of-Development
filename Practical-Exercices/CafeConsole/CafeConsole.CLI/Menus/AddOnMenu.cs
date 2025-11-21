using CafeConsole.App.Abstractions;
using CafeConsole.App.Dtos;
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
        var selections = new List<AddOnDto>();

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
            var input = (Console.ReadLine() ?? string.Empty).Trim();

            switch (input) 
            {
                case "1":
                    selections.Add(new AddOnDto("milk"));
                    break;
                case "2":
                    Console.WriteLine("Insert flavor: ");
                    var flavor = (Console.ReadLine() ?? string.Empty).Trim();
                    selections.Add(new AddOnDto("milk", flavor));
                    break;
                case "3":
                    selections.Add(new AddOnDto("extra shot"));
                    break;
                case "0":
                    return _assembler.Assemble(baseBeverage, selections);
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }
}