using System;
using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>
/// In-process session state. Tracks whether the player has started (or
/// returned to) a game session during this application run, and remembers
/// the last known player position so Continue can resume from there.
///
/// Set via <see cref="Start"/> when <c>GameBootstrapNode</c> loads.
/// Read by <see cref="ISaveService"/> implementations to decide whether
/// the "Continue" button on the Main Menu should be enabled.
///
/// Pure C# — no Godot runtime required, fully testable with NUnit.
/// </summary>
public static class GameSession
{
    private static Dictionary<string, QuestState> _questStates = new(StringComparer.Ordinal);
    private static Dictionary<string, int> _inventoryItems = new(StringComparer.Ordinal);
    private static Dictionary<string, string?> _equippedItems = new(StringComparer.Ordinal);
    private static float _playerHealth = Constants.DefaultPlayerHealth;
    private static bool _introCutsceneRequested;
    private static string? _transitionSpawnPointId;

    /// <summary><c>true</c> when the player has started at least one session.</summary>
    public static bool HasSession { get; private set; }

    /// <summary><c>true</c> when a player position has been saved via <see cref="SavePlayerPosition"/>.</summary>
    public static bool HasPlayerPosition { get; private set; }

    /// <summary>Last saved player world-space X coordinate.</summary>
    public static float LastPlayerX { get; private set; }

    /// <summary>Last saved player world-space Y coordinate.</summary>
    public static float LastPlayerY { get; private set; }

    /// <summary>Quest states supplied by the currently loaded save, if any.</summary>
    public static IReadOnlyDictionary<string, QuestState> QuestStates => _questStates;

    /// <summary>Whether inventory, equipment, and health have been initialized for this session.</summary>
    public static bool HasGameplayState { get; private set; }

    public static IReadOnlyDictionary<string, int> InventoryItems => _inventoryItems;
    public static IReadOnlyDictionary<string, string?> EquippedItems => _equippedItems;
    public static float PlayerHealth => _playerHealth;

    /// <summary>Records the destination spawn point for the next area scene.</summary>
    public static void RequestTransitionSpawnPointId(string spawnPointId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(spawnPointId);
        _transitionSpawnPointId = spawnPointId;
    }

    /// <summary>Returns and clears the destination spawn point requested by an area transition.</summary>
    public static string? ConsumeTransitionSpawnPointId()
    {
        var spawnPointId = _transitionSpawnPointId;
        _transitionSpawnPointId = null;
        return spawnPointId;
    }

    /// <summary>Marks the next loaded game scene to play the new-game intro cutscene.</summary>
    public static void RequestIntroCutscene() => _introCutsceneRequested = true;

    /// <summary>Returns and clears the pending new-game intro request.</summary>
    public static bool ConsumeIntroCutsceneRequest()
    {
        var requested = _introCutsceneRequested;
        _introCutsceneRequested = false;
        return requested;
    }

    /// <summary>
    /// Marks a session as active and clears any saved player position so the
    /// next load uses the scene's spawn point. Call when a NEW game starts.
    /// </summary>
    public static void Start()
    {
        HasSession = true;
        HasPlayerPosition = false;
        _questStates.Clear();
        _inventoryItems.Clear();
        _equippedItems.Clear();
        _playerHealth = Constants.DefaultPlayerHealth;
        HasGameplayState = false;
        _introCutsceneRequested = false;
        _transitionSpawnPointId = null;
    }

    /// <summary>Stores quest progress until the target game scene composes its quest service.</summary>
    public static void SetQuestStates(IReadOnlyDictionary<string, QuestState> questStates)
    {
        ArgumentNullException.ThrowIfNull(questStates);
        _questStates = new Dictionary<string, QuestState>(questStates, StringComparer.Ordinal);
    }

    /// <summary>Replaces the session-level inventory, equipment, and health snapshot.</summary>
    public static void SetGameplayState(
        IReadOnlyDictionary<string, int> inventoryItems,
        IReadOnlyDictionary<string, string?> equippedItems,
        float playerHealth)
    {
        ArgumentNullException.ThrowIfNull(inventoryItems);
        ArgumentNullException.ThrowIfNull(equippedItems);
        if (!float.IsFinite(playerHealth) || playerHealth < 0f || playerHealth > Constants.DefaultPlayerHealth)
            throw new ArgumentOutOfRangeException(nameof(playerHealth));

        var inventoryCopy = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var item in inventoryItems)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(item.Key);
            if (item.Value <= 0)
                throw new ArgumentOutOfRangeException(nameof(inventoryItems), $"Item '{item.Key}' must have a positive quantity.");
            inventoryCopy.Add(item.Key, item.Value);
        }

        var equipmentCopy = new Dictionary<string, string?>(StringComparer.Ordinal);
        foreach (var item in equippedItems)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(item.Key);
            if (item.Value is not null)
                ArgumentException.ThrowIfNullOrWhiteSpace(item.Value);
            equipmentCopy.Add(item.Key, item.Value);
        }

        _inventoryItems = inventoryCopy;
        _equippedItems = equipmentCopy;
        _playerHealth = playerHealth;
        HasGameplayState = true;
    }

    /// <summary>Copies the current session snapshot into a save or transition payload.</summary>
    public static void CopyGameplayStateTo(SaveData saveData)
    {
        ArgumentNullException.ThrowIfNull(saveData);
        if (!HasGameplayState)
            return;

        saveData.InventoryItems = new Dictionary<string, int>(_inventoryItems, StringComparer.Ordinal);
        saveData.EquippedItems = new Dictionary<string, string?>(_equippedItems, StringComparer.Ordinal);
        saveData.PlayerHealth = _playerHealth;
    }

    /// <summary>Applies the session state required after a save is selected.</summary>
    public static void ApplyLoadedSave(SaveData saveData)
    {
        ArgumentNullException.ThrowIfNull(saveData);
        Start();
        SavePlayerPosition(saveData.PlayerX, saveData.PlayerY);
        SetQuestStates(saveData.QuestStates);
        SetGameplayState(saveData.InventoryItems, saveData.EquippedItems, saveData.PlayerHealth);
    }

    /// <summary>
    /// Saves the player's current world-space position so Continue can
    /// restore it. Call just before transitioning back to the Main Menu.
    /// </summary>
    public static void SavePlayerPosition(float x, float y)
    {
        LastPlayerX = x;
        LastPlayerY = y;
        HasPlayerPosition = true;
    }

    /// <summary>Clears all session state. Useful in tests and on new install.</summary>
    public static void Clear()
    {
        HasSession = false;
        HasPlayerPosition = false;
        LastPlayerX = 0f;
        LastPlayerY = 0f;
        _questStates.Clear();
        _inventoryItems.Clear();
        _equippedItems.Clear();
        _playerHealth = Constants.DefaultPlayerHealth;
        HasGameplayState = false;
        _introCutsceneRequested = false;
        _transitionSpawnPointId = null;
    }
}
