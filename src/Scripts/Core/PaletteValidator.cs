using System.Collections.Generic;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Validates whether a <see cref="Color"/> belongs to the approved
/// <see cref="Palette"/>.
///
/// Rules:
/// <list type="bullet">
///   <item>Fully transparent pixels (alpha &lt; 1) are never approved — they
///   are typically skipped by sprite checkers rather than validated.</item>
///   <item>A color is approved when its opaque RGB hex exactly matches one of
///   the 16 entries in <see cref="Palette.All"/>.</item>
/// </list>
///
/// Pure C# — no Godot runtime required. Testable with NUnit.
/// </summary>
public static class PaletteValidator
{
    // Build a hash-set of approved hex strings once at class initialisation.
    // Color.ToHtml(false) returns lowercase RGB hex without '#', e.g. "1a1a1a".
    private static readonly HashSet<string> _approvedHex = BuildApprovedSet();

    private static HashSet<string> BuildApprovedSet()
    {
        var set = new HashSet<string>(16);
        foreach (var color in Palette.All)
            set.Add(color.ToHtml(false));
        return set;
    }

    /// <summary>
    /// Returns <c>true</c> when <paramref name="color"/> is fully opaque and
    /// its RGB value exactly matches one of the 16 approved palette entries.
    /// </summary>
    public static bool IsApprovedColor(Color color)
    {
        // Reject any pixel that is not fully opaque.
        if (color.A < 1.0f)
            return false;

        return _approvedHex.Contains(color.ToHtml(false));
    }
}
