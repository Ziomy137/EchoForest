using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>Node2D</c> that bootstraps the Cottage Exterior test area at runtime.
///
/// Reads all layout data from <see cref="CottageSceneConfig"/> (pure C#, tested)
/// and populates the TileMapLayer, props, and boundary at <c>_Ready()</c>.
/// No tile data needs to be baked into the <c>.tscn</c> file.
///
/// Excluded from NUnit code coverage — requires the Godot engine scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot Node2D wrapper — requires scene tree")]
public partial class CottageAreaNode : Node2D
{
	public IEventBus EventBus { get; private set; } = null!;
	public IInputHandler InputHandler { get; private set; } = null!;
	public IQuestDatabase QuestDatabase { get; private set; } = null!;
	public IQuestService QuestService { get; private set; } = null!;
	private GameHudNode _hud = null!;

	public override void _EnterTree()
	{
		EventBus = new EventBus();
		InputHandler = new global::EchoForest.InputHandler();
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
		WireQuestHud();
		WireIntroCutscene();
	}

	public override void _ExitTree()
	{
		EventBus.Unsubscribe<QuestStartedEvent>(OnQuestStarted);
		EventBus.Unsubscribe<QuestObjectiveCompletedEvent>(OnQuestObjectiveCompleted);
		EventBus.Unsubscribe<QuestCompletedEvent>(OnQuestCompleted);
		EventBus.Unsubscribe<MageAttackStartedEvent>(OnMageAttackStarted);
	}

	private void WireIntroCutscene()
	{
		EventBus.Subscribe<MageAttackStartedEvent>(OnMageAttackStarted);
		if (GameSession.ConsumeIntroCutsceneRequest())
			GetNode<CutsceneDirectorNode>("CutsceneDirector").PlayIntroMageAttack(() => { });
	}

