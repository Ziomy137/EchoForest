using System.Collections.Generic;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class GameplayStatePersistenceTest
{
    [SetUp]
    public void SetUp() => GameSession.Clear();

    [TearDown]
    public void TearDown() => GameSession.Clear();

    [Test]
    public void InventoryManager_RestoresStackedItemsFromSnapshot()
    {
        var database = new MockItemDatabase();
        var source = new InventoryManager(Constants.MaxInventorySlots, database, new EventBus());
        source.AddItem("item_tincture_small", 12);
        var snapshot = source.GetItemQuantities();
        var restored = new InventoryManager(Constants.MaxInventorySlots, database, new EventBus());

        restored.RestoreItems(snapshot);

        Assert.Multiple(() =>
        {
            Assert.That(restored.GetItemQuantities(), Is.EqualTo(snapshot));
            Assert.That(restored.GetOccupiedSlots(), Is.EqualTo(2));
            Assert.That(restored.HasItem("item_tincture_small", 12), Is.True);
        });
    }

    [Test]
    public void EquipmentManager_RestoresEquippedItemsFromSnapshot()
    {
        var database = new MockItemDatabase();
        var eventBus = new EventBus();
        var sourceInventory = new InventoryManager(Constants.MaxInventorySlots, database, eventBus);
        sourceInventory.AddItem("item_sword_basic");
        sourceInventory.AddItem("item_shield_basic");
        var sourceEquipment = new EquipmentManager(database, sourceInventory, eventBus);
        sourceEquipment.Equip("item_sword_basic", EquipmentSlot.Weapon);
        sourceEquipment.Equip("item_shield_basic", EquipmentSlot.OffHand);
        var snapshot = sourceEquipment.GetEquippedItems();
        var restoredEquipment = new EquipmentManager(
            database,
            new InventoryManager(Constants.MaxInventorySlots, database, new EventBus()),
            new EventBus());

        restoredEquipment.RestoreEquippedItems(snapshot);

        Assert.Multiple(() =>
        {
            Assert.That(restoredEquipment.GetEquipped(EquipmentSlot.Weapon)?.Definition.Id, Is.EqualTo("item_sword_basic"));
            Assert.That(restoredEquipment.GetEquipped(EquipmentSlot.OffHand)?.Definition.Id, Is.EqualTo("item_shield_basic"));
        });
    }

    [Test]
    public void AreaTransition_CopiesGameplayStateIntoSaveBeforeLoading()
    {
        var inventoryItems = new Dictionary<string, int> { ["item_tincture_small"] = 3 };
        var equippedItems = new Dictionary<string, string?>
        {
            [nameof(EquipmentSlot.Weapon)] = "item_sword_basic",
            [nameof(EquipmentSlot.OffHand)] = null,
            [nameof(EquipmentSlot.Armor)] = "item_tunic_leather",
            [nameof(EquipmentSlot.Head)] = null,
        };
        GameSession.SetGameplayState(inventoryItems, equippedItems, 63f);
        var saveService = new CapturingSaveDataService();
        var service = new AreaTransitionService(new EventBus(), new MockSceneLoader(), saveService);
        var saveData = new SaveData { CurrentArea = MainMenuConfig.FarmScenePath };

        service.TriggerTransition(
            MainMenuConfig.FarmScenePath,
            MainMenuConfig.ForestPathScenePath,
            "south_entrance",
            saveData);

        Assert.Multiple(() =>
        {
            Assert.That(saveService.SavedData, Is.SameAs(saveData));
            Assert.That(saveData.PlayerHealth, Is.EqualTo(63f));
            Assert.That(saveData.InventoryItems, Is.EqualTo(inventoryItems));
            Assert.That(saveData.EquippedItems, Is.EqualTo(equippedItems));
        });
    }

    [Test]
    public void SaveService_RoundTripsInventoryEquipmentAndHealth()
    {
        var fileSystem = new MockFileSystem();
        var saveService = new SaveService(fileSystem);
        var saveData = new SaveData
        {
            PlayerHealth = 63f,
            InventoryItems = new Dictionary<string, int> { ["item_tincture_small"] = 3 },
            EquippedItems = new Dictionary<string, string?>
            {
                [nameof(EquipmentSlot.Weapon)] = "item_sword_basic",
                [nameof(EquipmentSlot.OffHand)] = "item_shield_basic",
            },
        };

        saveService.Save(saveData, slot: 1);
        var loaded = saveService.Load(slot: 1);

        Assert.Multiple(() =>
        {
            Assert.That(loaded.PlayerHealth, Is.EqualTo(63f));
            Assert.That(loaded.InventoryItems, Is.EqualTo(saveData.InventoryItems));
            Assert.That(loaded.EquippedItems, Is.EqualTo(saveData.EquippedItems));
        });
    }

    [Test]
    public void ApplyLoadedSave_RestoresInventoryEquipmentAndHealthForNewHudManagers()
    {
        var inventoryItems = new Dictionary<string, int> { ["item_tincture_small"] = 12 };
        var equippedItems = new Dictionary<string, string?>
        {
            [nameof(EquipmentSlot.Weapon)] = "item_sword_basic",
            [nameof(EquipmentSlot.OffHand)] = "item_shield_basic",
            [nameof(EquipmentSlot.Armor)] = null,
            [nameof(EquipmentSlot.Head)] = null,
        };
        var saveData = new SaveData
        {
            PlayerHealth = 37f,
            InventoryItems = inventoryItems,
            EquippedItems = equippedItems,
        };

        GameSession.ApplyLoadedSave(saveData);

        var database = new MockItemDatabase();
        var eventBus = new EventBus();
        var inventory = new InventoryManager(Constants.MaxInventorySlots, database, eventBus);
        inventory.RestoreItems(GameSession.InventoryItems);
        var equipment = new EquipmentManager(database, inventory, eventBus);
        equipment.RestoreEquippedItems(GameSession.EquippedItems);
        var health = new PlayerHealthManager(Constants.DefaultPlayerHealth, eventBus, GameSession.PlayerHealth);

        Assert.Multiple(() =>
        {
            Assert.That(inventory.GetItemQuantities(), Is.EqualTo(inventoryItems));
            Assert.That(equipment.GetEquipped(EquipmentSlot.Weapon)?.Definition.Id, Is.EqualTo("item_sword_basic"));
            Assert.That(equipment.GetEquipped(EquipmentSlot.OffHand)?.Definition.Id, Is.EqualTo("item_shield_basic"));
            Assert.That(health.CurrentHealth, Is.EqualTo(37f));
        });
    }

    [Test]
    public void Start_ClearsPreviousGameplaySnapshotForNewGame()
    {
        GameSession.SetGameplayState(
            new Dictionary<string, int> { ["item_tincture_small"] = 2 },
            new Dictionary<string, string?> { [nameof(EquipmentSlot.Weapon)] = "item_sword_basic" },
            25f);

        GameSession.Start();

        Assert.Multiple(() =>
        {
            Assert.That(GameSession.HasGameplayState, Is.False);
            Assert.That(GameSession.InventoryItems, Is.Empty);
            Assert.That(GameSession.EquippedItems, Is.Empty);
            Assert.That(GameSession.PlayerHealth, Is.EqualTo(Constants.DefaultPlayerHealth));
        });
    }

    private sealed class CapturingSaveDataService : ISaveDataService
    {
        public SaveData SavedData { get; private set; } = new();
        public bool HasSaveFile() => false;
        public bool HasSave(int slot) => false;
        public void Save(SaveData data, int slot) => SavedData = data;
        public SaveData Load(int slot) => new();
        public void Delete(int slot) { }
        public List<SaveSlotInfo> GetSaveSlots() => [];
    }
}