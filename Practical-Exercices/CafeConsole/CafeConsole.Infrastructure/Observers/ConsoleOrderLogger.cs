using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Events;
using CafeConsole.Domain.Models.Pricing;
using System.Globalization;

namespace CafeConsole.Infrastructure.Observers;

public sealed class ConsoleOrderLogger : IOrderEventSubscriber
{
    private readonly CurrencyOptions _currency;
    public ConsoleOrderLogger(CurrencyOptions currency) => _currency = currency;

    public void On(OrderPlaced orderEvent)
    {
        Console.WriteLine();
        Console.WriteLine($"""
            Order {orderEvent.Id} @ {orderEvent.At}
            Items: {orderEvent.Description}
            Subtotal: {_currency.Symbol}{orderEvent.Subtotal:0.00}
            Pricing: {FormatPolicy(orderEvent.Policy)}
            Total: {_currency.Symbol}{orderEvent.Total:0.00}
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