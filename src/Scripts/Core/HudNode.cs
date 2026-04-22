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

        // Full-viewport Control — all labels anchor relative to this so that
        // percentage-based anchors (e.g. bottom = 1.0) resolve to the actual
        // viewport size rather than being treated as raw pixel offsets.
        // CanvasLayer is not a Control, so its child Control has no implicit
        // parent size — we must set it explicitly and update on window resize.
        var root = new Control();
        root.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        root.MouseFilter = Control.MouseFilterEnum.Ignore;
        AddChild(root);
        root.Size = GetViewport().GetVisibleRect().Size;
        GetViewport().SizeChanged += () => root.Size = GetViewport().GetVisibleRect().Size;

        CreateTitleLabel(root);
        _hintLabel = CreateHintLabel(root);
        _debugLabel = CreateDebugLabel(root);

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

    private void CreateTitleLabel(Control root)
    {
        var label = new Label();
        label.Text = "EchoForest \u2014 Demo Build";
        label.AddThemeColorOverride("font_color", Palette.LightGray);
        // Top-left: all four sides anchored to top-left corner with small margins
        label.SetAnchorAndOffset(Side.Left, 0f, 8f);
        label.SetAnchorAndOffset(Side.Top, 0f, 8f);
        label.SetAnchorAndOffset(Side.Right, 0f, 300f);
        label.SetAnchorAndOffset(Side.Bottom, 0f, 40f);
        root.AddChild(label);
    }

    private Label CreateHintLabel(Control root)
    {
        var label = new Label();
        label.Text = "WASD / Arrows to move, Shift to run";
        label.AddThemeColorOverride("font_color", Palette.LightGray);
        label.HorizontalAlignment = HorizontalAlignment.Center;
        // Bottom-center: stretch full width, sit 48→16 px above the bottom edge
        label.SetAnchorAndOffset(Side.Left, 0f, 0f);
        label.SetAnchorAndOffset(Side.Right, 1f, 0f);
        label.SetAnchorAndOffset(Side.Top, 1f, -48f);
        label.SetAnchorAndOffset(Side.Bottom, 1f, -16f);
        root.AddChild(label);
        return label;
    }

    private Label CreateDebugLabel(Control root)
    {
        var label = new Label();
        label.Text = "State: Idle";
        label.AddThemeColorOverride("font_color", Palette.Gold);
        label.HorizontalAlignment = HorizontalAlignment.Right;
        // Top-right: 200 px wide, anchored to the right edge with 8 px margin
        label.SetAnchorAndOffset(Side.Left, 1f, -208f);
        label.SetAnchorAndOffset(Side.Right, 1f, -8f);
        label.SetAnchorAndOffset(Side.Top, 0f, 8f);
        label.SetAnchorAndOffset(Side.Bottom, 0f, 40f);
        root.AddChild(label);
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
