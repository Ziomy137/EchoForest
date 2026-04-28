using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// Unit tests for <see cref="CreditsController"/>.
/// All tests run without the Godot runtime.
/// </summary>
[TestFixture]
public class CreditsControllerTest
{
    // ── Construction ──────────────────────────────────────────────────────────

    [Test]
    public void Constructor_NullSceneLoader_Throws()
    {
        Assert.Throws<System.ArgumentNullException>(() => _ = new CreditsController(null!));
    }

    // ── OnBack ────────────────────────────────────────────────────────────────

    [Test]
    public void OnBack_NavigatesTo_MainMenu()
    {
        var loader = new MockSceneLoader();
        var ctrl = new CreditsController(loader);

        ctrl.OnBack();

        Assert.That(loader.WasLoadRequested, Is.True);
        Assert.That(loader.LastRequestedPath, Is.EqualTo(MainMenuConfig.SceneResPath));
    }

}
