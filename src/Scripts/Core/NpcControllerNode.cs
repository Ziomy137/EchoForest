using System;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot wrapper for an NPC's pure-C# interaction controller and idle animation.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot CharacterBody2D wrapper — requires scene tree")]
public partial class NpcControllerNode : CharacterBody2D
{
    [Export]
    public string NpcId { get; set; } = string.Empty;

    [Export]
    public string DisplayName { get; set; } = string.Empty;

    [Export]
    public float InteractionRadius { get; set; } = 96f;

    private NpcController _controller = null!;
    private AnimatedSprite2D _sprite = null!;
    private DialogueBoxNode? _dialogueBox;

    /// <summary>Pure-C# interaction controller exposed to nearby detectors.</summary>
    public INpc Controller => _controller;

    public override void _Ready()
    {
        _controller = new NpcController(NpcId, DisplayName, InteractionRadius);
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _sprite.Play("idle_down");
        CallDeferred(nameof(ConnectDialogue));
    }

    public override void _PhysicsProcess(double delta)
    {
        ZIndex = IsometricSorter.CalculateZIndex(GlobalPosition);
    }

    /// <summary>Turns the NPC's idle animation toward the specified world position.</summary>
    public void FaceToward(Vector2 targetPosition)
    {
        var offset = targetPosition - GlobalPosition;
        if (offset == Vector2.Zero)
            return;

        var direction = Math.Abs(offset.X) > Math.Abs(offset.Y)
            ? offset.X > 0f ? Direction.Right : Direction.Left
            : offset.Y > 0f ? Direction.Down : Direction.Up;

        _sprite.Play($"idle_{direction.ToString().ToLowerInvariant()}");
    }

    private void ConnectDialogue()
    {
        _dialogueBox = GetTree().CurrentScene.GetNodeOrNull<DialogueBoxNode>("DialogueBox");
        if (_dialogueBox is null)
            return;

        _controller = new NpcController(NpcId, DisplayName, InteractionRadius, _ => _dialogueBox.StartConversation(NpcId));
    }
}