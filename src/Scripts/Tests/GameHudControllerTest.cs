using System;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// Unit tests for <see cref="GameHudController"/>.
/// All tests run without the Godot runtime.
/// </summary>
[TestFixture]
public class GameHudControllerTest
{
    // ── Defaults ──────────────────────────────────────────────────────────────

    [Test]
    public void Constructor_HealthFillRatio_DefaultsToZero()
    {
        var ctrl = new GameHudController();
        Assert.That(ctrl.HealthFillRatio, Is.EqualTo(0f));
    }

    [Test]
    public void Constructor_IsInteractionPromptVisible_DefaultsFalse()
    {
        var ctrl = new GameHudController();
        Assert.That(ctrl.IsInteractionPromptVisible, Is.False);
    }

    [Test]
    public void Constructor_QuestFields_DefaultToEmpty()
    {
        var ctrl = new GameHudController();
        Assert.Multiple(() =>
        {
            Assert.That(ctrl.CurrentQuestName, Is.Empty);
            Assert.That(ctrl.CurrentObjectiveText, Is.Empty);
            Assert.That(ctrl.ObjectiveProgress, Is.Empty);
        });
    }

    // ── UpdateHealth ──────────────────────────────────────────────────────────

    [Test]
    public void HUD_HealthBar_ReflectsCurrentHealth()
    {
        var ctrl = new GameHudController();
        ctrl.UpdateHealth(50f, 100f);
        Assert.That(ctrl.HealthFillRatio, Is.EqualTo(0.5f).Within(0.001f));
    }

    [Test]
    public void HUD_HealthBar_ClampedBetween0And1_Overflow()
    {
        var ctrl = new GameHudController();
        ctrl.UpdateHealth(150f, 100f);
        Assert.That(ctrl.HealthFillRatio, Is.LessThanOrEqualTo(1f));
    }

    [Test]
    public void HUD_HealthBar_ClampedBetween0And1_Underflow()
    {
        var ctrl = new GameHudController();
        ctrl.UpdateHealth(-10f, 100f);
        Assert.That(ctrl.HealthFillRatio, Is.GreaterThanOrEqualTo(0f));
    }

    [Test]
    public void UpdateHealth_FullHealth_RatioIsOne()
    {
        var ctrl = new GameHudController();
        ctrl.UpdateHealth(100f, 100f);
        Assert.That(ctrl.HealthFillRatio, Is.EqualTo(1f).Within(0.001f));
    }

    [Test]
    public void UpdateHealth_ZeroHealth_RatioIsZero()
    {
        var ctrl = new GameHudController();
        ctrl.UpdateHealth(0f, 100f);
        Assert.That(ctrl.HealthFillRatio, Is.EqualTo(0f));
    }

    [Test]
    public void UpdateHealth_StoresCurrentAndMax()
    {
        var ctrl = new GameHudController();
        ctrl.UpdateHealth(75f, 200f);
        Assert.Multiple(() =>
        {
            Assert.That(ctrl.CurrentHealth, Is.EqualTo(75f));
            Assert.That(ctrl.MaxHealth, Is.EqualTo(200f));
        });
    }

    [Test]
    public void UpdateHealth_ZeroMax_Throws()
    {
        var ctrl = new GameHudController();
        Assert.Throws<ArgumentException>(() => ctrl.UpdateHealth(50f, 0f));
    }

    [Test]
    public void UpdateHealth_NegativeMax_Throws()
    {
        var ctrl = new GameHudController();
        Assert.Throws<ArgumentException>(() => ctrl.UpdateHealth(50f, -1f));
    }

    // ── Interaction prompt ────────────────────────────────────────────────────

    [Test]
    public void HUD_InteractionPrompt_Visible_WhenShown()
    {
        var ctrl = new GameHudController();
        ctrl.ShowInteractionPrompt("Talk");
        Assert.That(ctrl.IsInteractionPromptVisible, Is.True);
    }

