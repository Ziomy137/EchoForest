using System.Threading.Tasks;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// In-memory implementation of <see cref="ISceneLoader"/> for use in NUnit tests.
/// Records the scene path requested without performing any actual scene loading.
/// </summary>
public sealed class MockSceneLoader : ISceneLoader
{
    /// <summary>Whether <see cref="LoadScene"/> or <see cref="LoadSceneAsync"/> was called.</summary>
    public bool WasLoadRequested { get; private set; }

    /// <summary>The scene path passed to the most recent load call, or <c>null</c> if none.</summary>
    public string? LastRequestedPath { get; private set; }

    /// <inheritdoc/>
    public void LoadScene(string scenePath)
    {
        WasLoadRequested = true;
        LastRequestedPath = scenePath;
    }

    /// <inheritdoc/>
    public Task LoadSceneAsync(string scenePath)
    {
        WasLoadRequested = true;
        LastRequestedPath = scenePath;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Node? GetCurrentScene() => null;
}
