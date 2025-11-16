using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Events;
using CafeConsole.Domain.Models.Pricing;
using System.Globalization;

namespace CafeConsole.Infrastructure.Observers;

public class ConsoleOrderLogger : IOrderEventSubscriber
{
    public void On(OrderPlaced orderEvent)
    {
        Console.WriteLine();
        Console.WriteLine($"""
            Order {orderEvent.Id} @ {orderEvent.At}
            Items: {orderEvent.Description}
            Subtotal: {orderEvent.Subtotal.ToString("C")}
            Pricing: {FormatPolicy(orderEvent.Policy)}
            Total: {orderEvent.Total.ToString("C")};
            """);
    }

    private string FormatPolicy(PricingPolicy policy) 
    =>
        policy switch
        {
            PricingPolicy.Regular => "Regular",
            PricingPolicy.HappyHour => "Happy Hour (-20%)",
            _ => policy.ToString()
        };
    
}