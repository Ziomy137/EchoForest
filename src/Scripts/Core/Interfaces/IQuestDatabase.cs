using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>Provides immutable quest definitions loaded from game data.</summary>
public interface IQuestDatabase
{
    QuestData GetQuest(string questId);
    List<QuestData> GetAllQuests();
}