using System;

namespace EchoForest.Core;

/// <summary>Runtime item instance combining a static definition and quantity.</summary>
public sealed class InventoryItem
{
    public InventoryItem(ItemDefinition definition, int quantity = 1)
    {
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        Quantity = quantity > 0
            ? quantity
            : throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
    }

    public ItemDefinition Definition { get; }
    public int Quantity { get; internal set; }
}