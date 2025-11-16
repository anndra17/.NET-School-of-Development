using CafeConsole.App.Events;
using CafeConsole.Domain.Models.Events;
using CafeConsole.Domain.Models.Pricing;
using CafeConsole.Infrastructure.Observers;

namespace CafeConsole.Tests;

public class OrderAnalyticsTests
{
    [Fact]
    public void Analytics_Should_Accumulate_Order_Count_And_Revenue()
    {
        var analytics = new InMemoryOrderAnalytics();
        var publisher = new SimpleOrderEventPublisher();
        publisher.Register(analytics);

        var order1 = new OrderPlaced(
            Id: Guid.NewGuid(),
            At: DateTimeOffset.UtcNow,
            Description: "espresso",
            Subtotal: 3.50m,
            Policy: PricingPolicy.Regular,
            Total: 3.50m
        );

        var order2 = new OrderPlaced(
            Id: Guid.NewGuid(),
            At: DateTimeOffset.UtcNow,
            Description: "tea",
            Subtotal: 2.00m,
            Policy: PricingPolicy.Regular,
            Total: 2.00m
        );

        publisher.Publish(order1);
        publisher.Publish(order2);

        Assert.Equal(2, analytics.OrdersCount);
        Assert.Equal(5.50m, analytics.TotalRevenue);
    }
}
