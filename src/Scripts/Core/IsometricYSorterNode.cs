using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>Node2D</c> component that automatically updates <c>ZIndex</c>
/// every frame based on the node's global Y position.
///
/// Attach this node as a child of any game object (player, NPC, prop) that
/// needs correct isometric depth sorting.  The parent node must have
/// <c>ZIndex</c> mode set to relative (default) so this node's sorted value
/// propagates upward correctly.
///
/// Excluded from NUnit code coverage because it requires the Godot scene tree.
/// Behaviour is verified in <c>test_isometric_y_sorter.gd</c> (GUT).
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot Node2D wrapper — tested via GUT")]
public partial class IsometricYSorterNode : Node2D
{
    /// <summary>
    /// When <c>true</c> (default), <c>ZIndex</c> is recalculated every
    /// <c>_Process</c> tick.  Set to <c>false</c> to pause sorting (e.g.
    /// while a death animation plays and the sprite should stay in place).
    /// </summary>
    [Export]
    public bool AutoSort { get; set; } = true;

    public override void _Process(double delta)
    {
        if (!AutoSort) return;
        ZIndex = IsometricSorter.CalculateZIndex(GlobalPosition);
    }
}
