using System.Linq;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// TDD RED: these tests were written before Constants.cs existed.
/// </summary>
[TestFixture]
public class ConstantsTest
{
    // ── Movement ─────────────────────────────────────────────────────────────

    [Test]
    public void WalkSpeed_IsEightyPixelsPerSecond() =>
        Assert.That(Constants.WalkSpeed, Is.EqualTo(80f));

    [Test]
    public void RunSpeed_IsExactlyDoubleWalkSpeed() =>
        Assert.That(Constants.RunSpeed, Is.EqualTo(Constants.WalkSpeed * 2f));

    // ── Tiles ─────────────────────────────────────────────────────────────────

    [Test]
    public void TileWidth_IsSixtyFourPixels() =>
        Assert.That(Constants.TileWidth, Is.EqualTo(64));

    [Test]
    public void TileHeight_IsThirtyTwoPixels() =>
        Assert.That(Constants.TileHeight, Is.EqualTo(32));

    [Test]
    public void TileWidth_IsDoubleTileHeight() =>
        Assert.That(Constants.TileWidth, Is.EqualTo(Constants.TileHeight * 2));

    // ── Physics layers ────────────────────────────────────────────────────────

    [Test]
    public void Layers_World_IsOne() =>
        Assert.That(Constants.Layers.World, Is.EqualTo(1));

    [Test]
    public void Layers_Player_IsTwo() =>
        Assert.That(Constants.Layers.Player, Is.EqualTo(2));

    [Test]
    public void Layers_Npcs_IsThree() =>
        Assert.That(Constants.Layers.Npcs, Is.EqualTo(3));

    [Test]
    public void Layers_Interactables_IsFour() =>
        Assert.That(Constants.Layers.Interactables, Is.EqualTo(4));

    [Test]
    public void Layers_AllValues_AreUnique()
    {
        var layers = new[]
        {
            Constants.Layers.World,
            Constants.Layers.Player,
            Constants.Layers.Npcs,
            Constants.Layers.Interactables,
        };
        Assert.That(layers.Distinct().Count(), Is.EqualTo(layers.Length));
    }

    // ── Rendering ─────────────────────────────────────────────────────────────

    [Test]
    public void TargetFps_IsSixty() =>
        Assert.That(Constants.TargetFps, Is.EqualTo(60));

    [Test]
    public void PixelsPerUnit_IsPositive() =>
        Assert.That(Constants.PixelsPerUnit, Is.GreaterThan(0));

    // ── Layers.All ────────────────────────────────────────────────────────────

    [Test]
    public void Layers_All_ContainsAllFourLayers()
    {
        var all = Constants.Layers.All;
        Assert.That(all, Contains.Item(Constants.Layers.World));
        Assert.That(all, Contains.Item(Constants.Layers.Player));
        Assert.That(all, Contains.Item(Constants.Layers.Npcs));
        Assert.That(all, Contains.Item(Constants.Layers.Interactables));
    }

    [Test]
    public void Layers_All_HasExactlyFourEntries() =>
        Assert.That(Constants.Layers.All, Has.Length.EqualTo(4));

    // ── HUD ───────────────────────────────────────────────────────────────────

    [Test]
    public void TutorialHintTimeout_IsPositive() =>
        Assert.That(Constants.TutorialHintTimeout, Is.GreaterThan(0f));

    // ── Camera ────────────────────────────────────────────────────────────────

    [Test]
    public void CameraFollowSpeed_IsPositive() =>
        Assert.That(Constants.CameraFollowSpeed, Is.GreaterThan(0f));

    // ── Save ──────────────────────────────────────────────────────────────────

    [Test]
    public void SaveSlotCount_IsFive() =>
        Assert.That(Constants.SaveSlotCount, Is.EqualTo(5));

    [Test]
    public void DefaultPlayerHealth_IsOneHundred() =>
        Assert.That(Constants.DefaultPlayerHealth, Is.EqualTo(100f).Within(0.001f));

    // ── Credits ───────────────────────────────────────────────────────────────

    [Test]
    public void CreditsScrollDuration_IsPositive() =>
        Assert.That(Constants.CreditsScrollDuration, Is.GreaterThan(0f));
}
