using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>CanvasLayer</c> node that drives the Main Menu UI.
///
/// Wires each Godot button's <c>pressed</c> signal to the corresponding
/// <see cref="MainMenuController"/> action. The controller contains all
/// navigation logic and is unit-tested independently of the Godot runtime.
///
/// Excluded from NUnit code coverage — requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot CanvasLayer wrapper — requires scene tree")]
public partial class MainMenuNode : CanvasLayer
{
    private MainMenuController _controller = null!;

    public override void _Ready()
    {
        // Load persisted settings on every launch so display state is restored
        // before the first frame renders. Uses SettingsCache as the in-process
        // bridge between ConfigService and SettingsController.
        ApplyPersistedSettings();

        var sceneLoader = new GodotSceneLoader();
        var appCtrl = new GodotApplicationController();
        // Use the real SaveService (S5-04) so the Continue button reflects actual save files.
        var saveService = new SaveService(new GodotFileSystem());
        _controller = new MainMenuController(saveService, sceneLoader, appCtrl);

        // Navigation buttons use GetTree() directly — bypasses GodotSceneLoader/Engine.GetMainLoop()
        // which can return null in some Godot 4 configurations.
        WireNavButton("NewGameButton", () => GetTree().ChangeSceneToFile(MainMenuConfig.GameBootstrapScenePath));
        WireNavButton("ContinueButton", () => { if (_controller.IsContinueEnabled) GetTree().ChangeSceneToFile(MainMenuConfig.ContinueScenePath); });
        WireNavButton("LoadGameButton", () => GetTree().ChangeSceneToFile(MainMenuConfig.LoadGameScenePath));
        WireNavButton("SettingsButton", () => GetTree().ChangeSceneToFile(MainMenuConfig.SettingsScenePath));
        WireNavButton("CreditsButton", () => GetTree().ChangeSceneToFile(MainMenuConfig.CreditsScenePath));
        WireNavButton("ExitButton", () => _controller.OnExit());

        // Reflect initial state of Continue button
        if (FindChild("ContinueButton") is Button continueBtn)
            continueBtn.Disabled = !_controller.IsContinueEnabled;
    }

    private void WireNavButton(string nodeName, System.Action action)
    {
        var btn = FindChild(nodeName) as Button;
        if (btn != null)
            btn.Pressed += action;
    }

    // ── Config persistence ────────────────────────────────────────────────────

    private static void ApplyPersistedSettings()
    {
        var svc = new ConfigService(new GodotFileSystem());
        var cfg = svc.Load();

        // Push loaded values into SettingsCache so any subsequent
        // SettingsController construction starts from persisted values.
        SettingsCache.Save(cfg);

        // Apply display settings immediately so the window mode, FPS cap,
        // and brightness/gamma are correct from the very first frame.
        var display = new GodotDisplayServer();
        display.ApplyWindowMode(cfg.WindowMode);
        display.ApplyVSync(cfg.VSync);
        display.ApplyFpsLimit(cfg.VSync ? 0 : cfg.FpsLimit);
        display.ApplyMonitor(cfg.MonitorIndex);
        display.ApplyBrightness(cfg.Brightness);
        display.ApplyGamma(cfg.Gamma);
    }
}
