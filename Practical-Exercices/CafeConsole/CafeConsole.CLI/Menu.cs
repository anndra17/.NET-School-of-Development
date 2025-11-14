using CafeConsole.CLI.Menus.Abstractions;

namespace CafeConsole.CLI;

public class Menu
{
    private readonly IBeverageMenu _beverageMenu;
    private readonly IAddOnMenu _addOnMenu;
    private readonly IPricingMenu _pricingMenu;
    private readonly IReceiptMenu _receiptMenu;

    public Menu(IBeverageMenu beverageMenu, IAddOnMenu addOnMenu, IPricingMenu pricingMenu, IReceiptMenu receiptMenu)
    {
        _beverageMenu = beverageMenu;
        _addOnMenu = addOnMenu;
        _pricingMenu = pricingMenu;
        _receiptMenu = receiptMenu;
    }

    public void Run()
    {
        while (true)
        {
            PrintTitle();
            AddMenus();
            if (!AnotherOrder()) break;
        }
    }

    private void PrintTitle()
    {
        Console.WriteLine("---------------------- Cafe Console App ----------------------");
        Console.WriteLine();
    }

    private void AddMenus()
    {
        var beverage = _beverageMenu.ChooseBase();
        beverage = _addOnMenu.ChooseAddOns(beverage);
        var pricing = _pricingMenu.ChoosePricing();
        _receiptMenu.Print(beverage, pricing);
    }

    private bool AnotherOrder()
    {
        Console.WriteLine();
        Console.WriteLine("1) New order 0) Exit");
        Console.WriteLine();

        Console.Write("Select: ");
        var again = Console.ReadLine().Trim();

        if (again == "0") return false;
        Console.WriteLine();
        return true;
    }
}