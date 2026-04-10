using System.Linq;
using NUnit.Framework;
using EchoForest.Core;
using EchoForest.Core.Sprites;

namespace EchoForest.Tests;

/// <summary>
/// Tests for <see cref="PlayerSpriteSheet"/> and <see cref="AnimationClipDefinition"/>.
/// Validates sheet dimensions, frame counts, clip names, and frame ranges
/// without requiring the Godot runtime.
/// </summary>
[TestFixture]
public class PlayerSpriteSheetTest
{
    // ── Sheet dimensions ──────────────────────────────────────────────────────

    [Test]
    public void PlayerSpriteSheet_SheetWidth_Is640()
    {
        Assert.That(PlayerSpriteSheet.SheetWidth, Is.EqualTo(640));
    }

    [Test]
    public void PlayerSpriteSheet_SheetHeight_Is96()
    {
        Assert.That(PlayerSpriteSheet.SheetHeight, Is.EqualTo(96));
    }

    [Test]
    public void PlayerSpriteSheet_TotalFrames_Is40()
    {
        Assert.That(PlayerSpriteSheet.TotalFrames, Is.EqualTo(40));
    }

    [Test]
    public void PlayerSpriteSheet_FrameWidth_Is16()
    {
        Assert.That(PlayerSpriteSheet.FrameWidth, Is.EqualTo(16));
    }

    [Test]
    public void PlayerSpriteSheet_FrameHeight_Is24()
    {
        Assert.That(PlayerSpriteSheet.FrameHeight, Is.EqualTo(24));
    }

    // ── Clip count ────────────────────────────────────────────────────────────

    [Test]
    public void PlayerSpriteSheet_HasTwelveClips()
    {
        Assert.That(PlayerSpriteSheet.Clips, Has.Length.EqualTo(12));
    }

    [Test]
    public void PlayerSpriteSheet_AllClipNames_AreUnique()
    {
        var names = PlayerSpriteSheet.Clips.Select(c => c.Name).ToArray();
        Assert.That(names.Distinct().Count(), Is.EqualTo(names.Length));
    }

    // ── All 12 required clips exist ───────────────────────────────────────────

    [TestCase("idle_down")]
    [TestCase("idle_up")]
    [TestCase("idle_left")]
    [TestCase("idle_right")]
    [TestCase("walk_down")]
    [TestCase("walk_up")]
    [TestCase("walk_left")]
    [TestCase("walk_right")]
    [TestCase("run_down")]
    [TestCase("run_up")]
    [TestCase("run_left")]
    [TestCase("run_right")]
    public void PlayerSpriteSheet_Contains_RequiredClip(string clipName)
    {
        Assert.That(PlayerSpriteSheet.Clips.Select(c => c.Name), Does.Contain(clipName));
    }

    // ── Clip names match AnimationNames constants ──────────────────────────────

    [Test]
    public void PlayerSpriteSheet_ClipNames_MatchAnimationNameConstants()
    {
        var defined = new[]
        {
            AnimationNames.IdleDown,  AnimationNames.IdleUp,
            AnimationNames.IdleLeft,  AnimationNames.IdleRight,
            AnimationNames.WalkDown,  AnimationNames.WalkUp,
            AnimationNames.WalkLeft,  AnimationNames.WalkRight,
            AnimationNames.RunDown,   AnimationNames.RunUp,
            AnimationNames.RunLeft,   AnimationNames.RunRight,
        };

        foreach (var name in defined)
            Assert.That(PlayerSpriteSheet.Clips.Select(c => c.Name), Does.Contain(name),
                $"Missing clip for AnimationNames.{name}");
    }

    // ── Frame range validity ──────────────────────────────────────────────────

    [Test]
    public void PlayerSpriteSheet_AllClips_StartFrameIsNonNegative()
    {
        foreach (var clip in PlayerSpriteSheet.Clips)
            Assert.That(clip.StartFrame, Is.GreaterThanOrEqualTo(0),
                $"{clip.Name}: StartFrame must be >= 0");
    }

