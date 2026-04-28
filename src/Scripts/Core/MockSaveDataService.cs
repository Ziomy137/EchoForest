using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>
/// In-memory <see cref="ISaveDataService"/> test double.
/// Tracks whether <see cref="Save"/> was called without touching the file system.
/// </summary>
public sealed class MockSaveDataService : ISaveDataService
{
    /// <summary>Whether <see cref="Save"/> has been called at least once.</summary>
    public bool SaveWasCalled { get; private set; }

    /// <inheritdoc/>
    public bool HasSaveFile() => SaveWasCalled;

    /// <inheritdoc/>
    public bool HasSave(int slot) => SaveWasCalled;

    /// <inheritdoc/>
    public void Save(SaveData data, int slot) => SaveWasCalled = true;

    /// <inheritdoc/>
    public SaveData Load(int slot) => new SaveData();

    /// <inheritdoc/>
    public void Delete(int slot) { }

    /// <inheritdoc/>
    public List<SaveSlotInfo> GetSaveSlots() => [];
}
