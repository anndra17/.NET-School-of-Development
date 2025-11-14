namespace CafeConsole.Domain.Models.Events;

public readonly record struct OrderPlaced(
    Guid Id,
    DateTimeOffset At,
    string Description,
    decimal Subtotal,
    decimal Total
    );