using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>
/// In-memory implementation of <see cref="IInputHandler"/> for use in NUnit tests.
/// Allows tests to simulate arbitrary button states without requiring the Godot engine.
/// </summary>
public sealed class MockInputHandler : IInputHandler
{
    private readonly HashSet<string> _pressed = new();
    private readonly HashSet<string> _justPressed = new();

    // ── Control API (used by tests to set up state) ───────────────────────────

    public void SetPressed(string action, bool value)
    {
        if (value) _pressed.Add(action);
        else _pressed.Remove(action);
    }

    public void SetJustPressed(string action, bool value)
    {
        if (value) _justPressed.Add(action);
        else _justPressed.Remove(action);
    }

    /// <summary>Clears all simulated button state.</summary>
    public void Reset()
    {
        _pressed.Clear();
        _justPressed.Clear();
    }

    // ── IInputHandler ─────────────────────────────────────────────────────────

    public bool IsActionPressed(string action) => _pressed.Contains(action);
    public bool IsActionJustPressed(string action) => _justPressed.Contains(action);

    public float GetAxis(string negativeAction, string positiveAction)
    {
        float value = 0f;
        if (_pressed.Contains(positiveAction)) value += 1f;
        if (_pressed.Contains(negativeAction)) value -= 1f;
        return value;
    }
}
