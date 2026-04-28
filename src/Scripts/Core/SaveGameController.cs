using System;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# controller for the Save Game screen.
///
/// Manages in-memory slot state without depending on the Godot runtime so it
/// can be unit-tested with NUnit.  Actual disk persistence will be wired in
/// when the save-game service is implemented in a future sprint.
/// </summary>
public sealed class SaveGameController : ISaveGameController
{
    private readonly SaveSlot[] _slots;

    /// <inheritdoc/>
    public int SlotCount { get; }

    /// <param name="slotCount">
    ///   Number of save slots to expose.  Must be greater than zero.
    ///   Defaults to 5 as recommended by the UI/UX specification (§9.3).
    /// </param>
    public SaveGameController(int slotCount = 5)
    {
        if (slotCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(slotCount), "Must be greater than zero.");

        SlotCount = slotCount;
        _slots = new SaveSlot[slotCount];
        for (int i = 0; i < slotCount; i++)
            _slots[i] = new SaveSlot(i);
    }

    /// <inheritdoc/>
    /// <remarks>Returns a shallow array clone so callers cannot mutate internal state.</remarks>
    public SaveSlot[] GetSlots() => (SaveSlot[])_slots.Clone();

    /// <inheritdoc/>
    public void SaveToSlot(int slotIndex, string saveName)
    {
        if (slotIndex < 0 || slotIndex >= SlotCount)
            throw new ArgumentOutOfRangeException(nameof(slotIndex));
        if (saveName is null)
            throw new ArgumentNullException(nameof(saveName));

        _slots[slotIndex] = new SaveSlot(
            Index: slotIndex,
            SaveName: saveName,
            CharacterLevel: null,   // populated by save system in a future sprint
            Location: null,         // populated by save system in a future sprint
            PlayTime: null,         // populated by save system in a future sprint
            SavedAt: DateTime.Now);
    }
}
