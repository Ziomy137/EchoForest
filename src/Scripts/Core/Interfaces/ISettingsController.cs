namespace EchoForest.Core;

/// <summary>
/// Contract for the Settings screen controller.
/// All state is pending until <see cref="Apply"/> is called;
/// <see cref="Cancel"/> reverts pending changes to the last applied state.
/// </summary>
public interface ISettingsController
{
    // ── Pending state ─────────────────────────────────────────────────────────

    WindowMode WindowMode { get; }
    int MonitorIndex { get; }
    int FpsLimit { get; }
    bool VSync { get; }
    float Brightness { get; }
    float Gamma { get; }

    // ── Derived UI state ──────────────────────────────────────────────────────

    /// <summary><c>true</c> when VSync is off — FPS Limit control is interactive.</summary>
    bool IsFpsLimitEnabled { get; }

    /// <summary><c>true</c> when window mode is Borderless Fullscreen — Monitor dropdown is shown.</summary>
    bool IsMonitorDropdownVisible { get; }

    // ── Mutation ──────────────────────────────────────────────────────────────

    void SetWindowMode(WindowMode mode);
    void SetMonitor(int index);
    void SetFpsLimit(int fps);
    void SetVSync(bool enabled);

    /// <summary>Sets brightness; clamped to [0, 200].</summary>
    void SetBrightness(float value);

    /// <summary>Sets gamma; clamped to [0, 200].</summary>
    void SetGamma(float value);

    /// <summary>Commits pending changes and applies them via the display server.</summary>
    void Apply();

    /// <summary>Discards pending changes; reverts to last applied (or default) state.</summary>
    void Cancel();
}
