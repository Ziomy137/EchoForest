using System;
using System.Collections.Generic;
using System.Text.Json;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# JSON dialogue loader. One JSON file is stored per NPC under the
/// configured dialogue data directory.
/// </summary>
public sealed class DialogueService : IDialogueService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly IFileSystem _fileSystem;
    private readonly string _basePath;
    private readonly Dictionary<string, DialogueLine> _linesById = new(StringComparer.Ordinal);

    public DialogueService(IFileSystem fileSystem, string basePath = "res://src/Assets/Data/dialogues")
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _basePath = NormalizeBasePath(basePath);
    }

    /// <inheritdoc/>
    public DialogueTree LoadDialogue(string npcId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(npcId);
        var path = $"{_basePath}/{npcId}.json";
        if (!_fileSystem.Exists(path))
            throw new DialogueLoadException($"Dialogue file for NPC '{npcId}' does not exist at {path}.");

        DialogueTree dialogue;
        try
        {
            dialogue = JsonSerializer.Deserialize<DialogueTree>(_fileSystem.ReadText(path), JsonOptions)
                ?? throw new DialogueLoadException($"Dialogue file for NPC '{npcId}' deserialized to null.");
        }
        catch (JsonException exception)
        {
            throw new DialogueLoadException($"Dialogue file for NPC '{npcId}' contains invalid JSON.", exception);
        }
        catch (Exception exception) when (exception is not DialogueLoadException)
        {
            throw new DialogueLoadException($"Failed to read dialogue file for NPC '{npcId}'.", exception);
        }

        ValidateDialogue(npcId, dialogue);
        _linesById.Clear();
        foreach (var line in dialogue.Lines)
            _linesById.Add(line.Id, line);

        return dialogue;
    }

    /// <inheritdoc/>
    public DialogueLine GetLine(string lineId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(lineId);
        return _linesById.TryGetValue(lineId, out var line)
            ? line
            : throw new KeyNotFoundException($"Dialogue line '{lineId}' was not found in the active dialogue tree.");
    }

    /// <inheritdoc/>
    public DialogueLine? GetNextLine(string currentLineId)
    {
        var currentLine = GetLine(currentLineId);
        return string.IsNullOrEmpty(currentLine.NextLineId) ? null : GetLine(currentLine.NextLineId);
    }

    private static string NormalizeBasePath(string basePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(basePath);
        return basePath.TrimEnd('/');
    }

    private static void ValidateDialogue(string requestedNpcId, DialogueTree dialogue)
    {
        if (!string.Equals(dialogue.NpcId, requestedNpcId, StringComparison.Ordinal))
            throw new DialogueLoadException($"Dialogue file for NPC '{requestedNpcId}' declares NPC '{dialogue.NpcId}'.");

        if (dialogue.Lines.Count == 0)
            throw new DialogueLoadException($"Dialogue file for NPC '{requestedNpcId}' contains no lines.");

        var knownLineIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var line in dialogue.Lines)
        {
            if (string.IsNullOrWhiteSpace(line.Id) || string.IsNullOrWhiteSpace(line.Speaker) || string.IsNullOrWhiteSpace(line.Text))
                throw new DialogueLoadException($"Dialogue file for NPC '{requestedNpcId}' contains an incomplete line.");

            if (!knownLineIds.Add(line.Id))
                throw new DialogueLoadException($"Dialogue file for NPC '{requestedNpcId}' contains duplicate line ID '{line.Id}'.");
        }

        foreach (var line in dialogue.Lines)
        {
            if (!string.IsNullOrEmpty(line.NextLineId) && !knownLineIds.Contains(line.NextLineId))
                throw new DialogueLoadException($"Dialogue line '{line.Id}' references missing line '{line.NextLineId}'.");
        }
    }
}