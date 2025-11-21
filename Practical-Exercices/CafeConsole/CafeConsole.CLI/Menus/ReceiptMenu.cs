using CafeConsole.App.Services;
using CafeConsole.CLI.Menus.Abstractions;
using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Pricing;

namespace CafeConsole.CLI.Menus;

public class ReceiptMenu : IReceiptMenu
{
    private readonly OrderService _orderService;

    public ReceiptMenu(OrderService orderService)
    {  
        _orderService = orderService;
    }

    public void Print(IBeverage beverage, PricingPolicy policy)
    {
        _orderService.PlaceOrder(beverage, policy);
    }
}
