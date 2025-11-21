using CafeConsole.CLI.Menus.Abstractions;
using CafeConsole.Infrastructure.Observers;


namespace CafeConsole.CLI;

public class Menu
{
    private readonly IBeverageMenu _beverageMenu;
    private readonly IAddOnMenu _addOnMenu;
    private readonly IPricingMenu _pricingMenu;
    private readonly IReceiptMenu _receiptMenu;
    private readonly CurrencyOptions _currency;
    private readonly dynamic _analytics;

    public Menu(IBeverageMenu beverageMenu, IAddOnMenu addOnMenu, IPricingMenu pricingMenu, IReceiptMenu receiptMenu, CurrencyOptions currency, object analytics)
    {
        _beverageMenu = beverageMenu;
        _addOnMenu = addOnMenu;
        _pricingMenu = pricingMenu;
        _receiptMenu = receiptMenu;
        _currency = currency;
        _analytics = analytics;
    }

    public void Run()
    {
        do
        {
            PrintTitle();
            AddMenus();
        }
        while (AnotherOrder());
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
        var policy = _pricingMenu.ChoosePricing();
        _receiptMenu.Print(beverage, policy);
    }

    private bool AnotherOrder()
    {
        Console.WriteLine();
        Console.WriteLine($"[Analytics] Orders: {_analytics.OrdersCount}, Revenue: {_currency.Symbol}{_analytics.TotalRevenue:0.00}");
        Console.WriteLine();
        Console.WriteLine("1) New order 0) Exit");
        Console.WriteLine();

        Console.Write("Select: ");
        var again = Console.ReadLine().Trim() ?? string.Empty;

        if (again == "0") return false;
        Console.WriteLine();
        return true;
    }
}