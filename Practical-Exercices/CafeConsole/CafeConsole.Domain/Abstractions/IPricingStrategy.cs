using CafeConsole.Domain.Models.Pricing;

namespace CafeConsole.Domain.Abstractions;

public interface IPricingStrategy
{
    PricingPolicy Name { get; }
    decimal Apply(decimal subtotal);
}
