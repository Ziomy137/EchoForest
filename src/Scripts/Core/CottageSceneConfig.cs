using System;

namespace EchoForest.Core;

/// <summary>
/// Configuration constants and pure-C# layout rules for the Cottage Exterior
/// test area (S3-03). Defines the 30×20 isometric grid, tile placement priority,
/// prop spawn positions, and scene resource paths.
///
/// The companion class <see cref="CottageAreaNode"/> reads this config in its
/// <c>_Ready()</c> hook to populate the Godot TileMapLayer and prop sprites at
/// runtime — no tile data needs to be baked into the <c>.tscn</c> file.
///
/// Pure C# — no Godot runtime required. Testable with NUnit.
/// </summary>
public static class CottageSceneConfig
{
    // ─── Grid dimensions ──────────────────────────────────────────────────────

    /// <summary>Number of tile columns across the playable area.</summary>
    public const int GridColumns = 30;

    /// <summary>Number of tile rows across the playable area.</summary>
    public const int GridRows = 20;

    /// <summary>Total tiles in the grid (GridColumns × GridRows).</summary>
    public const int TotalCells = GridColumns * GridRows;   // 600

    // ─── Scene resource paths ─────────────────────────────────────────────────

    /// <summary>Godot resource path for the TestArea_Cottage scene.</summary>
    public const string SceneResPath = "res://src/Scenes/TestArea_Cottage.tscn";

    /// <summary>Godot resource path for the shared IsometricTileSet.</summary>
    public const string TileSetResPath = "res://src/Assets/Data/IsometricTileSet.tres";

    // ─── Player spawn point ───────────────────────────────────────────────────
    // Tile coords (col=14, row=12) → world x=(14−12)×32=64, y=(14+12)×16=416

    /// <summary>Column of the player spawn tile.</summary>
    public const int SpawnTileCol = 14;

    /// <summary>Row of the player spawn tile.</summary>
    public const int SpawnTileRow = 12;

    /// <summary>World-space X of the player spawn point.</summary>
    public const float SpawnWorldX = 64f;

    /// <summary>World-space Y of the player spawn point.</summary>
    public const float SpawnWorldY = 416f;

    // ─── Named node paths within the scene ───────────────────────────────────

    /// <summary>Name of the TileMapLayer child node.</summary>
    public const string TileMapLayerName = "TileMapLayer";

    /// <summary>Name of the Marker2D that marks the player spawn position.</summary>
    public const string PlayerSpawnName = "PlayerSpawnPoint";

    /// <summary>Name of the Node2D that holds all prop sprites.</summary>
    public const string PropsNodeName = "Props";

    /// <summary>Name of the boundary StaticBody2D node.</summary>
    public const string BoundaryNodeName = "Boundary";

    /// <summary>Name of the Camera2D node.</summary>
    public const string CameraNodeName = "Camera";

    // ─── Tile file name constants (match TileRegistry FileName values) ────────

    public const string TileGrass = "tile_grass.png";
    public const string TileGrassVar = "tile_grass_var.png";
    public const string TileDirt = "tile_dirt.png";
    public const string TileFarm = "tile_farm.png";
    public const string TileStone = "tile_stone.png";
    public const string TileWall = "tile_wall_front.png";
    public const string TileRoof = "tile_roof.png";
    public const string TileFenceH = "tile_fence_h.png";
    public const string TileFenceV = "tile_fence_v.png";

    // ─── TileSet source IDs ───────────────────────────────────────────────────
    // Must match IsometricTileSet.tres source numbering (sources/0 … sources/10).

    public const int SourceIdGrass = 0;
    public const int SourceIdGrassVar = 1;
    public const int SourceIdDirt = 2;
    public const int SourceIdFarm = 3;
    public const int SourceIdStone = 4;
    public const int SourceIdWater = 5;   // reserved, not painted in this scene
    public const int SourceIdWall = 6;
    public const int SourceIdRoof = 7;
    public const int SourceIdFenceH = 8;
    public const int SourceIdFenceV = 9;
    public const int SourceIdShadow = 10;  // used by IsometricYSorterNode

    // ─── World boundary ───────────────────────────────────────────────────────
    // Derived from grid corners using TileToWorld formula, plus a 32 px buffer.
    //   Top    = tile( 0,  0) → world (   0,   0)
    //   Right  = tile(29,  0) → world ( 928, 464)
    //   Bottom = tile(29, 19) → world ( 320, 768)
    //   Left   = tile( 0, 19) → world (-608, 304)

    /// <summary>Left edge of the playable boundary in world-space pixels.</summary>
    public const float WorldBoundaryLeft = -640f;

    /// <summary>Right edge of the playable boundary in world-space pixels.</summary>
    public const float WorldBoundaryRight = 960f;

    /// <summary>Top edge of the playable boundary in world-space pixels.</summary>
    public const float WorldBoundaryTop = -32f;

    /// <summary>Bottom edge of the playable boundary in world-space pixels.</summary>
    public const float WorldBoundaryBottom = 800f;

    // ─── TileZone ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Axis-aligned rectangle of cells within the isometric grid.
    /// <c>Col</c> and <c>Row</c> are the top-left corner; <c>Width</c> and
    /// <c>Height</c> are measured in tile units.
    /// </summary>
    public readonly record struct TileZone(int Col, int Row, int Width, int Height)
    {
        /// <summary>Returns true when (c, r) falls within this zone.</summary>
        public bool Contains(int c, int r) =>
            c >= Col && c < Col + Width && r >= Row && r < Row + Height;

        /// <summary>Total number of tiles in this zone (Width × Height).</summary>
        public int TileCount => Width * Height;

        /// <summary>Exclusive end column (Col + Width).</summary>
        public int EndCol => Col + Width;

        /// <summary>Exclusive end row (Row + Height).</summary>
        public int EndRow => Row + Height;
    }

