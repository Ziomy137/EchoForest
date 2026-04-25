using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// Unit tests for <see cref="SettingsController"/>.
/// All tests run without the Godot runtime.
/// </summary>
[TestFixture]
public class SettingsControllerTest
{
    // ── Helpers ───────────────────────────────────────────────────────────────

    [SetUp]
    public void SetUp() => SettingsCache.Reset();

    [TearDown]
    public void TearDown() => SettingsCache.Reset();

    private static SettingsController Make() => new(new MockDisplayServer());
    private static (SettingsController ctrl, MockDisplayServer display) MakeWithDisplay()
    {
        var d = new MockDisplayServer();
        return (new SettingsController(d), d);
    }

    // ── VSync / FPS Limit interaction ─────────────────────────────────────────

    [Test]
    public void Settings_VSync_Enabled_FpsLimitIsDisabled()
    {
        var ctrl = Make();
        ctrl.SetVSync(true);
        Assert.That(ctrl.IsFpsLimitEnabled, Is.False);
    }

    [Test]
    public void Settings_VSync_Disabled_FpsLimitIsEnabled()
    {
        var ctrl = Make();
        ctrl.SetVSync(false);
        Assert.That(ctrl.IsFpsLimitEnabled, Is.True);
    }

    // ── Window mode / Monitor dropdown ────────────────────────────────────────

    [Test]
    public void Settings_WindowMode_Borderless_ShowsMonitorDropdown()
    {
        var ctrl = Make();
        ctrl.SetWindowMode(WindowMode.BorderlessFullscreen);
        Assert.That(ctrl.IsMonitorDropdownVisible, Is.True);
    }

    [Test]
    public void Settings_WindowMode_Windowed_HidesMonitorDropdown()
    {
        var ctrl = Make();
        ctrl.SetWindowMode(WindowMode.Windowed);
        Assert.That(ctrl.IsMonitorDropdownVisible, Is.False);
    }

    // ── Brightness clamping ───────────────────────────────────────────────────

    [Test]
    public void Settings_Brightness_OutOfRange_ClampedTo0To200()
    {
        var ctrl = Make();

        ctrl.SetBrightness(250f);
        Assert.That(ctrl.Brightness, Is.EqualTo(200f));

        ctrl.SetBrightness(-10f);
        Assert.That(ctrl.Brightness, Is.EqualTo(0f));
    }

    [Test]
    public void Settings_Brightness_InRange_StoredUnchanged()
    {
        var ctrl = Make();
        ctrl.SetBrightness(120f);
        Assert.That(ctrl.Brightness, Is.EqualTo(120f));
    }

    // ── Gamma clamping ────────────────────────────────────────────────────────

    [Test]
    public void Settings_Gamma_OutOfRange_ClampedTo0To200()
    {
        var ctrl = Make();

        ctrl.SetGamma(300f);
        Assert.That(ctrl.Gamma, Is.EqualTo(200f));

        ctrl.SetGamma(-5f);
        Assert.That(ctrl.Gamma, Is.EqualTo(0f));
    }

    [Test]
    public void Settings_Gamma_InRange_StoredUnchanged()
    {
        var ctrl = Make();
        ctrl.SetGamma(80f);
        Assert.That(ctrl.Gamma, Is.EqualTo(80f));
    }

    // ── Cancel reverts uncommitted changes ────────────────────────────────────

    [Test]
    public void Settings_Cancel_RevertsUncommittedChanges()
    {
        var ctrl = Make();
        ctrl.SetBrightness(50f);
        ctrl.Cancel();
        Assert.That(ctrl.Brightness, Is.EqualTo(100f)); // default
    }

    [Test]
    public void Settings_Cancel_RevertsGamma()
    {
        var ctrl = Make();
        ctrl.SetGamma(150f);
        ctrl.Cancel();
        Assert.That(ctrl.Gamma, Is.EqualTo(100f));
    }

    [Test]
    public void Settings_Cancel_RevertsVSync()
    {
        var ctrl = Make();
        ctrl.SetVSync(true);
        ctrl.Cancel();
        Assert.That(ctrl.VSync, Is.False);
    }

    [Test]
    public void Settings_Cancel_RevertsWindowMode()
    {
        var ctrl = Make();
        ctrl.SetWindowMode(WindowMode.BorderlessFullscreen);
        ctrl.Cancel();
        Assert.That(ctrl.WindowMode, Is.EqualTo(WindowMode.Windowed));
    }

    [Test]
    public void Settings_Cancel_RevertsFpsLimit()
    {
        var ctrl = Make();
        ctrl.SetFpsLimit(30);
        ctrl.Cancel();
        Assert.That(ctrl.FpsLimit, Is.EqualTo(60)); // default
    }

    [Test]
    public void Settings_Cancel_RevertsMonitorIndex()
    {
        var ctrl = Make();
        ctrl.SetMonitor(2);
        ctrl.Cancel();
        Assert.That(ctrl.MonitorIndex, Is.EqualTo(0));
    }

    // ── Apply commits state ───────────────────────────────────────────────────

    [Test]
    public void Settings_Apply_CommitsState_CancelDoesNotRevert()
    {
        var ctrl = Make();
        ctrl.SetBrightness(80f);
        ctrl.Apply();
        ctrl.Cancel(); // cancel after apply — should keep 80f
        Assert.That(ctrl.Brightness, Is.EqualTo(80f));
    }

