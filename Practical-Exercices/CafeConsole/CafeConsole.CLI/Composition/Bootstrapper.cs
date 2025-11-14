using CafeConsole.App.Abstractions;
using CafeConsole.App.Events;
using CafeConsole.App.Services;
using CafeConsole.CLI.Menus;
using CafeConsole.Infrastructure.Composition;
using CafeConsole.Infrastructure.Factories;
using CafeConsole.Infrastructure.Observers;

namespace CafeConsole.CLI.Composition;

public static class Bootstrapper
{
    public static Task RunAsync()
    {
        //Infrastructure
        var publisher = new SimpleOrderEventPublisher();
        var logger = new ConsoleOrderLogger();
        var analytics = new InMemoryOrderAnalytics();
        publisher.Register(logger);
        publisher.Register(analytics);

        //Factories & services
        IBeverageFactory beverageFactory = new BeverageFactory();
        IBeverageAssembler beverageAssembler = new BeverageAssembler();
        var orderService = new OrderService(publisher);
        //var printer = new ReceiptPrinter(console, currencySymbol: "$");

        //Menus
        var beverageMenu = new BeverageMenu(beverageFactory);
        var addOnMenu = new AddOnMenu(beverageAssembler);
        var pricingMenu = new PricingMenu();
        var receiptMenu = new ReceiptMenu(orderService);
        var menu = new Menu(beverageMenu, addOnMenu, pricingMenu, receiptMenu);
        menu.Run();

        // Flow controller (application layer orchestrator for UI)
        //var controller = new OrderController(console, orderService, beverageMenu, addOnMenu, pricingMenu, printer, analytics);

        //controller.Run();
        return Task.CompletedTask;
    }
}
