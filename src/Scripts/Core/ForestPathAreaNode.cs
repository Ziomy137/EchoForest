using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>Godot root node that builds and composes the Forest Path area at runtime.</summary>
[ExcludeFromCodeCoverage(Justification = "Godot Node2D wrapper - requires scene tree")]
public partial class ForestPathAreaNode : Node2D, IAreaSceneContext
{
    public IEventBus EventBus { get; private set; } = null!;
    public IInputHandler InputHandler { get; private set; } = null!;
    public IAreaTransitionService AreaTransitionService { get; private set; } = null!;
    public IQuestDatabase QuestDatabase { get; private set; } = null!;
    public IQuestService QuestService { get; private set; } = null!;
    private GameHudNode _hud = null!;

    public override void _EnterTree()
    {
        EventBus = new EventBus();
        InputHandler = new global::EchoForest.InputHandler();
        AreaTransitionService = new AreaTransitionService(EventBus, new SceneLoader(), new SaveService(new GodotFileSystem()));
        QuestDatabase = new QuestDatabase(new GodotFileSystem());
        QuestDatabase.GetAllQuests();
        QuestService = new QuestService(QuestDatabase, EventBus);
        QuestService.ApplyQuestStates(GameSession.QuestStates);
    }

    public override void _Ready()
    {
        PopulateTiles();
        SpawnProps();
        AddStreamColliders();
        SetupBoundary();
        SpawnPlayer();
        SetupCamera();
        ConfigureAreaTransitions();
        WireQuestHud();
    }

    public override void _ExitTree()
    {
        EventBus.Unsubscribe<QuestStartedEvent>(OnQuestStarted);
        EventBus.Unsubscribe<QuestObjectiveCompletedEvent>(OnQuestObjectiveCompleted);
        EventBus.Unsubscribe<QuestCompletedEvent>(OnQuestCompleted);
    }

    private void WireQuestHud()
    {
        _hud = GetNode<GameHudNode>("HUD");
        EventBus.Subscribe<QuestStartedEvent>(OnQuestStarted);
        EventBus.Subscribe<QuestObjectiveCompletedEvent>(OnQuestObjectiveCompleted);
        EventBus.Subscribe<QuestCompletedEvent>(OnQuestCompleted);

        var activeQuests = QuestService.GetActiveQuests();
        if (activeQuests.Count > 0)
            ShowCurrentObjective(activeQuests[0].Id);
    }

    private void OnQuestStarted(QuestStartedEvent gameEvent) => ShowCurrentObjective(gameEvent.QuestId);

    private void OnQuestObjectiveCompleted(QuestObjectiveCompletedEvent gameEvent) => ShowCurrentObjective(gameEvent.QuestId);

    private void OnQuestCompleted(QuestCompletedEvent gameEvent)
    {
        var quest = QuestDatabase.GetQuest(gameEvent.QuestId);
        _hud.SetQuestObjective(quest.Title, "Quest completed", quest.Objectives.Count, quest.Objectives.Count);
    }

    private void ShowCurrentObjective(string questId)
    {
        var quest = QuestDatabase.GetQuest(questId);
        var activeObjectives = QuestService.GetActiveObjectives(questId);
        if (activeObjectives.Count == 0)
            return;

        var objective = activeObjectives[0];
        var completedCount = quest.Objectives.Count - activeObjectives.Count;
        _hud.SetQuestObjective(quest.Title, objective.Text, completedCount, quest.Objectives.Count);
    }

    private void PopulateTiles()
    {
        var tileMap = GetNode<TileMapLayer>(ForestPathSceneConfig.TileMapLayerName);
        for (var row = 0; row < ForestPathSceneConfig.GridRows; row++)
        {
            for (var col = 0; col < ForestPathSceneConfig.GridColumns; col++)
            {
                var tileFileName = ForestPathSceneConfig.GetTileFileName(col, row);
                tileMap.SetCell(new Vector2I(col, row), ForestPathSceneConfig.GetSourceId(tileFileName), Vector2I.Zero);
            }
        }
    }

    private void SpawnProps()
    {
        var tileMap = GetNode<TileMapLayer>(ForestPathSceneConfig.TileMapLayerName);
        foreach (var placement in ForestPathSceneConfig.Props)
        {
            var config = PropRegistry.GetByFileName(placement.FileName);
            if (config is null)
                continue;

            var worldPosition = tileMap.ToGlobal(tileMap.MapToLocal(new Vector2I(placement.Col, placement.Row)));
            var sorter = new IsometricYSorterNode { GlobalPosition = worldPosition };
            var sprite = new Sprite2D
            {
                Texture = GD.Load<Texture2D>(config.ResourcePath),
                Centered = true,
                Position = new Vector2(0f, -config.Height / 2f),
            };
            sorter.AddChild(sprite);
            AddChild(sorter);

            if (placement.IsBlocking)
            {
                var colliderWidth = placement.FileName == PropRegistry.DenseTree.FileName
                    ? ForestPathSceneConfig.DenseTreeColliderWidth
                    : config.Width * 0.32f;
                var colliderHeight = placement.FileName == PropRegistry.DenseTree.FileName
                    ? ForestPathSceneConfig.DenseTreeColliderHeight
                    : config.Height * 0.14f;
                AddCollider(worldPosition, colliderWidth, colliderHeight);
            }
        }
    }

