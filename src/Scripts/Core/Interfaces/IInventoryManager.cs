using System;
using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>Operations and change notifications for the player's inventory.</summary>
public interface IInventoryManager
{
    int MaxSlots { get; }
    int GetOccupiedSlots();
    bool AddItem(string itemId, int quantity = 1);
    bool RemoveItem(string itemId, int quantity = 1);
    bool HasItem(string itemId, int minQuantity = 1);
    InventoryItem? GetItem(string itemId);
    List<InventoryItem> GetAllItems();
    event Action? Changed;
}