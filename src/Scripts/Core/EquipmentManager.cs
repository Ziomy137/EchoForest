using System;
using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>Pure-C# equipment manager with inventory exchange and typed change events.</summary>
public sealed class EquipmentManager : IEquipmentManager
{
    private readonly IItemDatabase _itemDatabase;
    private readonly IInventoryManager _inventory;
    private readonly IEventBus _eventBus;
    private readonly Dictionary<EquipmentSlot, InventoryItem> _equipped = [];

    public EquipmentManager(IItemDatabase itemDatabase, IInventoryManager inventory, IEventBus eventBus)
    {
        _itemDatabase = itemDatabase ?? throw new ArgumentNullException(nameof(itemDatabase));
        _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public bool Equip(string itemId, EquipmentSlot slot)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(itemId);
        ValidateSlot(slot);

        if (_equipped.TryGetValue(slot, out var current) && current.Definition.Id == itemId)
            return true;

        ItemDefinition definition;
        try
        {
            definition = _itemDatabase.GetItem(itemId);
        }
        catch (KeyNotFoundException)
        {
            return false;
        }

        if (!IsCompatible(definition, slot) || !_inventory.HasItem(itemId))
            return false;

        if (!_inventory.RemoveItem(itemId))
            return false;

        if (current is not null && !_inventory.AddItem(current.Definition.Id))
        {
            _inventory.AddItem(itemId);
            return false;
        }

        _equipped[slot] = new InventoryItem(definition);
        _eventBus.Publish(new EquipmentChangedEvent(slot, itemId));
        return true;
    }

    public InventoryItem? Unequip(EquipmentSlot slot)
    {
        ValidateSlot(slot);
        if (!_equipped.TryGetValue(slot, out var equipped))
            return null;

        if (!_inventory.AddItem(equipped.Definition.Id))
            return null;

        _equipped.Remove(slot);
        _eventBus.Publish(new EquipmentChangedEvent(slot, null));
        return equipped;
    }

    public InventoryItem? GetEquipped(EquipmentSlot slot)
    {
        ValidateSlot(slot);
        return _equipped.TryGetValue(slot, out var equipped) ? equipped : null;
    }

    public bool IsSlotOccupied(EquipmentSlot slot)
    {
        ValidateSlot(slot);
        return _equipped.ContainsKey(slot);
    }

    private static bool IsCompatible(ItemDefinition item, EquipmentSlot slot)
    {
        if (item.EquipmentSlot != slot)
            return false;

        return slot switch
        {
            EquipmentSlot.Weapon or EquipmentSlot.OffHand => item.ItemType == ItemType.Weapon,
            EquipmentSlot.Armor or EquipmentSlot.Head => item.ItemType == ItemType.Armor,
            _ => false,
        };
    }

    private static void ValidateSlot(EquipmentSlot slot)
    {
        if (!Enum.IsDefined(slot))
            throw new ArgumentOutOfRangeException(nameof(slot));
    }
}