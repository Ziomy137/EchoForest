using System;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class NpcControllerTest
{
    [Test]
    public void Constructor_StoresNpcData()
    {
        var controller = new NpcController("wife", "Wife", 80f);

        Assert.Multiple(() =>
        {
            Assert.That(controller.NpcId, Is.EqualTo("wife"));
            Assert.That(controller.DisplayName, Is.EqualTo("Wife"));
            Assert.That(controller.InteractionRadius, Is.EqualTo(80f));
            Assert.That(controller.IsInteractable, Is.True);
        });
    }

    [Test]
    public void Interact_WhenInteractable_InvokesConfiguredAction()
    {
        var player = new MockPlayerController();
        IPlayerController? receivedPlayer = null;
        var controller = new NpcController("wife", "Wife", 80f, npcPlayer => receivedPlayer = npcPlayer);

        controller.Interact(player);

        Assert.That(receivedPlayer, Is.SameAs(player));
    }

    [Test]
    public void Interact_WhenNotInteractable_DoesNotInvokeConfiguredAction()
    {
        var invoked = false;
        var controller = new NpcController("wife", "Wife", 80f, _ => invoked = true)
        {
            IsInteractable = false,
        };

        controller.Interact(new MockPlayerController());

        Assert.That(invoked, Is.False);
    }

    [Test]
    public void Constructor_NonPositiveRadius_Throws()
    {
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new NpcController("wife", "Wife", 0f));
            Assert.Throws<ArgumentOutOfRangeException>(() => new NpcController("wife", "Wife", -1f));
        });
    }
}