using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class EquipmentManagerTest
{
    [Test]
    public void Constructor_ImplementsEquipmentInterface()
    {
        var manager = CreateEquipmentManager();

        Assert.That(manager, Is.InstanceOf<IEquipmentManager>());
    }

    [Test]
    public void Equip_SwordToWeaponSlot_SucceedsAndPublishesEvent()
    {
        var eventBus = new EventBus();
        var manager = CreateEquipmentManager(eventBus: eventBus, out var inventory);
        inventory.AddItem("item_sword_basic");
        EquipmentChangedEvent? equipmentEvent = null;
        eventBus.Subscribe<EquipmentChangedEvent>(gameEvent => equipmentEvent = gameEvent);

        var result = manager.Equip("item_sword_basic", EquipmentSlot.Weapon);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(manager.GetEquipped(EquipmentSlot.Weapon)?.Definition.Id, Is.EqualTo("item_sword_basic"));
            Assert.That(manager.IsSlotOccupied(EquipmentSlot.Weapon), Is.True);
            Assert.That(inventory.HasItem("item_sword_basic"), Is.False);
            Assert.That(equipmentEvent, Is.EqualTo(new EquipmentChangedEvent(EquipmentSlot.Weapon, "item_sword_basic")));
        });
    }

    [Test]
    public void Equip_ShieldToOffHand_Succeeds()
    {
        var manager = CreateEquipmentManager(out var inventory);
        inventory.AddItem("item_shield_basic");

        var result = manager.Equip("item_shield_basic", EquipmentSlot.OffHand);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(manager.GetEquipped(EquipmentSlot.OffHand)?.Definition.Id, Is.EqualTo("item_shield_basic"));
        });
    }

    [Test]
    public void Equip_ShieldToWeaponSlot_FailsWithoutMutation()
    {
        var manager = CreateEquipmentManager(out var inventory);
        inventory.AddItem("item_shield_basic");

        var result = manager.Equip("item_shield_basic", EquipmentSlot.Weapon);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(inventory.HasItem("item_shield_basic"), Is.True);
            Assert.That(manager.IsSlotOccupied(EquipmentSlot.Weapon), Is.False);
        });
    }

    [Test]
    public void Equip_ArmorToHeadSlot_Fails()
    {
        var manager = CreateEquipmentManager(out var inventory);
        inventory.AddItem("item_tunic_peasant");

        Assert.That(manager.Equip("item_tunic_peasant", EquipmentSlot.Head), Is.False);
    }

    [Test]
    public void Equip_ArmorToArmorSlot_Succeeds()
    {
        var manager = CreateEquipmentManager(out var inventory);
        inventory.AddItem("item_tunic_peasant");

        var result = manager.Equip("item_tunic_peasant", EquipmentSlot.Armor);

        Assert.That(result, Is.True);
        Assert.That(manager.GetEquipped(EquipmentSlot.Armor)?.Definition.Id, Is.EqualTo("item_tunic_peasant"));
    }

    [Test]
    public void Equip_ItemNotInInventory_Fails()
    {
        var manager = CreateEquipmentManager();

        Assert.That(manager.Equip("item_sword_basic", EquipmentSlot.Weapon), Is.False);
    }

    [Test]
    public void Equip_UnknownItem_Fails()
    {
        var manager = CreateEquipmentManager();

        Assert.That(manager.Equip("item_missing", EquipmentSlot.Weapon), Is.False);
    }

    [Test]
    public void Equip_AlreadyEquippedItem_IsIdempotent()
    {
        var manager = CreateEquipmentManager(out var inventory);
        inventory.AddItem("item_sword_basic");
        manager.Equip("item_sword_basic", EquipmentSlot.Weapon);

        Assert.That(manager.Equip("item_sword_basic", EquipmentSlot.Weapon), Is.True);
    }

    [Test]
    public void Equip_ReplacesExistingItemAndReturnsOldItemToInventory()
    {
        var manager = CreateEquipmentManager(maxSlots: 1, out var inventory);
        inventory.AddItem("item_sword_basic");
        manager.Equip("item_sword_basic", EquipmentSlot.Weapon);
        inventory.AddItem("item_bow_basic");

        var result = manager.Equip("item_bow_basic", EquipmentSlot.Weapon);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(manager.GetEquipped(EquipmentSlot.Weapon)?.Definition.Id, Is.EqualTo("item_bow_basic"));
            Assert.That(inventory.HasItem("item_sword_basic"), Is.True);
            Assert.That(inventory.HasItem("item_bow_basic"), Is.False);
        });
    }

    [Test]
    public void Unequip_ReturnsItemToInventoryAndPublishesEmptySlot()
    {
        var eventBus = new EventBus();
        var manager = CreateEquipmentManager(eventBus: eventBus, out var inventory);
        inventory.AddItem("item_sword_basic");
        manager.Equip("item_sword_basic", EquipmentSlot.Weapon);
        EquipmentChangedEvent? equipmentEvent = null;
        eventBus.Subscribe<EquipmentChangedEvent>(gameEvent => equipmentEvent = gameEvent);

        var removed = manager.Unequip(EquipmentSlot.Weapon);

        Assert.Multiple(() =>
        {
            Assert.That(removed?.Definition.Id, Is.EqualTo("item_sword_basic"));
            Assert.That(inventory.HasItem("item_sword_basic"), Is.True);
            Assert.That(manager.IsSlotOccupied(EquipmentSlot.Weapon), Is.False);
            Assert.That(equipmentEvent, Is.EqualTo(new EquipmentChangedEvent(EquipmentSlot.Weapon, null)));
        });
    }

    [Test]
    public void Unequip_WhenInventoryFull_LeavesItemEquipped()
    {
        var manager = CreateEquipmentManager(maxSlots: 1, out var inventory);
        inventory.AddItem("item_sword_basic");
        manager.Equip("item_sword_basic", EquipmentSlot.Weapon);
        inventory.AddItem("item_bow_basic");

        Assert.That(manager.Unequip(EquipmentSlot.Weapon), Is.Null);
        Assert.That(manager.IsSlotOccupied(EquipmentSlot.Weapon), Is.True);
    }

    [Test]
    public void Unequip_EmptySlot_ReturnsNull()
    {
        var manager = CreateEquipmentManager();

        Assert.That(manager.Unequip(EquipmentSlot.Weapon), Is.Null);
    }

    [Test]
    public void Equip_InvalidSlot_ThrowsArgumentOutOfRangeException()
    {
        var manager = CreateEquipmentManager();

        Assert.Throws<System.ArgumentOutOfRangeException>(() => manager.Equip("item_sword_basic", (EquipmentSlot)999));
    }

    [Test]
    public void Constructor_NullDependencies_ThrowsArgumentNullException()
    {
        var inventory = CreateInventory(out var database);
        var eventBus = new EventBus();

        Assert.Multiple(() =>
        {
            Assert.Throws<System.ArgumentNullException>(() => new EquipmentManager(null!, inventory, eventBus));
            Assert.Throws<System.ArgumentNullException>(() => new EquipmentManager(database, null!, eventBus));
            Assert.Throws<System.ArgumentNullException>(() => new EquipmentManager(database, inventory, null!));
        });
    }

    private static EquipmentManager CreateEquipmentManager(out InventoryManager inventory, int maxSlots = Constants.MaxInventorySlots)
    {
        var database = CreateItemDatabase();
        var eventBus = new EventBus();
        inventory = new InventoryManager(maxSlots, database, eventBus);
        return new EquipmentManager(database, inventory, eventBus);
    }

    private static InventoryManager CreateInventory(out MockItemDatabase database)
    {
        database = CreateItemDatabase();
        return new InventoryManager(Constants.MaxInventorySlots, database, new EventBus());
    }

    private static EquipmentManager CreateEquipmentManager(IEventBus eventBus, out InventoryManager inventory, int maxSlots = Constants.MaxInventorySlots)
    {
        var database = CreateItemDatabase();
        inventory = new InventoryManager(maxSlots, database, eventBus);
        return new EquipmentManager(database, inventory, eventBus);
    }

    private static EquipmentManager CreateEquipmentManager(int maxSlots, out InventoryManager inventory) =>
        CreateEquipmentManager(new EventBus(), out inventory, maxSlots);

    private static EquipmentManager CreateEquipmentManager() =>
        CreateEquipmentManager(new EventBus(), out _, Constants.MaxInventorySlots);

    private static EquipmentManager CreateEquipmentManager(IEventBus eventBus, out InventoryManager inventory) =>
        CreateEquipmentManager(eventBus, out inventory, Constants.MaxInventorySlots);

    private static MockItemDatabase CreateItemDatabase() => new(
        new ItemDefinition { Id = "item_sword_basic", DisplayName = "Basic Sword", Description = "Sword.", ItemType = ItemType.Weapon, EquipmentSlot = EquipmentSlot.Weapon, MaxStackSize = 1 },
        new ItemDefinition { Id = "item_bow_basic", DisplayName = "Basic Bow", Description = "Bow.", ItemType = ItemType.Weapon, EquipmentSlot = EquipmentSlot.Weapon, MaxStackSize = 1 },
        new ItemDefinition { Id = "item_shield_basic", DisplayName = "Basic Shield", Description = "Shield.", ItemType = ItemType.Weapon, EquipmentSlot = EquipmentSlot.OffHand, MaxStackSize = 1 },
        new ItemDefinition { Id = "item_tunic_peasant", DisplayName = "Peasant Tunic", Description = "Tunic.", ItemType = ItemType.Armor, EquipmentSlot = EquipmentSlot.Armor, MaxStackSize = 1 });
}