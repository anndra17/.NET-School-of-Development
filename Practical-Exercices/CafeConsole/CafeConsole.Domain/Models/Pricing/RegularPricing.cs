using CafeConsole.Domain.Abstractions;

namespace CafeConsole.Domain.Models.Pricing;

public class RegularPricing : IPricingStrategy
{
    public PricingPolicy Name => PricingPolicy.Regular;

    public decimal Apply(decimal subtotal)
    {
        return subtotal;
    }
}
