using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Events;

namespace CafeConsole.Infrastructure.Observers;

public class ConsoleOrderLogger : IOrderEventSubscriber
{
    public void On(OrderPlaced orderEvent)
    {
        Console.WriteLine($"""
            Order {orderEvent.Id} @ {orderEvent.At}
            Items: {orderEvent.Description}
            Subtotal: {orderEvent.Subtotal.ToString("C")}
            Pricing: TODO
            Total: {orderEvent.Total.ToString("C")}";
            """);
    }

}
