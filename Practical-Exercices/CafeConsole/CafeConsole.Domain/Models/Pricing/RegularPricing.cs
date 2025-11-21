using CafeConsole.Domain.Abstractions;

namespace CafeConsole.Domain.Models.Pricing;

public sealed class RegularPricing : IPricingStrategy
{
    public PricingPolicy Policy => PricingPolicy.Regular;

    public decimal Apply(decimal subtotal)
    {
        return subtotal;
    }
}
