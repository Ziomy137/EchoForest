using System;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// Tests for <see cref="PlayerAnimationController"/>, <see cref="AnimationNames"/>,
/// and <see cref="MockAnimatedSprite"/>.
/// </summary>
[TestFixture]
public class AnimationControllerTest
{
    private MockAnimatedSprite _sprite = null!;
    private PlayerAnimationController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _sprite = new MockAnimatedSprite();
        _controller = new PlayerAnimationController(_sprite);
    }

    // ── Construction ──────────────────────────────────────────────────────────

    [Test]
    public void Constructor_NullSprite_ThrowsArgumentNullException()
    {
        Assert.That(() => new PlayerAnimationController(null!),
            Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void CurrentAnimationName_BeforeAnyUpdate_IsEmpty()
    {
        Assert.That(_controller.CurrentAnimationName, Is.EqualTo(string.Empty));
    }

    // ── Idle-down explicit assertion (plan requirement) ───────────────────────

    [Test]
    public void Animation_IdleDown_PlaysCorrectClip()
    {
        _controller.UpdateAnimation(PlayerState.Idle, Direction.Down);

        Assert.That(_sprite.LastPlayedAnimation, Is.EqualTo("idle_down"));
    }

    // ── Core 12 combinations (3 states × 4 directions) ───────────────────────

    [TestCase(PlayerState.Idle, Direction.Down, "idle_down", TestName = "Animation_IdleDown_Correct")]
    [TestCase(PlayerState.Idle, Direction.Up, "idle_up", TestName = "Animation_IdleUp_Correct")]
    [TestCase(PlayerState.Idle, Direction.Left, "idle_left", TestName = "Animation_IdleLeft_Correct")]
    [TestCase(PlayerState.Idle, Direction.Right, "idle_right", TestName = "Animation_IdleRight_Correct")]
    [TestCase(PlayerState.Walking, Direction.Down, "walk_down", TestName = "Animation_WalkDown_Correct")]
    [TestCase(PlayerState.Walking, Direction.Up, "walk_up", TestName = "Animation_WalkUp_Correct")]
    [TestCase(PlayerState.Walking, Direction.Left, "walk_left", TestName = "Animation_WalkLeft_Correct")]
    [TestCase(PlayerState.Walking, Direction.Right, "walk_right", TestName = "Animation_WalkRight_Correct")]
    [TestCase(PlayerState.Running, Direction.Down, "run_down", TestName = "Animation_RunDown_Correct")]
    [TestCase(PlayerState.Running, Direction.Up, "run_up", TestName = "Animation_RunUp_Correct")]
    [TestCase(PlayerState.Running, Direction.Left, "run_left", TestName = "Animation_RunLeft_Correct")]
    [TestCase(PlayerState.Running, Direction.Right, "run_right", TestName = "Animation_RunRight_Correct")]
    public void Animation_AllCoreCombinations_MapCorrectly(
        PlayerState state, Direction dir, string expected)
    {
        _controller.UpdateAnimation(state, dir);

        Assert.That(_controller.CurrentAnimationName, Is.EqualTo(expected));
    }

    // ── Extended state coverage (Jumping / Combat) ────────────────────────────

    [TestCase(PlayerState.Jumping, Direction.Down, "jump_down", TestName = "Animation_JumpDown_Correct")]
    [TestCase(PlayerState.Jumping, Direction.Up, "jump_up", TestName = "Animation_JumpUp_Correct")]
    [TestCase(PlayerState.Jumping, Direction.Left, "jump_left", TestName = "Animation_JumpLeft_Correct")]
    [TestCase(PlayerState.Jumping, Direction.Right, "jump_right", TestName = "Animation_JumpRight_Correct")]
    [TestCase(PlayerState.Combat, Direction.Down, "combat_down", TestName = "Animation_CombatDown_Correct")]
    [TestCase(PlayerState.Combat, Direction.Up, "combat_up", TestName = "Animation_CombatUp_Correct")]
    [TestCase(PlayerState.Combat, Direction.Left, "combat_left", TestName = "Animation_CombatLeft_Correct")]
    [TestCase(PlayerState.Combat, Direction.Right, "combat_right", TestName = "Animation_CombatRight_Correct")]
    public void Animation_ExtendedStates_MapCorrectly(
        PlayerState state, Direction dir, string expected)
    {
        _controller.UpdateAnimation(state, dir);

        Assert.That(_controller.CurrentAnimationName, Is.EqualTo(expected));
    }

    // ── No restart when same animation is already playing ─────────────────────

    [Test]
    public void Animation_SameStateAgain_DoesNotRestart()
    {
        _controller.UpdateAnimation(PlayerState.Idle, Direction.Down);
        int playCount = _sprite.PlayCallCount;

        _controller.UpdateAnimation(PlayerState.Idle, Direction.Down);

        Assert.That(_sprite.PlayCallCount, Is.EqualTo(playCount));
    }

    [Test]
    public void Animation_SpriteAlreadyPlayingClip_DoesNotRestartOnFirstUpdate()
    {
        // Sprite already playing "idle_down" before the controller calls UpdateAnimation
        _sprite.Play("idle_down");
        int playCount = _sprite.PlayCallCount;

        _controller.UpdateAnimation(PlayerState.Idle, Direction.Down);

        Assert.That(_sprite.PlayCallCount, Is.EqualTo(playCount));
    }

    // ── Play IS called when animation changes ─────────────────────────────────

    [Test]
    public void Animation_DifferentState_CallsPlayAgain()
    {
        _controller.UpdateAnimation(PlayerState.Idle, Direction.Down);
        int playCount = _sprite.PlayCallCount;

        _controller.UpdateAnimation(PlayerState.Walking, Direction.Down);

        Assert.That(_sprite.PlayCallCount, Is.EqualTo(playCount + 1));
    }

    [Test]
    public void Animation_SameStateNewDirection_CallsPlayAgain()
    {
        _controller.UpdateAnimation(PlayerState.Idle, Direction.Down);
        int playCount = _sprite.PlayCallCount;

        _controller.UpdateAnimation(PlayerState.Idle, Direction.Right);

        Assert.That(_sprite.PlayCallCount, Is.EqualTo(playCount + 1));
    }

    // ── MockAnimatedSprite internals ──────────────────────────────────────────

    [Test]
    public void MockSprite_AfterTwoPlays_CurrentAnimationIsLatest()
    {
        _sprite.Play("idle_down");
        _sprite.Play("walk_right");

        Assert.That(_sprite.CurrentAnimation, Is.EqualTo("walk_right"));
    }

    [Test]
    public void MockSprite_PlayCallCount_TracksEveryCall()
    {
        _sprite.Play("idle_down");
        _sprite.Play("idle_down");

        Assert.That(_sprite.PlayCallCount, Is.EqualTo(2));
    }

    // ── AnimationNames invalid enum guards ─────────────────────────────────────

    [Test]
    public void AnimationNames_Get_InvalidPlayerState_ThrowsArgumentOutOfRangeException()
    {
        Assert.That(() => AnimationNames.Get((PlayerState)99, Direction.Down),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void AnimationNames_Get_InvalidDirection_ThrowsArgumentOutOfRangeException()
    {
        Assert.That(() => AnimationNames.Get(PlayerState.Idle, (Direction)99),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    // ── AnimationNames constants mirror Get() ─────────────────────────────────

    [Test]
    public void AnimationNames_Constants_MatchGetMethod()
    {
        Assert.Multiple(() =>
        {
            Assert.That(AnimationNames.IdleDown, Is.EqualTo(AnimationNames.Get(PlayerState.Idle, Direction.Down)));
            Assert.That(AnimationNames.IdleUp, Is.EqualTo(AnimationNames.Get(PlayerState.Idle, Direction.Up)));
            Assert.That(AnimationNames.IdleLeft, Is.EqualTo(AnimationNames.Get(PlayerState.Idle, Direction.Left)));
            Assert.That(AnimationNames.IdleRight, Is.EqualTo(AnimationNames.Get(PlayerState.Idle, Direction.Right)));
            Assert.That(AnimationNames.WalkDown, Is.EqualTo(AnimationNames.Get(PlayerState.Walking, Direction.Down)));
            Assert.That(AnimationNames.WalkUp, Is.EqualTo(AnimationNames.Get(PlayerState.Walking, Direction.Up)));
            Assert.That(AnimationNames.WalkLeft, Is.EqualTo(AnimationNames.Get(PlayerState.Walking, Direction.Left)));
            Assert.That(AnimationNames.WalkRight, Is.EqualTo(AnimationNames.Get(PlayerState.Walking, Direction.Right)));
            Assert.That(AnimationNames.RunDown, Is.EqualTo(AnimationNames.Get(PlayerState.Running, Direction.Down)));
            Assert.That(AnimationNames.RunUp, Is.EqualTo(AnimationNames.Get(PlayerState.Running, Direction.Up)));
            Assert.That(AnimationNames.RunLeft, Is.EqualTo(AnimationNames.Get(PlayerState.Running, Direction.Left)));
            Assert.That(AnimationNames.RunRight, Is.EqualTo(AnimationNames.Get(PlayerState.Running, Direction.Right)));
        });
    }
}
