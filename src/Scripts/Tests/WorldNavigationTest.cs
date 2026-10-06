using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class WorldNavigationTest
{
    private static readonly NavigationLeg[] ForwardRoute =
    [
        new(MainMenuConfig.ContinueScenePath, MainMenuConfig.FarmScenePath, "west_entrance"),
        new(MainMenuConfig.FarmScenePath, MainMenuConfig.ForestPathScenePath, "south_entrance"),
        new(MainMenuConfig.ForestPathScenePath, MainMenuConfig.CityScenePath, "south_entrance"),
        new(MainMenuConfig.CityScenePath, MainMenuConfig.MagesTowerScenePath, "city_entrance"),
        new(MainMenuConfig.MagesTowerScenePath, MainMenuConfig.PortalChamberScenePath, PortalChamberSceneConfig.TowerEntranceSpawnPointId),
    ];

    private static readonly NavigationLeg[] ReverseRoute =
    [
        new(MainMenuConfig.PortalChamberScenePath, MainMenuConfig.MagesTowerScenePath, MagesTowerSceneConfig.PortalExitSpawnPointId),
        new(MainMenuConfig.MagesTowerScenePath, MainMenuConfig.CityScenePath, "tower_exit"),
        new(MainMenuConfig.CityScenePath, MainMenuConfig.ForestPathScenePath, "north_entrance"),
        new(MainMenuConfig.ForestPathScenePath, MainMenuConfig.FarmScenePath, "north_entrance"),
        new(MainMenuConfig.FarmScenePath, MainMenuConfig.ContinueScenePath, "farm_entrance"),
    ];

    private static readonly NavigationLeg[] AllTransitionZones =
    [
        .. ForwardRoute,
        new(MainMenuConfig.ContinueScenePath, MainMenuConfig.ForestPathScenePath, "north_entrance"),
        new(MainMenuConfig.FarmScenePath, MainMenuConfig.ContinueScenePath, "farm_entrance"),
        new(MainMenuConfig.ForestPathScenePath, MainMenuConfig.FarmScenePath, "north_entrance"),
        new(MainMenuConfig.CityScenePath, MainMenuConfig.ForestPathScenePath, "north_entrance"),
        new(MainMenuConfig.MagesTowerScenePath, MainMenuConfig.CityScenePath, "tower_exit"),
        new(MainMenuConfig.MagesTowerScenePath, MainMenuConfig.MagesTowerScenePath, "interior_door"),
        new(MainMenuConfig.MagesTowerScenePath, MainMenuConfig.MagesTowerScenePath, "exterior_door"),
        new(MainMenuConfig.PortalChamberScenePath, MainMenuConfig.MagesTowerScenePath, MagesTowerSceneConfig.PortalExitSpawnPointId),
    ];

    private static readonly string[] AreaScenes =
    [
        MainMenuConfig.ContinueScenePath,
        MainMenuConfig.FarmScenePath,
        MainMenuConfig.ForestPathScenePath,
        MainMenuConfig.CityScenePath,
        MainMenuConfig.MagesTowerScenePath,
        MainMenuConfig.PortalChamberScenePath,
    ];

    [SetUp]
    public void SetUp() => GameSession.Clear();

    [TearDown]
    public void TearDown() => GameSession.Clear();

    [Test]
    public void Navigation_ForwardTraversal_ReachesPortalChamber()
    {
        Assert.That(ForwardRoute.Select(leg => leg.FromArea).Append(ForwardRoute[^1].TargetArea), Is.EqualTo(AreaScenes));
        Traverse(ForwardRoute);
    }

    [Test]
    public void Navigation_ReverseTraversal_ReturnsToCottage()
    {
        Assert.That(ReverseRoute.Select(leg => leg.FromArea).Append(ReverseRoute[^1].TargetArea), Is.EqualTo(AreaScenes.Reverse()));
        Traverse(ReverseRoute);
    }

    [Test]
    public void Navigation_AllTransitionZones_PublishEventsSaveAndQueueSpawn()
    {
        foreach (var route in AllTransitionZones)
            AssertTransition(route);
    }

    [Test]
    public void Navigation_AllAreas_HaveDefaultSpawnPoints()
    {
        var repositoryRoot = FindRepositoryRoot();
        foreach (var areaScene in AreaScenes)
        {
            var sceneContents = File.ReadAllText(ToFilePath(repositoryRoot, areaScene));
            Assert.That(ContainsSpawnPoint(sceneContents, "default"), Is.True, $"Missing default spawn point in {areaScene}");
        }
    }

    [Test]
    public void Navigation_EveryTransitionTarget_ContainsRequestedSpawnPoint()
    {
        var repositoryRoot = FindRepositoryRoot();
        foreach (var route in AllTransitionZones)
        {
            var sceneContents = File.ReadAllText(ToFilePath(repositoryRoot, route.TargetArea));
            Assert.That(ContainsSpawnPoint(sceneContents, route.SpawnPointId), Is.True,
                $"{route.TargetArea} has no spawn point '{route.SpawnPointId}' requested from {route.FromArea}");
        }
    }

    private static void Traverse(IEnumerable<NavigationLeg> route)
    {
        var index = 0;
        foreach (var leg in route)
            AssertTransition(leg, index++);
    }

    private static void AssertTransition(NavigationLeg leg, int sequence = 0)
    {
        var eventBus = new EventBus();
        var publishedEvents = new List<AreaTransitionEvent>();
        eventBus.Subscribe<AreaTransitionEvent>(publishedEvents.Add);

        var operations = new List<string>();
        var saveService = new RecordingSaveDataService(operations);
        var sceneLoader = new RecordingSceneLoader(operations);
        var transitionService = new AreaTransitionService(eventBus, sceneLoader, saveService);
        var questStates = new Dictionary<string, QuestState>
        {
            ["q_kidnapped"] = QuestState.Active,
            ["q_seek_mage"] = QuestState.Completed,
        };
        var saveData = new SaveData
        {
            CurrentArea = leg.FromArea,
            PlayerX = sequence * 64f + 16f,
            PlayerY = sequence * 32f + 8f,
            QuestStates = questStates,
        };

        transitionService.TriggerTransition(leg.FromArea, leg.TargetArea, leg.SpawnPointId, saveData);

        Assert.Multiple(() =>
        {
            Assert.That(publishedEvents, Is.EqualTo(new[] { new AreaTransitionEvent(leg.FromArea, leg.TargetArea) }));
            Assert.That(operations, Is.EqualTo(new[] { "save", "load" }));
            Assert.That(saveService.SavedData, Is.SameAs(saveData));
            Assert.That(saveService.SavedData.CurrentArea, Is.EqualTo(leg.FromArea));
            Assert.That(saveService.SavedData.PlayerX, Is.EqualTo(saveData.PlayerX));
            Assert.That(saveService.SavedData.PlayerY, Is.EqualTo(saveData.PlayerY));
            Assert.That(saveService.SavedData.QuestStates, Is.EqualTo(questStates));
            Assert.That(saveService.SavedSlot, Is.EqualTo(1));
            Assert.That(sceneLoader.LoadedScenePath, Is.EqualTo(leg.TargetArea));
            Assert.That(GameSession.ConsumeTransitionSpawnPointId(), Is.EqualTo(leg.SpawnPointId));
            Assert.That(GameSession.QuestStates, Is.EqualTo(questStates));
        });
    }

    private static bool ContainsSpawnPoint(string sceneContents, string spawnPointId) =>
        Regex.IsMatch(sceneContents,
            $"SpawnPointId\\s*=\\s*\\\"{Regex.Escape(spawnPointId)}\\\"",
            RegexOptions.CultureInvariant);

    private static string ToFilePath(string repositoryRoot, string scenePath) =>
        Path.Combine(repositoryRoot, scenePath["res://".Length..].Replace('/', Path.DirectorySeparatorChar));

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "EchoForest.sln")))
                return directory.FullName;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the repository root from the NUnit test directory.");
    }

    private readonly record struct NavigationLeg(string FromArea, string TargetArea, string SpawnPointId);

    private sealed class RecordingSceneLoader(List<string> operations) : ISceneLoader
    {
        public string LoadedScenePath { get; private set; } = string.Empty;

        public void LoadScene(string scenePath)
        {
            LoadedScenePath = scenePath;
            operations.Add("load");
        }

        public Task LoadSceneAsync(string scenePath)
        {
            LoadScene(scenePath);
            return Task.CompletedTask;
        }

        public Godot.Node? GetCurrentScene() => null;
    }

    private sealed class RecordingSaveDataService(List<string> operations) : ISaveDataService
    {
        public SaveData SavedData { get; private set; } = new();
        public int SavedSlot { get; private set; }

        public bool HasSaveFile() => false;
        public void Save(SaveData data, int slot)
        {
            SavedData = data;
            SavedSlot = slot;
            operations.Add("save");
        }

        public SaveData Load(int slot) => new();
        public void Delete(int slot) { }
        public List<SaveSlotInfo> GetSaveSlots() => [];
        public bool HasSave(int slot) => false;
    }
}