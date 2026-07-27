using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EchoForest.Core;

/// <summary>Serializable set of ordered dialogue lines for one NPC.</summary>
public sealed class DialogueTree
{
    [JsonPropertyName("npc_id")]
    public string NpcId { get; set; } = string.Empty;

    [JsonPropertyName("is_story_critical")]
    public bool IsStoryCritical { get; set; }

    [JsonPropertyName("lines")]
    public List<DialogueLine> Lines { get; set; } = [];

    /// <summary>Identifier of the line shown when a conversation begins.</summary>
    [JsonIgnore]
    public string FirstLineId => Lines.Count == 0 ? string.Empty : Lines[0].Id;
}