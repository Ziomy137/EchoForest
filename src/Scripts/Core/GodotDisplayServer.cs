using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Production <see cref="IDisplayServer"/> backed by Godot's <c>DisplayServer</c>,
/// <c>Engine</c>, and <c>RenderingServer</c> APIs.
///
/// Brightness/Gamma are stored so the Godot scene layer can read them and apply
/// them via a full-screen shader or <c>Environment</c> resource.
///
/// Excluded from NUnit code coverage — requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot DisplayServer wrapper — requires Godot runtime")]
public sealed class GodotDisplayServer : IDisplayServer
{
    // ── IDisplayServer ────────────────────────────────────────────────────────

    public void ApplyWindowMode(WindowMode mode)
    {
        if (mode == WindowMode.BorderlessFullscreen)
        {
            var screenSize = DisplayServer.ScreenGetSize();
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
            DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
            DisplayServer.WindowSetSize(screenSize);
        }
        else
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
            DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
        }
    }

    public void ApplyVSync(bool enabled)
    {
        var mode = enabled
            ? DisplayServer.VSyncMode.Enabled
            : DisplayServer.VSyncMode.Disabled;
        DisplayServer.WindowSetVsyncMode(mode);
    }

    public void ApplyFpsLimit(int fps) => Engine.MaxFps = fps;

    public void ApplyMonitor(int index)
    {
        if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen)
        {
            var pos = DisplayServer.ScreenGetPosition(index);
            DisplayServer.WindowSetPosition(pos);
        }
    }

    // Brightness and Gamma values are stored here; the SettingsScreenNode reads
    // them and applies them to the post-process shader.
    public float LastBrightness { get; private set; } = 100f;
    public float LastGamma { get; private set; } = 100f;

    public void ApplyBrightness(float value) => LastBrightness = value;
    public void ApplyGamma(float value) => LastGamma = value;

    public int GetScreenCount() => DisplayServer.GetScreenCount();
}
