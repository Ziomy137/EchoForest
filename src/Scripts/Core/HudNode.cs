using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>CanvasLayer</c> that renders the demo HUD.
///
/// Builds all UI children programmatically in <c>_Ready</c> — no separate
/// <c>.tscn</c> sub-resources are required. Three elements are created:
/// <list type="bullet">
///   <item>Top-left static label: "EchoForest — Demo Build"</item>
///   <item>Bottom-center hint label: fades after <see cref="Constants.TutorialHintTimeout"/> seconds</item>
///   <item>Top-right debug label: player state — only visible in debug mode</item>
/// </list>
///
/// State is tracked by the pure-C# <see cref="HudController"/> so logic can
/// be unit-tested independently of the Godot runtime.
///
/// Excluded from NUnit code coverage — requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot CanvasLayer wrapper — requires scene tree")]
public partial class HudNode : CanvasLayer
{
    private readonly HudController _controller = new();

    private Label _hintLabel = null!;
    private Label _debugLabel = null!;

    public override void _Ready()
    {
        Layer = 10; // always on top of game world

        CreateTitleLabel();
        _hintLabel = CreateHintLabel();
        _debugLabel = CreateDebugLabel();

        _controller.Initialize();
        _controller.SetDebugMode(OS.IsDebugBuild());

        SyncLabels();
    }

    public override void _Process(double delta)
    {
        if (!_controller.IsTutorialHintVisible) return;

        _controller.SimulateTimePassed((float)delta);
        SyncLabels();

        if (!_controller.IsTutorialHintVisible)
            FadeOutHint();
    }

    /// <summary>
    /// Called by the player controller node each frame to update the debug label.
    /// </summary>
    public void UpdatePlayerState(string stateName)
    {
        _controller.UpdatePlayerState(stateName);
        _debugLabel.Text = $"State: {_controller.PlayerStateText}";
    }

    // ── UI construction ───────────────────────────────────────────────────────

    private void CreateTitleLabel()
    {
        var label = new Label();
        label.Text = "EchoForest \u2014 Demo Build";
        label.AddThemeColorOverride("font_color", Palette.LightGray);
        label.SetAnchorAndOffset(Side.Left, 0f, 8f);
        label.SetAnchorAndOffset(Side.Top, 0f, 8f);
        AddChild(label);
    }

    private Label CreateHintLabel()
    {
        var label = new Label();
        label.Text = "WASD / Arrows to move, Shift to run";
        label.AddThemeColorOverride("font_color", Palette.LightGray);
        label.HorizontalAlignment = HorizontalAlignment.Center;
        // Anchor bottom-center
        label.SetAnchorsPreset(Control.LayoutPreset.BottomWide);
        label.SetAnchorAndOffset(Side.Bottom, 1f, -16f);
        AddChild(label);
        return label;
    }

    private Label CreateDebugLabel()
    {
        var label = new Label();
        label.Text = "State: Idle";
        label.AddThemeColorOverride("font_color", Palette.Gold);
        label.HorizontalAlignment = HorizontalAlignment.Right;
        label.SetAnchorsPreset(Control.LayoutPreset.TopRight);
        label.SetAnchorAndOffset(Side.Right, 1f, -8f);
        label.SetAnchorAndOffset(Side.Top, 0f, 8f);
        AddChild(label);
        return label;
    }

    // ── Sync ──────────────────────────────────────────────────────────────────

    private void SyncLabels()
    {
        _hintLabel.Visible = _controller.IsTutorialHintVisible;
        _debugLabel.Visible = _controller.IsDebugLabelVisible;
    }

    private void FadeOutHint()
    {
        var tween = CreateTween();
        tween.TweenProperty(_hintLabel, "modulate:a", 0f, 1.0);
        tween.TweenCallback(Callable.From(() => _hintLabel.Visible = false));
    }
}
