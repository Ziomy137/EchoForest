using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Production <see cref="ISceneLoader"/> that delegates to Godot's
/// <c>SceneTree.ChangeSceneToFile()</c> via <c>Engine.GetMainLoop()</c>.
///
/// Path validation (<c>null</c>, missing <c>res://</c> prefix or <c>.tscn</c>
/// suffix) is performed by <see cref="SceneLoader.ValidatePath"/> so this class
/// honours the same <see cref="ISceneLoader"/> contract and throws
/// <see cref="ArgumentNullException"/>/<see cref="ArgumentException"/>
/// consistently before touching the Godot API.
///
/// Excluded from NUnit code coverage — requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot SceneTree wrapper — requires scene tree")]
public sealed class GodotSceneLoader : ISceneLoader
{
    public void LoadScene(string scenePath)
    {
        SceneLoader.ValidatePath(scenePath); // throws on null / invalid path
        if (Engine.GetMainLoop() is SceneTree tree)
            tree.ChangeSceneToFile(scenePath);
    }

    public Task LoadSceneAsync(string scenePath)
    {
        SceneLoader.ValidatePath(scenePath); // throws on null / invalid path
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
