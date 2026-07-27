namespace EchoForest.Core;

/// <summary>In-memory NPC test double for NUnit interaction tests.</summary>
public sealed class MockNpc : INpc
{
    public string NpcId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public float InteractionRadius { get; set; }
    public bool IsInteractable { get; set; } = true;
    public int InteractionCount { get; private set; }
    public IPlayerController? LastPlayer { get; private set; }

    public void Interact(IPlayerController player)
    {
        InteractionCount++;
        LastPlayer = player;
    }
}