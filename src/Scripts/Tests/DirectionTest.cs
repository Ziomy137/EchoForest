using Godot;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// Unit tests for direction tracking via <see cref="PlayerController.SetVelocityForTest"/>.
/// Covers <see cref="Direction"/> enum mapping from raw velocity vectors.
/// </summary>
[TestFixture]
public class DirectionTest
{
    private MockInputHandler _input = null!;
    private PlayerStateMachine _sm = null!;
    private PlayerController _player = null!;

    [SetUp]
    public void SetUp()
    {
        _input = new MockInputHandler();
        _sm = new PlayerStateMachine();
        _player = new PlayerController(_input, _sm);
    }

    // ── Four cardinal directions ──────────────────────────────────────────────

    [TestCase(80f, 0f, Direction.Right, TestName = "Direction_PositiveX_FacesRight")]
    [TestCase(-80f, 0f, Direction.Left, TestName = "Direction_NegativeX_FacesLeft")]
    [TestCase(0f, 80f, Direction.Down, TestName = "Direction_PositiveY_FacesDown")]
    [TestCase(0f, -80f, Direction.Up, TestName = "Direction_NegativeY_FacesUp")]
    public void Direction_FromVelocity_IsCorrect(float vx, float vy, Direction expected)
    {
        _player.SetVelocityForTest(new Vector2(vx, vy));

        Assert.That(_player.FacingDirection, Is.EqualTo(expected));
    }

    // ── Diagonal tiebreaker: horizontal wins when |X| >= |Y| ─────────────────

    [TestCase(80f, 40f, Direction.Right, TestName = "Direction_DiagonalXDominates_FacesRight")]
    [TestCase(-80f, 40f, Direction.Left, TestName = "Direction_DiagonalXNeg_FacesLeft")]
    [TestCase(40f, 80f, Direction.Down, TestName = "Direction_DiagonalYDominates_FacesDown")]
    [TestCase(40f, -80f, Direction.Up, TestName = "Direction_DiagonalYNeg_FacesUp")]
    public void Direction_DiagonalVelocity_DominantAxisWins(float vx, float vy, Direction expected)
    {
        _player.SetVelocityForTest(new Vector2(vx, vy));

        Assert.That(_player.FacingDirection, Is.EqualTo(expected));
    }

    // ── Equal magnitude: horizontal axis wins ────────────────────────────────

    [Test]
    public void Direction_EqualMagnitudeXY_HorizontalAxisWins()
    {
        _player.SetVelocityForTest(new Vector2(80f, 80f));

        Assert.That(_player.FacingDirection, Is.EqualTo(Direction.Right));
    }

    // ── Zero velocity retains previous direction ──────────────────────────────

    [Test]
    public void Direction_ZeroVelocityAfterRight_RetainsRight()
    {
        _player.SetVelocityForTest(new Vector2(80f, 0f));
        Assert.That(_player.FacingDirection, Is.EqualTo(Direction.Right));

        _player.SetVelocityForTest(Vector2.Zero);

        Assert.That(_player.FacingDirection, Is.EqualTo(Direction.Right));
    }

    // ── Velocity is stored correctly ──────────────────────────────────────────

    [Test]
    public void SetVelocityForTest_SetsVelocityProperty()
    {
        var v = new Vector2(50f, -30f);
        _player.SetVelocityForTest(v);

        Assert.That(_player.Velocity, Is.EqualTo(v));
    }

    [Test]
    public void SetVelocityForTest_ZeroVelocity_SetsVelocityToZero()
    {
        _player.SetVelocityForTest(new Vector2(80f, 0f));
        _player.SetVelocityForTest(Vector2.Zero);

        Assert.That(_player.Velocity, Is.EqualTo(Vector2.Zero));
    }
}
