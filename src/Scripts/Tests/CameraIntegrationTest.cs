using Godot;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// S4-02 integration tests: camera bounds for the Cottage area and pixel-perfect
/// snapping. Uses only <see cref="CameraController"/> (pure C# — no Godot runtime).
/// </summary>
[TestFixture]
public class CameraIntegrationTest
{
    // ─── North edge bounds ────────────────────────────────────────────────────

    [Test]
    public void Camera_AtNorthEdge_DoesNotExceedBounds()
    {
        var cam = new CameraController();
        cam.SetBounds(new Rect2(0, 0, 1920, 1080));
        cam.ForcePosition(new Vector2(960, -100)); // above bounds
        cam.ApplyBounds();
        Assert.That(cam.Position.Y, Is.GreaterThanOrEqualTo(0f));
    }

    // ─── Pixel-perfect snapping ───────────────────────────────────────────────

    [Test]
    public void Camera_PixelSnapping_PositionIsIntegerAligned()
    {
        var cam = new CameraController { SnapToPixels = true };
        cam.ForcePosition(new Vector2(100.4f, 200.7f));
        cam.ApplyPixelSnap();
        Assert.That(cam.Position.X, Is.EqualTo(100f));
        Assert.That(cam.Position.Y, Is.EqualTo(201f));
    }

    // ─── All four cottage-area edges ──────────────────────────────────────────

    [Test]
    public void Camera_WithCottageBounds_AtNorthEdge_DoesNotExceedBoundsTop()
    {
        var cam = new CameraController();
        cam.SetBounds(CottageSceneConfig.CameraBounds);
        cam.ForcePosition(new Vector2(0f, CottageSceneConfig.WorldBoundaryTop - 500f));
        Assert.That(cam.Position.Y, Is.GreaterThanOrEqualTo(CottageSceneConfig.WorldBoundaryTop));
    }

    [Test]
    public void Camera_WithCottageBounds_AtSouthEdge_DoesNotExceedBoundsBottom()
    {
        var cam = new CameraController();
        cam.SetBounds(CottageSceneConfig.CameraBounds);
        cam.ForcePosition(new Vector2(0f, CottageSceneConfig.WorldBoundaryBottom + 500f));
        Assert.That(cam.Position.Y, Is.LessThanOrEqualTo(CottageSceneConfig.WorldBoundaryBottom));
    }

    [Test]
    public void Camera_WithCottageBounds_AtWestEdge_DoesNotExceedBoundsLeft()
    {
        var cam = new CameraController();
        cam.SetBounds(CottageSceneConfig.CameraBounds);
        cam.ForcePosition(new Vector2(CottageSceneConfig.WorldBoundaryLeft - 500f, 0f));
        Assert.That(cam.Position.X, Is.GreaterThanOrEqualTo(CottageSceneConfig.WorldBoundaryLeft));
    }

    [Test]
    public void Camera_WithCottageBounds_AtEastEdge_DoesNotExceedBoundsRight()
    {
        var cam = new CameraController();
        cam.SetBounds(CottageSceneConfig.CameraBounds);
        cam.ForcePosition(new Vector2(CottageSceneConfig.WorldBoundaryRight + 500f, 0f));
        Assert.That(cam.Position.X, Is.LessThanOrEqualTo(CottageSceneConfig.WorldBoundaryRight));
    }

    // ─── Pixel snap with cottage bounds ───────────────────────────────────────

    [Test]
    public void Camera_SnapToPixels_EliminatesSubPixelPositionInsideCottageBounds()
    {
        var cam = new CameraController { SnapToPixels = true };
        cam.SetBounds(CottageSceneConfig.CameraBounds);
        cam.SetTarget(new Vector2(100.75f, 200.25f));
        cam.Update(1.0); // large delta — arrive near target
        Assert.That(cam.Position.X % 1f, Is.EqualTo(0f).Within(0.001f));
        Assert.That(cam.Position.Y % 1f, Is.EqualTo(0f).Within(0.001f));
    }

    // ─── Cottage bounds dimensions ────────────────────────────────────────────

    [Test]
    public void CottageBounds_Width_MatchesHorizontalWorldBoundarySpan()
    {
        float expectedWidth = CottageSceneConfig.WorldBoundaryRight - CottageSceneConfig.WorldBoundaryLeft;
        Assert.That(CottageSceneConfig.CameraBounds.Size.X, Is.EqualTo(expectedWidth));
    }

    [Test]
    public void CottageBounds_Height_MatchesVerticalWorldBoundarySpan()
    {
        float expectedHeight = CottageSceneConfig.WorldBoundaryBottom - CottageSceneConfig.WorldBoundaryTop;
        Assert.That(CottageSceneConfig.CameraBounds.Size.Y, Is.EqualTo(expectedHeight));
    }

    [Test]
    public void Camera_AtExactBoundaryEdge_PositionIsNotClamped()
    {
        var cam = new CameraController();
        cam.SetBounds(CottageSceneConfig.CameraBounds);
        var edgePosition = new Vector2(CottageSceneConfig.WorldBoundaryLeft, CottageSceneConfig.WorldBoundaryTop);
        cam.ForcePosition(edgePosition);
        Assert.That(cam.Position.X, Is.EqualTo(CottageSceneConfig.WorldBoundaryLeft));
        Assert.That(cam.Position.Y, Is.EqualTo(CottageSceneConfig.WorldBoundaryTop));
    }
}
