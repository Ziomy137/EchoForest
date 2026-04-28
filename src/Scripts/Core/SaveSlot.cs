using System;

namespace EchoForest.Core;

/// <summary>
/// Immutable snapshot of a single save-game slot's persisted data.
///
/// All fields are optional — a slot with no <see cref="SavedAt"/> is considered
/// <see cref="IsEmpty"/> and has never been written to.
/// </summary>
public sealed record SaveSlot(
    int Index,
    string? SaveName = null,
    int? CharacterLevel = null,
    string? Location = null,
    TimeSpan? PlayTime = null,
    DateTime? SavedAt = null)
{
    /// <summary><c>true</c> when this slot has never been saved to.</summary>
    public bool IsEmpty => SavedAt is null;
}
