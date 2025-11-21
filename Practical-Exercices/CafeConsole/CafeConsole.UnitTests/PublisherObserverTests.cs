using CafeConsole.App.Events;
using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Events;
using CafeConsole.Domain.Models.Pricing;
using Moq;

namespace CafeConsole.Tests;

public class PublisherObserverTests
{
    [Fact]
    public void Publish_Notifies_Subscriber_Once_With_Matching_Total()
    {
        var publisher = new SimpleOrderEventPublisher();
        var subscriberMock = new Mock<IOrderEventSubscriber>(MockBehavior.Strict);
        publisher.Register(subscriberMock.Object);
        var evt = new OrderPlaced(
            Id: Guid.NewGuid(),
            At: DateTimeOffset.Parse("2025-10-25T10:22:13+02:00"),
            Description: "espresso, milk, extra shot",
            Subtotal: 10.00m,
            Policy: PricingPolicy.HappyHour,
            Total: 8.00m
        );
        subscriberMock
            .Setup(s => s.On(It.Is<OrderPlaced>(e =>
                e.Total == evt.Total &&
                e.Subtotal == evt.Subtotal &&
                e.Id == evt.Id)))
            .Verifiable();

        publisher.Publish(evt);

        subscriberMock.Verify(s => s.On(It.IsAny<OrderPlaced>()), Times.Once);
        subscriberMock.Verify(); 
    }

    [Fact]
    public void Publish_Notifies_All_Subscribers_Exactly_Once()
    {
        var publisher = new SimpleOrderEventPublisher();

        var sub1 = new Mock<IOrderEventSubscriber>(MockBehavior.Strict);
        var sub2 = new Mock<IOrderEventSubscriber>(MockBehavior.Strict);
        publisher.Register(sub1.Object);
        publisher.Register(sub2.Object);

        var evt = new OrderPlaced(
            Id: Guid.NewGuid(),
            At: DateTimeOffset.UtcNow,
            Description: "tea",
            Subtotal: 2.00m,
            Policy: PricingPolicy.Regular,
            Total: 2.00m
        );

        sub1.Setup(s => s.On(It.Is<OrderPlaced>(e => e.Total == 2.00m)));
        sub2.Setup(s => s.On(It.Is<OrderPlaced>(e => e.Total == 2.00m)));

        publisher.Publish(evt);

        sub1.Verify(s => s.On(It.IsAny<OrderPlaced>()), Times.Once);
        sub2.Verify(s => s.On(It.IsAny<OrderPlaced>()), Times.Once);
        sub1.Verify();
        sub2.Verify();
    }
}
