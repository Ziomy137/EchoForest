using Godot;

namespace EchoForest.Core;

/// <summary>
/// Contract for the player movement and state system.
/// Tested via GUT (requires Godot scene tree).
/// </summary>
public interface IPlayerController
{
    /// <summary>Current pixel velocity applied each physics frame.</summary>
    Vector2 Velocity { get; }

    /// <summary>The last direction the player was (or is) moving.</summary>
    Direction FacingDirection { get; }

    /// <summary>Current animation/movement state.</summary>
    PlayerState CurrentState { get; }

    /// <summary>
    /// Advances the controller by one simulated physics frame of <paramref name="delta"/> seconds.
    /// Used in tests that decouple from Godot's physics loop.
    /// </summary>
    void SimulatePhysicsFrame(float delta);
}
