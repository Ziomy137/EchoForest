using System;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# Credits screen controller. Handles the single navigation action
/// (back to Main Menu) via <see cref="ISceneLoader"/>.
///
/// Fully testable with NUnit — no Godot runtime required.
/// </summary>
public sealed class CreditsController : ICreditsController
{
    private readonly ISceneLoader _sceneLoader;

    public CreditsController(ISceneLoader sceneLoader)
    {
        _sceneLoader = sceneLoader ?? throw new ArgumentNullException(nameof(sceneLoader));
    }

    /// <inheritdoc/>
    public void OnBack() =>
        _sceneLoader.LoadScene(MainMenuConfig.SceneResPath);
}
