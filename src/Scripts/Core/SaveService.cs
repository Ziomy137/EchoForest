using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# implementation of <see cref="ISaveDataService"/> that serialises
/// <see cref="SaveData"/> to JSON files via <see cref="IFileSystem"/>.
///
/// Save files are stored at <c>user://save_slot_{slot}.json</c> (1-based).
/// In production the file system is <see cref="GodotFileSystem"/>;
/// in tests it is <see cref="MockFileSystem"/> — no Godot runtime required.
///
/// Also implements <see cref="ISaveService.HasSaveFile"/> so it can replace
/// <see cref="GodotSessionSaveService"/> in the Main Menu.
/// </summary>
public sealed class SaveService : ISaveDataService
{
    private readonly IFileSystem _fs;
    private readonly string _basePath;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true,
    };

    /// <param name="fileSystem">File-system abstraction (production or test double).</param>
    /// <param name="basePath">
    ///   Directory prefix for save files, e.g. <c>"user://"</c>.
    ///   Defaults to <c>"user://"</c> — individual files are named
    ///   <c>{basePath}save_slot_{slot}.json</c>.
    /// </param>
    public SaveService(IFileSystem fileSystem, string basePath = "user://")
    {
        _fs = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _basePath = NormaliseBasePath(basePath);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static string NormaliseBasePath(string basePath)
    {
        if (string.IsNullOrWhiteSpace(basePath))
            throw new ArgumentException("Base path must not be null, empty, or whitespace.", nameof(basePath));

        return basePath.EndsWith("/", StringComparison.Ordinal) ? basePath : $"{basePath}/";
    }
    private string PathForSlot(int slot) => $"{_basePath}save_slot_{slot}.json";

    // ── ISaveDataService ──────────────────────────────────────────────────────

    public void Save(SaveData data, int slot)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        data.SaveTimestamp = DateTime.UtcNow;
        var json = JsonSerializer.Serialize(data, JsonOpts);
        _fs.WriteText(PathForSlot(slot), json);
    }

    public SaveData Load(int slot)
    {
        var path = PathForSlot(slot);
        if (!_fs.Exists(path))
            throw new SaveDataException($"Save slot {slot} does not exist.");

        string json;
        try
        {
            json = _fs.ReadText(path);
        }
        catch (Exception ex)
        {
            throw new SaveDataException($"Failed to read save slot {slot}.", ex);
        }

        try
        {
            var data = JsonSerializer.Deserialize<SaveData>(json, JsonOpts);
            return data ?? throw new SaveDataException($"Save slot {slot} deserialised to null.");
        }
        catch (JsonException ex)
        {
            throw new SaveDataException($"Save slot {slot} contains invalid JSON.", ex);
        }
    }

    public void Delete(int slot)
    {
        var path = PathForSlot(slot);
        if (_fs.Exists(path))
            _fs.Delete(path);
    }

    public List<SaveSlotInfo> GetSaveSlots()
    {
        var result = new List<SaveSlotInfo>();
        for (int slot = 1; slot <= Constants.SaveSlotCount; slot++)
        {
            var path = PathForSlot(slot);
            if (!_fs.Exists(path))
            {
                result.Add(new SaveSlotInfo(slot, null, null, 0, null));
                continue;
            }

            try
            {
                var data = Load(slot);
                result.Add(new SaveSlotInfo(
                    Slot: slot,
                    SaveName: null,          // SaveData doesn't carry a display name yet
                    Area: data.CurrentArea,
                    PlaytimeTotalSeconds: data.PlaytimeTotalSeconds,
                    SavedAt: data.SaveTimestamp.ToLocalTime()));
            }
            catch (SaveDataException)
            {
                // Corrupt slot — surface as empty so UI doesn't crash.
                result.Add(new SaveSlotInfo(slot, null, null, 0, null));
            }
        }
        return result;
    }

    public bool HasSave(int slot) => _fs.Exists(PathForSlot(slot));

    // ── ISaveService (MainMenuController compat) ──────────────────────────────

    /// <summary>
    /// Returns <c>true</c> when at least one of the five save slots exists.
    /// Satisfies <see cref="ISaveService.HasSaveFile"/> used by
    /// <see cref="MainMenuController"/> to enable the "Continue" button.
    /// </summary>
    public bool HasSaveFile()
    {
        for (int slot = 1; slot <= Constants.SaveSlotCount; slot++)
            if (HasSave(slot))
                return true;
        return false;
    }
}
