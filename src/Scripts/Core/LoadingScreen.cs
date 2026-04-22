namespace EchoForest.Core;

/// <summary>
/// Pure-C# model tracking the visibility state of the loading screen overlay.
///
/// Godot-decoupled — fully testable with NUnit without the Godot runtime.
/// The Godot-coupled counterpart (<see cref="LoadingScreenNode"/>) owns an
/// instance of this class and syncs its actual <c>Visible</c> property from
/// <see cref="IsVisible"/>.
/// </summary>
public sealed class LoadingScreen
{
    /// <summary>
    /// Whether the loading screen overlay should currently be visible.
    /// Starts <c>false</c> — the screen is hidden until explicitly shown.
    /// </summary>
    public bool IsVisible { get; private set; } = false;

    /// <summary>Shows the loading screen.</summary>
    public void Show() => IsVisible = true;

    /// <summary>Hides the loading screen.</summary>
    public void Hide() => IsVisible = false;
}
