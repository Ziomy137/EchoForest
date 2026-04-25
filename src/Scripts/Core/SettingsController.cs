using System;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# Settings controller. Tracks a pending set of graphics options that
/// are previewed in the UI but only committed when <see cref="Apply"/> is called.
/// <see cref="Cancel"/> reverts all pending edits to the last committed snapshot.
///
/// Fully testable with NUnit — no Godot runtime required.
/// </summary>
public sealed class SettingsController : ISettingsController
{
    private readonly IDisplayServer _display;

    // ── Pending state (edited by UI controls) ─────────────────────────────────

    private WindowMode _windowMode = WindowMode.Windowed;
    private int _monitorIndex = 0;
    private int _fpsLimit = 60;
    private bool _vSync = false;
    private float _brightness = 100f;
    private float _gamma = 100f;

    // ── Committed snapshot (last Apply() or initial defaults) ─────────────────

    private WindowMode _cWindowMode = WindowMode.Windowed;
    private int _cMonitorIndex = 0;
    private int _cFpsLimit = 60;
    private bool _cVSync = false;
    private float _cBrightness = 100f;
    private float _cGamma = 100f;

    // ── Construction ──────────────────────────────────────────────────────────

    public SettingsController(IDisplayServer displayServer)
    {
        _display = displayServer ?? throw new ArgumentNullException(nameof(displayServer));

        // Restore last committed values from the in-process cache so the UI
        // shows previously applied settings instead of hard-coded defaults.
        _windowMode = _cWindowMode = SettingsCache.WindowMode;
        _monitorIndex = _cMonitorIndex = SettingsCache.MonitorIndex;
        _fpsLimit = _cFpsLimit = SettingsCache.FpsLimit;
        _vSync = _cVSync = SettingsCache.VSync;
        _brightness = _cBrightness = SettingsCache.Brightness;
        _gamma = _cGamma = SettingsCache.Gamma;
    }

    // ── ISettingsController — state ───────────────────────────────────────────

    public WindowMode WindowMode => _windowMode;
    public int MonitorIndex => _monitorIndex;
    public int FpsLimit => _fpsLimit;
    public bool VSync => _vSync;
    public float Brightness => _brightness;
    public float Gamma => _gamma;

    public bool IsFpsLimitEnabled => !_vSync;
    public bool IsMonitorDropdownVisible => _windowMode == WindowMode.BorderlessFullscreen;

    // ── ISettingsController — mutation ────────────────────────────────────────

    public void SetWindowMode(WindowMode mode) => _windowMode = mode;
    public void SetMonitor(int index) => _monitorIndex = index;
    public void SetFpsLimit(int fps) => _fpsLimit = fps;
    public void SetVSync(bool enabled) => _vSync = enabled;
    public void SetBrightness(float value) => _brightness = Math.Clamp(value, 0f, 200f);
    public void SetGamma(float value) => _gamma = Math.Clamp(value, 0f, 200f);

    /// <inheritdoc/>
    public void Apply()
    {
        _display.ApplyWindowMode(_windowMode);
        _display.ApplyVSync(_vSync);
        // When VSync is on, pass 0 (unlimited) so the GPU doesn't double-cap.
        _display.ApplyFpsLimit(_vSync ? 0 : _fpsLimit);
        _display.ApplyMonitor(_monitorIndex);
        _display.ApplyBrightness(_brightness);
        _display.ApplyGamma(_gamma);

        // Commit pending → saved snapshot
        _cWindowMode = _windowMode;
        _cMonitorIndex = _monitorIndex;
        _cFpsLimit = _fpsLimit;
        _cVSync = _vSync;
        _cBrightness = _brightness;
        _cGamma = _gamma;

        // Persist to static cache so the next SettingsController instance
        // (after scene reload) starts from these committed values.
        SettingsCache.Save(_windowMode, _monitorIndex, _fpsLimit, _vSync, _brightness, _gamma);
    }

    /// <inheritdoc/>
    public void Cancel()
    {
        _windowMode = _cWindowMode;
        _monitorIndex = _cMonitorIndex;
        _fpsLimit = _cFpsLimit;
        _vSync = _cVSync;
        _brightness = _cBrightness;
        _gamma = _cGamma;
    }
}
