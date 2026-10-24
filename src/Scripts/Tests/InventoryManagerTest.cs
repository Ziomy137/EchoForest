using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class InventoryManagerTest
{
    private EventBus _eventBus = null!;
    private MockItemDatabase _itemDatabase = null!;

    [SetUp]
    public void SetUp()
    {
        _eventBus = new EventBus();
        _itemDatabase = CreateItemDatabase();
    }

    [Test]
    public void Constructor_ImplementsInventoryInterface()
    {
        var inventory = CreateInventory();

        Assert.That(inventory, Is.InstanceOf<IInventoryManager>());
        Assert.That(inventory.MaxSlots, Is.EqualTo(Constants.MaxInventorySlots));
    }

    [Test]
    public void AddItem_SucceedsWhenSpaceAvailable_AndPublishesPickupEvent()
    {
        ItemPickedUpEvent? publishedEvent = null;
        _eventBus.Subscribe<ItemPickedUpEvent>(gameEvent => publishedEvent = gameEvent);
        var inventory = CreateInventory();

        var result = inventory.AddItem("item_sword_basic");

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(inventory.HasItem("item_sword_basic"), Is.True);
            Assert.That(publishedEvent, Is.EqualTo(new ItemPickedUpEvent("item_sword_basic", 1)));
        });
    }

    [Test]
    public void Changed_EventFiresOnceAfterSuccessfulMutation()
    {
        var inventory = CreateInventory();
        var changeCount = 0;
        inventory.Changed += () => changeCount++;

        inventory.AddItem("item_sword_basic");
        inventory.RemoveItem("item_sword_basic");

        Assert.That(changeCount, Is.EqualTo(2));
    }

    [Test]
    public void AddItem_FailsWhenFullWithoutChangingInventory()
    {
        var inventory = CreateInventory(maxSlots: 1);
        inventory.AddItem("item_sword_basic");

        var result = inventory.AddItem("item_bow_basic");

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(inventory.GetAllItems().Select(item => item.Definition.Id), Is.EqualTo(new[] { "item_sword_basic" }));
        });
    }

    [Test]
    public void AddItem_StacksExistingItem()
    {
        var inventory = CreateInventory(maxSlots: 5);
        inventory.AddItem("item_tincture_small");

        var result = inventory.AddItem("item_tincture_small", 2);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(inventory.GetItem("item_tincture_small")?.Quantity, Is.EqualTo(3));
            Assert.That(inventory.GetAllItems(), Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void AddItem_ExceedingStackLimit_UsesAdditionalSlot()
    {
        var inventory = CreateInventory(maxSlots: 2);

        var result = inventory.AddItem("item_tincture_small", 12);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(inventory.GetAllItems().Select(item => item.Quantity), Is.EqualTo(new[] { 10, 2 }));
            Assert.That(inventory.HasItem("item_tincture_small", 12), Is.True);
        });
    }

    [Test]
    public void AddItem_InsufficientSlotsForAllStacks_IsAtomic()
    {
        var inventory = CreateInventory(maxSlots: 1);
        inventory.AddItem("item_tincture_small", 5);

        var result = inventory.AddItem("item_tincture_small", 8);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(inventory.GetItem("item_tincture_small")?.Quantity, Is.EqualTo(5));
        });
    }

    [Test]
    public void AddItem_VeryLargeQuantity_ReturnsFalseWithoutOverflow()
    {
        var inventory = CreateInventory(maxSlots: int.MaxValue);
        inventory.AddItem("item_sword_basic");

        var result = inventory.AddItem("item_bow_basic", int.MaxValue);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(inventory.GetOccupiedSlots(), Is.EqualTo(1));
        });
    }

    [Test]
    public void RemoveItem_RemovesQuantityAndPublishesEvent()
    {
        var inventory = CreateInventory(maxSlots: 2);
        inventory.AddItem("item_tincture_small", 12);
        ItemRemovedEvent? publishedEvent = null;
        _eventBus.Subscribe<ItemRemovedEvent>(gameEvent => publishedEvent = gameEvent);

        var result = inventory.RemoveItem("item_tincture_small", 3);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(inventory.HasItem("item_tincture_small", 9), Is.True);
            Assert.That(publishedEvent, Is.EqualTo(new ItemRemovedEvent("item_tincture_small", 3)));
        });
    }

    [Test]
    public void RemoveItem_MoreThanAvailable_ReturnsFalseWithoutChangingInventory()
    {
        var inventory = CreateInventory();
        inventory.AddItem("item_tincture_small", 2);

        var result = inventory.RemoveItem("item_tincture_small", 3);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(inventory.GetItem("item_tincture_small")?.Quantity, Is.EqualTo(2));
        });
    }

    [Test]
    public void RemoveItem_ConsumesAcrossStacksAndFreesEmptySlots()
    {
        var inventory = CreateInventory(maxSlots: 2);
        inventory.AddItem("item_tincture_small", 12);

        var result = inventory.RemoveItem("item_tincture_small", 11);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(inventory.GetAllItems().Select(item => item.Quantity), Is.EqualTo(new[] { 1 }));
            Assert.That(inventory.GetOccupiedSlots(), Is.EqualTo(1));
        });
    }

    [Test]
    public void HasItem_UsesAggregateQuantityAcrossStacks()
    {
        var inventory = CreateInventory(maxSlots: 2);
        inventory.AddItem("item_tincture_small", 12);

        Assert.Multiple(() =>
        {
            Assert.That(inventory.HasItem("item_tincture_small", 12), Is.True);
            Assert.That(inventory.HasItem("item_tincture_small", 13), Is.False);
        });
    }

    [Test]
    public void GetItem_UnknownId_ReturnsNull() =>
        Assert.That(CreateInventory().GetItem("missing_item"), Is.Null);

    [Test]
    public void GetAllItems_ReturnsDefensiveList()
    {
        var inventory = CreateInventory();
        inventory.AddItem("item_sword_basic");
        var items = inventory.GetAllItems();
        items.Clear();

        Assert.That(inventory.GetOccupiedSlots(), Is.EqualTo(1));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void AddItem_NonPositiveQuantity_Throws(int quantity) =>
        Assert.Throws<System.ArgumentOutOfRangeException>(() => CreateInventory().AddItem("item_sword_basic", quantity));

    [TestCase(0)]
    [TestCase(-1)]
    public void RemoveItem_NonPositiveQuantity_Throws(int quantity) =>
        Assert.Throws<System.ArgumentOutOfRangeException>(() => CreateInventory().RemoveItem("item_sword_basic", quantity));

    [TestCase(0)]
    [TestCase(-1)]
    public void InventoryItem_AddQuantity_NonPositive_Throws(int quantity)
    {
        var item = new InventoryItem(CreateItemDatabase().GetItem("item_tincture_small"));

        Assert.Throws<System.ArgumentOutOfRangeException>(() => item.AddQuantity(quantity));
    }

    [TestCase(0)]
    [TestCase(2)]
    public void InventoryItem_RemoveQuantity_NonPartialRemoval_Throws(int quantity)
    {
        var item = new InventoryItem(CreateItemDatabase().GetItem("item_tincture_small"), 2);

        Assert.Throws<System.ArgumentOutOfRangeException>(() => item.RemoveQuantity(quantity));
    }

    private InventoryManager CreateInventory(int maxSlots = Constants.MaxInventorySlots) =>
        new(maxSlots, _itemDatabase, _eventBus);

    private static MockItemDatabase CreateItemDatabase() => new(
        new ItemDefinition { Id = "item_sword_basic", DisplayName = "Basic Sword", Description = "Sword.", ItemType = ItemType.Weapon, MaxStackSize = 1 },
        new ItemDefinition { Id = "item_bow_basic", DisplayName = "Basic Bow", Description = "Bow.", ItemType = ItemType.Weapon, MaxStackSize = 1 },
        new ItemDefinition { Id = "item_tincture_small", DisplayName = "Small Tincture", Description = "Restores health.", ItemType = ItemType.Consumable, MaxStackSize = 10 });
}