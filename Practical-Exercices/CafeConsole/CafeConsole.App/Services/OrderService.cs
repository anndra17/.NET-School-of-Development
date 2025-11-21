using CafeConsole.App.Abstractions;
using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Events;
using CafeConsole.Domain.Models.Pricing;

namespace CafeConsole.App.Services;

public sealed class OrderService
{
    private readonly IOrderEventPublisher _publisher;
    private readonly IPricingStrategySelector _selector;

    public OrderService(IOrderEventPublisher publisher, IPricingStrategySelector selector)
    {
        _publisher = publisher;
        _selector = selector;
    }

    public void PlaceOrder(IBeverage beverage, PricingPolicy policy)
    {
        var subtotal = beverage.Cost();
        var strategy = _selector.Resolve(policy);
        var total = strategy.Apply(subtotal);

        var orderEvent = new OrderPlaced(
            Id: Guid.NewGuid(),
            At: DateTimeOffset.Now,
            Description: beverage.Describe(),
            Subtotal: subtotal,
            Policy:policy,
            Total: total
        );

        _publisher.Publish(orderEvent);
    }
}