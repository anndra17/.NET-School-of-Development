using CafeConsole.App.Services;
using CafeConsole.CLI.Menus.Abstractions;
using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Events;
using CafeConsole.Infrastructure.Observers;

namespace CafeConsole.CLI.Menus;

public class ReceiptMenu : IReceiptMenu
{
    private readonly OrderService _orderService;

    public ReceiptMenu(OrderService orderService)
    {  
        _orderService = orderService;
    }

    public void Print(IBeverage beverage, IPricingStrategy strategy)
    {
        _orderService.PlaceOrder(beverage, strategy);
    }
}
