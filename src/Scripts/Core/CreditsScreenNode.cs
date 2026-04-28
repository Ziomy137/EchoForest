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
        var scrollContainer = GetNode<ScrollContainer>("VBox/ScrollContainer");
        var label = GetNode<RichTextLabel>("VBox/ScrollContainer/CreditsLabel");

        // Scroll the container from top (0) to bottom of content.
        scrollContainer.ScrollVertical = 0;
        // Defer tween start so layout is complete and GetContentHeight() is accurate.
        CallDeferred(nameof(BeginScrollTween), scrollContainer, label);
    }

    private void BeginScrollTween(ScrollContainer scrollContainer, RichTextLabel label)
    {
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
        GetNode<Button>("VBox/BackButton").Pressed += _ctrl.OnBack;
    }
}
