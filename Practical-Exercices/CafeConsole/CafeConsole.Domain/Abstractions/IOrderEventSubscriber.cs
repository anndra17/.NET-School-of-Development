using CafeConsole.Domain.Models.Events;

namespace CafeConsole.Domain.Abstractions;

public interface IOrderEventSubscriber
{
    void On(OrderPlaced orderPlacedEvent);
}