    [Test]
    public void HUD_InteractionPrompt_Hidden_WhenHidden()
    {
        var ctrl = new GameHudController();
        ctrl.ShowInteractionPrompt("Talk");
        ctrl.HideInteractionPrompt();
        Assert.That(ctrl.IsInteractionPromptVisible, Is.False);
    }

    [Test]
    public void ShowInteractionPrompt_SetsText()
    {
        var ctrl = new GameHudController();
        ctrl.ShowInteractionPrompt("Open");
        Assert.That(ctrl.InteractionPromptText, Is.EqualTo("Open"));
    }

    [Test]
    public void HideInteractionPrompt_ClearsText()
    {
        var ctrl = new GameHudController();
        ctrl.ShowInteractionPrompt("Talk");
        ctrl.HideInteractionPrompt();
        Assert.That(ctrl.InteractionPromptText, Is.Empty);
    }

    [Test]
    public void ShowInteractionPrompt_NullAction_SetsEmpty()
    {
        var ctrl = new GameHudController();
        ctrl.ShowInteractionPrompt(null!);
        Assert.That(ctrl.InteractionPromptText, Is.Empty);
        Assert.That(ctrl.IsInteractionPromptVisible, Is.True);
    }

    // ── Quest objective ───────────────────────────────────────────────────────

    [Test]
    public void HUD_QuestObjective_UpdatesText()
    {
        var ctrl = new GameHudController();
        ctrl.SetQuestObjective("Kidnapped", "Wake up", 1, 3);
        Assert.Multiple(() =>
        {
            Assert.That(ctrl.CurrentQuestName, Is.EqualTo("Kidnapped"));
            Assert.That(ctrl.CurrentObjectiveText, Is.EqualTo("Wake up"));
            Assert.That(ctrl.ObjectiveProgress, Is.EqualTo("1/3"));
        });
    }

    [Test]
    public void SetQuestObjective_ZeroProgress_Formats()
    {
        var ctrl = new GameHudController();
        ctrl.SetQuestObjective("Quest", "Do thing", 0, 5);
        Assert.That(ctrl.ObjectiveProgress, Is.EqualTo("0/5"));
    }

    [Test]
    public void SetQuestObjective_NullStrings_DefaultToEmpty()
    {
        var ctrl = new GameHudController();
        ctrl.SetQuestObjective(null!, null!, 1, 3);
        Assert.Multiple(() =>
        {
            Assert.That(ctrl.CurrentQuestName, Is.Empty);
            Assert.That(ctrl.CurrentObjectiveText, Is.Empty);
        });
    }

    // ── Active weapon ─────────────────────────────────────────────────────────

    [Test]
    public void SetActiveWeapon_StoresId()
    {
        var ctrl = new GameHudController();
        ctrl.SetActiveWeapon("sword_iron");
        Assert.That(ctrl.ActiveWeaponId, Is.EqualTo("sword_iron"));
    }

    [Test]
    public void SetActiveWeapon_NullId_SetsEmpty()
    {
        var ctrl = new GameHudController();
        ctrl.SetActiveWeapon("sword_iron");
        ctrl.SetActiveWeapon(null);
        Assert.That(ctrl.ActiveWeaponId, Is.Empty);
    }

    // ── Minimap ───────────────────────────────────────────────────────────────

    [Test]
    public void UpdateMinimap_StoresPlayerPosition()
    {
        var ctrl = new GameHudController();
        ctrl.UpdateMinimap(320f, 240f, "cottage");
        Assert.Multiple(() =>
        {
            Assert.That(ctrl.MinimapPlayerX, Is.EqualTo(320f));
            Assert.That(ctrl.MinimapPlayerY, Is.EqualTo(240f));
            Assert.That(ctrl.MinimapAreaId, Is.EqualTo("cottage"));
        });
    }

    [Test]
    public void UpdateMinimap_NullAreaId_SetsEmpty()
    {
        var ctrl = new GameHudController();
        ctrl.UpdateMinimap(0f, 0f, null!);
        Assert.That(ctrl.MinimapAreaId, Is.Empty);
    }
}
