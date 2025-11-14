using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Events;

namespace CafeConsole.App.Services;

public class OrderService
{
    private readonly IOrderEventPublisher _publisher;

    public OrderService(IOrderEventPublisher publisher)
    {
        _publisher = publisher;
    }

    public void PlaceOrder(IBeverage beverage, IPricingStrategy pricing)
    {
        var subtotal = beverage.Cost();
        var total = pricing.Apply(subtotal);

        var orderEvent = new OrderPlaced
        {
            Id = Guid.NewGuid(),
            At = DateTimeOffset.Now,
            Description = beverage.Describe(),
            Subtotal = subtotal,
            Total = total
        };

        _publisher.Publish(orderEvent); 
    }
}