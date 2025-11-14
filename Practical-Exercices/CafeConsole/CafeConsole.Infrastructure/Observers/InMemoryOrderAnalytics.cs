using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Events;

namespace CafeConsole.Infrastructure.Observers;

public class InMemoryOrderAnalytics : IOrderEventSubscriber
{
    public int OrdersCount { get; private set; }
    public decimal TotalRevenue { get; private set; }

    public void On(OrderPlaced orderEvent)
    {
        OrdersCount++;
        TotalRevenue += orderEvent.Total;
    }
}
