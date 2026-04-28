namespace EchoForest.Core;

/// <summary>
/// Drives the Save Game screen without depending on the Godot runtime.
/// </summary>
public interface ISaveGameController
{
    /// <summary>Total number of save slots available.</summary>
    int SlotCount { get; }

    /// <summary>Returns a snapshot of all save slots.</summary>
    SaveSlot[] GetSlots();

    /// <summary>
    /// Saves the current game session into <paramref name="slotIndex"/> using
    /// <paramref name="saveName"/> as the display name.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   Thrown when <paramref name="slotIndex"/> is out of range.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    ///   Thrown when <paramref name="saveName"/> is <c>null</c>.
    /// </exception>
    void SaveToSlot(int slotIndex, string saveName);
}
