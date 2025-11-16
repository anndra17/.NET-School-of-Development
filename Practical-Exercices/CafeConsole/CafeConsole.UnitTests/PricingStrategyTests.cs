using CafeConsole.Domain.Models.Pricing;
using Xunit;

namespace CafeConsole.Tests;

public class PricingStrategyTests
{
    [Fact]
    public void RegularPricing_Should_Not_Apply_Discount()
    {
        var strategy = new RegularPricing();

        var result = strategy.Apply(10.00m);

        Assert.Equal(10.00m, result);
        Assert.Equal(PricingPolicy.Regular, strategy.Policy);
    }

    [Fact]
    public void HappyHourPricing_Should_Apply_20Percent_Discount()
    {
        var strategy = new HappyHourPricing();

        var result = strategy.Apply(10.00m);

        Assert.Equal(8.00m, result);
        Assert.Equal(PricingPolicy.HappyHour, strategy.Policy);
    }
}
