using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EchoForest.Core;

/// <summary>Pure-C# item definition catalog backed by one JSON file.</summary>
public sealed class ItemDatabase
{
    public const string DefaultFilePath = "res://src/Assets/Data/Items/items.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly IFileSystem _fileSystem;
    private readonly string _filePath;
    private Dictionary<string, ItemDefinition> _itemsById = new(StringComparer.Ordinal);
    private bool _isLoaded;

    public ItemDatabase(IFileSystem fileSystem, string filePath = DefaultFilePath)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _filePath = string.IsNullOrWhiteSpace(filePath)
            ? throw new ArgumentException("File path cannot be null, empty, or whitespace.", nameof(filePath))
            : filePath;
    }

    public ItemDefinition GetItem(string itemId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(itemId);
        EnsureLoaded();

        return _itemsById.TryGetValue(itemId, out var item)
            ? item
            : throw new KeyNotFoundException($"Item '{itemId}' was not found in the item database.");
    }

    public List<ItemDefinition> GetAllItems()
    {
        EnsureLoaded();
        return _itemsById.Values.OrderBy(item => item.Id, StringComparer.Ordinal).ToList();
    }

    private void EnsureLoaded()
    {
        if (_isLoaded)
            return;

        string json;
        try
        {
            json = _fileSystem.ReadText(_filePath);
        }
        catch (Exception exception)
        {
            throw new ItemLoadException($"Failed to read item catalog '{_filePath}'.", exception);
        }

        ItemDefinition?[] definitions;
        try
        {
            definitions = JsonSerializer.Deserialize<ItemDefinition?[]>(json, JsonOptions)
                ?? throw new ItemLoadException($"Item catalog '{_filePath}' deserialized to null.");
        }
        catch (JsonException exception)
        {
            throw new ItemLoadException($"Item catalog '{_filePath}' contains invalid JSON.", exception);
        }

        if (definitions.Length == 0)
            throw new ItemLoadException($"Item catalog '{_filePath}' contains no definitions.");

        var itemsById = new Dictionary<string, ItemDefinition>(StringComparer.Ordinal);
        foreach (var item in definitions)
        {
            if (item is null)
                throw new ItemLoadException($"Item catalog '{_filePath}' contains a null definition.");

            ValidateItem(item);
            if (!itemsById.TryAdd(item.Id, item))
                throw new ItemLoadException($"Duplicate item ID '{item.Id}' in '{_filePath}'.");
        }

        _itemsById = itemsById;
        _isLoaded = true;
    }

    private static void ValidateItem(ItemDefinition item)
    {
        if (string.IsNullOrWhiteSpace(item.Id)
            || string.IsNullOrWhiteSpace(item.DisplayName)
            || string.IsNullOrWhiteSpace(item.Description))
        {
            throw new ItemLoadException("Item definition is missing an ID, display name, or description.");
        }

        if (!Enum.IsDefined(item.ItemType))
            throw new ItemLoadException($"Item '{item.Id}' has an unknown item type.");

        if (item.MaxStackSize <= 0)
            throw new ItemLoadException($"Item '{item.Id}' has an invalid maximum stack size.");
    }
}