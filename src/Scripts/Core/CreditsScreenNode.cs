using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>CanvasLayer</c> node for the Credits screen.
///
/// Starts the auto-scroll tween and wires the Back button to
/// <see cref="CreditsController.OnBack"/>.
///
/// All navigation logic lives in the pure-C# <see cref="CreditsController"/>
/// so it can be unit-tested independently of the Godot runtime.
///
/// Excluded from NUnit code coverage — requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot CanvasLayer wrapper — requires scene tree")]
public partial class CreditsScreenNode : CanvasLayer
{
    private const float ScrollDuration = 20f; // seconds for full scroll

    private CreditsController _ctrl = null!;

    public override void _Ready()
    {
        _ctrl = new CreditsController(new GodotSceneLoader());
        StartAutoScroll();
        WireBackButton();
    }

    // ── Scroll ────────────────────────────────────────────────────────────────

    private void StartAutoScroll()
    {
        if (FindChild("CreditsLabel") is not RichTextLabel label)
            return;

        // Tween scrolls the RichTextLabel from position 0 to fully off-screen.
        var tween = CreateTween();
        tween.TweenProperty(
            label,
            "position:y",
            -label.Size.Y,
            ScrollDuration);
    }

    // ── Button wiring ─────────────────────────────────────────────────────────

    private void WireBackButton()
    {
        if (FindChild("BackButton") is Button btn)
            btn.Pressed += _ctrl.OnBack;
    }
}
