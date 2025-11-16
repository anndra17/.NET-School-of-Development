using CafeConsole.App.Abstractions;
using CafeConsole.App.Events;
using CafeConsole.App.Selector;
using CafeConsole.App.Services;
using CafeConsole.CLI.Menus;
using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Pricing;
using CafeConsole.Infrastructure.Composition;
using CafeConsole.Infrastructure.Factories;
using CafeConsole.Infrastructure.Observers;

namespace CafeConsole.CLI.Composition;

public static class Bootstrapper
{
    public static Task RunAsync()
    {
        var currency = new CurrencyOptions { Symbol = "$" };
        var logger = new ConsoleOrderLogger(currency);
        var analytics = new InMemoryOrderAnalytics();

        var publisher = new SimpleOrderEventPublisher();
        publisher.Register(logger);
        publisher.Register(analytics);

        IBeverageFactory beverageFactory = new BeverageFactory();
        IBeverageAssembler beverageAssembler = new BeverageAssembler();

        IPricingStrategy[] strategies =
        {
            new RegularPricing(),
            new HappyHourPricing()
        };
        IPricingStrategySelector selector = new PricingStrategySelector(strategies);

        var orderService = new OrderService(publisher, selector);

        var beverageMenu = new BeverageMenu(beverageFactory, currency);
        var addOnMenu = new AddOnMenu(beverageAssembler, currency);
        var pricingMenu = new PricingMenu();
        var receiptMenu = new ReceiptMenu(orderService);
        
        var menu = new Menu(beverageMenu, addOnMenu, pricingMenu, receiptMenu, currency, analytics);
        menu.Run();


        return Task.CompletedTask;
    }
}
