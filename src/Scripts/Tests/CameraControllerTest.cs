using System;
using Godot;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class CameraControllerTest
{
    // ─── Constructor & defaults ───────────────────────────────────────────────

    [Test]
    public void Camera_DefaultPosition_IsZero()
    {
        var cam = new CameraController();
        Assert.That(cam.Position, Is.EqualTo(Vector2.Zero));
    }

    [Test]
    public void Camera_DefaultFollowSpeed_IsPositive()
    {
        var cam = new CameraController();
        Assert.That(cam.FollowSpeed, Is.GreaterThan(0f));
    }

    // ─── FollowSpeed validation ───────────────────────────────────────────────

    [Test]
    public void FollowSpeed_ZeroValue_ThrowsArgumentException()
    {
        var cam = new CameraController();
        Assert.That(() => cam.FollowSpeed = 0f, Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void FollowSpeed_NegativeValue_ThrowsArgumentException()
    {
        var cam = new CameraController();
        Assert.That(() => cam.FollowSpeed = -1f, Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void FollowSpeed_ValidValue_IsApplied()
    {
        var cam = new CameraController();
        cam.FollowSpeed = 3.5f;
        Assert.That(cam.FollowSpeed, Is.EqualTo(3.5f));
    }

    // ─── ForcePosition ────────────────────────────────────────────────────────

    [Test]
    public void ForcePosition_NoBounds_SetsPositionExactly()
    {
        var cam = new CameraController();
        cam.ForcePosition(new Vector2(100f, 200f));
        Assert.That(cam.Position, Is.EqualTo(new Vector2(100f, 200f)));
    }

    [Test]
    public void ForcePosition_WithBounds_ClampsPosition()
    {
        var cam = new CameraController();
        cam.SetBounds(new Rect2(0, 0, 500, 500));
        cam.ForcePosition(new Vector2(1000f, 1000f));
        Assert.That(cam.Position.X, Is.LessThanOrEqualTo(500f));
        Assert.That(cam.Position.Y, Is.LessThanOrEqualTo(500f));
    }

    [Test]
    public void ForcePosition_WithBounds_DoesNotGoBelowMinimum()
    {
        var cam = new CameraController();
        cam.SetBounds(new Rect2(100, 100, 400, 400));
        cam.ForcePosition(new Vector2(-50f, -50f));
        Assert.That(cam.Position.X, Is.GreaterThanOrEqualTo(100f));
        Assert.That(cam.Position.Y, Is.GreaterThanOrEqualTo(100f));
    }

    // ─── SetBounds / ApplyBounds ──────────────────────────────────────────────

    [Test]
    public void SetBounds_ClampsCameraPosition()
    {
        var cam = new CameraController();
        cam.SetBounds(new Rect2(0, 0, 500, 500));
        cam.ForcePosition(new Vector2(1000f, 1000f));
        Assert.That(cam.Position.X, Is.LessThanOrEqualTo(500f));
        Assert.That(cam.Position.Y, Is.LessThanOrEqualTo(500f));
    }

    [Test]
    public void ApplyBounds_WhenNoBoundsSet_DoesNotThrow()
    {
        var cam = new CameraController();
        cam.ForcePosition(new Vector2(9999f, 9999f));
        Assert.That(() => cam.ApplyBounds(), Throws.Nothing);
    }

    [Test]
    public void ApplyBounds_ClampsAllFourEdges()
    {
        var cam = new CameraController();
        cam.SetBounds(new Rect2(10, 20, 200, 300));

        // Beyond right/bottom
        cam.ForcePosition(new Vector2(9999f, 9999f));
        Assert.That(cam.Position.X, Is.LessThanOrEqualTo(210f));
        Assert.That(cam.Position.Y, Is.LessThanOrEqualTo(320f));

        // Beyond left/top — use ApplyBounds directly after setting raw position via method
        var cam2 = new CameraController();
        cam2.SetBounds(new Rect2(10, 20, 200, 300));
        cam2.ForcePosition(new Vector2(-999f, -999f));
        Assert.That(cam2.Position.X, Is.GreaterThanOrEqualTo(10f));
        Assert.That(cam2.Position.Y, Is.GreaterThanOrEqualTo(20f));
    }

    // ─── PixelSnap ───────────────────────────────────────────────────────────

    [Test]
    public void ApplyPixelSnap_RoundsPositionDown()
    {
        var cam = new CameraController();
        cam.ForcePosition(new Vector2(100.4f, 200.2f));
        cam.ApplyPixelSnap();
        Assert.That(cam.Position.X, Is.EqualTo(100f));
        Assert.That(cam.Position.Y, Is.EqualTo(200f));
    }

    [Test]
    public void ApplyPixelSnap_RoundsPositionUp()
    {
        var cam = new CameraController();
        cam.ForcePosition(new Vector2(100.6f, 200.7f));
        cam.ApplyPixelSnap();
        Assert.That(cam.Position.X, Is.EqualTo(101f));
        Assert.That(cam.Position.Y, Is.EqualTo(201f));
    }

    // ─── Update (pure logic) ─────────────────────────────────────────────────

    [Test]
    public void Camera_WithoutTarget_Update_DoesNotThrow()
    {
        var cam = new CameraController();
        Assert.That(() => cam.Update(0.016), Throws.Nothing);
    }

    [Test]
    public void Camera_WithoutTarget_Update_PositionStaysZero()
    {
        var cam = new CameraController();
        cam.Update(0.016);
        Assert.That(cam.Position, Is.EqualTo(Vector2.Zero));
    }

    [Test]
    public void Camera_WithTarget_Update_MovesPositionTowardTarget()
    {
        var cam = new CameraController();
        cam.SetTarget(new Vector2(200f, 300f));
        cam.Update(0.016);
        // After one small step, position has moved away from origin
        Assert.That(cam.Position.X, Is.GreaterThan(0f));
        Assert.That(cam.Position.Y, Is.GreaterThan(0f));
    }

    [Test]
    public void Camera_WithTarget_Update_HighDelta_SnapClose()
    {
        var cam = new CameraController();
        cam.FollowSpeed = 100f; // very fast
        cam.SetTarget(new Vector2(500f, 500f));
        cam.Update(1.0); // large delta — should be very close to target
        Assert.That(cam.Position.X, Is.GreaterThan(400f));
        Assert.That(cam.Position.Y, Is.GreaterThan(400f));
    }

    [Test]
    public void Camera_Offset_ShiftsFollowTarget()
    {
        var cam = new CameraController();
        cam.Offset = new Vector2(50f, 0f);
        cam.SetTarget(new Vector2(0f, 0f));
        // Target + offset = (50, 0). Camera should move toward (50, 0).
        cam.FollowSpeed = 100f;
        cam.Update(1.0);
        // Position should be pulled toward the offset, not origin
        Assert.That(cam.Position.X, Is.GreaterThan(0f));
    }

    [Test]
    public void ClearTarget_StopsFollowing()
    {
        var cam = new CameraController();
        cam.SetTarget(new Vector2(500f, 500f));
        cam.ClearTarget();
        cam.Update(0.016);
        Assert.That(cam.Position, Is.EqualTo(Vector2.Zero));
    }

    [Test]
    public void SnapToPixels_True_PositionIsIntegerAfterUpdate()
    {
        var cam = new CameraController();
        cam.SnapToPixels = true;
        cam.SetTarget(new Vector2(99.9f, 150.3f));
        cam.Update(0.016);
        // After snap, X and Y should be whole numbers
        Assert.That(cam.Position.X % 1f, Is.EqualTo(0f));
        Assert.That(cam.Position.Y % 1f, Is.EqualTo(0f));
    }
}
