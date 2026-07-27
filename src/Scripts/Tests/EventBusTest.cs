using System;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class EventBusTest
{
    [Test]
    public void Subscribe_ReceivesPublishedEvent()
    {
        var bus = new EventBus();
        var received = 0;
        bus.Subscribe<QuestStartedEvent>(_ => received++);

        bus.Publish(new QuestStartedEvent("q_kidnapped"));

        Assert.That(received, Is.EqualTo(1));
    }

    [Test]
    public void MultipleSubscribers_AllReceiveEvent()
    {
        var bus = new EventBus();
        var first = 0;
        var second = 0;
        bus.Subscribe<QuestStartedEvent>(_ => first++);
        bus.Subscribe<QuestStartedEvent>(_ => second++);

        bus.Publish(new QuestStartedEvent("q_seek_mage"));

        Assert.Multiple(() =>
        {
            Assert.That(first, Is.EqualTo(1));
            Assert.That(second, Is.EqualTo(1));
        });
    }

    [Test]
    public void Unsubscribe_StopsDelivery()
    {
        var bus = new EventBus();
        var received = 0;
        Action<QuestStartedEvent> handler = _ => received++;
        bus.Subscribe(handler);
        bus.Unsubscribe(handler);

        bus.Publish(new QuestStartedEvent("q_kidnapped"));

        Assert.That(received, Is.Zero);
    }

    [Test]
    public void Clear_RemovesAllSubscriptions()
    {
        var bus = new EventBus();
        var received = 0;
        bus.Subscribe<QuestStartedEvent>(_ => received++);
        bus.Clear();

        bus.Publish(new QuestStartedEvent("q_kidnapped"));

        Assert.That(received, Is.Zero);
    }

    [Test]
    public void Publish_DifferentEventType_DoesNotNotifySubscriber()
    {
        var bus = new EventBus();
        var received = false;
        bus.Subscribe<QuestStartedEvent>(_ => received = true);

        bus.Publish(new PlayerHealthChangedEvent(75f, 100f));

        Assert.That(received, Is.False);
    }

    [Test]
    public void Publish_WithoutSubscribers_DoesNotThrow()
    {
        var bus = new EventBus();

        Assert.DoesNotThrow(() => bus.Publish(new PlayerDiedEvent()));
    }

    [Test]
    public void Publish_AllowsSubscriberToUnsubscribeDuringDelivery()
    {
        var bus = new EventBus();
        var received = 0;
        Action<QuestStartedEvent>? handler = null;
        handler = _ =>
        {
            received++;
            bus.Unsubscribe(handler!);
        };
        bus.Subscribe(handler);

        bus.Publish(new QuestStartedEvent("q_kidnapped"));
        bus.Publish(new QuestStartedEvent("q_kidnapped"));

        Assert.That(received, Is.EqualTo(1));
    }
}