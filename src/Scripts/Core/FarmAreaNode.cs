using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>Godot root node that builds and composes the Farm area at runtime.</summary>
[ExcludeFromCodeCoverage(Justification = "Godot Node2D wrapper - requires scene tree")]
public partial class FarmAreaNode : Node2D, IAreaSceneContext
{
    public IEventBus EventBus { get; private set; } = null!;
    public IInputHandler InputHandler { get; private set; } = null!;
    public IAreaTransitionService AreaTransitionService { get; private set; } = null!;
    public IQuestDatabase QuestDatabase { get; private set; } = null!;
    public IQuestService QuestService { get; private set; } = null!;

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
        SetupBoundary();
        SpawnPlayer();
        SetupCamera();
        ConfigureAreaTransitions();
    }

    private void PopulateTiles()
    {
        var tileMap = GetNode<TileMapLayer>(FarmSceneConfig.TileMapLayerName);
        for (var row = 0; row < FarmSceneConfig.GridRows; row++)
        {
            for (var col = 0; col < FarmSceneConfig.GridColumns; col++)
            {
                var tileFileName = FarmSceneConfig.GetTileFileName(col, row);
                tileMap.SetCell(new Vector2I(col, row), FarmSceneConfig.GetSourceId(tileFileName), Vector2I.Zero);
            }
        }
    }

    private void SpawnProps()
    {
        var tileMap = GetNode<TileMapLayer>(FarmSceneConfig.TileMapLayerName);
        foreach (var placement in FarmSceneConfig.Props)
        {
            var config = PropRegistry.GetByFileName(placement.FileName);
            if (config is null)
                continue;

            var sprite = new Sprite2D
            {
                Texture = GD.Load<Texture2D>(config.ResourcePath),
                Centered = true,
            };
            var worldPosition = tileMap.ToGlobal(tileMap.MapToLocal(new Vector2I(placement.Col, placement.Row)));
            sprite.GlobalPosition = worldPosition + new Vector2(0f, -config.Height / 2f);
            AddChild(sprite);

            if (placement.IsBlocking)
                AddPropCollider(worldPosition, config.Width * 0.3f, config.Height * 0.12f);
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
        var shape = new CollisionShape2D
        {
            Shape = new RectangleShape2D { Size = new Vector2(width, height) },
        };
        body.AddChild(shape);
        AddChild(body);
    }

    private void SetupBoundary()
    {
        var boundary = GetNode<StaticBody2D>(FarmSceneConfig.BoundaryNodeName);
        var width = FarmSceneConfig.WorldBoundaryRight - FarmSceneConfig.WorldBoundaryLeft;
        var height = FarmSceneConfig.WorldBoundaryBottom - FarmSceneConfig.WorldBoundaryTop;
        AddWallSegment(boundary, new Vector2(240f, FarmSceneConfig.WorldBoundaryTop - 32f), width + 128f, 64f);
        AddWallSegment(boundary, new Vector2(240f, FarmSceneConfig.WorldBoundaryBottom + 32f), width + 128f, 64f);
        AddWallSegment(boundary, new Vector2(FarmSceneConfig.WorldBoundaryLeft - 32f, 504f), 64f, height + 128f);
        AddWallSegment(boundary, new Vector2(FarmSceneConfig.WorldBoundaryRight + 32f, 504f), 64f, height + 128f);
    }

    private static void AddWallSegment(StaticBody2D parent, Vector2 center, float width, float height)
    {
        var shape = new CollisionShape2D
        {
            Shape = new RectangleShape2D { Size = new Vector2(width, height) },
            Position = center,
        };
        parent.AddChild(shape);
    }

    private void SpawnPlayer()
    {
        var player = GetNode<Node2D>("Player");
        var spawnPointId = GameSession.ConsumeTransitionSpawnPointId();
        if (spawnPointId is not null && TryGetSpawnPoint(spawnPointId, out var transitionSpawnPoint))
        {
            player.GlobalPosition = transitionSpawnPoint.GlobalPosition;
        }
        else if (GameSession.HasPlayerPosition)
        {
            player.GlobalPosition = new Vector2(GameSession.LastPlayerX, GameSession.LastPlayerY);
        }
        else
        {
            player.GlobalPosition = GetNode<Marker2D>(FarmSceneConfig.PlayerSpawnName).GlobalPosition;
        }
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
        var camera = GetNode<IsometricCameraNode>(FarmSceneConfig.CameraNodeName);
        camera.FollowTarget = GetNode<Node2D>("Player");
        camera.SetBounds(new Rect2(
            FarmSceneConfig.WorldBoundaryLeft,
            FarmSceneConfig.WorldBoundaryTop,
            FarmSceneConfig.WorldBoundaryRight - FarmSceneConfig.WorldBoundaryLeft,
            FarmSceneConfig.WorldBoundaryBottom - FarmSceneConfig.WorldBoundaryTop));
        camera.SnapToPixels = true;
        camera.SnapToTarget();
    }

    private void ConfigureAreaTransitions()
    {
        GetNode<AreaTransitionNode>("ToCottageTransition").Configure(
            CottageSceneConfig.SceneResPath,
            "farm_entrance",
            TransitionType.FadeToBlack,
            AreaTransitionService,
            InputHandler,
            CreateTransitionSaveData);
        GetNode<AreaTransitionNode>("ToForestPathTransition").Configure(
            MainMenuConfig.ForestPathScenePath,
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