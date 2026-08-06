using System.Collections.Generic;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class QuestJournalControllerTest
{
    [Test]
    public void OnQuestStarted_AddsQuestToActiveList()
    {
        var bus = new EventBus();
        using var controller = new QuestJournalController(bus);

        bus.Publish(new QuestStartedEvent("q_kidnapped"));

        Assert.That(controller.ActiveQuestIds, Is.EqualTo(["q_kidnapped"]));
    }

    [Test]
    public void OnQuestCompleted_MovesQuestToCompletedList()
    {
        var bus = new EventBus();
        using var controller = new QuestJournalController(bus);
        bus.Publish(new QuestStartedEvent("q_kidnapped"));

        bus.Publish(new QuestCompletedEvent("q_kidnapped"));

        Assert.Multiple(() =>
        {
            Assert.That(controller.ActiveQuestIds, Is.Empty);
            Assert.That(controller.CompletedQuestIds, Is.EqualTo(["q_kidnapped"]));
        });
    }

    [Test]
    public void OnObjectiveCompleted_MarksObjectiveCompleteAndRaisesChangeEvent()
    {
        var bus = new EventBus();
        using var controller = new QuestJournalController(bus);
        var changes = 0;
        controller.Changed += () => changes++;

        bus.Publish(new QuestStartedEvent("q_kidnapped"));
        bus.Publish(new QuestObjectiveCompletedEvent("q_kidnapped", "wake_up"));

        Assert.Multiple(() =>
        {
            Assert.That(controller.IsObjectiveCompleted("q_kidnapped", "wake_up"), Is.True);
            Assert.That(changes, Is.EqualTo(2));
        });
    }

    [Test]
    public void Synchronize_UsesLoadedQuestStates()
    {
        var bus = new EventBus();
        using var controller = new QuestJournalController(bus);

        controller.Synchronize(new Dictionary<string, QuestState>
        {
            ["q_kidnapped"] = QuestState.Completed,
            ["q_seek_mage"] = QuestState.Active,
        });

        Assert.Multiple(() =>
        {
            Assert.That(controller.ActiveQuestIds, Is.EqualTo(["q_seek_mage"]));
            Assert.That(controller.CompletedQuestIds, Is.EqualTo(["q_kidnapped"]));
        });
    }

    [Test]
    public void Dispose_UnsubscribesFromEventBus()
    {
        var bus = new EventBus();
        var controller = new QuestJournalController(bus);
        controller.Dispose();

        bus.Publish(new QuestStartedEvent("q_kidnapped"));

        Assert.That(controller.ActiveQuestIds, Is.Empty);
    }
}