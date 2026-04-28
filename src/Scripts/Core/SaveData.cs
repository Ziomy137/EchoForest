using System;
using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>
/// Serialisable snapshot of all game-critical state for a single save slot.
///
/// Populated by the game systems when saving, restored by those same systems
/// when loading. All fields are nullable or have safe defaults so that saves
/// from older versions load gracefully.
/// </summary>
public sealed class SaveData
{
    // ── Player ────────────────────────────────────────────────────────────────

    /// <summary>Player world-space X coordinate.</summary>
    public float PlayerX { get; set; }

    /// <summary>Player world-space Y coordinate.</summary>
    public float PlayerY { get; set; }

    /// <summary>Scene resource path of the area the player was in.</summary>
    public string CurrentArea { get; set; } = string.Empty;

    /// <summary>Player health at save time.</summary>
    public float PlayerHealth { get; set; } = 100f;

    // ── Quests ────────────────────────────────────────────────────────────────

    /// <summary>Maps quest ID to its <see cref="QuestState"/>.</summary>
    public Dictionary<string, QuestState> QuestStates { get; set; } = new();

    // ── Inventory ─────────────────────────────────────────────────────────────

    /// <summary>Maps item ID to quantity held.</summary>
    public Dictionary<string, int> InventoryItems { get; set; } = new();

    /// <summary>Maps equipment slot name to the equipped item ID. <c>null</c> value = empty slot.</summary>
    public Dictionary<string, string?> EquippedItems { get; set; } = new();

    // ── Meta ──────────────────────────────────────────────────────────────────

    /// <summary>UTC time at which this save was created.</summary>
    public DateTime SaveTimestamp { get; set; } = DateTime.UtcNow;

    /// <summary>Accumulated play time in seconds at the time of saving.</summary>
    public double PlaytimeTotalSeconds { get; set; }
}
