using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// Unit tests for <see cref="MainMenuController"/>.
/// All tests run without the Godot runtime.
/// </summary>
[TestFixture]
public class MainMenuControllerTest
{
    // ── IsContinueEnabled ─────────────────────────────────────────────────────

    [Test]
    public void MainMenu_ContinueButton_DisabledWhenNoSaveFile()
    {
        var ctrl = new MainMenuController(new MockSaveService(hasSave: false));
        Assert.That(ctrl.IsContinueEnabled, Is.False);
    }

    [Test]
    public void MainMenu_ContinueButton_EnabledWhenSaveExists()
    {
        var ctrl = new MainMenuController(new MockSaveService(hasSave: true));
        Assert.That(ctrl.IsContinueEnabled, Is.True);
    }

    // ── OnNewGame ─────────────────────────────────────────────────────────────

    [Test]
    public void MainMenu_OnNewGame_RequestsSceneLoad()
    {
        var sceneLoader = new MockSceneLoader();
        var ctrl = new MainMenuController(new MockSaveService(), sceneLoader);
        ctrl.OnNewGame();
        Assert.That(sceneLoader.WasLoadRequested, Is.True);
    }

    [Test]
    public void MainMenu_OnNewGame_LoadsGameBootstrapScene()
    {
        var sceneLoader = new MockSceneLoader();
        var ctrl = new MainMenuController(new MockSaveService(), sceneLoader);
        ctrl.OnNewGame();
        Assert.That(sceneLoader.LastRequestedPath, Is.EqualTo(MainMenuConfig.GameBootstrapScenePath));
    }

    [Test]
    public void MainMenu_OnNewGame_WithNoSceneLoader_DoesNotThrow()
    {
        var ctrl = new MainMenuController(new MockSaveService());
        Assert.DoesNotThrow(() => ctrl.OnNewGame());
    }

    // ── OnContinue ────────────────────────────────────────────────────────────

    [Test]
    public void MainMenu_OnContinue_RequestsSceneLoad()
    {
        var sceneLoader = new MockSceneLoader();
        var ctrl = new MainMenuController(new MockSaveService(hasSave: true), sceneLoader);
        ctrl.OnContinue();
        Assert.That(sceneLoader.WasLoadRequested, Is.True);
    }

    [Test]
    public void MainMenu_OnContinue_LoadsContinueScene()
    {
        var sceneLoader = new MockSceneLoader();
        var ctrl = new MainMenuController(new MockSaveService(hasSave: true), sceneLoader);
        ctrl.OnContinue();
        Assert.That(sceneLoader.LastRequestedPath, Is.EqualTo(MainMenuConfig.ContinueScenePath));
    }

    // ── OnLoadGame ────────────────────────────────────────────────────────────

    [Test]
    public void MainMenu_OnLoadGame_RequestsSceneLoad()
    {
        var sceneLoader = new MockSceneLoader();
        var ctrl = new MainMenuController(new MockSaveService(), sceneLoader);
        ctrl.OnLoadGame();
        Assert.That(sceneLoader.WasLoadRequested, Is.True);
    }

    [Test]
    public void MainMenu_OnLoadGame_LoadsLoadGameScene()
    {
        var sceneLoader = new MockSceneLoader();
        var ctrl = new MainMenuController(new MockSaveService(), sceneLoader);
        ctrl.OnLoadGame();
        Assert.That(sceneLoader.LastRequestedPath, Is.EqualTo(MainMenuConfig.LoadGameScenePath));
    }

    // ── OnSettings ────────────────────────────────────────────────────────────

    [Test]
    public void MainMenu_OnSettings_RequestsSceneLoad()
    {
        var sceneLoader = new MockSceneLoader();
        var ctrl = new MainMenuController(new MockSaveService(), sceneLoader);
        ctrl.OnSettings();
        Assert.That(sceneLoader.WasLoadRequested, Is.True);
    }

    [Test]
    public void MainMenu_OnSettings_LoadsSettingsScene()
    {
        var sceneLoader = new MockSceneLoader();
        var ctrl = new MainMenuController(new MockSaveService(), sceneLoader);
        ctrl.OnSettings();
        Assert.That(sceneLoader.LastRequestedPath, Is.EqualTo(MainMenuConfig.SettingsScenePath));
    }

    // ── OnCredits ─────────────────────────────────────────────────────────────

    [Test]
    public void MainMenu_OnCredits_RequestsSceneLoad()
    {
        var sceneLoader = new MockSceneLoader();
        var ctrl = new MainMenuController(new MockSaveService(), sceneLoader);
        ctrl.OnCredits();
        Assert.That(sceneLoader.WasLoadRequested, Is.True);
    }

    [Test]
    public void MainMenu_OnCredits_LoadsCreditsScene()
    {
        var sceneLoader = new MockSceneLoader();
        var ctrl = new MainMenuController(new MockSaveService(), sceneLoader);
        ctrl.OnCredits();
        Assert.That(sceneLoader.LastRequestedPath, Is.EqualTo(MainMenuConfig.CreditsScenePath));
    }

