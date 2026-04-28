using System;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class PauseMenuControllerTest
{
    // ── Constructor ───────────────────────────────────────────────────────────

    [Test]
    public void Constructor_NullSaveService_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new PauseMenuController(null!));
    }

    [Test]
    public void Constructor_Defaults_IsPaused_False()
    {
        var ctrl = new PauseMenuController(new MockSaveDataService());
        Assert.IsFalse(ctrl.IsPaused);
    }

    [Test]
    public void Constructor_Defaults_GameSaved_False()
    {
        var ctrl = new PauseMenuController(new MockSaveDataService());
        Assert.IsFalse(ctrl.GameSaved);
    }

    // ── Open / Resume ─────────────────────────────────────────────────────────

    [Test]
    public void Open_SetsPaused_True()
    {
        var ctrl = new PauseMenuController(new MockSaveDataService());
        ctrl.Open();
        Assert.IsTrue(ctrl.IsPaused);
    }

    [Test]
    public void OnResume_AfterOpen_SetsPaused_False()
    {
        var ctrl = new PauseMenuController(new MockSaveDataService());
        ctrl.Open();
        ctrl.OnResume();
        Assert.IsFalse(ctrl.IsPaused);
    }

    // ── Save ──────────────────────────────────────────────────────────────────

    [Test]
    public void OnSaveGame_WhenCalled_SetsSavedFlag()
    {
        var ctrl = new PauseMenuController(new MockSaveDataService());
        ctrl.OnSaveGame();
        Assert.IsTrue(ctrl.GameSaved);
    }

    [Test]
    public void PauseMenu_OnSave_TriggersSave()
    {
        var saveService = new MockSaveDataService();
        var ctrl = new PauseMenuController(saveService);
        ctrl.OnSaveGame();
        Assert.IsTrue(saveService.SaveWasCalled);
    }

    // ── Navigation ────────────────────────────────────────────────────────────

    [Test]
    public void OnSettings_LoadsSettingsScene()
    {
        var loader = new MockSceneLoader();
        var ctrl = new PauseMenuController(new MockSaveDataService(), loader);
        ctrl.OnSettings();
        Assert.AreEqual(MainMenuConfig.SettingsScenePath, loader.LastRequestedPath);
    }

    [Test]
    public void OnMainMenu_LoadsMainMenuScene()
    {
        var loader = new MockSceneLoader();
        var ctrl = new PauseMenuController(new MockSaveDataService(), loader);
        ctrl.OnMainMenu();
        Assert.AreEqual(MainMenuConfig.SceneResPath, loader.LastRequestedPath);
    }

    [Test]
    public void OnSettings_WithoutSceneLoader_DoesNotThrow()
    {
        var ctrl = new PauseMenuController(new MockSaveDataService());
        Assert.DoesNotThrow(() => ctrl.OnSettings());
    }

    [Test]
    public void OnMainMenu_WithoutSceneLoader_DoesNotThrow()
    {
        var ctrl = new PauseMenuController(new MockSaveDataService());
        Assert.DoesNotThrow(() => ctrl.OnMainMenu());
    }
}