    // ─── Zone definitions ─────────────────────────────────────────────────────
    //
    // Cottage (cols 12-15, rows 9-13):
    //   Row  9, cols 12-15 →  4 tiles  roof
    //   Row 10, cols 12-15 →  4 tiles  front wall
    //   Rows 11-13, cols 12-15 → 12 tiles  stone interior floor
    //
    // Farm (top-right, rows 1-5, cols 23-27):
    //   Fence perimeter sits just outside the farm zone: row 0 / row 6 / col 22 / col 28
    //
    // Dirt path runs from south of cottage (row 14) to south edge (row 19).

    public static readonly TileZone RoofZone = new(12, 9, 4, 1);  //  4 — cottage roof
    public static readonly TileZone WallZone = new(12, 10, 4, 1);  //  4 — front wall
    public static readonly TileZone InteriorZone = new(12, 11, 4, 3);  // 12 — stone floor
    public static readonly TileZone FarmZone = new(23, 1, 5, 5);  // 25 — farmland
    public static readonly TileZone FenceNorth = new(23, 0, 5, 1);  //  5 — fence_h top
    public static readonly TileZone FenceSouth = new(23, 6, 5, 1);  //  5 — fence_h bottom
    public static readonly TileZone FenceWest = new(22, 1, 1, 5);  //  5 — fence_v left
    public static readonly TileZone FenceEast = new(28, 1, 1, 5);  //  5 — fence_v right
    public static readonly TileZone DirtPath = new(13, 14, 3, 6);  // 18 — dirt path south

    // ─── PropPlacement ────────────────────────────────────────────────────────

    /// <summary>Position of a prop sprite in tile coordinates.</summary>
    public readonly record struct PropPlacement(string FileName, int Col, int Row);

    /// <summary>Total number of prop instances placed in the scene.</summary>
    public const int PropCount = 12;

    /// <summary>All props placed in the Cottage scene, ordered by prop type.</summary>
    public static readonly PropPlacement[] Props =
    {
        new(PropRegistry.Well.FileName,       10, 11),  // well — left of cottage
        new(PropRegistry.Tree.FileName,        2,  1),  // tree NW corner
        new(PropRegistry.Tree.FileName,        5,  2),  // tree W
        new(PropRegistry.Tree.FileName,        1,  5),  // tree W lower
        new(PropRegistry.Tree.FileName,       29,  0),  // tree N edge
        new(PropRegistry.HayBale.FileName,    22,  8),  // haybale near farm (1)
        new(PropRegistry.HayBale.FileName,    25,  8),  // haybale near farm (2)
        new(PropRegistry.FencePost.FileName,  22,  0),  // fence corner NW
        new(PropRegistry.FencePost.FileName,  28,  0),  // fence corner NE
        new(PropRegistry.FencePost.FileName,  22,  6),  // fence corner SW
        new(PropRegistry.FencePost.FileName,  28,  6),  // fence corner SE
        new(PropRegistry.Door.FileName,       14, 10),  // cottage door (wall row)
    };

    // ─── GetTileFileName ──────────────────────────────────────────────────────

    /// <summary>
    /// Returns the tile sprite file name for the given grid cell.
    /// Priority (highest first):
    ///   Roof → Wall → Interior (stone) → Farm → FenceH (N/S) → FenceV (W/E)
    ///   → Dirt → GrassVar → Grass (default).
    /// </summary>
    public static string GetTileFileName(int col, int row)
    {
        if (RoofZone.Contains(col, row)) return TileRoof;
        if (WallZone.Contains(col, row)) return TileWall;
        if (InteriorZone.Contains(col, row)) return TileStone;
        if (FarmZone.Contains(col, row)) return TileFarm;
        if (FenceNorth.Contains(col, row)) return TileFenceH;
        if (FenceSouth.Contains(col, row)) return TileFenceH;
        if (FenceWest.Contains(col, row)) return TileFenceV;
        if (FenceEast.Contains(col, row)) return TileFenceV;
        if (DirtPath.Contains(col, row)) return TileDirt;
        if (IsGrassVariation(col, row)) return TileGrassVar;
        return TileGrass;
    }

    /// <summary>
    /// Returns the <c>IsometricTileSet.tres</c> source ID for the given tile file name.
    /// Throws <see cref="ArgumentException"/> for unknown file names.
    /// </summary>
    public static int GetSourceId(string tileFileName) => tileFileName switch
    {
        TileGrass => SourceIdGrass,
        TileGrassVar => SourceIdGrassVar,
        TileDirt => SourceIdDirt,
        TileFarm => SourceIdFarm,
        TileStone => SourceIdStone,
        TileWall => SourceIdWall,
        TileRoof => SourceIdRoof,
        TileFenceH => SourceIdFenceH,
        TileFenceV => SourceIdFenceV,
        _ => throw new ArgumentException($"Unknown tile file name: '{tileFileName}'", nameof(tileFileName)),
    };

    /// <summary>
    /// Returns <c>true</c> when the cell should use the grass variation sprite.
    /// Uses a deterministic dithering function so the result is always the same
    /// for the same (col, row) input.
    /// </summary>
    public static bool IsGrassVariation(int col, int row) =>
        (col * 3 + row * 7) % 5 == 0;

    // ─── Coordinate helpers ───────────────────────────────────────────────────
    // Consistent with <see cref="IsometricMath"/>: x=(col−row)×32, y=(col+row)×16.

    /// <summary>World-space X coordinate for a tile at (col, row).</summary>
    public static float TileToWorldX(int col, int row) => (col - row) * 32f;

    /// <summary>World-space Y coordinate for a tile at (col, row).</summary>
    public static float TileToWorldY(int col, int row) => (col + row) * 16f;
}