    [Test]
    public void Settings_Apply_CallsDisplayServer_WindowMode()
    {
        var (ctrl, display) = MakeWithDisplay();
        ctrl.SetWindowMode(WindowMode.BorderlessFullscreen);
        ctrl.Apply();
        Assert.That(display.AppliedWindowMode, Is.EqualTo(WindowMode.BorderlessFullscreen));
    }

    [Test]
    public void Settings_Apply_CallsDisplayServer_VSync()
    {
        var (ctrl, display) = MakeWithDisplay();
        ctrl.SetVSync(true);
        ctrl.Apply();
        Assert.That(display.AppliedVSync, Is.True);
    }

    [Test]
    public void Settings_Apply_WhenVSyncOn_SetsFpsLimitZero()
    {
        var (ctrl, display) = MakeWithDisplay();
        ctrl.SetVSync(true);
        ctrl.SetFpsLimit(60);
        ctrl.Apply();
        Assert.That(display.AppliedFpsLimit, Is.EqualTo(0));
    }

    [Test]
    public void Settings_Apply_WhenVSyncOff_SetsFpsLimitValue()
    {
        var (ctrl, display) = MakeWithDisplay();
        ctrl.SetVSync(false);
        ctrl.SetFpsLimit(144);
        ctrl.Apply();
        Assert.That(display.AppliedFpsLimit, Is.EqualTo(144));
    }

    [Test]
    public void Settings_Apply_CallsDisplayServer_Brightness()
    {
        var (ctrl, display) = MakeWithDisplay();
        ctrl.SetBrightness(130f);
        ctrl.Apply();
        Assert.That(display.AppliedBrightness, Is.EqualTo(130f));
    }

    [Test]
    public void Settings_Apply_CallsDisplayServer_Gamma()
    {
        var (ctrl, display) = MakeWithDisplay();
        ctrl.SetGamma(90f);
        ctrl.Apply();
        Assert.That(display.AppliedGamma, Is.EqualTo(90f));
    }

    [Test]
    public void Settings_Apply_CallsDisplayServer_Monitor()
    {
        var (ctrl, display) = MakeWithDisplay();
        ctrl.SetMonitor(1);
        ctrl.Apply();
        Assert.That(display.AppliedMonitorIndex, Is.EqualTo(1));
    }

    // ── Default state ─────────────────────────────────────────────────────────

    [Test]
    public void Settings_DefaultBrightness_Is100()
    {
        Assert.That(Make().Brightness, Is.EqualTo(100f));
    }

    [Test]
    public void Settings_DefaultGamma_Is100()
    {
        Assert.That(Make().Gamma, Is.EqualTo(100f));
    }

    [Test]
    public void Settings_DefaultWindowMode_IsWindowed()
    {
        Assert.That(Make().WindowMode, Is.EqualTo(WindowMode.Windowed));
    }

    [Test]
    public void Settings_DefaultVSync_IsFalse()
    {
        Assert.That(Make().VSync, Is.False);
    }

    [Test]
    public void Settings_DefaultFpsLimit_Is60()
    {
        Assert.That(Make().FpsLimit, Is.EqualTo(60));
    }

    [Test]
    public void Settings_DefaultMonitorIndex_IsZero()
    {
        Assert.That(Make().MonitorIndex, Is.EqualTo(0));
    }

    [Test]
    public void Settings_Default_IsFpsLimitEnabled_IsTrue()
    {
        Assert.That(Make().IsFpsLimitEnabled, Is.True);
    }

    [Test]
    public void Settings_Default_IsMonitorDropdownVisible_IsFalse()
    {
        Assert.That(Make().IsMonitorDropdownVisible, Is.False);
    }

    // ── Constructor ───────────────────────────────────────────────────────────

    [Test]
    public void Constructor_NullDisplayServer_ThrowsArgumentNullException()
    {
        Assert.That(
            () => new SettingsController(null!),
            Throws.TypeOf<System.ArgumentNullException>());
    }

    [Test]
    public void SettingsController_ImplementsInterface()
    {
        Assert.That(Make(), Is.InstanceOf<ISettingsController>());
    }

    // ── MockDisplayServer ─────────────────────────────────────────────────────

    [Test]
    public void MockDisplayServer_DefaultValues_AreCorrect()
    {
        var d = new MockDisplayServer();
        Assert.That(d.AppliedWindowMode, Is.EqualTo(WindowMode.Windowed));
        Assert.That(d.AppliedVSync, Is.False);
        Assert.That(d.AppliedFpsLimit, Is.EqualTo(0));
        Assert.That(d.AppliedMonitorIndex, Is.EqualTo(0));
        Assert.That(d.AppliedBrightness, Is.EqualTo(100f));
        Assert.That(d.AppliedGamma, Is.EqualTo(100f));
    }

    [Test]
    public void MockDisplayServer_GetScreenCount_ReturnsScreenCount()
    {
        var d = new MockDisplayServer { ScreenCount = 3 };
        Assert.That(d.GetScreenCount(), Is.EqualTo(3));
    }

    // ── WindowMode enum ───────────────────────────────────────────────────────

    [Test]
    public void WindowMode_WindwedAndBorderlessFullscreen_AreDefined()
    {
        Assert.That(System.Enum.IsDefined(typeof(WindowMode), WindowMode.Windowed), Is.True);
        Assert.That(System.Enum.IsDefined(typeof(WindowMode), WindowMode.BorderlessFullscreen), Is.True);
    }
}