    private void AddStreamColliders()
    {
        var tileMap = GetNode<TileMapLayer>(ForestPathSceneConfig.TileMapLayerName);
        for (var row = ForestPathSceneConfig.StreamZone.Row; row < ForestPathSceneConfig.StreamZone.Row + ForestPathSceneConfig.StreamZone.Height; row++)
        {
            for (var col = 0; col < ForestPathSceneConfig.GridColumns; col++)
            {
                if (ForestPathSceneConfig.GetTileFileName(col, row) == TileRegistry.Mud.FileName)
                    continue;

                var worldPosition = tileMap.ToGlobal(tileMap.MapToLocal(new Vector2I(col, row)));
                AddCollider(worldPosition, 40f, 20f);
            }
        }
    }

    private void AddCollider(Vector2 worldPosition, float width, float height)
    {
        var body = new StaticBody2D
        {
            CollisionLayer = 1u << (PhysicsLayers.World - 1),
            CollisionMask = 1u << (PhysicsLayers.World - 1),
            GlobalPosition = worldPosition,
        };
        body.AddChild(new CollisionShape2D
        {
            Shape = new RectangleShape2D { Size = new Vector2(width, height) },
        });
        AddChild(body);
    }

    private void SetupBoundary()
    {
        var boundary = GetNode<StaticBody2D>(ForestPathSceneConfig.BoundaryNodeName);
        var width = ForestPathSceneConfig.WorldBoundaryRight - ForestPathSceneConfig.WorldBoundaryLeft;
        var height = ForestPathSceneConfig.WorldBoundaryBottom - ForestPathSceneConfig.WorldBoundaryTop;
        AddWallSegment(boundary, new Vector2(-240f, ForestPathSceneConfig.WorldBoundaryTop - 32f), width + 128f, 64f);
        AddWallSegment(boundary, new Vector2(-240f, ForestPathSceneConfig.WorldBoundaryBottom + 32f), width + 128f, 64f);
        AddWallSegment(boundary, new Vector2(ForestPathSceneConfig.WorldBoundaryLeft - 32f, 672f), 64f, height + 128f);
        AddWallSegment(boundary, new Vector2(ForestPathSceneConfig.WorldBoundaryRight + 32f, 672f), 64f, height + 128f);
    }

    private static void AddWallSegment(StaticBody2D parent, Vector2 center, float width, float height)
    {
        parent.AddChild(new CollisionShape2D
        {
            Shape = new RectangleShape2D { Size = new Vector2(width, height) },
            Position = center,
        });
    }

    private void SpawnPlayer()
    {
        var player = GetNode<Node2D>("Player");
        var spawnPointId = GameSession.ConsumeTransitionSpawnPointId();
        if (spawnPointId is not null && TryGetSpawnPoint(spawnPointId, out var transitionSpawnPoint))
            player.GlobalPosition = transitionSpawnPoint.GlobalPosition;
        else if (GameSession.HasPlayerPosition)
            player.GlobalPosition = new Vector2(GameSession.LastPlayerX, GameSession.LastPlayerY);
        else
            player.GlobalPosition = GetNode<Marker2D>(ForestPathSceneConfig.PlayerSpawnName).GlobalPosition;
    }

    private bool TryGetSpawnPoint(string spawnPointId, out SpawnPointNode spawnPoint)
    {
        foreach (var child in GetChildren())
        {
            if (child is SpawnPointNode candidate && candidate.SpawnPointId == spawnPointId)
            {
                spawnPoint = candidate;
                return true;
            }
        }

        spawnPoint = null!;
        return false;
    }

    private void SetupCamera()
    {
        var camera = GetNode<IsometricCameraNode>(ForestPathSceneConfig.CameraNodeName);
        camera.FollowTarget = GetNode<Node2D>("Player");
        camera.SetBounds(new Rect2(
            ForestPathSceneConfig.WorldBoundaryLeft,
            ForestPathSceneConfig.WorldBoundaryTop,
            ForestPathSceneConfig.WorldBoundaryRight - ForestPathSceneConfig.WorldBoundaryLeft,
            ForestPathSceneConfig.WorldBoundaryBottom - ForestPathSceneConfig.WorldBoundaryTop));
        camera.SnapToPixels = true;
        camera.SnapToTarget();
    }

    private void ConfigureAreaTransitions()
    {
        GetNode<AreaTransitionNode>("ToFarmTransition").Configure(
            MainMenuConfig.FarmScenePath,
            "north_entrance",
            TransitionType.FadeToBlack,
            AreaTransitionService,
            InputHandler,
            CreateTransitionSaveData);
        GetNode<AreaTransitionNode>("ToCityTransition").Configure(
            MainMenuConfig.CityScenePath,
            "south_entrance",
            TransitionType.FadeToBlack,
            AreaTransitionService,
            InputHandler,
            CreateTransitionSaveData);
    }

    private SaveData CreateTransitionSaveData(PlayerControllerNode player) => new()
    {
        CurrentArea = SceneFilePath,
        PlayerX = player.GlobalPosition.X,
        PlayerY = player.GlobalPosition.Y,
        QuestStates = new Dictionary<string, QuestState>(QuestService.GetQuestStates()),
    };
}