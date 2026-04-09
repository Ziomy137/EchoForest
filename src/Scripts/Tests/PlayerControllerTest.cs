using Godot;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class PlayerControllerTest
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

    // ── Construction ──────────────────────────────────────────────────────────

    [Test]
    public void Constructor_NullInput_ThrowsArgumentNullException()
    {
        Assert.That(() => new PlayerController(null!, _sm),
            Throws.TypeOf<System.ArgumentNullException>());
    }

    [Test]
    public void Constructor_NullStateMachine_ThrowsArgumentNullException()
    {
        Assert.That(() => new PlayerController(_input, null!),
            Throws.TypeOf<System.ArgumentNullException>());
    }

    [Test]
    public void Constructor_InitialState_IsIdle()
    {
        Assert.That(_player.CurrentState, Is.EqualTo(PlayerState.Idle));
    }

    [Test]
    public void Constructor_InitialFacingDirection_IsDown()
    {
        Assert.That(_player.FacingDirection, Is.EqualTo(Direction.Down));
    }

    // ── No input ──────────────────────────────────────────────────────────────

    [Test]
    public void Movement_NoInput_VelocityIsZero()
    {
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.Velocity, Is.EqualTo(Vector2.Zero));
    }

    [Test]
    public void Movement_NoInput_StateRemainsIdle()
    {
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.CurrentState, Is.EqualTo(PlayerState.Idle));
    }

    // ── Walking ───────────────────────────────────────────────────────────────

    [Test]
    public void Movement_MoveRight_VelocityXIsPositive()
    {
        _input.SetPressed(InputActionNames.MoveRight, true);
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.Velocity.X, Is.GreaterThan(0f));
    }

    [Test]
    public void Movement_MoveLeft_VelocityXIsNegative()
    {
        _input.SetPressed(InputActionNames.MoveLeft, true);
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.Velocity.X, Is.LessThan(0f));
    }

    [Test]
    public void Movement_MoveDown_VelocityYIsPositive()
    {
        _input.SetPressed(InputActionNames.MoveDown, true);
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.Velocity.Y, Is.GreaterThan(0f));
    }

    [Test]
    public void Movement_MoveUp_VelocityYIsNegative()
    {
        _input.SetPressed(InputActionNames.MoveUp, true);
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.Velocity.Y, Is.LessThan(0f));
    }

    [Test]
    public void Movement_Cardinal_SpeedEqualsWalkSpeed()
    {
        _input.SetPressed(InputActionNames.MoveRight, true);
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.Velocity.Length(), Is.EqualTo(Constants.WalkSpeed).Within(0.01f));
    }

    // ── Running ───────────────────────────────────────────────────────────────

    [Test]
    public void Movement_RunHeld_SpeedIsRunSpeed()
    {
        _input.SetPressed(InputActionNames.MoveRight, true);
        _input.SetPressed(InputActionNames.Run, true);
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.Velocity.Length(), Is.EqualTo(Constants.RunSpeed).Within(0.01f));
    }

    [Test]
    public void Movement_RunHeld_SpeedIsDoubleWalkSpeed()
    {
        _input.SetPressed(InputActionNames.MoveRight, true);
        _input.SetPressed(InputActionNames.Run, true);
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.Velocity.Length(),
            Is.EqualTo(Constants.WalkSpeed * 2f).Within(0.01f));
    }

    // ── Diagonal normalization ────────────────────────────────────────────────

    [Test]
    public void Movement_DiagonalInput_SpeedEqualsCardinalSpeed()
    {
        _input.SetPressed(InputActionNames.MoveRight, true);
        _input.SetPressed(InputActionNames.MoveUp, true);
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.Velocity.Length(), Is.EqualTo(Constants.WalkSpeed).Within(0.01f));
    }

    [Test]
    public void Movement_DiagonalRunInput_SpeedEqualsRunSpeed()
    {
        _input.SetPressed(InputActionNames.MoveRight, true);
        _input.SetPressed(InputActionNames.MoveDown, true);
        _input.SetPressed(InputActionNames.Run, true);
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.Velocity.Length(), Is.EqualTo(Constants.RunSpeed).Within(0.01f));
    }

    // ── State transitions ─────────────────────────────────────────────────────

    [Test]
    public void State_OnMovement_TransitionsToWalking()
    {
        _input.SetPressed(InputActionNames.MoveRight, true);
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.CurrentState, Is.EqualTo(PlayerState.Walking));
    }

    [Test]
    public void State_RunHeld_TransitionsToRunning()
    {
        _input.SetPressed(InputActionNames.MoveRight, true);
        _input.SetPressed(InputActionNames.Run, true);
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.CurrentState, Is.EqualTo(PlayerState.Running));
    }

    [Test]
    public void State_OnNoInput_AfterMoving_ReturnsToIdle()
    {
        _input.SetPressed(InputActionNames.MoveRight, true);
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.CurrentState, Is.EqualTo(PlayerState.Walking));

        _input.Reset();
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.CurrentState, Is.EqualTo(PlayerState.Idle));
    }

    [Test]
    public void State_OnNoInput_AfterRunning_ReturnsToIdle()
    {
        _input.SetPressed(InputActionNames.MoveRight, true);
        _input.SetPressed(InputActionNames.Run, true);
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.CurrentState, Is.EqualTo(PlayerState.Running));

        _input.Reset();
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.CurrentState, Is.EqualTo(PlayerState.Idle));
    }

    // ── Facing direction ──────────────────────────────────────────────────────

    [Test]
    public void Direction_MoveRight_FacesRight()
    {
        _input.SetPressed(InputActionNames.MoveRight, true);
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.FacingDirection, Is.EqualTo(Direction.Right));
    }

    [Test]
    public void Direction_MoveLeft_FacesLeft()
    {
        _input.SetPressed(InputActionNames.MoveLeft, true);
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.FacingDirection, Is.EqualTo(Direction.Left));
    }

    [Test]
    public void Direction_MoveDown_FacesDown()
    {
        _input.SetPressed(InputActionNames.MoveDown, true);
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.FacingDirection, Is.EqualTo(Direction.Down));
    }

    [Test]
    public void Direction_MoveUp_FacesUp()
    {
        _input.SetPressed(InputActionNames.MoveUp, true);
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.FacingDirection, Is.EqualTo(Direction.Up));
    }

    [Test]
    public void Direction_WhenStopped_RetainsPreviousDirection()
    {
        _input.SetPressed(InputActionNames.MoveRight, true);
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.FacingDirection, Is.EqualTo(Direction.Right));

        _input.Reset();
        _player.SimulatePhysicsFrame(0.016f);
        // Velocity is zero but facing direction must be retained
        Assert.That(_player.FacingDirection, Is.EqualTo(Direction.Right));
    }


    // ── Position integration ──────────────────────────────────────────────────

    [Test]
    public void Position_MovingRight_PositionXIncreasesOverTime()
    {
        _input.SetPressed(InputActionNames.MoveRight, true);
        float startX = _player.Position.X;
        for (int i = 0; i < 10; i++)
            _player.SimulatePhysicsFrame(1f / 60f);
        Assert.That(_player.Position.X, Is.GreaterThan(startX));
    }

    [Test]
    public void Position_NoInput_PositionDoesNotChange()
    {
        var start = _player.Position;
        _player.SimulatePhysicsFrame(0.016f);
        Assert.That(_player.Position, Is.EqualTo(start));
    }

    // ── IPlayerController interface ───────────────────────────────────────────

    [Test]
    public void PlayerController_ImplementsInterface()
    {
        Assert.That(_player, Is.InstanceOf<IPlayerController>());
    }
}
