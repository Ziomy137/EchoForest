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

    public Direction FacingDirection => _controller.FacingDirection;
    public PlayerState CurrentState => _controller.CurrentState;

    public override void _Ready()
    {
        var input = new InputHandler();
        var stateMachine = new PlayerStateMachine();
        _controller = new PlayerController(input, stateMachine);

        var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _animController = new PlayerAnimationController(new GodotAnimatedSprite(sprite));
    }

    public override void _PhysicsProcess(double delta)
    {
        _controller.SimulatePhysicsFrame((float)delta);
        Velocity = _controller.Velocity;
        MoveAndSlide();

        _animController.UpdateAnimation(_controller.CurrentState, _controller.FacingDirection);
        ZIndex = IsometricSorter.CalculateZIndex(GlobalPosition);
    }
}
