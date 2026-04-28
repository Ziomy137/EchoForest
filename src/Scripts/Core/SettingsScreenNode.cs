using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>CanvasLayer</c> node for the Settings screen.
///
/// Wires UI controls to <see cref="SettingsController"/> and reflects the
/// controller's derived state (e.g. FPS Limit disabled when VSync is on,
/// Monitor dropdown hidden in Windowed mode).
///
/// All logic lives in the pure-C# <see cref="SettingsController"/> so it can
/// be unit-tested independently of the Godot runtime.
///
/// Excluded from NUnit code coverage — requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot CanvasLayer wrapper — requires scene tree")]
public partial class SettingsScreenNode : CanvasLayer
{
    // FPS limit values that mirror the items added in PopulateDropdowns().
    // Shared between WireFpsLimitOption() and RefreshUI() so a single change
    // here keeps both paths in sync.
    private static readonly int[] FpsLimitValues = { 30, 60, 120, 144, 0 }; // 0 = Unlimited

    private SettingsController _ctrl = null!;
    private GodotDisplayServer _display = null!;
    private IConfigService _config = null!;

    public override void _Ready()
    {
        _display = new GodotDisplayServer();
        _config = new ConfigService(new GodotFileSystem());

        // Load persisted settings and push them into SettingsCache so the
        // controller initialises from the last saved values.
        ApplyConfigToCache(_config.Load());

        _ctrl = new SettingsController(_display);

        PopulateDropdowns();
        WireWindowModeOption();
        WireVSyncToggle();
        WireFpsLimitOption();
        WireMonitorOption();
        WireBrightnessSlider();
        WireGammaSlider();
        WireButtons();

        RefreshUI();

        // Create the screen overlay and initialise it with cached values so
        // brightness/gamma take effect as soon as the Settings screen opens.
        ApplyPostProcess();
    }

    // ── Dropdown population ───────────────────────────────────────────────────

    private void PopulateDropdowns()
    {
        if (FindChild("WindowModeOption") is OptionButton wm)
        {
            wm.AddItem("Windowed", 0);
            wm.AddItem("Borderless Fullscreen", 1);
        }

        if (FindChild("FpsLimitOption") is OptionButton fps)
        {
            fps.AddItem("30", 0);
            fps.AddItem("60", 1);
            fps.AddItem("120", 2);
            fps.AddItem("144", 3);
            fps.AddItem("Unlimited", 4);
        }

        if (FindChild("MonitorOption") is OptionButton mon)
        {
            int count = _display.GetScreenCount();
            for (int i = 0; i < count; i++)
                mon.AddItem($"Monitor {i + 1}", i);
        }
    }

    // ── Signal wiring ─────────────────────────────────────────────────────────

    private void WireWindowModeOption()
    {
        if (FindChild("WindowModeOption") is not OptionButton opt) return;
        opt.ItemSelected += idx =>
        {
            var mode = idx == 1 ? WindowMode.BorderlessFullscreen : WindowMode.Windowed;
            _ctrl.SetWindowMode(mode);
            _display.ApplyWindowMode(mode); // apply immediately
            RefreshUI();
        };
    }

    private void WireVSyncToggle()
    {
        if (FindChild("VSyncCheckBox") is not CheckBox cb) return;
        cb.Toggled += enabled =>
        {
            _ctrl.SetVSync(enabled);
            _display.ApplyVSync(enabled); // apply immediately
            // restore fps cap when vsync is turned off
            if (!enabled) _display.ApplyFpsLimit(_ctrl.FpsLimit);
            RefreshUI();
        };
    }

    private void WireFpsLimitOption()
    {
        if (FindChild("FpsLimitOption") is not OptionButton opt) return;
        opt.ItemSelected += idx =>
        {
            if (idx >= 0 && idx < FpsLimitValues.Length)
            {
                _ctrl.SetFpsLimit(FpsLimitValues[idx]);
                if (_ctrl.IsFpsLimitEnabled)
                    _display.ApplyFpsLimit(FpsLimitValues[idx]); // apply immediately
            }
        };
    }

    private void WireMonitorOption()
    {
        if (FindChild("MonitorOption") is not OptionButton opt) return;
        opt.ItemSelected += idx =>
        {
            _ctrl.SetMonitor((int)idx);
            _display.ApplyMonitor((int)idx); // apply immediately
        };
    }

