using System.Diagnostics.CodeAnalysis;
using Godot;
using EchoForest.Core;

namespace EchoForest;

/// <summary>
/// Production implementation of <see cref="IInputHandler"/> backed by Godot's
/// <c>Input</c> singleton. Tested via GUT (requires the Godot scene tree).
///
/// Pass a <see cref="MockInputHandler"/> in NUnit tests instead.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class InputHandler : IInputHandler
{
    public bool IsActionPressed(string action) =>
        Input.IsActionPressed(action);

    public bool IsActionJustPressed(string action) =>
        Input.IsActionJustPressed(action);

    public float GetAxis(string negativeAction, string positiveAction) =>
        Input.GetAxis(negativeAction, positiveAction);
}
