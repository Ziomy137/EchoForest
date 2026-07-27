using System.Text.Json.Serialization;

namespace EchoForest.Core;

/// <summary>A future player-selectable response branching to another dialogue line.</summary>
public sealed class DialogueOption
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("next")]
    public string? NextLineId { get; set; }
}