    private void WireBrightnessSlider()
    {
        if (FindChild("BrightnessSlider") is not HSlider sl) return;
        sl.ValueChanged += v => { _ctrl.SetBrightness((float)v); ApplyPostProcess(); };
    }

    private void WireGammaSlider()
    {
        if (FindChild("GammaSlider") is not HSlider sl) return;
        sl.ValueChanged += v => { _ctrl.SetGamma((float)v); ApplyPostProcess(); };
    }

    private void WireButtons()
    {
        if (FindChild("ApplyButton") is Button apply)
            apply.Pressed += () => { _ctrl.Apply(); SaveConfig(); RefreshUI(); };

        if (FindChild("CancelButton") is Button cancel)
            cancel.Pressed += () => { _ctrl.Cancel(); RevertDisplay(); RefreshUI(); };

        if (FindChild("BackButton") is Button back)
            back.Pressed += OnBack;
    }

    // ── UI refresh ────────────────────────────────────────────────────────────

    private void RefreshUI()
    {
        if (FindChild("WindowModeOption") is OptionButton wmOpt)
            wmOpt.Selected = _ctrl.WindowMode == WindowMode.BorderlessFullscreen ? 1 : 0;

        if (FindChild("FpsLimitOption") is OptionButton fpsOpt)
        {
            fpsOpt.Disabled = !_ctrl.IsFpsLimitEnabled;
            int idx = System.Array.IndexOf(FpsLimitValues, _ctrl.FpsLimit);
            fpsOpt.Selected = idx >= 0 ? idx : 1; // default 60
        }

        if (FindChild("MonitorRow") is Control monRow)
            monRow.Visible = _ctrl.IsMonitorDropdownVisible;

        if (FindChild("MonitorOption") is OptionButton monOpt && monOpt.ItemCount > 0)
            monOpt.Selected = System.Math.Clamp(_ctrl.MonitorIndex, 0, monOpt.ItemCount - 1);

        if (FindChild("BrightnessSlider") is HSlider bSlider)
            bSlider.Value = _ctrl.Brightness;

        if (FindChild("GammaSlider") is HSlider gSlider)
            gSlider.Value = _ctrl.Gamma;

        if (FindChild("VSyncCheckBox") is CheckBox cb)
            cb.ButtonPressed = _ctrl.VSync;
    }

    private void ApplyPostProcess()
    {
        // GodotDisplayServer already applies brightness/gamma through its
        // persistent screen-overlay shader; keep it in sync with controller state.
        _display.ApplyBrightness(_ctrl.Brightness);
        _display.ApplyGamma(_ctrl.Gamma);
    }

    /// <summary>
    /// Re-applies all committed settings to the display server after
    /// <see cref="ISettingsController.Cancel"/> reverts pending state.
    /// Mirrors the <see cref="ISettingsController.Apply"/> call-through so that
    /// runtime display state matches the controller's committed snapshot.
    /// </summary>
    private void RevertDisplay()
    {
        _display.ApplyWindowMode(_ctrl.WindowMode);
        _display.ApplyVSync(_ctrl.VSync);
        _display.ApplyFpsLimit(_ctrl.VSync ? 0 : _ctrl.FpsLimit);
        _display.ApplyMonitor(_ctrl.MonitorIndex);
        _display.ApplyBrightness(_ctrl.Brightness);
        _display.ApplyGamma(_ctrl.Gamma);
    }

    private void OnBack()
    {
        _ctrl.Cancel();
        RevertDisplay();
        var loader = new GodotSceneLoader();
        loader.LoadScene(MainMenuConfig.SceneResPath);
    }

    // ── Config persistence ────────────────────────────────────────────────────

    private static void ApplyConfigToCache(UserConfig cfg) => SettingsCache.Save(cfg);

    private void SaveConfig()
    {
        _config.Save(new UserConfig
        {
            WindowMode = _ctrl.WindowMode,
            MonitorIndex = _ctrl.MonitorIndex,
            FpsLimit = _ctrl.FpsLimit,
            VSync = _ctrl.VSync,
            Brightness = _ctrl.Brightness,
            Gamma = _ctrl.Gamma,
        });
    }
}
