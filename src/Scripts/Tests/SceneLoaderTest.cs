using System;
using System.Threading.Tasks;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// Unit tests for <see cref="SceneLoader"/>.
/// All tests run without the Godot runtime — only path validation and interface
/// conformance are covered at this level. Actual scene loading is tested via GUT.
/// </summary>
[TestFixture]
public class SceneLoaderTest
{
    private SceneLoader _loader = null!;

    [SetUp]
    public void SetUp() => _loader = new SceneLoader();

    // ── Interface conformance ─────────────────────────────────────────────────

    [Test]
    public void SceneLoader_ImplementsInterface() =>
        Assert.That(_loader, Is.InstanceOf<ISceneLoader>());

    // ── LoadScene — null / invalid path ───────────────────────────────────────

    [Test]
    public void SceneLoader_NullPath_ThrowsArgumentNullException()
    {
        Assert.That(() => _loader.LoadScene(null!),
            Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void SceneLoader_InvalidPath_ThrowsArgumentException()
    {
        Assert.That(() => _loader.LoadScene("invalid/path/that/doesnt/exist"),
            Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void LoadScene_EmptyString_ThrowsArgumentException()
    {
        Assert.That(() => _loader.LoadScene(string.Empty),
            Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void LoadScene_PathWithoutResProtocol_ThrowsArgumentException()
    {
        Assert.That(() => _loader.LoadScene("Scenes/TestArea_Cottage.tscn"),
            Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void LoadScene_PathWithoutTscnExtension_ThrowsArgumentException()
    {
        Assert.That(() => _loader.LoadScene("res://Scenes/TestArea_Cottage.gd"),
            Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void LoadScene_ValidPath_DoesNotThrow()
    {
        Assert.That(
            () => _loader.LoadScene("res://src/Scenes/TestArea_Cottage.tscn"),
            Throws.Nothing);
    }

    // ── LoadSceneAsync — null / invalid path ──────────────────────────────────

    [Test]
    public void LoadSceneAsync_NullPath_ThrowsArgumentNullException()
    {
        Assert.That(
            async () => await _loader.LoadSceneAsync(null!),
            Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void LoadSceneAsync_InvalidPath_ThrowsArgumentException()
    {
        Assert.That(
            async () => await _loader.LoadSceneAsync("invalid/path"),
            Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public async Task LoadSceneAsync_ValidPath_CompletesSuccessfully()
    {
        await _loader.LoadSceneAsync("res://src/Scenes/GameBootstrap.tscn");
        Assert.Pass();
    }

    // ── GetCurrentScene ───────────────────────────────────────────────────────

    [Test]
    public void GetCurrentScene_Initially_ReturnsNull()
    {
        Assert.That(_loader.GetCurrentScene(), Is.Null);
    }

    // ── SetCurrentScene ───────────────────────────────────────────────────────

    [Test]
    public void SetCurrentScene_Null_LeavesCurrentSceneNull()
    {
        _loader.SetCurrentScene(null);
        Assert.That(_loader.GetCurrentScene(), Is.Null);
    }
}
