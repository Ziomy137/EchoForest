using System;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# player movement controller. Contains no Godot Node inheritance and is
/// fully testable with NUnit without the Godot runtime.
///
/// Input is read via <see cref="IInputHandler"/>. Velocity is calculated each
/// simulated frame and used by the coupled Godot node to drive MoveAndSlide().
///
/// Isometric axis mapping:
///   move_right → (+1,  0) raw → top-right screen direction
///   move_left  → (-1,  0) raw → bottom-left screen direction
///   move_down  → ( 0, +1) raw → bottom-right screen direction
///   move_up    → ( 0, -1) raw → top-left screen direction
///
/// Velocity.X / Velocity.Y are used for direction detection and animation selection.
/// </summary>
public sealed class PlayerController : IPlayerController
{
    private readonly IInputHandler _input;
    private readonly PlayerStateMachine _stateMachine;

    // ── IPlayerController ─────────────────────────────────────────────────────

    public Vector2 Velocity { get; private set; } = Vector2.Zero;
    public Direction FacingDirection { get; private set; } = Direction.Down;
    public PlayerState CurrentState => _stateMachine.CurrentState;

    /// <summary>Accumulated world-space position (updated by SimulatePhysicsFrame).</summary>
    public Vector2 Position { get; private set; } = Vector2.Zero;

    // ── Construction ──────────────────────────────────────────────────────────

    public PlayerController(IInputHandler input, PlayerStateMachine stateMachine)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
    }

    // ── Simulation ────────────────────────────────────────────────────────────

    /// <summary>
    /// Advances the controller by one simulated physics frame.
    /// Reads input, updates <see cref="Velocity"/>, <see cref="FacingDirection"/>,
    /// state machine, and integrates <see cref="Position"/> (no collision).
    /// </summary>
    public void SimulatePhysicsFrame(float delta)
    {
        float h = _input.GetAxis(InputActionNames.MoveLeft, InputActionNames.MoveRight);
        float v = _input.GetAxis(InputActionNames.MoveUp, InputActionNames.MoveDown);

        var rawDir = new Vector2(h, v);

        if (rawDir.LengthSquared() > 0f)
        {
            var dir = rawDir.Normalized();
            bool run = _input.IsActionPressed(InputActionNames.Run);
            float speed = run ? Constants.RunSpeed : Constants.WalkSpeed;
            Velocity = dir * speed;
            FacingDirection = DominantDirection(Velocity);
        }
        else
        {
            Velocity = Vector2.Zero;
        }

        UpdateStateMachine();
        Position += Velocity * delta;
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void UpdateStateMachine()
    {
        PlayerState target;

        if (Velocity.LengthSquared() < 0.001f)
        {
            target = PlayerState.Idle;
        }
        else if (_input.IsActionPressed(InputActionNames.Run))
        {
            target = PlayerState.Running;
        }
        else
        {
            target = PlayerState.Walking;
        }

        if (_stateMachine.CurrentState != target &&
            _stateMachine.IsValidTransition(_stateMachine.CurrentState, target))
        {
            _stateMachine.TransitionTo(target);
        }
    }

    // ── Test helpers ──────────────────────────────────────────────────────────

    /// <summary>
    /// Test-only helper: directly sets <see cref="Velocity"/> and updates
    /// <see cref="FacingDirection"/> without going through input handling or
    /// state machine updates. When <paramref name="velocity"/> is zero the
    /// facing direction is intentionally kept unchanged (same as SimulatePhysicsFrame).
    /// </summary>
    internal void SetVelocityForTest(Vector2 velocity)
    {
        Velocity = velocity;
        if (velocity.LengthSquared() > 0f)
            FacingDirection = DominantDirection(velocity);
    }

    private static Direction DominantDirection(Vector2 v)
    {
        if (MathF.Abs(v.X) >= MathF.Abs(v.Y))
            return v.X > 0f ? Direction.Right : Direction.Left;
        return v.Y > 0f ? Direction.Down : Direction.Up;
    }
}
