using System;
using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# proximity tracker for NPC interactions.
/// Selects the closest interactable NPC and keeps the HUD prompt synchronized.
/// </summary>
public sealed class InteractionDetector
{
    private readonly IGameHudController _gameHud;
    private readonly Dictionary<INpc, float> _nearbyNpcs = new();

    public InteractionDetector(IGameHudController gameHud)
    {
        _gameHud = gameHud ?? throw new ArgumentNullException(nameof(gameHud));
    }

    /// <summary>Raised when the active interaction target changes to an NPC.</summary>
    public event Action<INpc>? InteractableEntered;

    /// <summary>Raised when an NPC stops being the active interaction target.</summary>
    public event Action<INpc>? InteractableExited;

    /// <summary>The closest NPC currently within its interaction radius.</summary>
    public INpc? NearestInteractable { get; private set; }

    /// <summary>Tracks an NPC at its current distance from the player.</summary>
    public void TrackNpc(INpc npc, float distance)
    {
        ArgumentNullException.ThrowIfNull(npc);
        ValidateDistance(distance);

        UpdateTrackedNpc(npc, distance);
    }

    /// <summary>Updates the current distance for a tracked NPC.</summary>
    public void UpdateNpcDistance(INpc npc, float distance)
    {
        ArgumentNullException.ThrowIfNull(npc);
        ValidateDistance(distance);

        UpdateTrackedNpc(npc, distance);
    }

    /// <summary>Stops tracking an NPC that left the detector area.</summary>
    public void RemoveNpc(INpc npc)
    {
        ArgumentNullException.ThrowIfNull(npc);

        if (_nearbyNpcs.Remove(npc))
            UpdateNearestInteractable();
    }

    /// <summary>Attempts to interact with the current nearest target.</summary>
    public bool TryInteract(IPlayerController player)
    {
        ArgumentNullException.ThrowIfNull(player);

        var npc = NearestInteractable;
        if (npc is null)
            return false;

        if (!npc.IsInteractable)
        {
            RemoveNpc(npc);
            return false;
        }

        npc.Interact(player);
        return true;
    }

    private void UpdateTrackedNpc(INpc npc, float distance)
    {
        if (!npc.IsInteractable || distance > npc.InteractionRadius)
            _nearbyNpcs.Remove(npc);
        else
            _nearbyNpcs[npc] = distance;

        UpdateNearestInteractable();
    }

    private void UpdateNearestInteractable()
    {
        var previous = NearestInteractable;
        INpc? nearest = null;
        var nearestDistance = float.MaxValue;

        foreach (var pair in _nearbyNpcs)
        {
            if (pair.Value >= nearestDistance)
                continue;

            nearest = pair.Key;
            nearestDistance = pair.Value;
        }

        if (ReferenceEquals(previous, nearest))
            return;

        if (previous is not null)
            InteractableExited?.Invoke(previous);

        NearestInteractable = nearest;

        if (nearest is null)
        {
            _gameHud.HideInteractionPrompt();
            return;
        }

        _gameHud.ShowInteractionPrompt("Talk");
        InteractableEntered?.Invoke(nearest);
    }

    private static void ValidateDistance(float distance)
    {
        if (distance < 0f)
            throw new ArgumentOutOfRangeException(nameof(distance), "Distance cannot be negative.");
    }
}