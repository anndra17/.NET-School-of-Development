using CafeConsole.Domain.Models.Pricing;

namespace CafeConsole.Domain.Models.Events;

public readonly record struct OrderPlaced(
    Guid Id,
    DateTimeOffset At,
    string Description,
    decimal Subtotal,
    PricingPolicy Policy,
    decimal Total
    );