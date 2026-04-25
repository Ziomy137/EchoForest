using System;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# Main Menu navigation controller. Handles button actions and tracks
/// state that drives button enable/disable in the Godot UI layer.
///
/// All navigation is delegated to <see cref="ISceneLoader"/> and all
/// application lifecycle calls to <see cref="IApplicationController"/>, keeping
/// this class fully testable with NUnit without the Godot runtime.
/// </summary>
public sealed class MainMenuController : IMainMenuController
{
    private readonly ISaveService _saveService;
    private readonly ISceneLoader? _sceneLoader;
    private readonly IApplicationController? _appCtrl;

    // ── IMainMenuController ───────────────────────────────────────────────────

    /// <inheritdoc/>
    public bool IsContinueEnabled => _saveService.HasSaveFile();

    // ── Construction ──────────────────────────────────────────────────────────

    /// <param name="saveService">Provides save-file existence check.</param>
    /// <param name="sceneLoader">Handles scene transitions. May be <c>null</c> in unit tests that do not exercise navigation.</param>
    /// <param name="appCtrl">Handles application quit. May be <c>null</c> in unit tests that do not exercise exit.</param>
    public MainMenuController(
        ISaveService saveService,
        ISceneLoader? sceneLoader = null,
        IApplicationController? appCtrl = null)
    {
        _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
        _sceneLoader = sceneLoader;
        _appCtrl = appCtrl;
    }

    // ── Navigation ────────────────────────────────────────────────────────────

    /// <inheritdoc/>
    public void OnNewGame() =>
        _sceneLoader?.LoadScene(MainMenuConfig.GameBootstrapScenePath);

    /// <inheritdoc/>
    public void OnContinue()
    {
        if (!IsContinueEnabled)
        {
            return;
        }

        _sceneLoader?.LoadScene(MainMenuConfig.GameBootstrapScenePath);
    }

    /// <inheritdoc/>
    public void OnLoadGame() =>
        _sceneLoader?.LoadScene(MainMenuConfig.LoadGameScenePath);

    /// <inheritdoc/>
    public void OnSettings() =>
        _sceneLoader?.LoadScene(MainMenuConfig.SettingsScenePath);

    /// <inheritdoc/>
    public void OnCredits() =>
        _sceneLoader?.LoadScene(MainMenuConfig.CreditsScenePath);

    /// <inheritdoc/>
    public void OnExit() =>
        _appCtrl?.Quit();
}
