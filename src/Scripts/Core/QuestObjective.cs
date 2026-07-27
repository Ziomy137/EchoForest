using System.Text.Json.Serialization;

namespace EchoForest.Core;

/// <summary>Serializable objective belonging to a quest definition.</summary>
public sealed class QuestObjective
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("required")]
    public bool Required { get; set; }
}