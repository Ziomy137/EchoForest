using System.Text.Json.Serialization;

namespace EchoForest.Core;

/// <summary>Serializable reward granted when a quest is completed.</summary>
public sealed class QuestReward
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}