    // ── OnExit ────────────────────────────────────────────────────────────────

    [Test]
    public void MainMenu_OnExit_CallsQuit()
    {
        var appCtrl = new MockApplicationController();
        var ctrl = new MainMenuController(new MockSaveService(), appCtrl: appCtrl);
        ctrl.OnExit();
        Assert.That(appCtrl.QuitWasCalled, Is.True);
    }

    [Test]
    public void MainMenu_OnExit_WithNoAppCtrl_DoesNotThrow()
    {
        var ctrl = new MainMenuController(new MockSaveService());
        Assert.DoesNotThrow(() => ctrl.OnExit());
    }

    // ── Constructor ───────────────────────────────────────────────────────────

    [Test]
    public void Constructor_NullSaveService_ThrowsArgumentNullException()
    {
        Assert.That(
            () => new MainMenuController(null!),
            Throws.TypeOf<System.ArgumentNullException>());
    }

    [Test]
    public void MainMenuController_ImplementsInterface()
    {
        var ctrl = new MainMenuController(new MockSaveService());
        Assert.That(ctrl, Is.InstanceOf<IMainMenuController>());
    }

    // ── NullSaveService ───────────────────────────────────────────────────────

    [Test]
    public void NullSaveService_HasSaveFile_ReturnsFalse()
    {
        var svc = new NullSaveService();
        Assert.That(svc.HasSaveFile(), Is.False);
    }

    // ── MockSaveService ───────────────────────────────────────────────────────

    [Test]
    public void MockSaveService_DefaultConstructor_HasSaveFile_ReturnsFalse()
    {
        var svc = new MockSaveService();
        Assert.That(svc.HasSaveFile(), Is.False);
    }

    [Test]
    public void MockSaveService_HasSaveTrue_HasSaveFile_ReturnsTrue()
    {
        var svc = new MockSaveService(hasSave: true);
        Assert.That(svc.HasSaveFile(), Is.True);
    }

    // ── MockApplicationController ─────────────────────────────────────────────

    [Test]
    public void MockApplicationController_BeforeQuit_QuitWasCalled_IsFalse()
    {
        var appCtrl = new MockApplicationController();
        Assert.That(appCtrl.QuitWasCalled, Is.False);
    }

    [Test]
    public void MockApplicationController_AfterQuit_QuitWasCalled_IsTrue()
    {
        var appCtrl = new MockApplicationController();
        appCtrl.Quit();
        Assert.That(appCtrl.QuitWasCalled, Is.True);
    }

    // ── MockSceneLoader ───────────────────────────────────────────────────────

    [Test]
    public void MockSceneLoader_BeforeLoad_WasLoadRequested_IsFalse()
    {
        var loader = new MockSceneLoader();
        Assert.That(loader.WasLoadRequested, Is.False);
    }

    [Test]
    public void MockSceneLoader_AfterLoadScene_WasLoadRequested_IsTrue()
    {
        var loader = new MockSceneLoader();
        loader.LoadScene("res://test.tscn");
        Assert.That(loader.WasLoadRequested, Is.True);
    }

    [Test]
    public void MockSceneLoader_AfterLoadScene_LastRequestedPath_IsSet()
    {
        var loader = new MockSceneLoader();
        loader.LoadScene("res://test.tscn");
        Assert.That(loader.LastRequestedPath, Is.EqualTo("res://test.tscn"));
    }

    [Test]
    public void MockSceneLoader_GetCurrentScene_ReturnsNull()
    {
        var loader = new MockSceneLoader();
        Assert.That(loader.GetCurrentScene(), Is.Null);
    }

    [Test]
    public async System.Threading.Tasks.Task MockSceneLoader_LoadSceneAsync_SetsWasLoadRequested()
    {
        var loader = new MockSceneLoader();
        await loader.LoadSceneAsync("res://test.tscn");
        Assert.That(loader.WasLoadRequested, Is.True);
        Assert.That(loader.LastRequestedPath, Is.EqualTo("res://test.tscn"));
    }

    // ── MainMenuConfig ────────────────────────────────────────────────────────

    [Test]
    public void MainMenuConfig_SceneResPath_IsNotEmpty()
    {
        Assert.That(MainMenuConfig.SceneResPath, Is.Not.Empty);
    }

    [Test]
    public void MainMenuConfig_GameBootstrapScenePath_IsNotEmpty()
    {
        Assert.That(MainMenuConfig.GameBootstrapScenePath, Is.Not.Empty);
    }

    [Test]
    public void MainMenuConfig_LoadGameScenePath_IsNotEmpty()
    {
        Assert.That(MainMenuConfig.LoadGameScenePath, Is.Not.Empty);
    }

    [Test]
    public void MainMenuConfig_SettingsScenePath_IsNotEmpty()
    {
        Assert.That(MainMenuConfig.SettingsScenePath, Is.Not.Empty);
    }

    [Test]
    public void MainMenuConfig_CreditsScenePath_IsNotEmpty()
    {
        Assert.That(MainMenuConfig.CreditsScenePath, Is.Not.Empty);
    }
}
