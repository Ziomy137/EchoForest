using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>Node2D</c> attached to <c>GameBootstrap.tscn</c>. Orchestrates
/// the demo startup sequence:
///
/// <list type="number">
///   <item>Shows the <see cref="LoadingScreenNode"/> overlay.</item>
///   <item>Validates the target scene path via <see cref="SceneLoader"/>.</item>
///   <item>Defers the actual scene switch to the next frame so the loading
///     screen is rendered for at least one frame before the transition.</item>
/// </list>
///
/// Excluded from NUnit code coverage — requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot Node2D wrapper — requires scene tree")]
public partial class GameBootstrapNode : Node2D
{
    private readonly SceneLoader _sceneLoader = new();

    public override void _Ready()
    {
        var loadingScreen = GetNodeOrNull<LoadingScreenNode>("LoadingScreen");
        loadingScreen?.Show();

        // Validate the path eagerly — throws at startup if misconfigured.
        _sceneLoader.LoadScene(CottageSceneConfig.SceneResPath);

        // Defer the scene transition so the loading screen renders for at
        // least one frame before the SceneTree replaces this scene.
        CallDeferred(MethodName.TransitionToGame);
    }

    private void TransitionToGame()
    {
        GetTree().ChangeSceneToFile(CottageSceneConfig.SceneResPath);
    }
}
