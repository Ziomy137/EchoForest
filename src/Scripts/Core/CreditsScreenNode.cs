using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>CanvasLayer</c> node for the Credits screen.
///
/// Wires the Back button to <see cref="CreditsController.OnBack"/>.
///
/// All navigation logic lives in the pure-C# <see cref="CreditsController"/>
/// so it can be unit-tested independently of the Godot runtime.
///
/// Excluded from NUnit code coverage — requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot CanvasLayer wrapper — requires scene tree")]
public partial class CreditsScreenNode : CanvasLayer
{
    public override void _Ready()
    {
        GD.Print("[Credits] _Ready() called");
        GD.Print($"[Credits] Children: {GetChildCount()}");

        // List all direct and nested children for debugging
        foreach (var child in GetChildren())
            GD.Print($"[Credits] Child: {child.Name}");

        WireBackButton();
    }

    // ── Button wiring ─────────────────────────────────────────────────────────

    private void WireBackButton()
    {
        var btn = FindChild("BackButton") as Button;
        GD.Print($"[Credits] BackButton found: {btn != null}");
        if (btn != null)
            btn.Pressed += OnBack;
    }

    private void OnBack()
    {
        GD.Print("[Credits] OnBack called, navigating to MainMenu");
        GetTree().ChangeSceneToFile(MainMenuConfig.SceneResPath);
    }
}
