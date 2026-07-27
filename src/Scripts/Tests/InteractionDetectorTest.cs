using System;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class InteractionDetectorTest
{
    private InteractionDetector _detector = null!;
    private GameHudController _hud = null!;
    private MockPlayerController _player = null!;

    [SetUp]
    public void SetUp()
    {
        _hud = new GameHudController();
        _player = new MockPlayerController();
        _detector = new InteractionDetector(_hud);
    }

    [Test]
    public void TrackNpc_WithinRadius_RaisesEnteredEventAndShowsTalkPrompt()
    {
        var wife = new MockNpc { NpcId = "wife", InteractionRadius = 100f };
        INpc? enteredNpc = null;
        _detector.InteractableEntered += npc => enteredNpc = npc;

        _detector.TrackNpc(wife, distance: 50f);

        Assert.Multiple(() =>
        {
            Assert.That(enteredNpc, Is.SameAs(wife));
            Assert.That(_detector.NearestInteractable, Is.SameAs(wife));
            Assert.That(_hud.IsInteractionPromptVisible, Is.True);
            Assert.That(_hud.InteractionPromptText, Is.EqualTo("Talk"));
        });
    }

    [Test]
    public void TrackNpc_OutsideRadius_DoesNotRaiseEnteredEvent()
    {
        var wife = new MockNpc { NpcId = "wife", InteractionRadius = 50f };
        var eventRaised = false;
        _detector.InteractableEntered += _ => eventRaised = true;

        _detector.TrackNpc(wife, distance: 100f);

        Assert.Multiple(() =>
        {
            Assert.That(eventRaised, Is.False);
            Assert.That(_detector.NearestInteractable, Is.Null);
            Assert.That(_hud.IsInteractionPromptVisible, Is.False);
        });
    }

    [Test]
    public void TrackNpc_MultipleNpcs_TargetsNearest()
    {
        var wife = new MockNpc { NpcId = "wife", InteractionRadius = 100f };
        var mage = new MockNpc { NpcId = "mage", InteractionRadius = 100f };

        _detector.TrackNpc(wife, distance: 80f);
        _detector.TrackNpc(mage, distance: 40f);

        Assert.That(_detector.NearestInteractable, Is.SameAs(mage));
    }

    [Test]
    public void RemoveNpc_CurrentTarget_RaisesExitedEventAndPromotesNextNearest()
    {
        var wife = new MockNpc { NpcId = "wife", InteractionRadius = 100f };
        var mage = new MockNpc { NpcId = "mage", InteractionRadius = 100f };
        INpc? exitedNpc = null;
        _detector.InteractableExited += npc => exitedNpc = npc;

        _detector.TrackNpc(wife, distance: 80f);
        _detector.TrackNpc(mage, distance: 40f);
        _detector.RemoveNpc(mage);

        Assert.Multiple(() =>
        {
            Assert.That(exitedNpc, Is.SameAs(mage));
            Assert.That(_detector.NearestInteractable, Is.SameAs(wife));
            Assert.That(_hud.IsInteractionPromptVisible, Is.True);
        });
    }

    [Test]
    public void UpdateNpcDistance_BeyondRadius_RaisesExitedEventAndHidesPrompt()
    {
        var wife = new MockNpc { NpcId = "wife", InteractionRadius = 100f };
        INpc? exitedNpc = null;
        _detector.InteractableExited += npc => exitedNpc = npc;
        _detector.TrackNpc(wife, distance: 50f);

        _detector.UpdateNpcDistance(wife, distance: 101f);

        Assert.Multiple(() =>
        {
            Assert.That(exitedNpc, Is.SameAs(wife));
            Assert.That(_detector.NearestInteractable, Is.Null);
            Assert.That(_hud.IsInteractionPromptVisible, Is.False);
        });
    }

    [Test]
    public void TryInteract_WithNearestNpc_InvokesNpcWithPlayer()
    {
        var wife = new MockNpc { NpcId = "wife", InteractionRadius = 100f };
        _detector.TrackNpc(wife, distance: 50f);

        var interacted = _detector.TryInteract(_player);

        Assert.Multiple(() =>
        {
            Assert.That(interacted, Is.True);
            Assert.That(wife.InteractionCount, Is.EqualTo(1));
            Assert.That(wife.LastPlayer, Is.SameAs(_player));
        });
    }

    [Test]
    public void TryInteract_WithoutNearestNpc_ReturnsFalse()
    {
        var interacted = _detector.TryInteract(_player);

        Assert.That(interacted, Is.False);
    }

    [Test]
    public void TrackNpc_NotInteractable_DoesNotTargetNpc()
    {
        var wife = new MockNpc
        {
            NpcId = "wife",
            InteractionRadius = 100f,
            IsInteractable = false,
        };

        _detector.TrackNpc(wife, distance: 50f);

        Assert.That(_detector.NearestInteractable, Is.Null);
    }

    [Test]
    public void TrackNpc_NegativeDistance_Throws()
    {
        var wife = new MockNpc { NpcId = "wife", InteractionRadius = 100f };

        Assert.Throws<ArgumentOutOfRangeException>(() => _detector.TrackNpc(wife, distance: -1f));
    }
}