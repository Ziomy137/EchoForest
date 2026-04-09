using System;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# camera follow controller. Manages a logical camera position with
/// smooth lerp-based following, configurable bounds clamping, and optional
/// pixel-snap output. Contains no Godot <c>Node</c> inheritance — fully
/// testable with NUnit without the Godot runtime.
///
/// The Godot-coupled counterpart (<see cref="IsometricCameraNode"/>) owns a
/// <c>CameraController</c> instance, calls <see cref="Update"/> every
/// <c>_Process</c> tick, and syncs its own <c>Position</c> from
/// <see cref="Position"/>.
/// </summary>
public sealed class CameraController
{
    // ─── Position ─────────────────────────────────────────────────────────────

    /// <summary>Current logical camera position in world space.</summary>
    public Vector2 Position { get; private set; } = Vector2.Zero;

    // ─── FollowSpeed ──────────────────────────────────────────────────────────

    private float _followSpeed = 5f;

    /// <summary>
    /// Lerp factor applied each second when following a target.
    /// Must be greater than zero.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when set to zero or negative.</exception>
    public float FollowSpeed
    {
        get => _followSpeed;
        set
        {
            if (value <= 0f)
                throw new ArgumentException("FollowSpeed must be greater than zero.", nameof(value));
            _followSpeed = value;
        }
    }

    // ─── Offset ───────────────────────────────────────────────────────────────

    /// <summary>
    /// World-space offset added to the follow target position, useful for
    /// isometric centering adjustments.
    /// </summary>
    public Vector2 Offset { get; set; } = Vector2.Zero;

    // ─── SnapToPixels ─────────────────────────────────────────────────────────

    /// <summary>
    /// When <c>true</c>, <see cref="Position"/> is rounded to integer values
    /// after every <see cref="Update"/> to prevent sub-pixel sprite blurring.
    /// </summary>
    public bool SnapToPixels { get; set; } = false;

    // ─── Target ───────────────────────────────────────────────────────────────

    private Vector2? _target;

    /// <summary>Sets the world-space position the camera should follow.</summary>
    public void SetTarget(Vector2 target) => _target = target;

    /// <summary>Clears the follow target; camera stops moving.</summary>
    public void ClearTarget() => _target = null;

    // ─── Bounds ───────────────────────────────────────────────────────────────

    private Rect2 _bounds;
    private bool _hasBounds;

    /// <summary>
    /// Restricts <see cref="Position"/> to stay inside <paramref name="bounds"/>.
    /// Applied automatically in <see cref="ForcePosition"/> and
    /// <see cref="Update"/>.
    /// </summary>
    public void SetBounds(Rect2 bounds)
    {
        _bounds = bounds;
        _hasBounds = true;
    }

    // ─── ForcePosition ────────────────────────────────────────────────────────

    /// <summary>
    /// Instantly moves the camera to <paramref name="pos"/>, then clamps to
    /// bounds if bounds have been set.
    /// </summary>
    public void ForcePosition(Vector2 pos)
    {
        Position = pos;
        ApplyBounds();
    }

    // ─── ApplyBounds ──────────────────────────────────────────────────────────

    /// <summary>
    /// Clamps <see cref="Position"/> to the configured bounds. No-op when no
    /// bounds have been set via <see cref="SetBounds"/>.
    /// </summary>
    public void ApplyBounds()
    {
        if (!_hasBounds) return;

        float minX = _bounds.Position.X;
        float minY = _bounds.Position.Y;
        float maxX = _bounds.Position.X + _bounds.Size.X;
        float maxY = _bounds.Position.Y + _bounds.Size.Y;

        Position = new Vector2(
            Math.Clamp(Position.X, minX, maxX),
            Math.Clamp(Position.Y, minY, maxY)
        );
    }

    // ─── ApplyPixelSnap ───────────────────────────────────────────────────────

    /// <summary>
    /// Rounds <see cref="Position"/> to the nearest integer coordinates.
    /// Call this directly, or enable <see cref="SnapToPixels"/> to apply
    /// automatically after every <see cref="Update"/>.
    /// </summary>
    public void ApplyPixelSnap()
    {
        Position = new Vector2(
            MathF.Round(Position.X, MidpointRounding.AwayFromZero),
            MathF.Round(Position.Y, MidpointRounding.AwayFromZero)
        );
    }

    // ─── Update ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Advances camera logic by <paramref name="delta"/> seconds. Lerps toward
    /// the follow target (if set), applies bounds, and optionally pixel-snaps.
    /// Call this from a Godot wrapper's <c>_Process</c> override.
    /// </summary>
    public void Update(double delta)
    {
        if (_target != null)
        {
            Vector2 destination = _target.Value + Offset;
            float t = Math.Clamp((float)(_followSpeed * delta), 0f, 1f);
            Position = Position.Lerp(destination, t);
        }

        ApplyBounds();

        if (SnapToPixels)
            ApplyPixelSnap();
    }
}
