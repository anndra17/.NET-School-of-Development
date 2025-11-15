using CafeConsole.Domain.Abstractions;

namespace CafeConsole.Domain.Models.Pricing;

public class HappyHourPricing : IPricingStrategy
{
    private readonly decimal _discount = 0.2m;
    public PricingPolicy Name => PricingPolicy.HappyHour;

    public decimal Apply(decimal subtotal)
    {
        var discount = subtotal * _discount;

        return subtotal - discount;
    }
}