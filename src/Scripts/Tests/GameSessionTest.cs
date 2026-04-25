using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// Unit tests for <see cref="GameSession"/>.
/// </summary>
[TestFixture]
public class GameSessionTest
{
    [SetUp]
    public void SetUp() => GameSession.Clear();

    [TearDown]
    public void TearDown() => GameSession.Clear();

    [Test]
    public void GameSession_InitialState_HasSession_IsFalse()
    {
        Assert.That(GameSession.HasSession, Is.False);
    }

    [Test]
    public void GameSession_AfterStart_HasSession_IsTrue()
    {
        GameSession.Start();
        Assert.That(GameSession.HasSession, Is.True);
    }

    [Test]
    public void GameSession_AfterClear_HasSession_IsFalse()
    {
        GameSession.Start();
        GameSession.Clear();
        Assert.That(GameSession.HasSession, Is.False);
    }

    [Test]
    public void GameSession_StartIsIdempotent()
    {
        GameSession.Start();
        GameSession.Start();
        Assert.That(GameSession.HasSession, Is.True);
    }

    [Test]
    public void MockSaveService_WithSession_HasSaveFile_ReturnsTrue()
    {
        // Verify the same pattern works end-to-end via MockSaveService:
        // MainMenuController uses IsContinueEnabled which delegates to ISaveService.
        GameSession.Start();
        var ctrl = new MainMenuController(new MockSaveService(hasSave: GameSession.HasSession));
        Assert.That(ctrl.IsContinueEnabled, Is.True);
    }
}
