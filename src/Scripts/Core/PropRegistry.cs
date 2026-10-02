using System.Linq;

namespace EchoForest.Core;

/// <summary>
/// Central registry of all environment prop sprite assets used by area scenes.
///
/// Contains metadata for the props required by the Cottage and Farm areas.
///
/// Note: <c>prop_tree.png</c> omits the non-approved color <c>#2d5a2d</c> that
/// appears in the sprint plan spec; only palette-approved colors are used.
///
/// Pure C# — no Godot runtime required. Testable with NUnit.
/// </summary>
public static class PropRegistry
{
    /// <summary>Total number of prop sprites defined in S3-02.</summary>
    public const int ExpectedPropCount = 12;

    // ─── Prop definitions ─────────────────────────────────────────────────────

    public static readonly PropConfig Door = new(
        "Cottage Door",
        "prop_door.png",
        width: 48,
        height: 64,
        expectedColorHexCodes: new[] { "5c3d2e", "8b7355", "ffd700" });

    public static readonly PropConfig Well = new(
        "Well",
        "prop_well.png",
        width: 48,
        height: 48,
        expectedColorHexCodes: new[] { "5a5a5a", "8b7355", "1a3a5c", "2d2416" });

    public static readonly PropConfig Tree = new(
        "Deciduous Tree",
        "prop_tree.png",
        width: 48,
        height: 64,
        // #2d5a2d is NOT in the approved palette — omitted per PaletteValidator rules.
        expectedColorHexCodes: new[] { "1a3a1a", "2d2416" });

    public static readonly PropConfig HayBale = new(
        "Hay Bale",
        "prop_haybale.png",
        width: 48,
        height: 32,
        expectedColorHexCodes: new[] { "8b7355", "ffd700", "2d2416" });

    public static readonly PropConfig FencePost = new(
        "Fence Post",
        "prop_fencepost.png",
        width: 16,
        height: 32,
        expectedColorHexCodes: new[] { "5c3d2e", "8b7355" });

    public static readonly PropConfig Barn = new(
        "Farm Barn",
        "prop_barn.png",
        width: 192,
        height: 160,
        expectedColorHexCodes: new[] { "8b0000", "5c3d2e", "8b7355", "2d2416", "3d3d3d", "ffd700" });

    public static readonly PropConfig Scarecrow = new(
        "Scarecrow",
        "prop_scarecrow.png",
        width: 32,
        height: 64,
        expectedColorHexCodes: new[] { "8b7355", "5c3d2e", "ffd700", "2d2416" });

    public static readonly PropConfig Plow = new(
        "Wooden Plow",
        "prop_plow.png",
        width: 48,
        height: 32,
        expectedColorHexCodes: new[] { "5c3d2e", "8b7355", "2d2416", "5a5a5a" });

    public static readonly PropConfig DenseTree = new(
        "Dense Forest Tree",
        "prop_dense_tree.png",
        width: 64,
        height: 96,
        expectedColorHexCodes: new[] { "1a3a1a", "2d2416", "5c3d2e" });

    public static readonly PropConfig FallenLog = new(
        "Fallen Log",
        "prop_fallen_log.png",
        width: 96,
        height: 48,
        expectedColorHexCodes: new[] { "2d2416", "5c3d2e", "8b7355" });

    public static readonly PropConfig Mushrooms = new(
        "Mushrooms",
        "prop_mushrooms.png",
        width: 32,
        height: 24,
        expectedColorHexCodes: new[] { "8b7355", "5a5a5a", "1a3a1a" });

    public static readonly PropConfig Boulder = new(
        "Forest Boulder",
        "prop_boulder.png",
        width: 64,
        height: 64,
        expectedColorHexCodes: new[] { "5a5a5a", "3d3d3d", "1a1a1a" });

    // ─── Collection access ────────────────────────────────────────────────────

    private static readonly PropConfig[] _all =
    {
        Door, Well, Tree, HayBale, FencePost,
        Barn, Scarecrow, Plow,
        DenseTree, FallenLog, Mushrooms, Boulder,
    };

    /// <summary>
    /// Returns all registered prop configurations.
    /// A defensive copy is returned so callers cannot mutate the registry.
    /// </summary>
    public static PropConfig[] All => (PropConfig[])_all.Clone();

    /// <summary>
    /// Looks up a prop by file name. Returns null if not found.
    /// </summary>
    public static PropConfig? GetByFileName(string fileName) =>
        _all.FirstOrDefault(p => p.FileName == fileName);

    /// <summary>
    /// Looks up a prop by display name. Returns null if not found.
    /// </summary>
    public static PropConfig? GetByName(string name) =>
        _all.FirstOrDefault(p => p.Name == name);
}
