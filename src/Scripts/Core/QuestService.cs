using System;
using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# quest progression service. Runtime progress is kept separate from
/// static definitions loaded by <see cref="IQuestDatabase"/>.
/// </summary>
public sealed class QuestService : IQuestService
{
    private readonly IQuestDatabase _database;
    private readonly IEventBus _eventBus;
    private readonly Dictionary<string, QuestState> _statesByQuestId = new(StringComparer.Ordinal);
    private readonly Dictionary<string, HashSet<string>> _completedObjectivesByQuestId = new(StringComparer.Ordinal);

    public QuestService(IQuestDatabase database, IEventBus eventBus)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    /// <inheritdoc/>
    public void StartQuest(string questId)
    {
        _ = _database.GetQuest(questId);
        if (GetQuestState(questId) != QuestState.NotStarted)
            return;

        _statesByQuestId[questId] = QuestState.Active;
        _eventBus.Publish(new QuestStartedEvent(questId));
    }

    /// <inheritdoc/>
    public void CompleteObjective(string questId, string objectiveId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(objectiveId);
        var quest = _database.GetQuest(questId);
        if (GetQuestState(questId) != QuestState.Active)
            throw new InvalidOperationException($"Quest '{questId}' must be active before objectives can be completed.");

        var objective = quest.Objectives.Find(candidate => candidate.Id == objectiveId)
            ?? throw new KeyNotFoundException($"Objective '{objectiveId}' was not found in quest '{questId}'.");
        var completedObjectives = GetCompletedObjectives(questId);
        if (!completedObjectives.Add(objective.Id))
            return;

        _eventBus.Publish(new QuestObjectiveCompletedEvent(questId, objective.Id));

        if (AreRequiredObjectivesComplete(quest, completedObjectives))
            CompleteQuest(questId);
    }

    /// <inheritdoc/>
    public void CompleteQuest(string questId)
    {
        var quest = _database.GetQuest(questId);
        if (GetQuestState(questId) != QuestState.Active)
            throw new InvalidOperationException($"Quest '{questId}' must be active before it can be completed.");

        _statesByQuestId[questId] = QuestState.Completed;
        _eventBus.Publish(new QuestCompletedEvent(questId));

        if (!string.IsNullOrWhiteSpace(quest.TriggersQuestId))
            StartQuest(quest.TriggersQuestId);
    }

    /// <inheritdoc/>
    public QuestState GetQuestState(string questId)
    {
        _ = _database.GetQuest(questId);
        return _statesByQuestId.GetValueOrDefault(questId, QuestState.NotStarted);
    }

    /// <inheritdoc/>
    public List<QuestData> GetActiveQuests()
    {
        var activeQuests = new List<QuestData>();
        foreach (var quest in _database.GetAllQuests())
        {
            if (GetQuestState(quest.Id) == QuestState.Active)
                activeQuests.Add(quest);
        }

        return activeQuests;
    }

    /// <inheritdoc/>
    public List<QuestObjective> GetActiveObjectives(string questId)
    {
        var quest = _database.GetQuest(questId);
        if (GetQuestState(questId) != QuestState.Active)
            return [];

        var completedObjectives = GetCompletedObjectives(questId);
        return quest.Objectives.FindAll(objective => !completedObjectives.Contains(objective.Id));
    }

    /// <inheritdoc/>
    public void ApplyQuestStates(IReadOnlyDictionary<string, QuestState> questStates)
    {
        ArgumentNullException.ThrowIfNull(questStates);
        _statesByQuestId.Clear();
        _completedObjectivesByQuestId.Clear();

        foreach (var (questId, questState) in questStates)
        {
            _ = _database.GetQuest(questId);
            _statesByQuestId.Add(questId, questState);
        }
    }

    /// <inheritdoc/>
    public Dictionary<string, QuestState> GetQuestStates() => new(_statesByQuestId, StringComparer.Ordinal);

    private HashSet<string> GetCompletedObjectives(string questId)
    {
        if (!_completedObjectivesByQuestId.TryGetValue(questId, out var completedObjectives))
        {
            completedObjectives = new HashSet<string>(StringComparer.Ordinal);
            _completedObjectivesByQuestId.Add(questId, completedObjectives);
        }

        return completedObjectives;
    }

    private static bool AreRequiredObjectivesComplete(QuestData quest, HashSet<string> completedObjectives)
    {
        foreach (var objective in quest.Objectives)
        {
            if (objective.Required && !completedObjectives.Contains(objective.Id))
                return false;
        }

        return true;
    }
}