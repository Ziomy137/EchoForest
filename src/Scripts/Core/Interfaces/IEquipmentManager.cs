namespace EchoForest.Core;

/// <summary>Equipping and unequipping operations for the player.</summary>
public interface IEquipmentManager
{
    bool Equip(string itemId, EquipmentSlot slot);
    InventoryItem? Unequip(EquipmentSlot slot);
    InventoryItem? GetEquipped(EquipmentSlot slot);
    bool IsSlotOccupied(EquipmentSlot slot);
}