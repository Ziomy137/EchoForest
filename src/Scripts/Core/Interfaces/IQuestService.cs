using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>Tracks quest progress and publishes quest lifecycle events.</summary>
public interface IQuestService
{
    void StartQuest(string questId);
    void CompleteObjective(string questId, string objectiveId);
    void CompleteQuest(string questId);
    QuestState GetQuestState(string questId);
    List<QuestData> GetActiveQuests();
    List<QuestObjective> GetActiveObjectives(string questId);
    void ApplyQuestStates(IReadOnlyDictionary<string, QuestState> questStates);
    Dictionary<string, QuestState> GetQuestStates();
}