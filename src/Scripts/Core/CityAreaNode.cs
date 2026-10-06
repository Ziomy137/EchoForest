using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>Godot root that composes the City exterior and its market sub-scene.</summary>
[ExcludeFromCodeCoverage(Justification = "Godot Node2D wrapper - requires scene tree")]
public partial class CityAreaNode : Node2D, IAreaSceneContext
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
        ConfigureMarkers();
        PopulateTiles();
        SetupBoundaries();
        SetupBuildingCollisions();
        SpawnProps();
        CreateNpcAnchors();
        SpawnPlayer();
        SetupCamera();
        ConfigureTransitions();
        WireQuestHud();
    }

    public override void _ExitTree()
    {
        EventBus.Unsubscribe<QuestStartedEvent>(OnQuestStarted);
        EventBus.Unsubscribe<QuestObjectiveCompletedEvent>(OnQuestObjectiveCompleted);
        EventBus.Unsubscribe<QuestCompletedEvent>(OnQuestCompleted);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (InputHandler.IsBlocked || !@event.IsActionPressed(InputActionNames.Pause))
            return;

        GetViewport().SetInputAsHandled();
        var player = GetNode<Node>("Player");
        player.ProcessMode = ProcessModeEnum.Disabled;
        var pauseMenu = GD.Load<PackedScene>(MainMenuConfig.PauseMenuScenePath).Instantiate<PauseMenuNode>();
        pauseMenu.TreeExiting += () =>
        {
            if (GodotObject.IsInstanceValid(player))
                player.ProcessMode = ProcessModeEnum.Inherit;
        };
        GetTree().Root.AddChild(pauseMenu);
    }

    private void ConfigureMarkers()
    {
        GetNode<Marker2D>(CitySceneConfig.PlayerSpawnName).Position = ToVector(CitySceneConfig.GridToWorld(CitySceneConfig.DefaultSpawnPosition));
        GetNode<Marker2D>("SouthEntranceSpawnPoint").Position = ToVector(CitySceneConfig.GridToWorld(CitySceneConfig.SouthEntranceSpawnGridPosition));
        GetNode<Marker2D>("TowerExitSpawnPoint").Position = ToVector(CitySceneConfig.GridToWorld(CitySceneConfig.TowerExitSpawnGridPosition));
    }

    private void PopulateTiles()
    {
        var tileMap = GetNode<TileMapLayer>(CitySceneConfig.TileMapLayerName);
        for (var row = 0; row < CitySceneConfig.GridRows; row++)
        {
            for (var col = 0; col < CitySceneConfig.GridColumns; col++)
            {
                var tileFileName = CitySceneConfig.GetTileFileName(col, row);
                tileMap.SetCell(new Vector2I(col, row), CitySceneConfig.GetSourceId(tileFileName), Vector2I.Zero);
            }
        }
    }

    private void SetupBoundaries()
    {
        var boundary = GetNode<StaticBody2D>(CitySceneConfig.BoundaryNodeName);
        var width = CitySceneConfig.WorldBoundaryRight - CitySceneConfig.WorldBoundaryLeft;
        var height = CitySceneConfig.WorldBoundaryBottom - CitySceneConfig.WorldBoundaryTop;
        AddBoundarySegment(boundary, new Vector2(0f, CitySceneConfig.WorldBoundaryTop - 32f), width + 128f, 64f);
        AddBoundarySegment(boundary, new Vector2(0f, CitySceneConfig.WorldBoundaryBottom + 32f), width + 128f, 64f);
        AddBoundarySegment(boundary, new Vector2(CitySceneConfig.WorldBoundaryLeft - 32f, 960f), 64f, height + 128f);
        AddBoundarySegment(boundary, new Vector2(CitySceneConfig.WorldBoundaryRight + 32f, 960f), 64f, height + 128f);
    }

    private static void AddBoundarySegment(StaticBody2D parent, Vector2 center, float width, float height)
    {
        parent.AddChild(new CollisionShape2D
        {
            Shape = new RectangleShape2D { Size = new Vector2(width, height) },
            Position = center,
        });
    }

    private void SetupBuildingCollisions()
    {
        foreach (var footprint in CitySceneConfig.BuildingFootprints)
        {
            var collision = new StaticBody2D
            {
                CollisionLayer = 1u << (PhysicsLayers.World - 1),
                CollisionMask = 1u << (PhysicsLayers.World - 1),
            };
            collision.AddChild(new CollisionPolygon2D
            {
                Polygon =
                [
                    ToVector(CitySceneConfig.GridToWorld(footprint.Col - 0.5f, footprint.Row - 0.5f)),
                    ToVector(CitySceneConfig.GridToWorld(footprint.Col + footprint.Width - 0.5f, footprint.Row - 0.5f)),
                    ToVector(CitySceneConfig.GridToWorld(footprint.Col + footprint.Width - 0.5f, footprint.Row + footprint.Height - 0.5f)),
                    ToVector(CitySceneConfig.GridToWorld(footprint.Col - 0.5f, footprint.Row + footprint.Height - 0.5f)),
                ],
            });
            AddChild(collision);
        }
    }

    private void SpawnProps()
    {
        var tileMap = GetNode<TileMapLayer>(CitySceneConfig.TileMapLayerName);
        foreach (var placement in CitySceneConfig.Props)
        {
            var config = PropRegistry.GetByFileName(placement.FileName);
            if (config is null)
                continue;

            var worldPosition = placement.MarkerName is null
                ? tileMap.ToGlobal(tileMap.MapToLocal(new Vector2I(placement.Col, placement.Row)))
                : GetNode<Marker2D>($"{CitySceneConfig.MarketDistrictNodeName}/{placement.MarkerName}").GlobalPosition;
            var sorter = new IsometricYSorterNode { GlobalPosition = worldPosition };
            sorter.AddChild(new Sprite2D
            {
                Texture = GD.Load<Texture2D>(config.ResourcePath),
                Centered = true,
                Position = new Vector2(0f, -config.Height / 2f),
            });
            AddChild(sorter);

            if (placement.IsBlocking)
                AddPropCollider(worldPosition, config.Width * 0.32f, config.Height * 0.12f);
        }
    }

    private void AddPropCollider(Vector2 worldPosition, float width, float height)
    {
        var body = new StaticBody2D
        {
            CollisionLayer = 1u << (PhysicsLayers.World - 1),
            CollisionMask = 1u << (PhysicsLayers.World - 1),
            GlobalPosition = worldPosition,
        };
        body.AddChild(new CollisionShape2D { Shape = new RectangleShape2D { Size = new Vector2(width, height) } });
        AddChild(body);
    }

    private void CreateNpcAnchors()
    {
        foreach (var anchor in CitySceneConfig.NpcAnchors)
        {
            AddChild(new Marker2D
            {
                Name = anchor.Name,
                Position = ToVector(CitySceneConfig.GridToWorld(anchor.Col, anchor.Row)),
            });
        }

        for (var index = 0; index < CitySceneConfig.GuardPostPositions.Length; index++)
        {
            AddChild(new Marker2D
            {
                Name = $"GuardPost{index + 1}",
                Position = ToVector(CitySceneConfig.GridToWorld(CitySceneConfig.GuardPostPositions[index])),
            });
        }
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
            player.GlobalPosition = GetNode<Marker2D>(CitySceneConfig.PlayerSpawnName).GlobalPosition;
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
        var camera = GetNode<IsometricCameraNode>(CitySceneConfig.CameraNodeName);
        camera.FollowTarget = GetNode<Node2D>("Player");
        camera.SetBounds(new Rect2(
            CitySceneConfig.WorldBoundaryLeft,
            CitySceneConfig.WorldBoundaryTop,
            CitySceneConfig.WorldBoundaryRight - CitySceneConfig.WorldBoundaryLeft,
            CitySceneConfig.WorldBoundaryBottom - CitySceneConfig.WorldBoundaryTop));
        camera.SnapToPixels = true;
        camera.SnapToTarget();
    }

    private void ConfigureTransitions()
    {
        var forestTransition = GetNode<AreaTransitionNode>(CitySceneConfig.Transitions[0].NodePath);
        forestTransition.Position = ToVector(CitySceneConfig.GridToWorld(CitySceneConfig.SouthTransitionGridPosition));

        var towerTransition = GetNode<AreaTransitionNode>(CitySceneConfig.Transitions[1].NodePath);
        towerTransition.Position = ToVector(CitySceneConfig.GridToWorld(CitySceneConfig.TowerTransitionGridPosition));
        foreach (var endpoint in CitySceneConfig.Transitions)
            GetNode<AreaTransitionNode>(endpoint.NodePath).Configure(
                endpoint.TargetArea,
                endpoint.SpawnPointId,
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
        var objectives = QuestService.GetActiveObjectives(questId);
        if (objectives.Count == 0)
            return;

        _hud.SetQuestObjective(quest.Title, objectives[0].Text, quest.Objectives.Count - objectives.Count, quest.Objectives.Count);
    }

    private static Vector2 ToVector(CitySceneConfig.WorldPosition position) => new(position.X, position.Y);
}