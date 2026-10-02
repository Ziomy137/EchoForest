using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>Godot root that composes the Tower approach, interior and local Mage anchor.</summary>
[ExcludeFromCodeCoverage(Justification = "Godot Node2D wrapper - requires scene tree")]
public partial class MagesTowerAreaNode : Node2D, IAreaSceneContext
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
        AddNonWalkableTileColliders();
        SpawnProps();
        CreateMageAnchor();
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
        GetNode<Marker2D>(MagesTowerSceneConfig.PlayerSpawnName).Position = ToVector(MagesTowerSceneConfig.DefaultSpawnWorldPosition);
        GetNode<Marker2D>("CityEntranceSpawnPoint").Position = ToVector(MagesTowerSceneConfig.CityEntranceWorldPosition);
        GetNode<Marker2D>("ExteriorDoorSpawnPoint").Position = ToVector(MagesTowerSceneConfig.ExteriorDoorSpawnWorldPosition);
        GetNode<Marker2D>("InteriorDoorSpawnPoint").Position = ToVector(MagesTowerSceneConfig.InteriorEntryWorldPosition);
        GetNode<Marker2D>("CityExitSpawnPoint").Position = ToVector(MagesTowerSceneConfig.CityExitSpawnWorldPosition);
    }

    private void PopulateTiles()
    {
        var tileMap = GetNode<TileMapLayer>(MagesTowerSceneConfig.TileMapLayerName);
        for (var row = 0; row < MagesTowerSceneConfig.GridRows; row++)
        {
            for (var col = 0; col < MagesTowerSceneConfig.GridColumns; col++)
            {
                var tileFileName = MagesTowerSceneConfig.GetTileFileName(col, row);
                tileMap.SetCell(new Vector2I(col, row), MagesTowerSceneConfig.GetSourceId(tileFileName), Vector2I.Zero);
            }
        }
    }

    private void AddNonWalkableTileColliders()
    {
        var tileMap = GetNode<TileMapLayer>(MagesTowerSceneConfig.TileMapLayerName);
        for (var row = 0; row < MagesTowerSceneConfig.GridRows; row++)
        {
            for (var col = 0; col < MagesTowerSceneConfig.GridColumns; col++)
            {
                var tileFileName = MagesTowerSceneConfig.GetTileFileName(col, row);
                if (TileRegistry.GetByFileName(tileFileName)?.IsWalkable != false)
                    continue;

                var worldPosition = tileMap.ToGlobal(tileMap.MapToLocal(new Vector2I(col, row)));
                AddCollider(worldPosition, TileRegistry.TileWidth * 0.75f, TileRegistry.TileHeight * 0.75f);
            }
        }
    }

    private void SetupBoundaries()
    {
        var boundary = GetNode<StaticBody2D>(MagesTowerSceneConfig.BoundaryNodeName);
        var width = MagesTowerSceneConfig.WorldBoundaryRight - MagesTowerSceneConfig.WorldBoundaryLeft;
        var height = MagesTowerSceneConfig.WorldBoundaryBottom - MagesTowerSceneConfig.WorldBoundaryTop;
        AddWallSegment(boundary, new Vector2(320f, MagesTowerSceneConfig.WorldBoundaryTop - 32f), width + 128f, 64f);
        AddWallSegment(boundary, new Vector2(320f, MagesTowerSceneConfig.WorldBoundaryBottom + 32f), width + 128f, 64f);
        AddWallSegment(boundary, new Vector2(MagesTowerSceneConfig.WorldBoundaryLeft - 32f, 560f), 64f, height + 128f);
        AddWallSegment(boundary, new Vector2(MagesTowerSceneConfig.WorldBoundaryRight + 32f, 560f), 64f, height + 128f);
    }

    private static void AddWallSegment(StaticBody2D parent, Vector2 center, float width, float height)
    {
        parent.AddChild(new CollisionShape2D
        {
            Shape = new RectangleShape2D { Size = new Vector2(width, height) },
            Position = center,
        });
    }

    private void SpawnProps()
    {
        var tileMap = GetNode<TileMapLayer>(MagesTowerSceneConfig.TileMapLayerName);
        foreach (var placement in MagesTowerSceneConfig.Props)
        {
            var config = PropRegistry.GetByFileName(placement.FileName);
            if (config is null)
                continue;

            var worldPosition = tileMap.ToGlobal(tileMap.MapToLocal(new Vector2I(placement.Col, placement.Row)));
            var sorter = new IsometricYSorterNode { GlobalPosition = worldPosition };
            sorter.AddChild(new Sprite2D
            {
                Texture = GD.Load<Texture2D>(config.ResourcePath),
                Centered = true,
                Position = new Vector2(0f, -config.Height / 2f),
            });
            AddChild(sorter);

            if (placement.IsBlocking)
                AddCollider(worldPosition, config.Width * 0.38f, config.Height * 0.14f);
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
        body.AddChild(new CollisionShape2D { Shape = new RectangleShape2D { Size = new Vector2(width, height) } });
        AddChild(body);
    }

    private void CreateMageAnchor()
    {
        AddChild(new Marker2D
        {
            Name = "MageAnchor",
            Position = ToVector(MagesTowerSceneConfig.GridToWorld(MagesTowerSceneConfig.MageAnchor)),
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
            player.GlobalPosition = GetNode<Marker2D>(MagesTowerSceneConfig.PlayerSpawnName).GlobalPosition;
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
        var camera = GetNode<IsometricCameraNode>(MagesTowerSceneConfig.CameraNodeName);
        camera.FollowTarget = GetNode<Node2D>("Player");
        camera.SetBounds(new Rect2(
            MagesTowerSceneConfig.WorldBoundaryLeft,
            MagesTowerSceneConfig.WorldBoundaryTop,
            MagesTowerSceneConfig.WorldBoundaryRight - MagesTowerSceneConfig.WorldBoundaryLeft,
            MagesTowerSceneConfig.WorldBoundaryBottom - MagesTowerSceneConfig.WorldBoundaryTop));
        camera.SnapToPixels = true;
        camera.SnapToTarget();
    }

    private void ConfigureTransitions()
    {
        foreach (var endpoint in MagesTowerSceneConfig.Transitions)
        {
            var transition = GetNode<AreaTransitionNode>($"Transitions/{endpoint.Type}");
            transition.Position = ToVector(MagesTowerSceneConfig.GridToWorld(endpoint.Position));
            transition.Configure(
                endpoint.TargetArea,
                endpoint.SpawnPointId,
                TransitionType.FadeToBlack,
                AreaTransitionService,
                InputHandler,
                CreateTransitionSaveData);
        }
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

    private static Vector2 ToVector(MagesTowerSceneConfig.WorldPosition position) => new(position.X, position.Y);
}