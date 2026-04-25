using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Production <see cref="ISceneLoader"/> that delegates to Godot's
/// <c>SceneTree.ChangeSceneToFile()</c> via <c>Engine.GetMainLoop()</c>.
///
/// Excluded from NUnit code coverage — requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot SceneTree wrapper — requires scene tree")]
public sealed class GodotSceneLoader : ISceneLoader
{
    public void LoadScene(string scenePath)
    {
        if (Engine.GetMainLoop() is SceneTree tree)
            tree.ChangeSceneToFile(scenePath);
    }

    public Task LoadSceneAsync(string scenePath)
    {
        if (Engine.GetMainLoop() is SceneTree tree)
            tree.ChangeSceneToFile(scenePath);
        return Task.CompletedTask;
    }

    public Node? GetCurrentScene()
    {
        if (Engine.GetMainLoop() is SceneTree tree)
            return tree.CurrentScene;
        return null;
    }
}
