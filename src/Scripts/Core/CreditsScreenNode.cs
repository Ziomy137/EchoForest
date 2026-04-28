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
    private CreditsController _ctrl = null!;

    public override void _Ready()
    {
        _ctrl = new CreditsController(new GodotSceneLoader());
        WireBackButton();
    }

    // ── Button wiring ─────────────────────────────────────────────────────────

    private void WireBackButton()
    {
        GetNode<Button>("VBox/BackButton").Pressed += _ctrl.OnBack;
    }
}