	private void OnMageAttackStarted(MageAttackStartedEvent _)
	{
		QuestService.StartQuest("q_kidnapped");
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

	// ─── Tile population ──────────────────────────────────────────────────────

	private void PopulateTiles()
	{
		var tileMap = GetNode<TileMapLayer>(CottageSceneConfig.TileMapLayerName);
		for (var row = 0; row < CottageSceneConfig.GridRows; row++)
		{
			for (var col = 0; col < CottageSceneConfig.GridColumns; col++)
			{
				var tile = CottageSceneConfig.GetTileFileName(col, row);
				var sourceId = CottageSceneConfig.GetSourceId(tile);
				tileMap.SetCell(new Vector2I(col, row), sourceId, Vector2I.Zero);
			}
		}
	}

	// ─── Prop spawning ────────────────────────────────────────────────────────

	private void SpawnProps()
	{
		var tileMap = GetNode<TileMapLayer>(CottageSceneConfig.TileMapLayerName);

		// Props are added directly to the root so they share the same Y-sorted
		// parent as Player, enabling correct isometric depth interleaving.
		foreach (var placement in CottageSceneConfig.Props)
		{
			var config = PropRegistry.GetByFileName(placement.FileName);
			if (config is null) continue;

			// Use Godot's authoritative cell→world mapping so props always land
			// exactly on their tile regardless of tile_layout setting.
			var worldPos = tileMap.ToGlobal(tileMap.MapToLocal(new Vector2I(placement.Col, placement.Row)));

			// Y-sorter must be the parent so its ZIndex affects the rendered sprite.
			var sorter = new IsometricYSorterNode();
			sorter.GlobalPosition = worldPos;

			var sprite = new Sprite2D();
			sprite.Texture = GD.Load<Texture2D>(config.ResourcePath);
			// Place sprite so its bottom edge sits at the tile centre (ground level).
			sprite.Position = new Vector2(0, -config.Height / 2f);

			sorter.AddChild(sprite);
			AddChild(sorter);

			// Blocking props also get a small collision circle
			if (placement.FileName == PropRegistry.Well.FileName
				|| placement.FileName == PropRegistry.Tree.FileName
				|| placement.FileName == PropRegistry.FencePost.FileName)
			{
				AddPropCollider(this, worldPos, radius: 6f);
			}
		}
	}

	private static void AddPropCollider(Node2D parent, Vector2 worldPos, float radius)
	{
		var body = new StaticBody2D();
		body.CollisionLayer = 1u << (PhysicsLayers.World - 1);
		body.CollisionMask = 1u << (PhysicsLayers.World - 1);

		var shape = new CollisionShape2D();
		var circle = new CircleShape2D();
		circle.Radius = radius;
		shape.Shape = circle;

		body.AddChild(shape);
		body.GlobalPosition = worldPos;
		parent.AddChild(body);
	}

	// ─── Boundary walls ───────────────────────────────────────────────────────

	private void SetupBoundary()
	{
		var boundary = GetNode<StaticBody2D>(CottageSceneConfig.BoundaryNodeName);

		float cx = (CottageSceneConfig.WorldBoundaryLeft + CottageSceneConfig.WorldBoundaryRight) / 2f;
		float cy = (CottageSceneConfig.WorldBoundaryTop + CottageSceneConfig.WorldBoundaryBottom) / 2f;
		float w = CottageSceneConfig.WorldBoundaryRight - CottageSceneConfig.WorldBoundaryLeft + 128f;
		float h = CottageSceneConfig.WorldBoundaryBottom - CottageSceneConfig.WorldBoundaryTop + 128f;

		// North wall
		AddWallSegment(boundary, new Vector2(cx, CottageSceneConfig.WorldBoundaryTop - 32f), w, 64f);
		// South wall
		AddWallSegment(boundary, new Vector2(cx, CottageSceneConfig.WorldBoundaryBottom + 32f), w, 64f);
		// West wall
		AddWallSegment(boundary, new Vector2(CottageSceneConfig.WorldBoundaryLeft - 32f, cy), 64f, h);
		// East wall
		AddWallSegment(boundary, new Vector2(CottageSceneConfig.WorldBoundaryRight + 32f, cy), 64f, h);
	}

	private static void AddWallSegment(StaticBody2D parent, Vector2 center, float width, float height)
	{
		var shape = new CollisionShape2D();
		var rect = new RectangleShape2D();
		rect.Size = new Vector2(width, height);
		shape.Shape = rect;
		shape.Position = center;
		parent.AddChild(shape);
	}

	// ─── Player spawn ─────────────────────────────────────────────────────────

	private void SpawnPlayer()
	{
		var player = GetNode<Node2D>("Player");

		if (GameSession.HasPlayerPosition)
		{
			// Continue: restore last saved position instead of using spawn point.
			player.GlobalPosition = new Vector2(GameSession.LastPlayerX, GameSession.LastPlayerY);
		}
		else
		{
			// New Game (or first load): place player at the scene spawn marker.
			var spawnPoint = GetNode<Marker2D>(CottageSceneConfig.PlayerSpawnName);
			player.GlobalPosition = spawnPoint.GlobalPosition;
		}
	}

	// ─── Camera setup ─────────────────────────────────────────────────────────

	private void SetupCamera()
	{
		var camera = GetNode<IsometricCameraNode>(CottageSceneConfig.CameraNodeName);
		var player = GetNode<Node2D>("Player");
		camera.FollowTarget = player;
		camera.SetBounds(CottageSceneConfig.CameraBounds);
		camera.SnapToPixels = true;
		camera.SnapToTarget();
	}

	// ─── Input ────────────────────────────────────────────────────────────────

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!@event.IsActionPressed("pause")) return;

		var existingMenu = GetTree().Root.FindChild("PauseMenu", recursive: false, owned: false);

		if (existingMenu is not null)
		{
			if (existingMenu is CanvasLayer { Visible: false })
			{
				GetViewport().SetInputAsHandled();
				return;
			}

			// Escape pressed while menu is open — close it.
			// Primary handler is PauseMenuNode._Input; this fires only if that
			// node didn't receive/consume the event (e.g. added to Root sibling).
			GetViewport().SetInputAsHandled();
			existingMenu.QueueFree();
			return;
		}

		// No menu open — open it.
		GetViewport().SetInputAsHandled();

		// Freeze the player node so the character cannot move while the menu is
		// open. We deliberately avoid GetTree().Paused because in Godot 4 the
		// pause system blocks _gui_input on ALL nodes — including those marked
		// ProcessMode.Always — preventing button Pressed signals from firing.
		var player = GetNode<Node>("Player");
		player.ProcessMode = ProcessModeEnum.Disabled;

		var pauseMenu = GD.Load<PackedScene>(MainMenuConfig.PauseMenuScenePath).Instantiate<PauseMenuNode>();

		// Restore the player when the pause menu is freed (any button or Esc).
		pauseMenu.TreeExiting += () =>
		{
			if (GodotObject.IsInstanceValid(player))
				player.ProcessMode = ProcessModeEnum.Inherit;
		};

		GetTree().Root.AddChild(pauseMenu);
	}
}
