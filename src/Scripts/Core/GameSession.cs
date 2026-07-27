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

    /// <summary>
    /// Marks a session as active and clears any saved player position so the
    /// next load uses the scene's spawn point. Call when a NEW game starts.
    /// </summary>
    public static void Start()
    {
        HasSession = true;
        HasPlayerPosition = false;
        _questStates.Clear();
    }

    /// <summary>Stores quest progress until the target game scene composes its quest service.</summary>
    public static void SetQuestStates(IReadOnlyDictionary<string, QuestState> questStates)
    {
        ArgumentNullException.ThrowIfNull(questStates);
        _questStates = new Dictionary<string, QuestState>(questStates, StringComparer.Ordinal);
    }

    /// <summary>Applies the session state required after a save is selected.</summary>
    public static void ApplyLoadedSave(SaveData saveData)
    {
        ArgumentNullException.ThrowIfNull(saveData);
        Start();
        SavePlayerPosition(saveData.PlayerX, saveData.PlayerY);
        SetQuestStates(saveData.QuestStates);
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
    }
}
