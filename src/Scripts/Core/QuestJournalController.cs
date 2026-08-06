using System;
using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# projection of quest events for the journal UI.
/// </summary>
public sealed class QuestJournalController : IDisposable
{
    private readonly IEventBus _eventBus;
    private readonly List<string> _activeQuestIds = [];
    private readonly List<string> _completedQuestIds = [];
    private readonly Dictionary<string, HashSet<string>> _completedObjectivesByQuestId = new(StringComparer.Ordinal);
    private bool _isDisposed;

    public QuestJournalController(IEventBus eventBus)
    {
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _eventBus.Subscribe<QuestStartedEvent>(OnQuestStarted);
        _eventBus.Subscribe<QuestObjectiveCompletedEvent>(OnQuestObjectiveCompleted);
        _eventBus.Subscribe<QuestCompletedEvent>(OnQuestCompleted);
    }

    /// <summary>Raised after a quest event changes the journal projection.</summary>
    public event Action? Changed;

    /// <summary>IDs of quests currently displayed in the Active section.</summary>
    public IReadOnlyList<string> ActiveQuestIds => _activeQuestIds;

    /// <summary>IDs of quests currently displayed in the Completed section.</summary>
    public IReadOnlyList<string> CompletedQuestIds => _completedQuestIds;

    /// <summary>Records completed objective IDs for checkbox rendering.</summary>
    public bool IsObjectiveCompleted(string questId, string objectiveId)
    {
        return _completedObjectivesByQuestId.TryGetValue(questId, out var completedObjectives)
            && completedObjectives.Contains(objectiveId);
    }

    /// <summary>Seeds the journal from a loaded save without replaying game events.</summary>
    public void Synchronize(IReadOnlyDictionary<string, QuestState> questStates)
    {
        ArgumentNullException.ThrowIfNull(questStates);
        _activeQuestIds.Clear();
        _completedQuestIds.Clear();
        _completedObjectivesByQuestId.Clear();

        foreach (var (questId, questState) in questStates)
        {
            if (questState == QuestState.Active)
                _activeQuestIds.Add(questId);
            else if (questState == QuestState.Completed)
                _completedQuestIds.Add(questId);
        }

        Changed?.Invoke();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        _eventBus.Unsubscribe<QuestStartedEvent>(OnQuestStarted);
        _eventBus.Unsubscribe<QuestObjectiveCompletedEvent>(OnQuestObjectiveCompleted);
        _eventBus.Unsubscribe<QuestCompletedEvent>(OnQuestCompleted);
        _isDisposed = true;
    }

    private void OnQuestStarted(QuestStartedEvent gameEvent)
    {
        var changed = _completedQuestIds.Remove(gameEvent.QuestId);
        if (!_activeQuestIds.Contains(gameEvent.QuestId))
        {
            _activeQuestIds.Add(gameEvent.QuestId);
            changed = true;
        }

        if (changed)
            Changed?.Invoke();
    }

    private void OnQuestObjectiveCompleted(QuestObjectiveCompletedEvent gameEvent)
    {
        if (!_completedObjectivesByQuestId.TryGetValue(gameEvent.QuestId, out var completedObjectives))
        {
            completedObjectives = new HashSet<string>(StringComparer.Ordinal);
            _completedObjectivesByQuestId.Add(gameEvent.QuestId, completedObjectives);
        }

        if (completedObjectives.Add(gameEvent.ObjectiveId))
            Changed?.Invoke();
    }

    private void OnQuestCompleted(QuestCompletedEvent gameEvent)
    {
        var changed = _activeQuestIds.Remove(gameEvent.QuestId);
        if (!_completedQuestIds.Contains(gameEvent.QuestId))
        {
            _completedQuestIds.Add(gameEvent.QuestId);
            changed = true;
        }

        if (changed)
            Changed?.Invoke();
    }
}