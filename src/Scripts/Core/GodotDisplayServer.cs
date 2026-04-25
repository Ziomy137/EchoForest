using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Production <see cref="IDisplayServer"/> backed by Godot's <c>DisplayServer</c>,
/// <c>Engine</c>, and <c>RenderingServer</c> APIs.
///
/// Brightness and Gamma are applied via a persistent full-screen
/// <c>ColorRect</c> overlay at <c>CanvasLayer</c> 1000, using the
/// <c>screen_adjustments.gdshader</c> post-process shader.  The overlay is
/// added to <c>SceneTree.Root</c> so it survives scene transitions.
///
/// Excluded from NUnit code coverage — requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot DisplayServer wrapper — requires Godot runtime")]
public sealed class GodotDisplayServer : IDisplayServer
{
    private const string OverlayNodeName = "ScreenAdjustmentsOverlay";
    private const string ShaderPath = "res://src/Assets/Shaders/screen_adjustments.gdshader";

    // Shared across all GodotDisplayServer instances (one per Settings screen open).
    // The Godot object lives in SceneTree.Root so it's never garbage-collected.
    private static ShaderMaterial? _sharedMat;

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

    public void ApplyBrightness(float value)
    {
        LastBrightness = value;
        GetOrCreateMaterial()?.SetShaderParameter("brightness", value / 100f);
    }

    public void ApplyGamma(float value)
    {
        LastGamma = value;
        GetOrCreateMaterial()?.SetShaderParameter("gamma", value / 100f);
    }

    public int GetScreenCount() => DisplayServer.GetScreenCount();

    // Retained for any SettingsScreenNode code that reads them directly.
    public float LastBrightness { get; private set; } = 100f;
    public float LastGamma { get; private set; } = 100f;

    // ── Overlay management ────────────────────────────────────────────────────

    private static ShaderMaterial? GetOrCreateMaterial()
    {
        if (_sharedMat != null && GodotObject.IsInstanceValid(_sharedMat))
            return _sharedMat;

        if (Engine.GetMainLoop() is not SceneTree tree) return null;

        var shader = GD.Load<Shader>(ShaderPath);
        if (shader is null) return null;

        _sharedMat = new ShaderMaterial { Shader = shader };
        _sharedMat.SetShaderParameter("brightness", SettingsCache.Brightness / 100f);
        _sharedMat.SetShaderParameter("gamma", SettingsCache.Gamma / 100f);

        var rect = new ColorRect { Name = "Rect", Color = Colors.White, Material = _sharedMat }; rect.MouseFilter = Control.MouseFilterEnum.Ignore; // must not block input        rect.AnchorLeft = 0f;
        rect.AnchorTop = 0f;
        rect.AnchorRight = 1f;
        rect.AnchorBottom = 1f;
        rect.GrowHorizontal = Control.GrowDirection.Both;
        rect.GrowVertical = Control.GrowDirection.Both;
        rect.OffsetLeft = rect.OffsetTop = rect.OffsetRight = rect.OffsetBottom = 0f;

        var layer = new CanvasLayer { Name = OverlayNodeName, Layer = 1000 };
        layer.AddChild(rect);

        // Add to Root so the overlay persists across scene changes.
        tree.Root.CallDeferred(Node.MethodName.AddChild, layer);

        return _sharedMat;
    }
}
