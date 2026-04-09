using Godot;

namespace EchoForest.Core;

/// <summary>
/// All 16 approved palette colors for the EchoForest demo.
///
/// Every sprite and UI element must use only these colors.
/// Use <see cref="PaletteValidator.IsApprovedColor"/> to verify compliance.
///
/// Pure C# — no Godot runtime required. Testable with NUnit.
/// </summary>
public static class Palette
{
    // ─── 16 approved colors ───────────────────────────────────────────────────

    public static readonly Color DeepBlack = new Color("#1a1a1a");
    public static readonly Color DarkBrown = new Color("#2d2416");
    public static readonly Color DarkGray = new Color("#3d3d3d");
    public static readonly Color MediumGray = new Color("#5a5a5a");
    public static readonly Color WarmBrown = new Color("#8b7355");
    public static readonly Color DarkLeather = new Color("#5c3d2e");
    public static readonly Color DarkRed = new Color("#8b0000");
    public static readonly Color DeepPurple = new Color("#2a1a4a");
    public static readonly Color DarkOrange = new Color("#ff6b00");
    public static readonly Color Gold = new Color("#ffd700");
    public static readonly Color DarkGreen = new Color("#1a3a1a");
    public static readonly Color DeepWater = new Color("#1a3a5c");
    public static readonly Color SkinTone = new Color("#8b6f47");
    public static readonly Color LightSkin = new Color("#a88860");
    public static readonly Color White = new Color("#ffffff");
    public static readonly Color LightGray = new Color("#cccccc");

    // ─── Backing array (16 entries) ───────────────────────────────────────────

    private static readonly Color[] _all =
    {
        DeepBlack,   DarkBrown,   DarkGray,    MediumGray,
        WarmBrown,   DarkLeather, DarkRed,     DeepPurple,
        DarkOrange,  Gold,        DarkGreen,   DeepWater,
        SkinTone,    LightSkin,   White,       LightGray,
    };

    /// <summary>
    /// Returns all 16 approved palette colors.
    /// A defensive copy is returned so callers cannot mutate the internal palette definition.
    /// </summary>
    public static Color[] All => (Color[])_all.Clone();
}
