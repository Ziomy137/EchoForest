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
    private Node? _playerStateSource;
    private bool _hasSearchedForPlayerStateSource;
    private string? _lastPlayerStateName;

    public override void _Ready()
    {
        Layer = 10; // always on top of game world

        CreateTitleLabel();
        _hintLabel = CreateHintLabel();
        _debugLabel = CreateDebugLabel();

        // Use anchors/offsets on the child Controls instead of manually
        // recomputing layout from the viewport size on every resize.
        // Hint — full-width strip 32 px tall, 16 px above the bottom edge
        _hintLabel.SetAnchorsPreset(Control.LayoutPreset.BottomWide);
        _hintLabel.OffsetLeft = 0f;
        _hintLabel.OffsetTop = -48f;
        _hintLabel.OffsetRight = 0f;
        _hintLabel.OffsetBottom = -16f;

        // Debug — 200 px wide, 8 px from right and top
        _debugLabel.SetAnchorsPreset(Control.LayoutPreset.TopRight);
        _debugLabel.OffsetLeft = -208f;
        _debugLabel.OffsetTop = 8f;
        _debugLabel.OffsetRight = -8f;
        _debugLabel.OffsetBottom = 40f;

        _controller.Initialize();
        _controller.SetDebugMode(OS.IsDebugBuild());

        SyncLabels();
    }

    public override void _Process(double delta)
    {
        TrySyncPlayerStateFromScene();

        if (!_controller.IsTutorialHintVisible) return;

        _controller.SimulateTimePassed((float)delta);

        if (_controller.IsTutorialHintVisible)
        {
            SyncLabels();
            return;
        }

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

    private void TrySyncPlayerStateFromScene()
    {
        if (_playerStateSource is not null && !GodotObject.IsInstanceValid(_playerStateSource))
        {
            _playerStateSource = null;
            _hasSearchedForPlayerStateSource = false;
        }

        if (_playerStateSource is null && !_hasSearchedForPlayerStateSource)
        {
            var sceneRoot = GetTree().CurrentScene ?? GetTree().Root;
            _playerStateSource = sceneRoot is null ? null : FindPlayerStateSource(sceneRoot);
            _hasSearchedForPlayerStateSource = true;
        }

        if (_playerStateSource is null) return;

        var stateVariant = _playerStateSource.Get("CurrentState");
        var stateName = stateVariant.ToString();
        if (string.IsNullOrEmpty(stateName) || stateName == _lastPlayerStateName) return;

        _lastPlayerStateName = stateName;
        UpdatePlayerState(stateName);
    }

    private static bool HasProperty(Node node, string propertyName)
    {
        foreach (Godot.Collections.Dictionary property in node.GetPropertyList())
        {
            if (property.TryGetValue("name", out var name) && name?.ToString() == propertyName)
                return true;
        }

        return false;
    }

    private static Node? FindPlayerStateSource(Node root)
    {
        if (HasProperty(root, "CurrentState"))
            return root;

        foreach (Node child in root.GetChildren())
        {
            var match = FindPlayerStateSource(child);
            if (match is not null)
                return match;
        }

        return null;
    }

    // ── UI construction ───────────────────────────────────────────────────────

    private void CreateTitleLabel()
    {
        var label = new Label();
        label.Text = "EchoForest \u2014 Demo Build";
        label.AddThemeColorOverride("font_color", Palette.LightGray);
        // Top-left: fixed position, no anchor required
        label.Position = new Vector2(8f, 8f);
        label.Size = new Vector2(300f, 32f);
        AddChild(label);
    }

    private Label CreateHintLabel()
    {
        var label = new Label();
        label.Text = "WASD / Arrows to move, Shift to run";
        label.AddThemeColorOverride("font_color", Palette.LightGray);
        label.HorizontalAlignment = HorizontalAlignment.Center;
        // Layout is configured later in _Ready via anchors/offsets.
        AddChild(label);
        return label;
    }

    private Label CreateDebugLabel()
    {
        var label = new Label();
        label.Text = "State: Idle";
        label.AddThemeColorOverride("font_color", Palette.Gold);
        label.HorizontalAlignment = HorizontalAlignment.Right;
        // Layout is configured later in _Ready via anchors/offsets.
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
        _hintLabel.Visible = true;

        var tween = CreateTween();
        tween.TweenProperty(_hintLabel, "modulate:a", 0f, 1.0);
        tween.TweenCallback(Callable.From(() => _hintLabel.Visible = false));
    }
}
