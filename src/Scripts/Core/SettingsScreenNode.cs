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
    private SettingsController _ctrl = null!;
    private GodotDisplayServer _display = null!;

    public override void _Ready()
    {
        _display = new GodotDisplayServer();
        _ctrl = new SettingsController(_display);

        PopulateDropdowns();
        WireWindowModeOption();
        WireVSyncToggle();
        WireFpsLimitOption();
        WireBrightnessSlider();
        WireGammaSlider();
        WireButtons();

        RefreshUI();
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
            int count = Godot.DisplayServer.GetScreenCount();
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
            _ctrl.SetWindowMode(idx == 1 ? WindowMode.BorderlessFullscreen : WindowMode.Windowed);
            RefreshUI();
        };
    }

    private void WireVSyncToggle()
    {
        if (FindChild("VSyncCheckBox") is not CheckBox cb) return;
        cb.Toggled += enabled =>
        {
            _ctrl.SetVSync(enabled);
            RefreshUI();
        };
    }

    private void WireFpsLimitOption()
    {
        if (FindChild("FpsLimitOption") is not OptionButton opt) return;
        opt.ItemSelected += idx =>
        {
            int[] values = { 30, 60, 120, 144, 0 }; // 0 = Unlimited
            if (idx >= 0 && idx < values.Length)
                _ctrl.SetFpsLimit(values[idx]);
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
            apply.Pressed += () => { _ctrl.Apply(); RefreshUI(); };

        if (FindChild("CancelButton") is Button cancel)
            cancel.Pressed += () => { _ctrl.Cancel(); RefreshUI(); };

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
            int[] values = { 30, 60, 120, 144, 0 };
            int idx = System.Array.IndexOf(values, _ctrl.FpsLimit);
            fpsOpt.Selected = idx >= 0 ? idx : 1; // default 60
        }

        if (FindChild("MonitorRow") is Control monRow)
            monRow.Visible = _ctrl.IsMonitorDropdownVisible;

        if (FindChild("MonitorOption") is OptionButton monOpt)
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
        // Brightness/Gamma are stored in GodotDisplayServer; hook a ColorRect
        // shader or Environment here in a future audio/post-process sprint.
        _display.ApplyBrightness(_ctrl.Brightness);
        _display.ApplyGamma(_ctrl.Gamma);
    }

    private void OnBack()
    {
        _ctrl.Cancel();
        var loader = new GodotSceneLoader();
        loader.LoadScene(MainMenuConfig.SceneResPath);
    }
}
