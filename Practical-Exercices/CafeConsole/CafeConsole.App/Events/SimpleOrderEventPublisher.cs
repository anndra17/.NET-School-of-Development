using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Events;

namespace CafeConsole.App.Events;

public class SimpleOrderEventPublisher : IOrderEventPublisher
{
    private readonly IEnumerable<IOrderEventSubscriber> _subscribers;
    
    public SimpleOrderEventPublisher(IEnumerable<IOrderEventSubscriber> subscribers)
    {
        _subscribers = subscribers;
    }

    public void Publish(OrderPlaced orderEvent)
    {
        foreach (var subscriber in _subscribers) 
        {
            subscriber.On(orderEvent);
        }
    }
}
