using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>CharacterBody2D</c> node that drives player movement using
/// <see cref="PlayerController"/>.
///
/// Bridges the pure-C# controller to the Godot physics engine:
/// <list type="bullet">
///   <item>Calls <see cref="PlayerController.SimulatePhysicsFrame"/> each tick</item>
///   <item>Copies the resulting <c>Velocity</c> onto the <c>CharacterBody2D</c></item>
///   <item>Calls <c>MoveAndSlide()</c> for physics-engine collision resolution</item>
/// </list>
///
/// All movement logic lives in <see cref="PlayerController"/> (pure C#, NUnit-tested).
/// This node contains only Godot engine integration glue.
///
/// Excluded from NUnit code coverage because it requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot CharacterBody2D wrapper — requires scene tree")]
public partial class PlayerControllerNode : CharacterBody2D
{
    private PlayerController _controller = null!;
    private PlayerAnimationController _animController = null!;
    private Sprite2D _equipmentOverlay = null!;
    private IEventBus? _eventBus;
    private ItemDatabase? _itemDatabase;

    /// <summary>Pure-C# controller used by gameplay components that interact with the player.</summary>
    public IPlayerController Controller => _controller;

    public Direction FacingDirection => _controller.FacingDirection;
    public PlayerState CurrentState => _controller.CurrentState;

    public override void _Ready()
    {
        var input = (GetTree().CurrentScene as IAreaSceneContext)?.InputHandler
            ?? new global::EchoForest.InputHandler();
        var stateMachine = new PlayerStateMachine();
        _controller = new PlayerController(input, stateMachine);

        var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _animController = new PlayerAnimationController(new GodotAnimatedSprite(sprite));
        _equipmentOverlay = GetNode<Sprite2D>("EquipmentOverlay");
        _eventBus = (GetTree().CurrentScene as IAreaSceneContext)?.EventBus;
        if (_eventBus is not null)
        {
            _itemDatabase = new ItemDatabase(new GodotFileSystem());
            _eventBus.Subscribe<EquipmentChangedEvent>(OnEquipmentChanged);
        }
    }

    public override void _ExitTree() => _eventBus?.Unsubscribe<EquipmentChangedEvent>(OnEquipmentChanged);

    public override void _PhysicsProcess(double delta)
    {
        _controller.SimulatePhysicsFrame((float)delta);
        Velocity = _controller.Velocity;
        MoveAndSlide();

        _animController.UpdateAnimation(_controller.CurrentState, _controller.FacingDirection);
        ZIndex = IsometricSorter.CalculateZIndex(GlobalPosition);
    }

    private void OnEquipmentChanged(EquipmentChangedEvent gameEvent)
    {
        if (gameEvent.Slot != EquipmentSlot.Weapon)
            return;

        if (gameEvent.NewItemId is null)
        {
            _equipmentOverlay.Texture = null;
            _equipmentOverlay.Visible = false;
            return;
        }

        var item = _itemDatabase!.GetItem(gameEvent.NewItemId);
        _equipmentOverlay.Texture = GD.Load<Texture2D>(item.IconTexturePath);
        _equipmentOverlay.Visible = true;
    }
}
