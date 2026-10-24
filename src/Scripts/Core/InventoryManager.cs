using System;
using System.Collections.Generic;
using System.Linq;

namespace EchoForest.Core;

/// <summary>Pure-C# inventory with bounded slots, stack-aware changes, and event publication.</summary>
public sealed class InventoryManager : IInventoryManager
{
    private readonly IItemDatabase _itemDatabase;
    private readonly IEventBus _eventBus;
    private readonly List<InventoryItem> _items = [];

    public InventoryManager(int maxSlots, IItemDatabase itemDatabase, IEventBus eventBus)
    {
        if (maxSlots <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxSlots), "Inventory must have at least one slot.");

        MaxSlots = maxSlots;
        _itemDatabase = itemDatabase ?? throw new ArgumentNullException(nameof(itemDatabase));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public int MaxSlots { get; }
    public event Action? Changed;

    public int GetOccupiedSlots() => _items.Count;

    public bool AddItem(string itemId, int quantity = 1)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(itemId);
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

        var definition = _itemDatabase.GetItem(itemId);
        var matchingStacks = _items.Where(item => item.Definition.Id == itemId).ToArray();
        var remainingAfterExistingStacks = quantity;
        foreach (var stack in matchingStacks)
        {
            var amount = Math.Min(remainingAfterExistingStacks, definition.MaxStackSize - stack.Quantity);
            remainingAfterExistingStacks -= amount;
            if (remainingAfterExistingStacks == 0)
                break;
        }

        var additionalSlots = remainingAfterExistingStacks / definition.MaxStackSize
            + (remainingAfterExistingStacks % definition.MaxStackSize == 0 ? 0 : 1);

        if ((long)_items.Count + additionalSlots > MaxSlots)
            return false;

        var remaining = quantity;
        foreach (var stack in matchingStacks)
        {
            var amount = Math.Min(remaining, definition.MaxStackSize - stack.Quantity);
            if (amount > 0)
                stack.AddQuantity(amount);
            remaining -= amount;
            if (remaining == 0)
                break;
        }

        while (remaining > 0)
        {
            var amount = Math.Min(remaining, definition.MaxStackSize);
            _items.Add(new InventoryItem(definition, amount));
            remaining -= amount;
        }

        _eventBus.Publish(new ItemPickedUpEvent(itemId, quantity));
        Changed?.Invoke();
        return true;
    }

    public bool RemoveItem(string itemId, int quantity = 1)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(itemId);
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

        var matchingStacks = _items.Where(item => item.Definition.Id == itemId).ToArray();
        var remainingToRemove = quantity;
        foreach (var stack in matchingStacks)
        {
            remainingToRemove -= Math.Min(remainingToRemove, stack.Quantity);
            if (remainingToRemove == 0)
                break;
        }

        if (remainingToRemove > 0)
            return false;

        var remaining = quantity;
        foreach (var stack in matchingStacks)
        {
            var amount = Math.Min(remaining, stack.Quantity);
            remaining -= amount;
            if (amount == stack.Quantity)
                _items.Remove(stack);
            else
                stack.RemoveQuantity(amount);
            if (remaining == 0)
                break;
        }

        _eventBus.Publish(new ItemRemovedEvent(itemId, quantity));
        Changed?.Invoke();
        return true;
    }

    public bool HasItem(string itemId, int minQuantity = 1)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(itemId);
        if (minQuantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(minQuantity), "Minimum quantity must be greater than zero.");

        var remainingToFind = minQuantity;
        foreach (var stack in _items)
        {
            if (stack.Definition.Id != itemId)
                continue;

            remainingToFind -= Math.Min(remainingToFind, stack.Quantity);
            if (remainingToFind == 0)
                return true;
        }

        return false;
    }

    public InventoryItem? GetItem(string itemId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(itemId);
        return _items.FirstOrDefault(item => item.Definition.Id == itemId);
    }

    public List<InventoryItem> GetAllItems() => [.. _items];
}