using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// Unit tests for <see cref="SettingsCache"/> and its integration with
/// <see cref="SettingsController"/> across multiple controller instances.
/// </summary>
[TestFixture]
public class SettingsCacheTest
{
    [SetUp]
    public void SetUp() => SettingsCache.Reset();

    [TearDown]
    public void TearDown() => SettingsCache.Reset();

    // ── Initial state ─────────────────────────────────────────────────────────

    [Test]
    public void SettingsCache_InitialState_HasSaved_IsFalse()
    {
        Assert.That(SettingsCache.HasSaved, Is.False);
    }

    [Test]
    public void SettingsCache_InitialState_HasDefaultValues()
    {
        Assert.Multiple(() =>
        {
            Assert.That(SettingsCache.WindowMode, Is.EqualTo(WindowMode.Windowed));
            Assert.That(SettingsCache.MonitorIndex, Is.EqualTo(0));
            Assert.That(SettingsCache.FpsLimit, Is.EqualTo(60));
            Assert.That(SettingsCache.VSync, Is.False);
            Assert.That(SettingsCache.Brightness, Is.EqualTo(100f).Within(0.001f));
            Assert.That(SettingsCache.Gamma, Is.EqualTo(100f).Within(0.001f));
        });
    }

    // ── Save / Reset ──────────────────────────────────────────────────────────

    [Test]
    public void SettingsCache_Save_StoresValues()
    {
        SettingsCache.Save(WindowMode.BorderlessFullscreen, 1, 144, true, 120f, 80f);

        Assert.Multiple(() =>
        {
            Assert.That(SettingsCache.WindowMode, Is.EqualTo(WindowMode.BorderlessFullscreen));
            Assert.That(SettingsCache.MonitorIndex, Is.EqualTo(1));
            Assert.That(SettingsCache.FpsLimit, Is.EqualTo(144));
            Assert.That(SettingsCache.VSync, Is.True);
            Assert.That(SettingsCache.Brightness, Is.EqualTo(120f).Within(0.001f));
            Assert.That(SettingsCache.Gamma, Is.EqualTo(80f).Within(0.001f));
            Assert.That(SettingsCache.HasSaved, Is.True);
        });
    }

    [Test]
    public void SettingsCache_Reset_RestoresDefaults()
    {
        SettingsCache.Save(WindowMode.BorderlessFullscreen, 1, 144, true, 120f, 80f);
        SettingsCache.Reset();

        Assert.Multiple(() =>
        {
            Assert.That(SettingsCache.WindowMode, Is.EqualTo(WindowMode.Windowed));
            Assert.That(SettingsCache.FpsLimit, Is.EqualTo(60));
            Assert.That(SettingsCache.HasSaved, Is.False);
        });
    }

    // ── Integration: controller reads cache on construction ───────────────────

    [Test]
    public void SettingsController_NewInstance_ReadsFromCache()
    {
        SettingsCache.Save(WindowMode.BorderlessFullscreen, 0, 120, false, 150f, 90f);

        var ctrl = new SettingsController(new MockDisplayServer());

        Assert.Multiple(() =>
        {
            Assert.That(ctrl.WindowMode, Is.EqualTo(WindowMode.BorderlessFullscreen));
            Assert.That(ctrl.FpsLimit, Is.EqualTo(120));
            Assert.That(ctrl.Brightness, Is.EqualTo(150f).Within(0.001f));
            Assert.That(ctrl.Gamma, Is.EqualTo(90f).Within(0.001f));
        });
    }

    [Test]
    public void SettingsController_Apply_UpdatesCache()
    {
        var ctrl = new SettingsController(new MockDisplayServer());
        ctrl.SetWindowMode(WindowMode.BorderlessFullscreen);
        ctrl.SetFpsLimit(144);
        ctrl.SetBrightness(130f);
        ctrl.Apply();

        Assert.Multiple(() =>
        {
            Assert.That(SettingsCache.WindowMode, Is.EqualTo(WindowMode.BorderlessFullscreen));
            Assert.That(SettingsCache.FpsLimit, Is.EqualTo(144));
            Assert.That(SettingsCache.Brightness, Is.EqualTo(130f).Within(0.001f));
            Assert.That(SettingsCache.HasSaved, Is.True);
        });
    }

    [Test]
    public void SettingsController_Cancel_DoesNotUpdateCache()
    {
        SettingsCache.Save(WindowMode.Windowed, 0, 60, false, 100f, 100f);

        var ctrl = new SettingsController(new MockDisplayServer());
        ctrl.SetFpsLimit(144);
        ctrl.Cancel(); // revert without Apply

        Assert.That(SettingsCache.FpsLimit, Is.EqualTo(60));
    }

    // ── Simulate reopen scenario ──────────────────────────────────────────────

    [Test]
    public void SettingsController_SecondInstance_SeesFirstAppliedValues()
    {
        // First visit: user changes and applies settings
        var ctrl1 = new SettingsController(new MockDisplayServer());
        ctrl1.SetVSync(true);
        ctrl1.SetBrightness(80f);
        ctrl1.Apply();

        // User navigates away → scene unloaded → ctrl1 destroyed

        // Second visit: a fresh controller should restore applied values
        var ctrl2 = new SettingsController(new MockDisplayServer());

        Assert.Multiple(() =>
        {
            Assert.That(ctrl2.VSync, Is.True);
            Assert.That(ctrl2.Brightness, Is.EqualTo(80f).Within(0.001f));
        });
    }
}
