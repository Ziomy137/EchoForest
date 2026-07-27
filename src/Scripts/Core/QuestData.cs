using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EchoForest.Core;

/// <summary>Serializable static definition of one quest.</summary>
public sealed class QuestData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("objectives")]
    public List<QuestObjective> Objectives { get; set; } = [];

    [JsonPropertyName("rewards")]
    public List<QuestReward> Rewards { get; set; } = [];

    [JsonPropertyName("triggers_quest")]
    public string? TriggersQuestId { get; set; }
}