using Godot;

namespace EchoForest.Core;

/// <summary>
/// Production implementation of <see cref="ITileMapService"/> backed by a Godot
/// <c>TileMapLayer</c> node. Tested via GUT (requires the loaded scene tree).
///
/// Inject a <see cref="MockTileMapService"/> in NUnit tests instead.
///
/// Walkability is determined by a custom boolean data property named
/// <c>"walkable"</c> on the TileSet's custom data layer.
/// </summary>
public sealed class TileMapService : ITileMapService
{
    private readonly TileMapLayer _layer;
    private const string WalkableProperty = "walkable";

    public TileMapService(TileMapLayer layer)
    {
        _layer = layer;
    }

    public Vector2I WorldToTile(Vector2 worldPos) =>
        IsometricMath.WorldToTile(worldPos);

    public Vector2 TileToWorld(Vector2I tilePos) =>
        IsometricMath.TileToWorld(tilePos);

    public bool IsWalkable(Vector2I tilePos)
    {
        var data = _layer.GetCellTileData(tilePos);
        if (data == null) return false;
        return data.GetCustomData(WalkableProperty).AsBool();
    }

    public TileData? GetTileAtPosition(Vector2 worldPos)
    {
        var tilePos = WorldToTile(worldPos);
        return _layer.GetCellTileData(tilePos);
    }
}
