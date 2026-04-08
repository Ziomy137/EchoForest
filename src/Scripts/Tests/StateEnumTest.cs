using System;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// TDD RED: these tests were written before the enums existed.
/// </summary>
[TestFixture]
public class StateEnumTest
{
    [Test]
    public void PlayerState_HasAllRequiredStates()
    {
        Assert.That(Enum.IsDefined(typeof(PlayerState), "Idle"), Is.True, "Missing: Idle");
        Assert.That(Enum.IsDefined(typeof(PlayerState), "Walking"), Is.True, "Missing: Walking");
        Assert.That(Enum.IsDefined(typeof(PlayerState), "Running"), Is.True, "Missing: Running");
        Assert.That(Enum.IsDefined(typeof(PlayerState), "Jumping"), Is.True, "Missing: Jumping");
        Assert.That(Enum.IsDefined(typeof(PlayerState), "Combat"), Is.True, "Missing: Combat");
    }

    [Test]
    public void PlayerState_HasExactlyFiveValues() =>
        Assert.That(Enum.GetValues<PlayerState>().Length, Is.EqualTo(5));

    [Test]
    public void GameState_HasAllRequiredStates()
    {
        Assert.That(Enum.IsDefined(typeof(GameState), "Playing"), Is.True, "Missing: Playing");
        Assert.That(Enum.IsDefined(typeof(GameState), "Paused"), Is.True, "Missing: Paused");
        Assert.That(Enum.IsDefined(typeof(GameState), "Cutscene"), Is.True, "Missing: Cutscene");
    }

    [Test]
    public void GameState_HasExactlyThreeValues() =>
        Assert.That(Enum.GetValues<GameState>().Length, Is.EqualTo(3));

    [Test]
    public void Direction_HasAllFourDirections()
    {
        Assert.That(Enum.IsDefined(typeof(Direction), "Up"), Is.True, "Missing: Up");
        Assert.That(Enum.IsDefined(typeof(Direction), "Down"), Is.True, "Missing: Down");
        Assert.That(Enum.IsDefined(typeof(Direction), "Left"), Is.True, "Missing: Left");
        Assert.That(Enum.IsDefined(typeof(Direction), "Right"), Is.True, "Missing: Right");
    }

    [Test]
    public void Direction_HasExactlyFourValues() =>
        Assert.That(Enum.GetValues<Direction>().Length, Is.EqualTo(4));

    [Test]
    public void PlayerState_DefaultValue_IsIdle() =>
        Assert.That(default(PlayerState), Is.EqualTo(PlayerState.Idle));
}
