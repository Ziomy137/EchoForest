using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class QuestServiceTest
{
    [Test]
    public void StartQuest_ChangesStateToActive()
    {
        var service = CreateService();

        service.StartQuest("q_kidnapped");

        Assert.That(service.GetQuestState("q_kidnapped"), Is.EqualTo(QuestState.Active));
    }

    [Test]
    public void StartQuest_PublishesStartedEvent()
    {
        var bus = new EventBus();
        string? publishedQuestId = null;
        bus.Subscribe<QuestStartedEvent>(gameEvent => publishedQuestId = gameEvent.QuestId);
        var service = CreateService(bus);

        service.StartQuest("q_kidnapped");

        Assert.That(publishedQuestId, Is.EqualTo("q_kidnapped"));
    }

    [Test]
    public void CompleteObjective_PublishesObjectiveCompletedEvent()
    {
        var bus = new EventBus();
        QuestObjectiveCompletedEvent? publishedEvent = null;
        bus.Subscribe<QuestObjectiveCompletedEvent>(gameEvent => publishedEvent = gameEvent);
        var service = CreateService(bus);
        service.StartQuest("q_kidnapped");

        service.CompleteObjective("q_kidnapped", "wake_up");

        Assert.That(publishedEvent, Is.EqualTo(new QuestObjectiveCompletedEvent("q_kidnapped", "wake_up")));
    }

    [Test]
    public void AllRequiredObjectivesComplete_CompletesQuest()
    {
        var service = CreateService();
        service.StartQuest("q_kidnapped");

        service.CompleteObjective("q_kidnapped", "wake_up");
        service.CompleteObjective("q_kidnapped", "find_portal");

        Assert.That(service.GetQuestState("q_kidnapped"), Is.EqualTo(QuestState.Completed));
    }

    [Test]
    public void CompleteQuest_PublishesCompletedEventAndTriggersNextQuest()
    {
        var bus = new EventBus();
        var startedQuestIds = new List<string>();
        string? completedQuestId = null;
        bus.Subscribe<QuestStartedEvent>(gameEvent => startedQuestIds.Add(gameEvent.QuestId));
        bus.Subscribe<QuestCompletedEvent>(gameEvent => completedQuestId = gameEvent.QuestId);
        var service = CreateService(bus);
        service.StartQuest("q_kidnapped");

        service.CompleteObjective("q_kidnapped", "wake_up");
        service.CompleteObjective("q_kidnapped", "find_portal");

        Assert.Multiple(() =>
        {
            Assert.That(completedQuestId, Is.EqualTo("q_kidnapped"));
            Assert.That(startedQuestIds, Is.EqualTo(["q_kidnapped", "q_seek_mage"]));
            Assert.That(service.GetQuestState("q_seek_mage"), Is.EqualTo(QuestState.Active));
        });
    }

    [Test]
    public void CompleteObjective_OptionalObjectiveDoesNotCompleteQuest()
    {
        var database = new MockQuestDatabase(new QuestData
        {
            Id = "q_optional",
            Title = "Optional objective",
            Objectives =
            [
                new QuestObjective { Id = "required", Text = "Required", Required = true },
                new QuestObjective { Id = "optional", Text = "Optional", Required = false },
            ],
        });
        var service = new QuestService(database, new EventBus());
        service.StartQuest("q_optional");

        service.CompleteObjective("q_optional", "optional");

        Assert.That(service.GetQuestState("q_optional"), Is.EqualTo(QuestState.Active));
    }

    [Test]
    public void GetActiveObjectives_ExcludesCompletedObjectives()
    {
        var service = CreateService();
        service.StartQuest("q_kidnapped");
        service.CompleteObjective("q_kidnapped", "wake_up");

        var objectives = service.GetActiveObjectives("q_kidnapped");

        Assert.That(objectives.Select(objective => objective.Id), Is.EqualTo(["find_portal"]));
    }

    [Test]
    public void GetActiveQuests_ReturnsOnlyActiveQuestData()
    {
        var service = CreateService();
        service.StartQuest("q_kidnapped");
        service.StartQuest("q_side");
        service.CompleteObjective("q_kidnapped", "wake_up");
        service.CompleteObjective("q_kidnapped", "find_portal");

        Assert.That(service.GetActiveQuests().Select(quest => quest.Id), Is.EqualTo(["q_seek_mage", "q_side"]));
    }

    [Test]
    public void ApplyQuestStates_RestoresSavedStateWithoutPublishingEvents()
    {
        var bus = new EventBus();
        var eventsPublished = 0;
        bus.Subscribe<QuestStartedEvent>(_ => eventsPublished++);
        bus.Subscribe<QuestCompletedEvent>(_ => eventsPublished++);
        var service = CreateService(bus);

        service.ApplyQuestStates(new Dictionary<string, QuestState>
        {
            ["q_kidnapped"] = QuestState.Completed,
            ["q_seek_mage"] = QuestState.Active,
        });

        Assert.Multiple(() =>
        {
            Assert.That(service.GetQuestState("q_kidnapped"), Is.EqualTo(QuestState.Completed));
            Assert.That(service.GetQuestState("q_seek_mage"), Is.EqualTo(QuestState.Active));
            Assert.That(eventsPublished, Is.Zero);
        });
    }

    [Test]
    public void CompleteObjective_WhenQuestIsNotActive_Throws()
    {
        var service = CreateService();

        Assert.Throws<InvalidOperationException>(() => service.CompleteObjective("q_kidnapped", "wake_up"));
    }

    private static QuestService CreateService(IEventBus? eventBus = null)
    {
        return new QuestService(new MockQuestDatabase(
            new QuestData
            {
                Id = "q_kidnapped",
                Title = "Kidnapped",
                Objectives =
                [
                    new QuestObjective { Id = "wake_up", Text = "Wake up", Required = true },
                    new QuestObjective { Id = "find_portal", Text = "Find the portal", Required = true },
                ],
                TriggersQuestId = "q_seek_mage",
            },
            new QuestData
            {
                Id = "q_seek_mage",
                Title = "Seek the Mage",
                Objectives = [new QuestObjective { Id = "talk_to_mage", Text = "Talk to the mage", Required = true }],
            },
            new QuestData
            {
                Id = "q_side",
                Title = "Side quest",
                Objectives = [new QuestObjective { Id = "complete", Text = "Complete", Required = true }],
            }), eventBus ?? new EventBus());
    }
}