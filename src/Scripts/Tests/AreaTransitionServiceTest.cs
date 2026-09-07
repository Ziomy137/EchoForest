using System.Collections.Generic;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class AreaTransitionServiceTest
{
    [SetUp]
    public void SetUp() => GameSession.Clear();

    [TearDown]
    public void TearDown() => GameSession.Clear();

    [Test]
    public void AreaTransitionService_ImplementsInterface()
    {
        var service = CreateService(out _, out _);

        Assert.That(service, Is.InstanceOf<IAreaTransitionService>());
    }

    [Test]
    public void TriggerTransition_PublishesEventWithSourceAndTargetAreas()
    {
        var service = CreateService(out var eventBus, out _);
        AreaTransitionEvent? publishedEvent = null;
        eventBus.Subscribe<AreaTransitionEvent>(gameEvent => publishedEvent = gameEvent);

        service.TriggerTransition("cottage", "res://src/Scenes/Scene_ForestPath.tscn", "north_entrance", new SaveData());

        Assert.That(publishedEvent, Is.EqualTo(new AreaTransitionEvent("cottage", "res://src/Scenes/Scene_ForestPath.tscn")));
    }

    [Test]
    public void TriggerTransition_SavesCurrentStateBeforeRequestingSceneLoad()
    {
        var service = CreateService(out _, out var saveService);
        var saveData = new SaveData
        {
            CurrentArea = "res://src/Scenes/Scene_ForestPath.tscn",
            PlayerX = 42f,
            PlayerY = 84f,
            QuestStates = new Dictionary<string, QuestState>
            {
                ["q_kidnapped"] = QuestState.Active,
            },
        };

        service.TriggerTransition("cottage", saveData.CurrentArea, "north_entrance", saveData);

        Assert.Multiple(() =>
        {
            Assert.That(saveService.SaveWasCalled, Is.True);
            Assert.That(saveService.LastSavedData, Is.SameAs(saveData));
            Assert.That(saveService.LastSavedSlot, Is.EqualTo(1));
        });
    }

    [Test]
    public void TriggerTransition_SavesBeforeRequestingSceneLoad()
    {
        var operations = new List<string>();
        var service = new AreaTransitionService(
            new EventBus(),
            new RecordingSceneLoader(operations),
            new RecordingSaveDataService(operations));

        service.TriggerTransition("cottage", "res://src/Scenes/Scene_Farm.tscn", "west_entrance", new SaveData());

        Assert.That(operations, Is.EqualTo(["save", "load"]));
    }

    [Test]
    public void TriggerTransition_RequestsTargetSceneAndQueuesNamedSpawnPoint()
    {
        var service = CreateService(out _, out _);
        const string targetArea = "res://src/Scenes/Scene_Farm.tscn";
        var sceneLoader = new MockSceneLoader();
        service = new AreaTransitionService(new EventBus(), sceneLoader, new MockSaveDataService());

        service.TriggerTransition("cottage", targetArea, "west_entrance", new SaveData());

        Assert.Multiple(() =>
        {
            Assert.That(sceneLoader.LastRequestedPath, Is.EqualTo(targetArea));
            Assert.That(GameSession.ConsumeTransitionSpawnPointId(), Is.EqualTo("west_entrance"));
        });
    }

    [Test]
    public void TriggerTransition_PreservesQuestStatesForTargetArea()
    {
        var questStates = new Dictionary<string, QuestState>
        {
            ["q_kidnapped"] = QuestState.Active,
        };
        var service = CreateService(out _, out _);

        service.TriggerTransition("cottage", MainMenuConfig.FarmScenePath, "west_entrance", new SaveData
        {
            QuestStates = questStates,
        });

        Assert.That(GameSession.QuestStates, Is.EqualTo(questStates));
    }

    private static AreaTransitionService CreateService(out EventBus eventBus, out MockSaveDataService saveService)
    {
        eventBus = new EventBus();
        saveService = new MockSaveDataService();
        return new AreaTransitionService(eventBus, new MockSceneLoader(), saveService);
    }

    private sealed class RecordingSceneLoader(List<string> operations) : ISceneLoader
    {
        public void LoadScene(string scenePath) => operations.Add("load");
        public System.Threading.Tasks.Task LoadSceneAsync(string scenePath)
        {
            operations.Add("load");
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public Godot.Node? GetCurrentScene() => null;
    }

    private sealed class RecordingSaveDataService(List<string> operations) : ISaveDataService
    {
        public bool HasSaveFile() => false;
        public void Save(SaveData data, int slot) => operations.Add("save");
        public SaveData Load(int slot) => new();
        public void Delete(int slot) { }
        public List<SaveSlotInfo> GetSaveSlots() => [];
        public bool HasSave(int slot) => false;
    }
}