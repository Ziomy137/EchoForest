namespace EchoForest.Core;

/// <summary>
/// Plain data class that holds all user-configurable graphics settings.
/// Serialised to JSON by <see cref="ConfigService"/> and stored at
/// <c>user://settings.json</c>.
///
/// Pure C# — no Godot runtime required, fully testable with NUnit.
/// </summary>
public sealed class UserConfig
{
    private const int DefaultMonitorIndex = 0;
    private const float DefaultBrightness = 100f;
    private const float DefaultGamma = 100f;

    public WindowMode WindowMode { get; set; } = WindowMode.Windowed;
    public int MonitorIndex { get; set; } = DefaultMonitorIndex;
    public int FpsLimit { get; set; } = Constants.TargetFps;
    public bool VSync { get; set; } = false;
    public float Brightness { get; set; } = DefaultBrightness;
    public float Gamma { get; set; } = DefaultGamma;
}
