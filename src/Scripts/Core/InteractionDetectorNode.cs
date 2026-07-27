using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <see cref="Area2D"/> bridge for <see cref="InteractionDetector"/>.
/// Tracks NPC bodies in the player interaction area and forwards the interact input.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot Area2D wrapper — requires scene tree")]
public partial class InteractionDetectorNode : Area2D
{
    private readonly Dictionary<INpc, NpcControllerNode> _npcNodes = new();
    private InteractionDetector _detector = null!;
    private PlayerControllerNode _player = null!;

    public override void _Ready()
    {
        CallDeferred(nameof(Initialize));
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_detector is null)
            return;

        foreach (var pair in _npcNodes)
        {
            if (!GodotObject.IsInstanceValid(pair.Value))
                continue;

            _detector.UpdateNpcDistance(pair.Key, GlobalPosition.DistanceTo(pair.Value.GlobalPosition));
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_detector is null || !@event.IsActionPressed(InputActionNames.Interact))
            return;

        if (_detector.NearestInteractable is INpc npc && _npcNodes.TryGetValue(npc, out var npcNode))
            npcNode.FaceToward(_player.GlobalPosition);

        if (_detector.TryInteract(_player.Controller))
            GetViewport().SetInputAsHandled();
    }

    private void Initialize()
    {
        _player = GetParent<PlayerControllerNode>();
        var hud = GetTree().CurrentScene.GetNode<GameHudNode>("HUD");
        _detector = new InteractionDetector(hud);
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is not NpcControllerNode npcNode)
            return;

        var npc = npcNode.Controller;
        _npcNodes[npc] = npcNode;
        _detector.TrackNpc(npc, GlobalPosition.DistanceTo(npcNode.GlobalPosition));
    }

    private void OnBodyExited(Node2D body)
    {
        if (body is not NpcControllerNode npcNode)
            return;

        var npc = npcNode.Controller;
        _npcNodes.Remove(npc);
        _detector.RemoveNpc(npc);
    }
}