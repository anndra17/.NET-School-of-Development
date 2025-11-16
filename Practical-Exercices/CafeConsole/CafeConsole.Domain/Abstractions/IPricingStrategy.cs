using CafeConsole.Domain.Models.Pricing;

namespace CafeConsole.Domain.Abstractions;

public interface IPricingStrategy
{
    PricingPolicy Policy { get; }

    decimal Apply(decimal subtotal);
}
