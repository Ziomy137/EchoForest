using System;

namespace EchoForest.Core;

/// <summary>
/// Lightweight display metadata for a single save slot, used by the Load/Save
/// Game screens to populate slot entries without deserialising the full
/// <see cref="SaveData"/> payload.
/// </summary>
public sealed record SaveSlotInfo(
    int Slot,
    string? SaveName,
    string? Area,
    double PlaytimeTotalSeconds,
    DateTime? SavedAt)
{
    /// <summary><c>true</c> when this slot has never been saved to.</summary>
    public bool IsEmpty => SavedAt is null;

    /// <summary>Formatted play time as <c>h:mm:ss</c>.</summary>
    public string PlaytimeDisplay
    {
        get
        {
            if (IsEmpty)
            {
                return "—";
            }

            var playtime = TimeSpan.FromSeconds(PlaytimeTotalSeconds);
            return $"{(int)playtime.TotalHours}:{playtime.Minutes:D2}:{playtime.Seconds:D2}";
        }
    }
}
