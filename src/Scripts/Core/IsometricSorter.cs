using Godot;

namespace EchoForest.Core;

/// <summary>
/// Static utility class for isometric depth (Z-index) sorting.
///
/// In isometric view, objects with a higher world Y position appear "closer"
/// to the viewer and must render on top of objects with lower Y.
/// Setting <c>ZIndex</c> to the rounded Y coordinate achieves correct draw order.
///
/// Pure C# — no Godot runtime required. Testable with NUnit.
///
/// The Godot-coupled auto-sorting component (<see cref="IsometricYSorterNode"/>)
/// calls this class every frame to update node Z indices.
///
/// Usage:
/// <code>
/// node.ZIndex = IsometricSorter.CalculateZIndex(node.GlobalPosition);
/// </code>
/// </summary>
public static class IsometricSorter
{
    /// <summary>
    /// Computes the Z-index for an object at the given world position.
    /// Higher Y (further down the screen) yields a higher Z index, placing
    /// the object in front of objects with lower Y.
    /// </summary>
    /// <param name="worldPos">World-space position of the object's pivot point.</param>
    /// <returns>Integer Z index — use as <c>Node2D.ZIndex</c>.</returns>
    public static int CalculateZIndex(Vector2 worldPos) =>
        IsometricMath.CalculateZIndex(worldPos);
}
