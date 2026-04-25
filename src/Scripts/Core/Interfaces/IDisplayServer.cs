namespace EchoForest.Core;

/// <summary>
/// Abstracts Godot's <c>DisplayServer</c> and <c>Engine</c> APIs so that
/// <see cref="SettingsController"/> can be unit-tested without the Godot runtime.
/// </summary>
public interface IDisplayServer
{
    /// <summary>Applies the given window mode (Windowed / Borderless Fullscreen).</summary>
    void ApplyWindowMode(WindowMode mode);

    /// <summary>Enables or disables vertical sync.</summary>
    void ApplyVSync(bool enabled);

    /// <summary>Sets the FPS cap. Pass <c>0</c> for unlimited.</summary>
    void ApplyFpsLimit(int fps);

    /// <summary>Moves the window to the specified monitor (used in Borderless Fullscreen).</summary>
    void ApplyMonitor(int index);

    /// <summary>Applies brightness post-process (0–200, where 100 = neutral).</summary>
    void ApplyBrightness(float value);

    /// <summary>Applies gamma post-process (0–200, where 100 = neutral).</summary>
    void ApplyGamma(float value);

    /// <summary>Returns the number of connected displays.</summary>
    int GetScreenCount();
}
