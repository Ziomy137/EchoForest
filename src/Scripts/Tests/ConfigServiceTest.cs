using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// Unit tests for <see cref="ConfigService"/> and <see cref="MockFileSystem"/>.
/// All tests run without the Godot runtime.
/// </summary>
[TestFixture]
public class ConfigServiceTest
{
    // ── Helpers ───────────────────────────────────────────────────────────────

    private static ConfigService Make(MockFileSystem fs) => new(fs);

    // ── GetDefaults ───────────────────────────────────────────────────────────

    [Test]
    public void ConfigService_GetDefaults_ReturnsFactoryValues()
    {
        var svc = Make(new MockFileSystem());
        var cfg = svc.GetDefaults();

        Assert.Multiple(() =>
        {
            Assert.That(cfg.WindowMode, Is.EqualTo(WindowMode.Windowed));
            Assert.That(cfg.MonitorIndex, Is.EqualTo(0));
            Assert.That(cfg.FpsLimit, Is.EqualTo(60));
            Assert.That(cfg.VSync, Is.False);
            Assert.That(cfg.Brightness, Is.EqualTo(100f).Within(0.001f));
            Assert.That(cfg.Gamma, Is.EqualTo(100f).Within(0.001f));
        });
    }

    // ── Save ──────────────────────────────────────────────────────────────────

    [Test]
    public void ConfigService_Save_WritesJsonToFileSystem()
    {
        var fs = new MockFileSystem();
        var svc = Make(fs);
        svc.Save(new UserConfig { Brightness = 150f });

        Assert.That(fs.WrittenContent, Is.Not.Null.And.Contains("150"));
    }

    [Test]
    public void ConfigService_Save_NullConfig_Throws()
    {
        var svc = Make(new MockFileSystem());
        Assert.Throws<System.ArgumentNullException>(() => svc.Save(null!));
    }

    // ── Load — round-trip ─────────────────────────────────────────────────────

    [Test]
    public void ConfigService_SaveAndLoad_RoundTrip_Brightness()
    {
        var fs = new MockFileSystem();
        var svc = Make(fs);
        svc.Save(new UserConfig { Brightness = 120f });

        var loaded = svc.Load();

        Assert.That(loaded.Brightness, Is.EqualTo(120f).Within(0.001f));
    }

    [Test]
    public void ConfigService_SaveAndLoad_RoundTrip_AllFields()
    {
        var fs = new MockFileSystem();
        var svc = Make(fs);
        var original = new UserConfig
        {
            WindowMode = WindowMode.BorderlessFullscreen,
            MonitorIndex = 1,
            FpsLimit = 144,
            VSync = true,
            Brightness = 80f,
            Gamma = 120f,
        };
        svc.Save(original);

        var loaded = svc.Load();

        Assert.Multiple(() =>
        {
            Assert.That(loaded.WindowMode, Is.EqualTo(WindowMode.BorderlessFullscreen));
            Assert.That(loaded.MonitorIndex, Is.EqualTo(1));
            Assert.That(loaded.FpsLimit, Is.EqualTo(144));
            Assert.That(loaded.VSync, Is.True);
            Assert.That(loaded.Brightness, Is.EqualTo(80f).Within(0.001f));
            Assert.That(loaded.Gamma, Is.EqualTo(120f).Within(0.001f));
        });
    }

    // ── Load — missing file ───────────────────────────────────────────────────

    [Test]
    public void ConfigService_MissingFile_ReturnsDefaults()
    {
        var svc = Make(new MockFileSystem(fileExists: false));
        var cfg = svc.Load();

        Assert.Multiple(() =>
        {
            Assert.That(cfg.Brightness, Is.EqualTo(100f).Within(0.001f));
            Assert.That(cfg.Gamma, Is.EqualTo(100f).Within(0.001f));
            Assert.That(cfg.FpsLimit, Is.EqualTo(60));
        });
    }

    // ── Load — corrupt file ───────────────────────────────────────────────────

    [Test]
    public void ConfigService_CorruptFile_ReturnsDefaults()
    {
        var fs = new MockFileSystem(content: "not valid json{{{{");
        var svc = Make(fs);

        var cfg = svc.Load();

        Assert.Multiple(() =>
        {
            Assert.That(cfg.WindowMode, Is.EqualTo(WindowMode.Windowed));
            Assert.That(cfg.FpsLimit, Is.EqualTo(60));
            Assert.That(cfg.Brightness, Is.EqualTo(100f).Within(0.001f));
            Assert.That(cfg.Gamma, Is.EqualTo(100f).Within(0.001f));
        });
    }

    [Test]
    public void ConfigService_EmptyFile_ReturnsDefaults()
    {
        var fs = new MockFileSystem(content: "");
        var svc = Make(fs);

        var cfg = svc.Load();

        Assert.Multiple(() =>
        {
            Assert.That(cfg.WindowMode, Is.EqualTo(WindowMode.Windowed));
            Assert.That(cfg.FpsLimit, Is.EqualTo(60));
            Assert.That(cfg.Brightness, Is.EqualTo(100f).Within(0.001f));
            Assert.That(cfg.Gamma, Is.EqualTo(100f).Within(0.001f));
        });
    }

    // ── Save overwrites previous ──────────────────────────────────────────────

    [Test]
    public void ConfigService_Save_OverwritesPreviousValue()
    {
        var fs = new MockFileSystem();
        var svc = Make(fs);

        svc.Save(new UserConfig { FpsLimit = 30 });
        svc.Save(new UserConfig { FpsLimit = 144 });

        var loaded = svc.Load();
        Assert.That(loaded.FpsLimit, Is.EqualTo(144));
    }

    // ── Custom path ───────────────────────────────────────────────────────────

    [Test]
    public void ConfigService_CustomPath_UsedForSaveLoad()
    {
        var fs = new MockFileSystem();
        var svc = new ConfigService(fs, "user://custom.json");

        svc.Save(new UserConfig { VSync = true });
        var loaded = svc.Load();

        Assert.That(loaded.VSync, Is.True);
    }

    // ── MockFileSystem behaviour ──────────────────────────────────────────────

    [Test]
    public void MockFileSystem_BeforeWrite_ExistsReturnsFalse()
    {
        var fs = new MockFileSystem(fileExists: true); // fileExists=true but no content yet
        Assert.That(fs.Exists("any"), Is.False);
    }

    [Test]
    public void MockFileSystem_AfterWrite_ExistsReturnsTrue()
    {
        var fs = new MockFileSystem();
        fs.WriteText("any", "data");
        Assert.That(fs.Exists("any"), Is.True);
    }

    [Test]
    public void MockFileSystem_FileExistsFalse_ReadThrows()
    {
        var fs = new MockFileSystem(fileExists: false);
        Assert.Throws<System.IO.FileNotFoundException>(() => fs.ReadText("any"));
    }

    [Test]
    public void MockFileSystem_PreseededContent_ExistsTrue()
    {
        var fs = new MockFileSystem(content: "{\"FpsLimit\":30}");
        Assert.That(fs.Exists("any"), Is.True);
        Assert.That(fs.ReadText("any"), Does.Contain("FpsLimit"));
    }

    // ── Null constructor guard ────────────────────────────────────────────────

    [Test]
    public void ConfigService_NullFileSystem_Throws()
    {
        Assert.Throws<System.ArgumentNullException>(() => new ConfigService(null!));
    }
}
