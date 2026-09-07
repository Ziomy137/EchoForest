using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>Camera2D</c> node that drives smooth isometric camera follow
/// using <see cref="CameraController"/>.
///
/// Attach this node to the scene. Assign the <see cref="FollowTarget"/> export
/// to any <c>Node2D</c> in the scene (typically the player). The controller
/// will lerp toward it every frame.
///
/// Excluded from NUnit code coverage because it requires the Godot scene tree.
/// Behaviour is verified in <c>test_isometric_camera.gd</c> (GUT).
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot Camera2D wrapper — tested via GUT")]
public partial class IsometricCameraNode : Camera2D
{
    /// <summary>
    /// The node the camera will follow. Leave <c>null</c> to disable following.
    /// </summary>
    [Export]
    public Node2D? FollowTarget { get; set; }

    /// <summary>
    /// Camera lerp speed (units per second). Must be greater than zero.
    /// </summary>
    private float _followSpeed = Constants.CameraFollowSpeed;

    [Export]
    public float FollowSpeed
    {
        get => _followSpeed;
        set
        {
            _followSpeed = value;
            SyncControllerSettings();
        }
    }

    /// <summary>
    /// World-space offset applied to the follow target position. Useful for
    /// adjusting vertical centering in isometric perspective.
    /// </summary>
    private Vector2 _followOffset = Vector2.Zero;

    [Export]
    public Vector2 FollowOffset
    {
        get => _followOffset;
        set
        {
            _followOffset = value;
            SyncControllerSettings();
        }
    }

    /// <summary>
    /// When enabled, camera position is snapped to integer pixels every frame
    /// to eliminate sub-pixel sprite blurring during movement.
    /// </summary>
    private bool _snapToPixels = false;

    [Export]
    public bool SnapToPixels
    {
        get => _snapToPixels;
        set
        {
            _snapToPixels = value;
            SyncControllerSettings();
        }
    }

    private CameraController _controller = new();
    private Tween? _cutscenePanTween;
    private bool _isCutscenePanning;

    private void SyncControllerSettings()
    {
        _controller.FollowSpeed = FollowSpeed > 0f ? FollowSpeed : Constants.CameraFollowSpeed;
        _controller.Offset = FollowOffset;
        _controller.SnapToPixels = SnapToPixels;
    }

    public override void _Ready()
    {
        SyncControllerSettings();
        _controller.ForcePosition(GlobalPosition);
    }

    /// <summary>
    /// Restricts the camera to <paramref name="bounds"/> in world space,
    /// preventing it from panning into empty areas beyond the tile grid.
    /// </summary>
    public void SetBounds(Rect2 bounds) => _controller.SetBounds(bounds);

    /// <summary>
    /// Immediately snaps the camera to <see cref="FollowTarget"/>'s position,
    /// bypassing the lerp. Call this after repositioning the follow target at
    /// scene load time to avoid an initial catch-up pan.
    /// </summary>
    public void SnapToTarget()
    {
        if (FollowTarget != null)
            _controller.ForcePosition(FollowTarget.GlobalPosition + FollowOffset);
        GlobalPosition = _controller.Position;
    }

    /// <summary>Pans the camera while temporarily suspending follow updates.</summary>
    public void PanTo(Vector2 target, float duration, System.Action onComplete)
    {
        System.ArgumentNullException.ThrowIfNull(onComplete);
        if (duration < 0f)
            throw new System.ArgumentOutOfRangeException(nameof(duration));

        _cutscenePanTween?.Kill();
        _isCutscenePanning = true;
        if (duration == 0f)
        {
            GlobalPosition = target;
            FinishCutscenePan(onComplete);
            return;
        }

        _cutscenePanTween = CreateTween();
        _cutscenePanTween.TweenProperty(this, "global_position", target, duration);
        _cutscenePanTween.Finished += () => FinishCutscenePan(onComplete);
    }

    public override void _Process(double delta)
    {
        if (_isCutscenePanning)
            return;

        if (FollowTarget != null)
            _controller.SetTarget(FollowTarget.GlobalPosition);
        else
            _controller.ClearTarget();

        _controller.Update(delta);
        GlobalPosition = _controller.Position;
    }

    private void FinishCutscenePan(System.Action onComplete)
    {
        _controller.ForcePosition(GlobalPosition);
        _isCutscenePanning = false;
        onComplete();
    }
}
