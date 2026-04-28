using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>
/// Full save/load service contract for S5-04.
///
/// Extends <see cref="ISaveService"/> so implementations also satisfy
/// the simple <c>HasSaveFile()</c> check used by <see cref="MainMenuController"/>.
/// </summary>
public interface ISaveDataService : ISaveService
{
    /// <summary>
    /// Serialises <paramref name="data"/> and writes it to the save file for
    /// <paramref name="slot"/>.
    /// </summary>
    void Save(SaveData data, int slot);

    /// <summary>
    /// Reads and deserialises the save data for <paramref name="slot"/>.
    /// </summary>
    /// <exception cref="SaveDataException">
    ///   Thrown when the file exists but is corrupt or unreadable.
    /// </exception>
    SaveData Load(int slot);

    /// <summary>Deletes the save file for <paramref name="slot"/>. No-op if it does not exist.</summary>
    void Delete(int slot);

    /// <summary>Returns slot metadata for all known slots (1-based index range).</summary>
    List<SaveSlotInfo> GetSaveSlots();

    /// <summary>Returns <c>true</c> when a save file exists for <paramref name="slot"/>.</summary>
    bool HasSave(int slot);
}
