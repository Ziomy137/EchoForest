namespace EchoForest.Core;

/// <summary>
/// Abstracts Godot's <c>Input</c> singleton behind a testable interface.
/// Implementations wrap <c>Input.IsActionPressed()</c> etc.
/// Mock implementations are used in NUnit tests.
/// </summary>
public interface IInputHandler
{
    /// <summary>Returns true while the named input action is held.</summary>
    bool IsActionPressed(string action);

    /// <summary>Returns true only on the frame the action was first pressed.</summary>
    bool IsActionJustPressed(string action);

    /// <summary>
    /// Returns a value in [-1, 1] representing the axis formed by
    /// <paramref name="negativeAction"/> (−1) and <paramref name="positiveAction"/> (+1).
    /// </summary>
    float GetAxis(string negativeAction, string positiveAction);
}
