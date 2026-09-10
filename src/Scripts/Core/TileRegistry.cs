using System.Linq;

namespace EchoForest.Core;

/// <summary>
/// Central registry of all tile sprite assets used by area scenes.
///
/// Every registered tile is configured here with
/// its metadata, expected palette colors, and walkability flag.
///
/// Pure C# — no Godot runtime required. Testable with NUnit.
/// </summary>
public static class TileRegistry
{
    // ─── Tile dimension constants ─────────────────────────────────────────────

    /// <summary>Standard isometric tile width (pixels).</summary>
    public const int TileWidth = 64;

    /// <summary>Standard isometric tile height (pixels).</summary>
    public const int TileHeight = 32;

    /// <summary>Total number of tile sprites required for S3-01.</summary>
    public const int ExpectedTileCount = 20;

    // ─── Tile definitions ─────────────────────────────────────────────────────

    public static readonly TileConfig Grass = new(
        "Grass Ground", "tile_grass.png", TileWidth, TileHeight, true,
        new[] { "1a3a1a", "2d2416" });

    public static readonly TileConfig GrassVariation = new(
        "Grass Variation", "tile_grass_var.png", TileWidth, TileHeight, true,
        new[] { "1a3a1a", "1a1a1a" });

    public static readonly TileConfig Dirt = new(
        "Dirt Path", "tile_dirt.png", TileWidth, TileHeight, true,
        new[] { "2d2416", "3d3d3d" });

    public static readonly TileConfig Farmland = new(
        "Farmland", "tile_farm.png", TileWidth, TileHeight, true,
        new[] { "2d2416", "8b7355" });

    public static readonly TileConfig Stone = new(
        "Stone Floor", "tile_stone.png", TileWidth, TileHeight, true,
        new[] { "5a5a5a", "8b7355" });

    public static readonly TileConfig Water = new(
        "Water", "tile_water.png", TileWidth, TileHeight, false,
        new[] { "1a3a5c", "5a5a5a" });

    public static readonly TileConfig CottageWall = new(
        "Cottage Wall", "tile_wall_front.png", TileWidth, TileHeight, false,
        new[] { "8b7355", "5c3d2e" });

    public static readonly TileConfig CottageRoof = new(
        "Cottage Roof", "tile_roof.png", TileWidth, TileHeight, false,
        new[] { "8b0000", "5c3d2e" });

    public static readonly TileConfig FenceHorizontal = new(
        "Fence H", "tile_fence_h.png", TileWidth, TileHeight, false,
        new[] { "5c3d2e", "8b7355" });

    public static readonly TileConfig FenceVertical = new(
        "Fence V", "tile_fence_v.png", TileWidth, TileHeight, false,
        new[] { "5c3d2e", "8b7355" });

    public static readonly TileConfig Shadow = new(
        "Shadow", "tile_shadow.png", TileWidth, TileHeight, true,
        new[] { "1a1a1a" });

    public static readonly TileConfig CropYoung = new(
        "Young Crop", "tile_crop_young.png", TileWidth, TileHeight, true,
        new[] { "1a3a1a", "8b7355", "2d2416" });

    public static readonly TileConfig CropMature = new(
        "Mature Crop", "tile_crop_mature.png", TileWidth, TileHeight, true,
        new[] { "1a3a1a", "ffd700", "8b7355", "2d2416" });

    public static readonly TileConfig SoilDry = new(
        "Dry Soil", "tile_soil_dry.png", TileWidth, TileHeight, true,
        new[] { "2d2416", "8b7355", "3d3d3d" });

    public static readonly TileConfig Irrigation = new(
        "Irrigation", "tile_irrigation.png", TileWidth, TileHeight, true,
        new[] { "1a3a5c", "5a5a5a", "2d2416" });

    public static readonly TileConfig Hay = new(
        "Hay Ground", "tile_hay.png", TileWidth, TileHeight, true,
        new[] { "8b7355", "ffd700", "2d2416" });

    public static readonly TileConfig ForestFloor = new(
        "Forest Floor", "tile_forest_floor.png", TileWidth, TileHeight, true,
        new[] { "1a3a1a", "2d2416", "3d3d3d" });

    public static readonly TileConfig ForestShadow = new(
        "Forest Shadow", "tile_forest_shadow.png", TileWidth, TileHeight, true,
        new[] { "1a1a1a", "1a3a1a", "2d2416" });

    public static readonly TileConfig Mud = new(
        "Forest Mud", "tile_mud.png", TileWidth, TileHeight, true,
        new[] { "2d2416", "3d3d3d", "8b7355" });

    public static readonly TileConfig RockSmall = new(
        "Small Rock Ground", "tile_rock_small.png", TileWidth, TileHeight, true,
        new[] { "5a5a5a", "3d3d3d", "2d2416" });

    // ─── Collection access ────────────────────────────────────────────────────

    private static readonly TileConfig[] _all =
    {
        Grass, GrassVariation, Dirt, Farmland, Stone,
        Water, CottageWall, CottageRoof,
        FenceHorizontal, FenceVertical, Shadow,
        CropYoung, CropMature, SoilDry, Irrigation, Hay,
        ForestFloor, ForestShadow, Mud, RockSmall,
    };

    /// <summary>
    /// Returns all registered tile configurations.
    /// A defensive copy is returned so callers cannot mutate the registry.
    /// </summary>
    public static TileConfig[] All => (TileConfig[])_all.Clone();

    /// <summary>
    /// Returns only tiles marked as walkable.
    /// </summary>
    public static TileConfig[] Walkable => _all.Where(t => t.IsWalkable).ToArray();

    /// <summary>
    /// Returns only tiles marked as non-walkable (blocking).
    /// </summary>
    public static TileConfig[] Blocking => _all.Where(t => !t.IsWalkable).ToArray();

    /// <summary>
    /// Looks up a tile by file name. Returns null if not found.
    /// </summary>
    public static TileConfig? GetByFileName(string fileName) =>
        _all.FirstOrDefault(t => t.FileName == fileName);

    /// <summary>
    /// Looks up a tile by display name. Returns null if not found.
    /// </summary>
    public static TileConfig? GetByName(string name) =>
        _all.FirstOrDefault(t => t.Name == name);
}
