namespace EchoForest.Core.Scenes;

/// <summary>
/// Pure-data configuration for the Cottage Exterior test area scene.
/// Centralises layout constants so the Godot scene, collision setup, and tests
/// all reference a single source of truth.
/// </summary>
public static class TestAreaConfig
{
    // ── Scene resource path ───────────────────────────────────────────────────

    /// <summary>Godot resource path to the test area scene.</summary>
    public const string ScenePath = "res://src/Scenes/TestArea_Cottage.tscn";

    // ── Grid dimensions ───────────────────────────────────────────────────────

    /// <summary>Horizontal tile count of the playable area.</summary>
    public const int GridWidth = 30;

    /// <summary>Vertical tile count of the playable area.</summary>
    public const int GridHeight = 20;

    // ── Player spawn ──────────────────────────────────────────────────────────

    /// <summary>Tile-column index of the player spawn point (stone-yard centre).</summary>
    public const int SpawnTileX = 15;

    /// <summary>Tile-row index of the player spawn point.</summary>
    public const int SpawnTileY = 12;

    // ── Required scene node names ─────────────────────────────────────────────

    /// <summary>Name of the TileMapLayer node inside the scene.</summary>
    public const string TileMapLayerNodeName = "TileMapLayer";

    /// <summary>Name of the player spawn point Marker2D node.</summary>
    public const string PlayerSpawnPointNodeName = "PlayerSpawnPoint";

    /// <summary>Name of the collision boundary StaticBody2D node.</summary>
    public const string CollisionBoundaryNodeName = "CollisionBoundary";

    /// <summary>
    /// An ordered list of all node names that MUST exist in the scene tree
    /// for the test area to be considered correctly set up.
    /// </summary>
    public static readonly string[] RequiredNodeNames =
    {
        TileMapLayerNodeName,
        PlayerSpawnPointNodeName,
        CollisionBoundaryNodeName,
    };

    // ── Zone tile counts ──────────────────────────────────────────────────────

    /// <summary>Number of farm-zone tiles (5×5 block of tile_farm).</summary>
    public const int FarmZoneTileCount = 25;

    /// <summary>Number of cottage-floor tiles (4×4 block of tile_stone).</summary>
    public const int CottageFloorTileCount = 16;
}
