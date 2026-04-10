using System;
using System.Threading.Tasks;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# implementation of <see cref="ISceneLoader"/>. Validates scene paths
/// and delegates the actual Godot scene transition to injected callbacks, making
/// this class fully testable without the Godot runtime.
///
/// For production use, wrap this in <c>SceneLoaderNode</c> (a Godot Node that
/// passes <c>GetTree().ChangeSceneToFile</c> as the sync delegate).
/// </summary>
public sealed class SceneLoader : ISceneLoader
{
    private readonly Action<string>? _changeScene;
    private readonly Func<string, Task>? _changeSceneAsync;

    /// <summary>
    /// Creates a <see cref="SceneLoader"/> with optional Godot-backed delegates.
    /// When delegates are <c>null</c> (e.g. in unit tests) the loader only
    /// performs path validation and does not attempt any scene transition.
    /// </summary>
    /// <param name="changeScene">Synchronous scene-change callback (pass <c>GetTree().ChangeSceneToFile</c>).</param>
    /// <param name="changeSceneAsync">Asynchronous scene-change callback.</param>
    public SceneLoader(
        Action<string>? changeScene = null,
        Func<string, Task>? changeSceneAsync = null)
    {
        _changeScene = changeScene;
        _changeSceneAsync = changeSceneAsync;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">When <paramref name="scenePath"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">When <paramref name="scenePath"/> is not a valid scene resource path.</exception>
    public void LoadScene(string scenePath)
    {
        ValidatePath(scenePath);
        _changeScene?.Invoke(scenePath);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">When <paramref name="scenePath"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">When <paramref name="scenePath"/> is not a valid scene resource path.</exception>
    public Task LoadSceneAsync(string scenePath)
    {
        ValidatePath(scenePath);
        return _changeSceneAsync?.Invoke(scenePath) ?? Task.CompletedTask;
    }

    // ── Internal validation ───────────────────────────────────────────────────

    private static void ValidatePath(string? scenePath)
    {
        if (scenePath is null)
            throw new ArgumentNullException(nameof(scenePath),
                "Scene path must not be null.");

        if (scenePath.Length == 0)
            throw new ArgumentException(
                "Scene path must not be empty.", nameof(scenePath));

        if (!scenePath.StartsWith("res://", StringComparison.Ordinal))
            throw new ArgumentException(
                $"Scene path must start with 'res://'. Got: '{scenePath}'",
                nameof(scenePath));

        if (!scenePath.EndsWith(".tscn", StringComparison.Ordinal))
            throw new ArgumentException(
                $"Scene path must end with '.tscn'. Got: '{scenePath}'",
                nameof(scenePath));
    }
}
