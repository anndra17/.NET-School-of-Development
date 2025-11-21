using CafeConsole.Domain.Abstractions;

namespace CafeConsole.Domain.Models.Pricing;

public sealed class HappyHourPricing : IPricingStrategy
{
    private readonly decimal _discount = 0.2m;
    public PricingPolicy Policy => PricingPolicy.HappyHour;

    public decimal Apply(decimal subtotal)
    {
        var discount = subtotal * _discount;

        return subtotal - discount;
    }
}