using Godot;

namespace EchoForest.Core;

/// <summary>In-memory player controller test double for NUnit interaction tests.</summary>
public sealed class MockPlayerController : IPlayerController
{
    public Vector2 Velocity { get; set; }
    public Direction FacingDirection { get; set; } = Direction.Down;
    public PlayerState CurrentState { get; set; } = PlayerState.Idle;
    public int SimulatedFrameCount { get; private set; }

    public void SimulatePhysicsFrame(float delta)
    {
        SimulatedFrameCount++;
    }
}