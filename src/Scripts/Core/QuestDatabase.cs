using System;
using System.Collections.Generic;
using System.Text.Json;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# quest definition database backed by one JSON file per quest.
/// </summary>
public sealed class QuestDatabase : IQuestDatabase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly IFileSystem _fileSystem;
    private readonly string _basePath;
    private readonly Dictionary<string, QuestData> _questsById = new(StringComparer.Ordinal);
    private bool _isLoaded;

    public QuestDatabase(IFileSystem fileSystem, string basePath = "res://src/Assets/Data/Quests")
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _basePath = NormalizeBasePath(basePath);
    }

    /// <inheritdoc/>
    public QuestData GetQuest(string questId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(questId);
        EnsureLoaded();

        return _questsById.TryGetValue(questId, out var quest)
            ? quest
            : throw new KeyNotFoundException($"Quest '{questId}' was not found in the quest database.");
    }

    /// <inheritdoc/>
    public List<QuestData> GetAllQuests()
    {
        EnsureLoaded();
        return [.. _questsById.Values];
    }

    private void EnsureLoaded()
    {
        if (_isLoaded)
            return;

        var paths = _fileSystem.ListFiles(_basePath);
        paths.Sort(StringComparer.Ordinal);
        foreach (var path in paths)
        {
            if (!path.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                continue;

            var quest = LoadQuest(path);
            if (!_questsById.TryAdd(quest.Id, quest))
                throw new QuestLoadException($"Duplicate quest ID '{quest.Id}' in '{path}'.");
        }

        _isLoaded = true;
    }

    private QuestData LoadQuest(string path)
    {
        try
        {
            var quest = JsonSerializer.Deserialize<QuestData>(_fileSystem.ReadText(path), JsonOptions)
                ?? throw new QuestLoadException($"Quest file '{path}' deserialized to null.");
            ValidateQuest(path, quest);
            return quest;
        }
        catch (JsonException exception)
        {
            throw new QuestLoadException($"Quest file '{path}' contains invalid JSON.", exception);
        }
        catch (Exception exception) when (exception is not QuestLoadException)
        {
            throw new QuestLoadException($"Failed to read quest file '{path}'.", exception);
        }
    }

    private static string NormalizeBasePath(string basePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(basePath);
        return basePath.TrimEnd('/');
    }

    private static void ValidateQuest(string path, QuestData quest)
    {
        if (string.IsNullOrWhiteSpace(quest.Id) || string.IsNullOrWhiteSpace(quest.Title))
            throw new QuestLoadException($"Quest file '{path}' is missing an ID or title.");

        if (quest.Objectives.Count == 0)
            throw new QuestLoadException($"Quest '{quest.Id}' contains no objectives.");

        var objectiveIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var objective in quest.Objectives)
        {
            if (string.IsNullOrWhiteSpace(objective.Id) || string.IsNullOrWhiteSpace(objective.Text))
                throw new QuestLoadException($"Quest '{quest.Id}' contains an incomplete objective.");

            if (!objectiveIds.Add(objective.Id))
                throw new QuestLoadException($"Quest '{quest.Id}' contains duplicate objective ID '{objective.Id}'.");
        }
    }
}