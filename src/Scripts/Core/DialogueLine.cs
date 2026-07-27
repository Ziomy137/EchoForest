using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EchoForest.Core;

/// <summary>One displayable line in an NPC dialogue tree.</summary>
public sealed class DialogueLine
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("speaker")]
    public string Speaker { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("next")]
    public string? NextLineId { get; set; }

    [JsonPropertyName("options")]
    public List<DialogueOption> Options { get; set; } = [];
}