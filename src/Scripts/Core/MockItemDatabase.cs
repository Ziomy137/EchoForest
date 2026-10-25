using System;
using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>In-memory item database test double with the initial game catalog.</summary>
public sealed class MockItemDatabase : IItemDatabase
{
    private readonly Dictionary<string, ItemDefinition> _itemsById = new(StringComparer.Ordinal);

    public MockItemDatabase(params ItemDefinition[] definitions)
    {
        if (definitions.Length == 0)
            definitions = CreateDefaults();

        foreach (var definition in definitions)
            _itemsById.Add(definition.Id, definition);
    }

    public ItemDefinition GetItem(string itemId) =>
        _itemsById.TryGetValue(itemId, out var item)
            ? item
            : throw new KeyNotFoundException($"Item '{itemId}' was not found in the mock item database.");

    public List<ItemDefinition> GetAllItems() => [.. _itemsById.Values];

    private static ItemDefinition[] CreateDefaults() =>
    [
        new() { Id = "item_sword_basic", DisplayName = "Basic Sword", Description = "A simple sword for close combat.", ItemType = ItemType.Weapon, EquipmentSlot = EquipmentSlot.Weapon, MaxStackSize = 1 },
        new() { Id = "item_bow_basic", DisplayName = "Basic Bow", Description = "A simple bow for ranged combat.", ItemType = ItemType.Weapon, EquipmentSlot = EquipmentSlot.Weapon, MaxStackSize = 1 },
        new() { Id = "item_shield_basic", DisplayName = "Basic Shield", Description = "A sturdy wooden shield.", ItemType = ItemType.Weapon, EquipmentSlot = EquipmentSlot.OffHand, MaxStackSize = 1 },
        new() { Id = "item_tunic_peasant", DisplayName = "Peasant Tunic", Description = "A comfortable everyday tunic.", ItemType = ItemType.Armor, EquipmentSlot = EquipmentSlot.Armor, MaxStackSize = 1 },
        new() { Id = "item_tunic_leather", DisplayName = "Leather Tunic", Description = "A reinforced leather tunic.", ItemType = ItemType.Armor, EquipmentSlot = EquipmentSlot.Armor, MaxStackSize = 1 },
        new() { Id = "item_tincture_small", DisplayName = "Small Tincture", Description = "Restores 25 health.", ItemType = ItemType.Consumable, MaxStackSize = 10, HealthRestore = 25f },
        new() { Id = "item_tincture_large", DisplayName = "Large Tincture", Description = "Restores 75 health.", ItemType = ItemType.Consumable, MaxStackSize = 10, HealthRestore = 75f },
        new() { Id = "item_portal_key", DisplayName = "Portal Key", Description = "A key connected to the portal chamber.", ItemType = ItemType.Artifact, MaxStackSize = 1 },
    ];
}