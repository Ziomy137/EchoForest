namespace EchoForest.Core;

/// <summary>
/// Marker interface for objects that represent a named physics collision layer.
/// Implementations are typically static constants classes (e.g. <c>PhysicsLayers</c>).
/// </summary>
public interface ICollisionLayer
{
    /// <summary>Godot physics layer ID (1-based, matches Project Settings layer name).</summary>
    int LayerId { get; }

    /// <summary>Human-readable layer name matching the Project Settings label.</summary>
    string LayerName { get; }
}
