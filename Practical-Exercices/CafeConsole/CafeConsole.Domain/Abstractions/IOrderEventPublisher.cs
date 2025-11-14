using CafeConsole.Domain.Models.Events;

namespace CafeConsole.Domain.Abstractions;

public interface IOrderEventPublisher
{
    void Publish(OrderPlaced orderPlacedEvent);
}
