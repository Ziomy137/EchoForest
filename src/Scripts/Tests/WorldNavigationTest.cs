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
    private static readonly string[] AreaScenes =
    [
        MainMenuConfig.ContinueScenePath,
        MainMenuConfig.FarmScenePath,
        MainMenuConfig.ForestPathScenePath,
        MainMenuConfig.CityScenePath,
        MainMenuConfig.MagesTowerScenePath,
        MainMenuConfig.PortalChamberScenePath,
    ];

    private static readonly NavigationLeg[] AllTransitionZones =
    [
        .. ToNavigationLegs(CottageSceneConfig.SceneResPath, CottageSceneConfig.Transitions),
        .. ToNavigationLegs(FarmSceneConfig.SceneResPath, FarmSceneConfig.Transitions),
        .. ToNavigationLegs(ForestPathSceneConfig.SceneResPath, ForestPathSceneConfig.Transitions),
        .. ToNavigationLegs(CitySceneConfig.SceneResPath, CitySceneConfig.Transitions),
        .. MagesTowerSceneConfig.Transitions.Select(endpoint =>
            new NavigationLeg(MagesTowerSceneConfig.SceneResPath, endpoint.TargetArea, endpoint.SpawnPointId, $"Transitions/{endpoint.Type}")),
        .. PortalChamberSceneConfig.Transitions.Select(endpoint =>
            new NavigationLeg(PortalChamberSceneConfig.SceneResPath, endpoint.TargetArea, endpoint.SpawnPointId, $"Transitions/{endpoint.Type}")),
    ];

    private static readonly NavigationLeg[] ForwardRoute = GetRoute(
        (AreaScenes[0], AreaScenes[1]),
        (AreaScenes[1], AreaScenes[2]),
        (AreaScenes[2], AreaScenes[3]),
        (AreaScenes[3], AreaScenes[4]),
        (AreaScenes[4], AreaScenes[5]));

    private static readonly NavigationLeg[] ReverseRoute = GetRoute(
        (AreaScenes[5], AreaScenes[4]),
        (AreaScenes[4], AreaScenes[3]),
        (AreaScenes[3], AreaScenes[2]),
        (AreaScenes[2], AreaScenes[1]),
        (AreaScenes[1], AreaScenes[0]));

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
        Assert.That(AllTransitionZones, Has.Length.EqualTo(13));
        Assert.That(AllTransitionZones.Select(leg => (leg.FromArea, leg.TransitionNodePath)), Is.Unique);

        var repositoryRoot = FindRepositoryRoot();
        foreach (var route in AllTransitionZones)
        {
            var sourceScene = File.ReadAllText(ToFilePath(repositoryRoot, route.FromArea));
            Assert.That(ContainsAreaTransitionNode(sourceScene, route.TransitionNodePath), Is.True,
                $"{route.FromArea} has no transition zone '{route.TransitionNodePath}'");
            AssertTransition(route);
        }
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

    private static NavigationLeg[] ToNavigationLegs(string fromArea, IEnumerable<AreaTransitionDefinition> transitions) =>
        transitions.Select(endpoint =>
            new NavigationLeg(fromArea, endpoint.TargetArea, endpoint.SpawnPointId, endpoint.NodePath)).ToArray();

    private static NavigationLeg[] GetRoute(params (string FromArea, string TargetArea)[] edges) =>
        edges.Select(edge => AllTransitionZones.Single(leg =>
            leg.FromArea == edge.FromArea && leg.TargetArea == edge.TargetArea)).ToArray();

    private static bool ContainsSpawnPoint(string sceneContents, string spawnPointId) =>
        Regex.IsMatch(sceneContents, $"SpawnPointId\\s*=\\s*\\\"{Regex.Escape(spawnPointId)}\\\"", RegexOptions.CultureInvariant);

    private static bool ContainsAreaTransitionNode(string sceneContents, string nodePath)
    {
        var separator = nodePath.LastIndexOf('/');
        var nodeName = nodePath[(separator + 1)..];
        var parentPath = separator < 0 ? "." : nodePath[..separator];
        var node = Regex.Match(sceneContents,
            $"\\[node name=\"{Regex.Escape(nodeName)}\" type=\"Area2D\" parent=\"{Regex.Escape(parentPath)}\"[^\\]]*\\](?<body>.*?)(?=\\r?\\n\\[node |\\z)",
            RegexOptions.Singleline | RegexOptions.CultureInvariant);
        var script = Regex.Match(sceneContents,
            "\\[ext_resource(?=[^\\]]*type=\"Script\")(?=[^\\]]*path=\"res://src/Scripts/Core/AreaTransitionNode.cs\")[^\\]]*id=\"([^\"]+)\"[^\\]]*\\]",
            RegexOptions.CultureInvariant);
        return node.Success && script.Success && Regex.IsMatch(node.Groups["body"].Value,
            $"^script\\s*=\\s*ExtResource\\(\"{Regex.Escape(script.Groups[1].Value)}\"\\)$",
            RegexOptions.Multiline | RegexOptions.CultureInvariant);
    }

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

    private readonly record struct NavigationLeg(string FromArea, string TargetArea, string SpawnPointId, string TransitionNodePath);

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