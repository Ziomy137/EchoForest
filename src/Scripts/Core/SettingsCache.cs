namespace EchoForest.Core;

/// <summary>
/// In-process settings snapshot. Holds the last values committed via
/// <see cref="SettingsController.Apply"/> so they survive scene reloads.
///
/// When the player reopens the Settings screen a fresh
/// <see cref="SettingsController"/> is created; its constructor reads from
/// this cache so the UI reflects the previously applied values rather than
/// hard-coded defaults.
///
/// Pure C# — no Godot runtime required, fully testable with NUnit.
/// </summary>
public static class SettingsCache
{
    // ── Stored values ─────────────────────────────────────────────────────────

    public static WindowMode WindowMode { get; private set; } = WindowMode.Windowed;
    public static int MonitorIndex { get; private set; } = 0;
    public static int FpsLimit { get; private set; } = 60;
    public static bool VSync { get; private set; } = false;
    public static float Brightness { get; private set; } = 100f;
    public static float Gamma { get; private set; } = 100f;

    /// <summary><c>true</c> once <see cref="Save"/> has been called at least once.</summary>
    public static bool HasSaved { get; private set; }

    // ── Mutation ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Persists a committed settings snapshot. Called by
    /// <see cref="SettingsController.Apply"/> after all display calls succeed.
    /// </summary>
    public static void Save(
        WindowMode windowMode,
        int monitorIndex,
        int fpsLimit,
        bool vSync,
        float brightness,
        float gamma)
    {
        WindowMode = windowMode;
        MonitorIndex = monitorIndex;
        FpsLimit = fpsLimit;
        VSync = vSync;
        Brightness = brightness;
        Gamma = gamma;
        HasSaved = true;
    }

    /// <summary>
    /// Persists settings from a <see cref="UserConfig"/> instance.
    /// Centralises the UserConfig → SettingsCache field mapping so callers
    /// (e.g. <see cref="MainMenuNode"/>, <see cref="SettingsScreenNode"/>)
    /// don't each duplicate the mapping.
    /// </summary>
    public static void Save(UserConfig cfg) =>
        Save(cfg.WindowMode, cfg.MonitorIndex, cfg.FpsLimit, cfg.VSync, cfg.Brightness, cfg.Gamma);

    /// <summary>
    /// Resets to factory defaults. Useful in tests and on first launch.
    /// </summary>
    public static void Reset()
    {
        WindowMode = WindowMode.Windowed;
        MonitorIndex = 0;
        FpsLimit = 60;
        VSync = false;
        Brightness = 100f;
        Gamma = 100f;
        HasSaved = false;
    }
}
