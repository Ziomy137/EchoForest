using System;
using System.Threading.Tasks;
using NUnit.Framework;
using EchoForest.Core;
using EchoForest.Core.Scenes;

namespace EchoForest.Tests;

/// <summary>
/// Tests for <see cref="SceneLoader"/>. Validates path rules and delegate
/// invocation without requiring the Godot runtime.
/// </summary>
[TestFixture]
public class SceneLoaderTest
{
    // ── Interface contract ────────────────────────────────────────────────────

    [Test]
    public void SceneLoader_ImplementsISceneLoader()
    {
        Assert.That(new SceneLoader(), Is.InstanceOf<ISceneLoader>());
    }

    // ── Null / empty paths ────────────────────────────────────────────────────

    [Test]
    public void LoadScene_NullPath_ThrowsArgumentNullException()
    {
        var loader = new SceneLoader();

        Assert.That(() => loader.LoadScene(null!),
            Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void LoadScene_EmptyPath_ThrowsArgumentException()
    {
        var loader = new SceneLoader();

        Assert.That(() => loader.LoadScene(string.Empty),
            Throws.TypeOf<ArgumentException>());
    }

    // ── Invalid path format ───────────────────────────────────────────────────

    [Test]
    public void LoadScene_PathMissingResProtocol_ThrowsArgumentException()
    {
        var loader = new SceneLoader();

        Assert.That(() => loader.LoadScene("Scenes/TestArea_Cottage.tscn"),
            Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void LoadScene_PathMissingTscnExtension_ThrowsArgumentException()
    {
        var loader = new SceneLoader();

        Assert.That(() => loader.LoadScene("res://Scenes/TestArea_Cottage"),
            Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void LoadScene_PathWithWrongExtension_ThrowsArgumentException()
    {
        var loader = new SceneLoader();

        Assert.That(() => loader.LoadScene("res://Scenes/TestArea_Cottage.tres"),
            Throws.TypeOf<ArgumentException>());
    }

    // ── Valid path — no delegate ──────────────────────────────────────────────

    [Test]
    public void LoadScene_ValidPath_NoDelegate_DoesNotThrow()
    {
        var loader = new SceneLoader(); // no delegate

        Assert.That(() => loader.LoadScene("res://src/Scenes/TestArea_Cottage.tscn"),
            Throws.Nothing);
    }

    // ── Valid path — delegate invoked ─────────────────────────────────────────

    [Test]
    public void LoadScene_ValidPath_DelegateIsInvoked()
    {
        string? calledWith = null;
        var loader = new SceneLoader(changeScene: path => calledWith = path);

        loader.LoadScene("res://src/Scenes/TestArea_Cottage.tscn");

        Assert.That(calledWith, Is.EqualTo("res://src/Scenes/TestArea_Cottage.tscn"));
    }

    [Test]
    public void LoadScene_ValidPath_DelegateIsInvokedExactlyOnce()
    {
        int callCount = 0;
        var loader = new SceneLoader(changeScene: _ => callCount++);

        loader.LoadScene("res://src/Scenes/TestArea_Cottage.tscn");

        Assert.That(callCount, Is.EqualTo(1));
    }

    // ── LoadSceneAsync — null / empty ──────────────────────────────────────────

    [Test]
    public void LoadSceneAsync_NullPath_ThrowsArgumentNullException()
    {
        var loader = new SceneLoader();

        Assert.That(async () => await loader.LoadSceneAsync(null!),
            Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void LoadSceneAsync_EmptyPath_ThrowsArgumentException()
    {
        var loader = new SceneLoader();

        Assert.That(async () => await loader.LoadSceneAsync(string.Empty),
            Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void LoadSceneAsync_PathMissingResProtocol_ThrowsArgumentException()
    {
        var loader = new SceneLoader();

        Assert.That(async () => await loader.LoadSceneAsync("Scenes/Cottage.tscn"),
            Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void LoadSceneAsync_PathMissingTscnExtension_ThrowsArgumentException()
    {
        var loader = new SceneLoader();

        Assert.That(async () => await loader.LoadSceneAsync("res://Scenes/Cottage"),
            Throws.TypeOf<ArgumentException>());
    }

    // ── LoadSceneAsync — valid path ────────────────────────────────────────────

    [Test]
    public async Task LoadSceneAsync_ValidPath_NoDelegate_ReturnsCompletedTask()
    {
        var loader = new SceneLoader();

        await loader.LoadSceneAsync("res://src/Scenes/TestArea_Cottage.tscn");

        Assert.Pass(); // no exception = pass
    }

    [Test]
    public async Task LoadSceneAsync_ValidPath_DelegateIsInvoked()
    {
        string? calledWith = null;
        var loader = new SceneLoader(
            changeSceneAsync: path =>
            {
                calledWith = path;
                return Task.CompletedTask;
            });

        await loader.LoadSceneAsync("res://src/Scenes/TestArea_Cottage.tscn");

        Assert.That(calledWith, Is.EqualTo("res://src/Scenes/TestArea_Cottage.tscn"));
    }

    // ── TestAreaConfig integration ────────────────────────────────────────────

    [Test]
    public void LoadScene_WithTestAreaConfigPath_DoesNotThrow()
    {
        // Verify that TestAreaConfig.ScenePath is a valid scene path
        var loader = new SceneLoader();

        Assert.That(() => loader.LoadScene(TestAreaConfig.ScenePath),
            Throws.Nothing);
    }
}
