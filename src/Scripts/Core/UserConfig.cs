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
    public WindowMode WindowMode { get; set; } = WindowMode.Windowed;
    public int MonitorIndex { get; set; } = 0;
    public int FpsLimit { get; set; } = 60;
    public bool VSync { get; set; } = false;
    public float Brightness { get; set; } = 100f;
    public float Gamma { get; set; } = 100f;
}
