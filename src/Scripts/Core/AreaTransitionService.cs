using System;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# coordinator for an area transition. Godot wrappers own the visual
/// fade and scene-tree replacement; this service owns game state continuity.
/// </summary>
public sealed class AreaTransitionService : IAreaTransitionService
{
    private readonly IEventBus _eventBus;
    private readonly ISceneLoader _sceneLoader;
    private readonly ISaveDataService _saveService;

    public AreaTransitionService(IEventBus eventBus, ISceneLoader sceneLoader, ISaveDataService saveService)
    {
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _sceneLoader = sceneLoader ?? throw new ArgumentNullException(nameof(sceneLoader));
        _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
    }

    /// <inheritdoc/>
    public void TriggerTransition(string fromArea, string targetArea, string spawnPointId, SaveData saveData)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fromArea);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetArea);
        ArgumentException.ThrowIfNullOrWhiteSpace(spawnPointId);
        ArgumentNullException.ThrowIfNull(saveData);

        _saveService.Save(saveData, slot: 1);
        GameSession.SetQuestStates(saveData.QuestStates);
        GameSession.RequestTransitionSpawnPointId(spawnPointId);
        _sceneLoader.LoadScene(targetArea);
        _eventBus.Publish(new AreaTransitionEvent(fromArea, targetArea));
    }
}