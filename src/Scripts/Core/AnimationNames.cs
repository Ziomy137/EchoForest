using System;

namespace EchoForest.Core;

/// <summary>
/// Animation clip name constants and factory method for player character animations.
/// Names follow the pattern <c>{state}_{direction}</c> matching the AnimatedSprite2D
/// sprite-frames library.<br/>
/// Sprint 2 core: Idle, Walk, Run × 4 directions = 12 clips.
/// </summary>
public static class AnimationNames
{
    // ── Idle ─────────────────────────────────────────────────────────────────

    public const string IdleDown = "idle_down";
    public const string IdleUp = "idle_up";
    public const string IdleLeft = "idle_left";
    public const string IdleRight = "idle_right";

    // ── Walk ─────────────────────────────────────────────────────────────────

    public const string WalkDown = "walk_down";
    public const string WalkUp = "walk_up";
    public const string WalkLeft = "walk_left";
    public const string WalkRight = "walk_right";

    // ── Run ──────────────────────────────────────────────────────────────────

    public const string RunDown = "run_down";
    public const string RunUp = "run_up";
    public const string RunLeft = "run_left";
    public const string RunRight = "run_right";

    // ── Factory ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the animation clip name for a given <paramref name="state"/> and
    /// <paramref name="direction"/> pair.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when an unmapped <paramref name="state"/> or <paramref name="direction"/>
    /// value is passed.
    /// </exception>
    public static string Get(PlayerState state, Direction direction) =>
        $"{GetPrefix(state)}_{GetSuffix(direction)}";

    private static string GetPrefix(PlayerState state) => state switch
    {
        PlayerState.Idle => "idle",
        PlayerState.Walking => "walk",
        PlayerState.Running => "run",
        PlayerState.Jumping => "jump",
        PlayerState.Combat => "combat",
        _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
    };

    private static string GetSuffix(Direction direction) => direction switch
    {
        Direction.Down => "down",
        Direction.Up => "up",
        Direction.Left => "left",
        Direction.Right => "right",
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };
}
