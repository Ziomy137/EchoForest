using System;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# Pause Menu controller. Tracks paused state, triggers saves, and
/// handles navigation without depending on the Godot runtime.
///
/// The companion Godot node (<see cref="PauseMenuNode"/>) calls these methods
/// from button signals and applies <c>GetTree().Paused</c> accordingly.
///
/// Fully testable with NUnit.
/// </summary>
public sealed class PauseMenuController : IPauseMenuController
{
    private readonly ISaveDataService _saveService;
    private readonly ISceneLoader? _sceneLoader;

    /// <inheritdoc/>
    public bool IsPaused { get; private set; }

    /// <inheritdoc/>
    public bool GameSaved { get; private set; }

    /// <param name="saveService">Handles save persistence.</param>
    /// <param name="sceneLoader">Handles scene transitions. May be <c>null</c> in tests that do not exercise navigation.</param>
    public PauseMenuController(ISaveDataService saveService, ISceneLoader? sceneLoader = null)
    {
        _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
        _sceneLoader = sceneLoader;
    }

    /// <inheritdoc/>
    public void Open() => IsPaused = true;

    /// <inheritdoc/>
    public void OnResume() => IsPaused = false;

    /// <inheritdoc/>
    public void OnSettings() =>
        _sceneLoader?.LoadScene(MainMenuConfig.SettingsScenePath);

    /// <inheritdoc/>
    public void OnSaveGame()
    {
        _saveService.Save(new SaveData(), slot: 1);
        GameSaved = true;
    }

    /// <inheritdoc/>
    public void OnMainMenu() =>
        _sceneLoader?.LoadScene(MainMenuConfig.SceneResPath);
}
