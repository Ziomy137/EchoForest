using System.Text.Json.Serialization;

namespace EchoForest.Core;

/// <summary>Serializable static definition of an item.</summary>
public sealed class ItemDefinition
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("item_type")]
    [JsonRequired]
    public ItemType ItemType { get; set; }

    [JsonPropertyName("max_stack_size")]
    public int MaxStackSize { get; set; } = 1;

    [JsonPropertyName("icon_texture_path")]
    public string IconTexturePath { get; set; } = string.Empty;
}