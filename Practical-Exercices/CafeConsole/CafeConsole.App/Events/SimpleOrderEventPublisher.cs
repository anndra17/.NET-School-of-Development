using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Events;

namespace CafeConsole.App.Events;

public class SimpleOrderEventPublisher : IOrderEventPublisher
{
    private readonly List<IOrderEventSubscriber> _subscribers = new();

    public SimpleOrderEventPublisher()
    {
    }

    public SimpleOrderEventPublisher(IEnumerable<IOrderEventSubscriber> subscribers)
    {
        if (subscribers is not null)
            _subscribers.AddRange(subscribers);
    }

    public void Publish(OrderPlaced orderEvent)
    {
        foreach (var subscriber in _subscribers) 
        {
            subscriber.On(orderEvent);
        }
    }

    public void Register(IOrderEventSubscriber subscriber)
    {
        if (subscriber is null)
            throw new ArgumentNullException(nameof(subscriber));
        _subscribers.Add(subscriber);
    }
}