using System;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# interaction state for one non-player character.
/// </summary>
public sealed class NpcController : INpc
{
    private readonly Action<IPlayerController>? _onInteracted;

    public NpcController(
        string npcId,
        string displayName,
        float interactionRadius,
        Action<IPlayerController>? onInteracted = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(npcId);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        if (interactionRadius <= 0f)
            throw new ArgumentOutOfRangeException(nameof(interactionRadius), "Interaction radius must be greater than zero.");

        NpcId = npcId;
        DisplayName = displayName;
        InteractionRadius = interactionRadius;
        _onInteracted = onInteracted;
    }

    /// <inheritdoc/>
    public string NpcId { get; }

    /// <inheritdoc/>
    public string DisplayName { get; }

    /// <inheritdoc/>
    public float InteractionRadius { get; }

    /// <inheritdoc/>
    public bool IsInteractable { get; set; } = true;

    /// <inheritdoc/>
    public void Interact(IPlayerController player)
    {
        ArgumentNullException.ThrowIfNull(player);

        if (!IsInteractable)
            return;

        _onInteracted?.Invoke(player);
    }
}