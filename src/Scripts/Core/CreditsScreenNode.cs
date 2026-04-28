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
        if (FindChild("ScrollContainer") is not ScrollContainer scrollContainer)
            return;
        if (FindChild("CreditsLabel") is not RichTextLabel label)
            return;

        // Scroll the container from top (0) to bottom of content.
        scrollContainer.ScrollVertical = 0;
        var tween = CreateTween();
        tween.TweenProperty(
            scrollContainer,
            "scroll_vertical",
            label.GetContentHeight(),
            Constants.CreditsScrollDuration);
    }

    // ── Button wiring ─────────────────────────────────────────────────────────

    private void WireBackButton()
    {
        if (FindChild("BackButton") is Button btn)
            btn.Pressed += _ctrl.OnBack;
    }
}
