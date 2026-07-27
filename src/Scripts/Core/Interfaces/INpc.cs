namespace EchoForest.Core;

/// <summary>
/// Contract for an NPC that can be targeted and interacted with by the player.
/// </summary>
public interface INpc
{
    /// <summary>Stable identifier used by dialogue and quest systems.</summary>
    string NpcId { get; }

    /// <summary>Name displayed to the player.</summary>
    string DisplayName { get; }

    /// <summary>Maximum distance at which the player may interact with this NPC.</summary>
    float InteractionRadius { get; }

    /// <summary>Whether this NPC can currently be targeted for interaction.</summary>
    bool IsInteractable { get; }

    /// <summary>Handles an interaction initiated by <paramref name="player"/>.</summary>
    void Interact(IPlayerController player);
}