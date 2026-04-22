using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>CanvasLayer</c> that renders the loading screen overlay.
///
/// Builds its UI children programmatically in <c>_Ready</c> — no separate
/// <c>.tscn</c> file is required. A solid black background and a centered
/// "Loading…" label are shown while scenes are transitioning.
///
/// Visibility is tracked by the pure-C# <see cref="LoadingScreen"/> model so
/// state changes can be unit-tested independently of the Godot runtime.
///
/// Excluded from NUnit code coverage — requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot CanvasLayer wrapper — requires scene tree")]
public partial class LoadingScreenNode : CanvasLayer
{
    private readonly LoadingScreen _model = new();

    public override void _Ready()
    {
        Layer = 127; // render on top of everything

        var background = new ColorRect();
        background.Color = new Color(0f, 0f, 0f, 1f);
        background.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        AddChild(background);

        var label = new Label();
        label.Text = "Loading\u2026";
        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.VerticalAlignment = VerticalAlignment.Center;
        label.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        label.AddThemeColorOverride("font_color", Palette.LightGray);
        AddChild(label);

        SyncVisibility();
    }

    /// <summary>Shows the loading overlay.</summary>
    public new void Show()
    {
        _model.Show();
        SyncVisibility();
    }

    /// <summary>Hides the loading overlay.</summary>
    public new void Hide()
    {
        _model.Hide();
        SyncVisibility();
    }

    /// <summary>Whether the overlay is currently visible.</summary>
    public bool IsOverlayVisible => _model.IsVisible;

    private void SyncVisibility() => Visible = _model.IsVisible;
}
