using System;
using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>In-memory <see cref="IQuestDatabase"/> test double.</summary>
public sealed class MockQuestDatabase : IQuestDatabase
{
    private readonly Dictionary<string, QuestData> _questsById = new(StringComparer.Ordinal);

    public MockQuestDatabase(params QuestData[] quests)
    {
        foreach (var quest in quests)
            _questsById.Add(quest.Id, quest);
    }

    /// <inheritdoc/>
    public QuestData GetQuest(string questId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(questId);
        return _questsById.TryGetValue(questId, out var quest)
            ? quest
            : throw new KeyNotFoundException($"Quest '{questId}' was not found in the quest database.");
    }

    /// <inheritdoc/>
    public List<QuestData> GetAllQuests() => [.. _questsById.Values];
}