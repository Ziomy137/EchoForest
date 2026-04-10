using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <see cref="Node"/> wrapper for <see cref="SceneLoader"/>.
/// Bridges Godot's <c>SceneTree.ChangeSceneToFile</c> to the pure-C# loader.
///
/// Cannot be unit-tested without the Godot runtime — excluded from coverage.
/// </summary>
[ExcludeFromCodeCoverage]
public partial class SceneLoaderNode : Node
{
    private SceneLoader _loader = null!;

    public override void _Ready()
    {
        _loader = new SceneLoader(
            changeScene: path => GetTree().ChangeSceneToFile(path),
            changeSceneAsync: path =>
            {
                GetTree().ChangeSceneToFile(path);
                return System.Threading.Tasks.Task.CompletedTask;
            });
    }

    /// <summary>Synchronously switches to the scene at <paramref name="scenePath"/>.</summary>
    public void LoadScene(string scenePath) => _loader.LoadScene(scenePath);

    /// <summary>Asynchronously loads the scene at <paramref name="scenePath"/>.</summary>
    public System.Threading.Tasks.Task LoadSceneAsync(string scenePath) =>
        _loader.LoadSceneAsync(scenePath);
}
