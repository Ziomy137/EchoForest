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
        WireBackButton();
    }

    // ── Button wiring ─────────────────────────────────────────────────────────

    private void WireBackButton()
    {
        if (FindChild("BackButton") is Button btn)
            btn.Pressed += OnBack;
        else
            GD.PrintErr("[CreditsScreenNode] BackButton not found in scene tree");
    }

    private void OnBack()
    {
        new GodotSceneLoader().LoadScene(MainMenuConfig.SceneResPath);
    }
}
