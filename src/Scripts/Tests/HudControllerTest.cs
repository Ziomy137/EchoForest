using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// Unit tests for <see cref="HudController"/>.
/// All tests run without the Godot runtime.
/// </summary>
[TestFixture]
public class HudControllerTest
{
    // ── Initialize ────────────────────────────────────────────────────────────

    [Test]
    public void HudController_TutorialHint_IsVisible_OnStart()
    {
        var hud = new HudController();
        hud.Initialize();
        Assert.That(hud.IsTutorialHintVisible, Is.True);
    }

    [Test]
    public void HudController_BeforeInitialize_TutorialHint_IsNotVisible()
    {
        var hud = new HudController();
        Assert.That(hud.IsTutorialHintVisible, Is.False);
    }

    [Test]
    public void Initialize_ResetTimer_HintVisibleAgain()
    {
        var hud = new HudController(hintTimeoutSeconds: 1f);
        hud.Initialize();
        hud.SimulateTimePassed(2f);           // hide it
        Assert.That(hud.IsTutorialHintVisible, Is.False);

        hud.Initialize();                      // reset
        Assert.That(hud.IsTutorialHintVisible, Is.True);
    }

    // ── Tutorial hint timeout ─────────────────────────────────────────────────

    [Test]
    public void HudController_TutorialHint_BecomesInvisible_AfterTimeout()
    {
        var hud = new HudController(hintTimeoutSeconds: 0.01f);
        hud.Initialize();
        hud.SimulateTimePassed(1f);
        Assert.That(hud.IsTutorialHintVisible, Is.False);
    }

    [Test]
    public void TutorialHint_StillVisible_BeforeTimeout()
    {
        var hud = new HudController(hintTimeoutSeconds: 10f);
        hud.Initialize();
        hud.SimulateTimePassed(5f);
        Assert.That(hud.IsTutorialHintVisible, Is.True);
    }

    [Test]
    public void TutorialHint_HiddenExactlyAtTimeout()
    {
        var hud = new HudController(hintTimeoutSeconds: 5f);
        hud.Initialize();
        hud.SimulateTimePassed(5f);
        Assert.That(hud.IsTutorialHintVisible, Is.False);
    }

    [Test]
    public void SimulateTimePassed_BeforeInitialize_IsNoOp()
    {
        var hud = new HudController(hintTimeoutSeconds: 1f);
        hud.SimulateTimePassed(100f);           // not initialized — should not throw
        Assert.That(hud.IsTutorialHintVisible, Is.False);
    }

    [Test]
    public void SimulateTimePassed_Accumulates_AcrossCalls()
    {
        var hud = new HudController(hintTimeoutSeconds: 3f);
        hud.Initialize();
        hud.SimulateTimePassed(1f);
        hud.SimulateTimePassed(1f);
        hud.SimulateTimePassed(1f);             // total = 3 s → exactly at timeout
        Assert.That(hud.IsTutorialHintVisible, Is.False);
    }

    [Test]
    public void DefaultTimeout_MatchesConstant()
    {
        // Smoke test: default timeout uses Constants.TutorialHintTimeout (10 s).
        var hud = new HudController();
        hud.Initialize();
        hud.SimulateTimePassed(Constants.TutorialHintTimeout - 0.001f);
        Assert.That(hud.IsTutorialHintVisible, Is.True);

        hud.SimulateTimePassed(0.002f);         // push over the threshold
        Assert.That(hud.IsTutorialHintVisible, Is.False);
    }

    // ── Debug label ───────────────────────────────────────────────────────────

    [Test]
    public void HudController_DebugLabel_HiddenInReleaseBuild()
    {
        var hud = new HudController();
        hud.SetDebugMode(false);
        Assert.That(hud.IsDebugLabelVisible, Is.False);
    }

    [Test]
    public void HudController_DebugLabel_VisibleInDebugBuild()
    {
        var hud = new HudController();
        hud.SetDebugMode(true);
        Assert.That(hud.IsDebugLabelVisible, Is.True);
    }

    [Test]
    public void DebugLabel_InitiallyHidden()
    {
        var hud = new HudController();
        Assert.That(hud.IsDebugLabelVisible, Is.False);
    }

    [Test]
    public void SetDebugMode_CanToggleMultipleTimes()
    {
        var hud = new HudController();
        hud.SetDebugMode(true);
        hud.SetDebugMode(false);
        hud.SetDebugMode(true);
        Assert.That(hud.IsDebugLabelVisible, Is.True);
    }

    // ── PlayerStateText ───────────────────────────────────────────────────────

    [Test]
    public void PlayerStateText_InitiallyEmpty()
    {
        var hud = new HudController();
        Assert.That(hud.PlayerStateText, Is.EqualTo(string.Empty));
    }

    [Test]
    public void UpdatePlayerState_SetsText()
    {
        var hud = new HudController();
        hud.UpdatePlayerState("Walking");
        Assert.That(hud.PlayerStateText, Is.EqualTo("Walking"));
    }

    [Test]
    public void UpdatePlayerState_Null_SetsEmptyString()
    {
        var hud = new HudController();
        hud.UpdatePlayerState(null!);
        Assert.That(hud.PlayerStateText, Is.EqualTo(string.Empty));
    }

    [Test]
    public void UpdatePlayerState_CanBeUpdatedMultipleTimes()
    {
        var hud = new HudController();
        hud.UpdatePlayerState("Idle");
        hud.UpdatePlayerState("Running");
        Assert.That(hud.PlayerStateText, Is.EqualTo("Running"));
    }
}
