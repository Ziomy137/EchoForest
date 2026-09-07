namespace EchoForest.Core;

/// <summary>Coordinates persistence, event publication, and target scene loading for an area transition.</summary>
public interface IAreaTransitionService
{
    /// <summary>Saves the current game state and requests a transition to a target scene.</summary>
    void TriggerTransition(string fromArea, string targetArea, string spawnPointId, SaveData saveData);
}