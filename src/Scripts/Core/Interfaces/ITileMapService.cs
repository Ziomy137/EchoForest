using Godot;

namespace EchoForest.Core;

/// <summary>
/// Provides isometric grid coordinate queries.
/// Decouples game logic from the concrete Godot <c>TileMap</c> node.
/// Tested via GUT (requires loaded TileMap scene).
/// </summary>
public interface ITileMapService
{
    /// <summary>Converts a world-space position to the nearest isometric tile coordinate.</summary>
    Vector2I WorldToTile(Vector2 worldPos);

    /// <summary>Converts an isometric tile coordinate to its world-space center position.</summary>
    Vector2 TileToWorld(Vector2I tilePos);

    /// <summary>Returns true if the tile at <paramref name="tilePos"/> can be walked on.</summary>
    bool IsWalkable(Vector2I tilePos);

    /// <summary>
    /// Returns the tile metadata at the given world position,
    /// or <c>null</c> if the position is outside map bounds.
    /// </summary>
    TileData? GetTileAtPosition(Vector2 worldPos);
}
