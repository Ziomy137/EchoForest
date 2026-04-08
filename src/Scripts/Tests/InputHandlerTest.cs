using System;
using System.Linq;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// TDD RED: written before InputActionNames and MockInputHandler existed.
///
/// Note: InputHandler.cs itself wraps Godot's Input singleton and cannot be
/// instantiated in NUnit without the engine. It is tested via GUT (see
/// test_input_handler.gd). These NUnit tests cover the pure-C# layer:
///   - MockInputHandler (used in all other NUnit tests requiring input)
///   - InputActionNames constants (validates all expected actions are defined)
/// </summary>
[TestFixture]
public class InputHandlerTest
{
    // ── InputActionNames ──────────────────────────────────────────────────────

    [Test]
    public void InputActionNames_ContainsAllRequiredActions()
    {
        var required = new[]
        {
            InputActionNames.MoveUp,
            InputActionNames.MoveDown,
            InputActionNames.MoveLeft,
            InputActionNames.MoveRight,
            InputActionNames.Run,
            InputActionNames.Jump,
            InputActionNames.Interact,
            InputActionNames.Pause,
            InputActionNames.Inventory,
        };

        foreach (var action in required)
            Assert.That(action, Is.Not.Null.And.Not.Empty, $"Action name must not be empty");
    }

    [Test]
    public void InputActionNames_AllValues_AreUnique()
    {
        var all = new[]
        {
            InputActionNames.MoveUp,
            InputActionNames.MoveDown,
            InputActionNames.MoveLeft,
            InputActionNames.MoveRight,
            InputActionNames.Run,
            InputActionNames.Jump,
            InputActionNames.Interact,
            InputActionNames.Pause,
            InputActionNames.Inventory,
        };
        Assert.That(all.Distinct().Count(), Is.EqualTo(all.Length), "Action names must be unique");
    }

    [Test]
    public void InputActionNames_MoveUp_IsCorrectString() =>
        Assert.That(InputActionNames.MoveUp, Is.EqualTo("move_up"));

    [Test]
    public void InputActionNames_MoveDown_IsCorrectString() =>
        Assert.That(InputActionNames.MoveDown, Is.EqualTo("move_down"));

    [Test]
    public void InputActionNames_MoveLeft_IsCorrectString() =>
        Assert.That(InputActionNames.MoveLeft, Is.EqualTo("move_left"));

    [Test]
    public void InputActionNames_MoveRight_IsCorrectString() =>
        Assert.That(InputActionNames.MoveRight, Is.EqualTo("move_right"));

    [Test]
    public void InputActionNames_Run_IsCorrectString() =>
        Assert.That(InputActionNames.Run, Is.EqualTo("run"));

    [Test]
    public void InputActionNames_Jump_IsCorrectString() =>
        Assert.That(InputActionNames.Jump, Is.EqualTo("jump"));

    [Test]
    public void InputActionNames_Interact_IsCorrectString() =>
        Assert.That(InputActionNames.Interact, Is.EqualTo("interact"));

    [Test]
    public void InputActionNames_Pause_IsCorrectString() =>
        Assert.That(InputActionNames.Pause, Is.EqualTo("pause"));

    [Test]
    public void InputActionNames_Inventory_IsCorrectString() =>
        Assert.That(InputActionNames.Inventory, Is.EqualTo("inventory"));

    // ── MockInputHandler ──────────────────────────────────────────────────────

    [Test]
    public void MockInputHandler_ImplementsIInputHandler() =>
        Assert.That(new MockInputHandler(), Is.InstanceOf<IInputHandler>());

    [Test]
    public void MockInputHandler_NoActions_AllReturnFalse()
    {
        var mock = new MockInputHandler();
        Assert.That(mock.IsActionPressed(InputActionNames.MoveRight), Is.False);
        Assert.That(mock.IsActionJustPressed(InputActionNames.MoveRight), Is.False);
    }

    [Test]
    public void MockInputHandler_PressedAction_ReturnsTrue()
    {
        var mock = new MockInputHandler();
        mock.SetPressed(InputActionNames.MoveRight, true);
        Assert.That(mock.IsActionPressed(InputActionNames.MoveRight), Is.True);
    }

    [Test]
    public void MockInputHandler_JustPressedAction_ReturnsTrue()
    {
        var mock = new MockInputHandler();
        mock.SetJustPressed(InputActionNames.Jump, true);
        Assert.That(mock.IsActionJustPressed(InputActionNames.Jump), Is.True);
    }

    [Test]
    public void MockInputHandler_GetAxis_PositiveAction_ReturnsPositiveOne()
    {
        var mock = new MockInputHandler();
        mock.SetPressed(InputActionNames.MoveRight, true);
        float axis = mock.GetAxis(InputActionNames.MoveLeft, InputActionNames.MoveRight);
        Assert.That(axis, Is.EqualTo(1f));
    }

    [Test]
    public void MockInputHandler_GetAxis_NegativeAction_ReturnsNegativeOne()
    {
        var mock = new MockInputHandler();
        mock.SetPressed(InputActionNames.MoveLeft, true);
        float axis = mock.GetAxis(InputActionNames.MoveLeft, InputActionNames.MoveRight);
        Assert.That(axis, Is.EqualTo(-1f));
    }

    [Test]
    public void MockInputHandler_GetAxis_BothPressed_ReturnsZero()
    {
        var mock = new MockInputHandler();
        mock.SetPressed(InputActionNames.MoveLeft, true);
        mock.SetPressed(InputActionNames.MoveRight, true);
        float axis = mock.GetAxis(InputActionNames.MoveLeft, InputActionNames.MoveRight);
        Assert.That(axis, Is.EqualTo(0f));
    }

    [Test]
    public void MockInputHandler_GetAxis_NeitherPressed_ReturnsZero()
    {
        var mock = new MockInputHandler();
        float axis = mock.GetAxis(InputActionNames.MoveLeft, InputActionNames.MoveRight);
        Assert.That(axis, Is.EqualTo(0f));
    }

    [Test]
    public void MockInputHandler_Reset_ClearsAllPressedActions()
    {
        var mock = new MockInputHandler();
        mock.SetPressed(InputActionNames.Run, true);
        mock.SetJustPressed(InputActionNames.Jump, true);
        mock.Reset();
        Assert.That(mock.IsActionPressed(InputActionNames.Run), Is.False);
        Assert.That(mock.IsActionJustPressed(InputActionNames.Jump), Is.False);
    }
}
