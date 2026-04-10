using System;
using System.Linq;

namespace EchoForest.Core.Tiles;

/// <summary>
/// Static registry of all tile sprite assets required for the game's
/// isometric <c>TileSet</c>. Each entry describes the expected file, dimensions,
/// and approved palette colors for that tile.
///
/// All paths resolve to <c>res://src/Assets/Sprites/Tiles/</c>.
/// All tiles conform to 64×32 px isometric diamond format.
/// </summary>
public static class TileRegistry
{
    private const int IsoWidth = 64;
    private const int IsoHeight = 32;
    private const string BasePath = "res://src/Assets/Sprites/Tiles/";

    // ── 11 required tiles ─────────────────────────────────────────────────────

    public static readonly TileDefinition GrassGround = new(
        "GrassGround", BasePath + "tile_grass.png", IsoWidth, IsoHeight,
        new[] { "1a3a1a", "2d5a2d", "4a7a4a" });

    public static readonly TileDefinition GrassVariation = new(
        "GrassVariation", BasePath + "tile_grass_var.png", IsoWidth, IsoHeight,
        new[] { "1a3a1a", "2d5a2d", "4a7a4a", "1a1a1a" });

    public static readonly TileDefinition DirtPath = new(
        "DirtPath", BasePath + "tile_dirt.png", IsoWidth, IsoHeight,
        new[] { "2d2416", "3d3d3d" });

    public static readonly TileDefinition Farmland = new(
        "Farmland", BasePath + "tile_farm.png", IsoWidth, IsoHeight,
        new[] { "2d2416", "8b7355" });

    public static readonly TileDefinition StoneFloor = new(
        "StoneFloor", BasePath + "tile_stone.png", IsoWidth, IsoHeight,
        new[] { "5a5a5a", "8b7355" });

    public static readonly TileDefinition Water = new(
        "Water", BasePath + "tile_water.png", IsoWidth, IsoHeight,
        new[] { "1a3a5c", "5a5a5a" });

    public static readonly TileDefinition CottageWallFront = new(
        "CottageWallFront", BasePath + "tile_wall_front.png", IsoWidth, IsoHeight,
        new[] { "8b7355", "5c3d2e" });

    public static readonly TileDefinition CottageRoof = new(
        "CottageRoof", BasePath + "tile_roof.png", IsoWidth, IsoHeight,
        new[] { "8b0000", "5c3d2e" });

    public static readonly TileDefinition FenceHorizontal = new(
        "FenceHorizontal", BasePath + "tile_fence_h.png", IsoWidth, IsoHeight,
        new[] { "5c3d2e", "8b7355" });

    public static readonly TileDefinition FenceVertical = new(
        "FenceVertical", BasePath + "tile_fence_v.png", IsoWidth, IsoHeight,
        new[] { "5c3d2e", "8b7355" });

    public static readonly TileDefinition Shadow = new(
        "Shadow", BasePath + "tile_shadow.png", IsoWidth, IsoHeight,
        new[] { "1a1a1a" });

    // ── Registry collection ───────────────────────────────────────────────────

    /// <summary>All 11 required tile definitions in declaration order.</summary>
    public static readonly TileDefinition[] All =
    {
        GrassGround,
        GrassVariation,
        DirtPath,
        Farmland,
        StoneFloor,
        Water,
        CottageWallFront,
        CottageRoof,
        FenceHorizontal,
        FenceVertical,
        Shadow,
    };

    // ── Lookup ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the <see cref="TileDefinition"/> with the given <paramref name="name"/>,
    /// or <c>null</c> if no such tile is registered.
    /// </summary>
    public static TileDefinition? Find(string name) =>
        All.FirstOrDefault(t => t.Name == name);
}