    [Test]
    public void PlayerSpriteSheet_AllClips_EndFrameWithinTotalFrames()
    {
        foreach (var clip in PlayerSpriteSheet.Clips)
            Assert.That(clip.StartFrame + clip.FrameCount, Is.LessThanOrEqualTo(PlayerSpriteSheet.TotalFrames),
                $"{clip.Name}: StartFrame + FrameCount must not exceed TotalFrames");
    }

    [Test]
    public void PlayerSpriteSheet_IdleClips_HaveTwoFrames()
    {
        var idleClips = PlayerSpriteSheet.Clips.Where(c => c.Name.StartsWith("idle_"));
        foreach (var clip in idleClips)
            Assert.That(clip.FrameCount, Is.EqualTo(2), $"{clip.Name}: idle clip must have 2 frames");
    }

    [Test]
    public void PlayerSpriteSheet_WalkClips_HaveFourFrames()
    {
        var walkClips = PlayerSpriteSheet.Clips.Where(c => c.Name.StartsWith("walk_"));
        foreach (var clip in walkClips)
            Assert.That(clip.FrameCount, Is.EqualTo(4), $"{clip.Name}: walk clip must have 4 frames");
    }

    [Test]
    public void PlayerSpriteSheet_RunClips_HaveFourFrames()
    {
        var runClips = PlayerSpriteSheet.Clips.Where(c => c.Name.StartsWith("run_"));
        foreach (var clip in runClips)
            Assert.That(clip.FrameCount, Is.EqualTo(4), $"{clip.Name}: run clip must have 4 frames");
    }

    // ── Fps is positive ───────────────────────────────────────────────────────

    [Test]
    public void PlayerSpriteSheet_AllClips_FpsIsPositive()
    {
        foreach (var clip in PlayerSpriteSheet.Clips)
            Assert.That(clip.Fps, Is.GreaterThan(0f), $"{clip.Name}: Fps must be > 0");
    }

    // ── Path validity ─────────────────────────────────────────────────────────

    [Test]
    public void PlayerSpriteSheet_Path_StartsWithResProtocol()
    {
        Assert.That(PlayerSpriteSheet.Path, Does.StartWith("res://"));
    }

    [Test]
    public void PlayerSpriteSheet_Path_EndsWithPng()
    {
        Assert.That(PlayerSpriteSheet.Path, Does.EndWith(".png"));
    }

    // ── FindClip() ────────────────────────────────────────────────────────────

    [Test]
    public void PlayerSpriteSheet_FindClip_ReturnsCorrectClip()
    {
        var clip = PlayerSpriteSheet.FindClip("idle_down");

        Assert.That(clip, Is.Not.Null);
        Assert.That(clip!.Name, Is.EqualTo("idle_down"));
        Assert.That(clip.StartFrame, Is.EqualTo(0));
        Assert.That(clip.FrameCount, Is.EqualTo(2));
    }

    [Test]
    public void PlayerSpriteSheet_FindClip_UnknownName_ReturnsNull()
    {
        var clip = PlayerSpriteSheet.FindClip("does_not_exist");

        Assert.That(clip, Is.Null);
    }

    // ── AnimationClipDefinition record ────────────────────────────────────────

    [Test]
    public void AnimationClipDefinition_EqualRecords_AreEqual()
    {
        var a = new AnimationClipDefinition("idle_down", 0, 2, 4f);
        var b = new AnimationClipDefinition("idle_down", 0, 2, 4f);

        Assert.That(a, Is.EqualTo(b));
    }

    [Test]
    public void AnimationClipDefinition_DifferentName_AreNotEqual()
    {
        var a = new AnimationClipDefinition("idle_down", 0, 2, 4f);
        var b = new AnimationClipDefinition("idle_up", 0, 2, 4f);

        Assert.That(a, Is.Not.EqualTo(b));
    }
}
