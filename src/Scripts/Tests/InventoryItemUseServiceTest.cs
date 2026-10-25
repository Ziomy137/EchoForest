using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class InventoryItemUseServiceTest
{
    [Test]
    public void UseSmallTincture_ConsumesOneAndRestoresTwentyFiveHealth()
    {
        var eventBus = new EventBus();
        var inventory = CreateInventory(eventBus);
        inventory.AddItem("item_tincture_small", 2);
        var health = new PlayerHealthManager(100f, eventBus);
        health.TakeDamage(40f);
        using var useService = new InventoryItemUseService(inventory, health, eventBus);

        eventBus.Publish(new ItemUseRequestedEvent("item_tincture_small"));

        Assert.Multiple(() =>
        {
            Assert.That(health.CurrentHealth, Is.EqualTo(85f));
            Assert.That(inventory.GetItem("item_tincture_small")?.Quantity, Is.EqualTo(1));
        });
    }

    [Test]
    public void UseTincture_HealingIsCappedAtMaxHealth()
    {
        var eventBus = new EventBus();
        var inventory = CreateInventory(eventBus);
        inventory.AddItem("item_tincture_large");
        var health = new PlayerHealthManager(100f, eventBus);
        health.TakeDamage(10f);
        using var useService = new InventoryItemUseService(inventory, health, eventBus);

        eventBus.Publish(new ItemUseRequestedEvent("item_tincture_large"));

        Assert.That(health.CurrentHealth, Is.EqualTo(100f));
    }

    [Test]
    public void UseTincture_AtFullHealthDoesNotConsumeItem()
    {
        var eventBus = new EventBus();
        var inventory = CreateInventory(eventBus);
        inventory.AddItem("item_tincture_small");
        var health = new PlayerHealthManager(100f, eventBus);
        using var useService = new InventoryItemUseService(inventory, health, eventBus);

        eventBus.Publish(new ItemUseRequestedEvent("item_tincture_small"));

        Assert.That(inventory.HasItem("item_tincture_small"), Is.True);
    }

    [Test]
    public void UseWeapon_DoesNotConsumeOrChangeHealth()
    {
        var eventBus = new EventBus();
        var inventory = CreateInventory(eventBus);
        inventory.AddItem("item_sword_basic");
        var health = new PlayerHealthManager(100f, eventBus);
        health.TakeDamage(10f);
        using var useService = new InventoryItemUseService(inventory, health, eventBus);

        eventBus.Publish(new ItemUseRequestedEvent("item_sword_basic"));

        Assert.Multiple(() =>
        {
            Assert.That(inventory.HasItem("item_sword_basic"), Is.True);
            Assert.That(health.CurrentHealth, Is.EqualTo(90f));
        });
    }

    [Test]
    public void Dispose_UnsubscribesFromUseRequests()
    {
        var eventBus = new EventBus();
        var inventory = CreateInventory(eventBus);
        inventory.AddItem("item_tincture_small");
        var health = new PlayerHealthManager(100f, eventBus);
        health.TakeDamage(10f);
        var useService = new InventoryItemUseService(inventory, health, eventBus);
        useService.Dispose();

        eventBus.Publish(new ItemUseRequestedEvent("item_tincture_small"));

        Assert.Multiple(() =>
        {
            Assert.That(health.CurrentHealth, Is.EqualTo(90f));
            Assert.That(inventory.HasItem("item_tincture_small"), Is.True);
        });
    }

    private static InventoryManager CreateInventory(EventBus eventBus) => new(
        Constants.MaxInventorySlots,
        new MockItemDatabase(),
        eventBus);
}