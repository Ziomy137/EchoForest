using NUnit.Framework;
using System.Collections.Generic;
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
    public void GameSession_IntroCutsceneRequest_IsConsumedOnce()
    {
        GameSession.Start();
        GameSession.RequestIntroCutscene();

        Assert.Multiple(() =>
        {
            Assert.That(GameSession.ConsumeIntroCutsceneRequest(), Is.True);
            Assert.That(GameSession.ConsumeIntroCutsceneRequest(), Is.False);
        });
    }

    [Test]
    public void GameSession_ApplyLoadedSave_ClearsPendingIntroCutscene()
    {
        GameSession.RequestIntroCutscene();

        GameSession.ApplyLoadedSave(new SaveData());

        Assert.That(GameSession.ConsumeIntroCutsceneRequest(), Is.False);
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

    // ── Player position ───────────────────────────────────────────────────────

    [Test]
    public void GameSession_InitialState_HasPlayerPosition_IsFalse()
    {
        Assert.That(GameSession.HasPlayerPosition, Is.False);
    }

    [Test]
    public void GameSession_SavePlayerPosition_SetsCoordinates()
    {
        GameSession.SavePlayerPosition(123.4f, 567.8f);

        Assert.Multiple(() =>
        {
            Assert.That(GameSession.HasPlayerPosition, Is.True);
            Assert.That(GameSession.LastPlayerX, Is.EqualTo(123.4f).Within(0.001f));
            Assert.That(GameSession.LastPlayerY, Is.EqualTo(567.8f).Within(0.001f));
        });
    }

    [Test]
    public void GameSession_Start_ClearsPlayerPosition()
    {
        GameSession.SavePlayerPosition(10f, 20f);
        GameSession.Start();

        Assert.That(GameSession.HasPlayerPosition, Is.False);
    }

    [Test]
    public void GameSession_Clear_ClearsPlayerPosition()
    {
        GameSession.SavePlayerPosition(10f, 20f);
        GameSession.Clear();

        Assert.Multiple(() =>
        {
            Assert.That(GameSession.HasPlayerPosition, Is.False);
            Assert.That(GameSession.LastPlayerX, Is.EqualTo(0f));
            Assert.That(GameSession.LastPlayerY, Is.EqualTo(0f));
        });
    }

    [Test]
    public void GameSession_SavePlayerPosition_OverwritesPrevious()
    {
        GameSession.SavePlayerPosition(10f, 20f);
        GameSession.SavePlayerPosition(99f, 88f);

        Assert.Multiple(() =>
        {
            Assert.That(GameSession.LastPlayerX, Is.EqualTo(99f).Within(0.001f));
            Assert.That(GameSession.LastPlayerY, Is.EqualTo(88f).Within(0.001f));
        });
    }

    [Test]
    public void GameSession_ApplyLoadedSave_RestoresPositionAndQuestStates()
    {
        var saveData = new SaveData
        {
            PlayerX = 123.4f,
            PlayerY = 567.8f,
            QuestStates = new Dictionary<string, QuestState>
            {
                ["q_kidnapped"] = QuestState.Completed,
                ["q_seek_mage"] = QuestState.Active,
            },
        };

        GameSession.ApplyLoadedSave(saveData);

        Assert.Multiple(() =>
        {
            Assert.That(GameSession.HasSession, Is.True);
            Assert.That(GameSession.LastPlayerX, Is.EqualTo(123.4f));
            Assert.That(GameSession.LastPlayerY, Is.EqualTo(567.8f));
            Assert.That(GameSession.QuestStates, Is.EqualTo(saveData.QuestStates));
        });
    }

    // ── GodotSessionSaveService ───────────────────────────────────────────────

    [Test]
    public void GodotSessionSaveService_NoSession_HasSaveFile_ReturnsFalse()
    {
        var svc = new GodotSessionSaveService();
        Assert.That(svc.HasSaveFile(), Is.False);
    }

    [Test]
    public void GodotSessionSaveService_AfterStart_HasSaveFile_ReturnsTrue()
    {
        GameSession.Start();
        var svc = new GodotSessionSaveService();
        Assert.That(svc.HasSaveFile(), Is.True);
    }

    [Test]
    public void GodotSessionSaveService_AfterClear_HasSaveFile_ReturnsFalse()
    {
        GameSession.Start();
        GameSession.Clear();
        var svc = new GodotSessionSaveService();
        Assert.That(svc.HasSaveFile(), Is.False);
    }
}
