using System;
using System.Threading.Tasks;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# scene loader implementing <see cref="ISceneLoader"/>.
///
/// Path validation is performed before any Godot API calls, keeping this class
/// fully unit-testable with NUnit without the Godot runtime.
///
/// The Godot-coupled wrapper node is responsible for calling
/// <see cref="SceneTree.ChangeSceneToFile"/> and invoking
/// <see cref="SetCurrentScene"/> after instantiation.
/// </summary>
public sealed class SceneLoader : ISceneLoader
{
    private Node? _currentScene;

    // ── ISceneLoader ──────────────────────────────────────────────────────────

    /// <inheritdoc/>
    public void LoadScene(string scenePath)
    {
        ValidatePath(scenePath);
        // Actual scene switching is delegated to the Godot node wrapper.
        // Path validity has been confirmed at this point.
    }

    /// <inheritdoc/>
    public async Task LoadSceneAsync(string scenePath)
    {
        ValidatePath(scenePath);
        // Background loading would use ResourceLoader.LoadThreadedRequest in
        // the Godot wrapper. Here we satisfy the async contract for testability.
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Node? GetCurrentScene() => _currentScene;

    // ── Scene tracking ────────────────────────────────────────────────────────

    /// <summary>
    /// Called by the Godot wrapper node after a scene has been loaded and
    /// instantiated, to update the tracked current scene reference.
    /// Pass <c>null</c> to clear the tracked scene.
    /// </summary>
    public void SetCurrentScene(Node? scene) => _currentScene = scene;

    // ── Validation ────────────────────────────────────────────────────────────

    /// <summary>
    /// Validates a scene path.
    /// </summary>
    /// <exception cref="ArgumentNullException">Path is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// Path does not start with <c>res://</c> or does not end with <c>.tscn</c>.
    /// </exception>
    internal static void ValidatePath(string scenePath)
    {
        if (scenePath is null)
            throw new ArgumentNullException(nameof(scenePath), "Scene path must not be null.");

        if (!scenePath.StartsWith("res://", StringComparison.Ordinal) ||
            !scenePath.EndsWith(".tscn", StringComparison.Ordinal))
            throw new ArgumentException(
                $"Scene path must start with 'res://' and end with '.tscn'. Got: '{scenePath}'",
                nameof(scenePath));
    }
}
