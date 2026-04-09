using System;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# isometric coordinate math for a 64×32 pixel tile grid.
/// No Godot scene tree required — fully testable in NUnit.
///
/// Coordinate system:
///   Tile (col, row) → screen position where:
///     - Increasing col moves right and down (isometric right)
///     - Increasing row moves left and down (isometric left)
///
/// Formulas:
///   TileToWorld:  x = (col - row) * (TileWidth  / 2)
///                 y = (col + row) * (TileHeight / 2)
///
///   WorldToTile:  col = floor((x / halfW + y / halfH) / 2)
///                 row = floor((y / halfH - x / halfW) / 2)
/// </summary>
public static class IsometricMath
{
    private static readonly float HalfWidth = Constants.TileWidth / 2f; // 32
    private static readonly float HalfHeight = Constants.TileHeight / 2f; // 16

    /// <summary>
    /// Converts an isometric tile coordinate to its world-space center position.
    /// </summary>
    public static Vector2 TileToWorld(Vector2I tilePos)
    {
        float x = (tilePos.X - tilePos.Y) * HalfWidth;
        float y = (tilePos.X + tilePos.Y) * HalfHeight;
        return new Vector2(x, y);
    }

    /// <summary>
    /// Converts a world-space position to its containing isometric tile coordinate.
    /// Uses floor-snapping (Math.Floor) so each position deterministically maps
    /// to the tile whose origin is at or below-left of the given point.
    /// </summary>
    public static Vector2I WorldToTile(Vector2 worldPos)
    {
        float col = worldPos.X / HalfWidth + worldPos.Y / HalfHeight;
        float row = worldPos.Y / HalfHeight - worldPos.X / HalfWidth;
        return new Vector2I(
            (int)Math.Floor(col / 2f),
            (int)Math.Floor(row / 2f)
        );
    }

    /// <summary>
    /// Returns the Z-index for depth sorting.
    /// Higher Y (further down the screen) = higher Z index = drawn on top.
    /// </summary>
    public static int CalculateZIndex(Vector2 worldPos) =>
        (int)Math.Round(worldPos.Y);
}
