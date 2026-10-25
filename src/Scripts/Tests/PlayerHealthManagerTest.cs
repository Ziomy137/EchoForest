using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class PlayerHealthManagerTest
{
    [Test]
    public void Constructor_StartsAtMaxHealth()
    {
        var manager = new PlayerHealthManager(100f, new EventBus());

        Assert.Multiple(() =>
        {
            Assert.That(manager.MaxHealth, Is.EqualTo(100f));
            Assert.That(manager.CurrentHealth, Is.EqualTo(100f));
            Assert.That(manager.IsDead, Is.False);
        });
    }

    [Test]
    public void TakeDamage_ReducesHealthAndPublishesEvent()
    {
        var eventBus = new EventBus();
        PlayerHealthChangedEvent? publishedEvent = null;
        eventBus.Subscribe<PlayerHealthChangedEvent>(gameEvent => publishedEvent = gameEvent);
        var manager = new PlayerHealthManager(100f, eventBus);

        manager.TakeDamage(30f);

        Assert.Multiple(() =>
        {
            Assert.That(manager.CurrentHealth, Is.EqualTo(70f));
            Assert.That(publishedEvent, Is.EqualTo(new PlayerHealthChangedEvent(70f, 100f)));
        });
    }

    [Test]
    public void TakeDamage_ClampsToZeroAndPublishesDeathOnce()
    {
        var eventBus = new EventBus();
        var diedCount = 0;
        eventBus.Subscribe<PlayerDiedEvent>(_ => diedCount++);
        var manager = new PlayerHealthManager(50f, eventBus);

        manager.TakeDamage(80f);
        manager.TakeDamage(10f);

        Assert.Multiple(() =>
        {
            Assert.That(manager.CurrentHealth, Is.EqualTo(0f));
            Assert.That(manager.IsDead, Is.True);
            Assert.That(diedCount, Is.EqualTo(1));
        });
    }

    [Test]
    public void Heal_ClampsToMaxHealthAndPublishesEvent()
    {
        var eventBus = new EventBus();
        PlayerHealthChangedEvent? publishedEvent = null;
        eventBus.Subscribe<PlayerHealthChangedEvent>(gameEvent => publishedEvent = gameEvent);
        var manager = new PlayerHealthManager(100f, eventBus);
        manager.TakeDamage(20f);

        manager.Heal(50f);

        Assert.Multiple(() =>
        {
            Assert.That(manager.CurrentHealth, Is.EqualTo(100f));
            Assert.That(publishedEvent, Is.EqualTo(new PlayerHealthChangedEvent(100f, 100f)));
            Assert.That(manager.IsDead, Is.False);
        });
    }

    [Test]
    public void Heal_TinctureSmall_RestoresExactlyTwentyFiveHealth()
    {
        var manager = new PlayerHealthManager(100f, new EventBus());
        manager.TakeDamage(40f);

        manager.Heal(25f);

        Assert.That(manager.CurrentHealth, Is.EqualTo(85f));
    }

    [TestCase(0f)]
    [TestCase(-1f)]
    public void TakeDamage_NonPositiveAmount_Throws(float amount) =>
        Assert.Throws<System.ArgumentOutOfRangeException>(() => new PlayerHealthManager(100f, new EventBus()).TakeDamage(amount));

    [TestCase(0f)]
    [TestCase(-1f)]
    public void Heal_NonPositiveAmount_Throws(float amount) =>
        Assert.Throws<System.ArgumentOutOfRangeException>(() => new PlayerHealthManager(100f, new EventBus()).Heal(amount));

    [Test]
    public void Constructor_NonPositiveMaxHealth_Throws() =>
        Assert.Throws<System.ArgumentOutOfRangeException>(() => new PlayerHealthManager(0f, new EventBus()));
}