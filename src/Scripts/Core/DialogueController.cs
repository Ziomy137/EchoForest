using System;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# conversation state machine. It publishes line changes for UI adapters
/// and auto-saves only story-critical dialogues when they conclude.
/// </summary>
public sealed class DialogueController : IDialogueController
{
    private readonly IDialogueService _dialogueService;
    private readonly ISaveDataService? _saveService;
    private readonly Func<SaveData>? _saveDataFactory;
    private readonly int _autoSaveSlot;
    private DialogueTree? _activeDialogue;

    public DialogueController(
        IDialogueService dialogueService,
        ISaveDataService? saveService = null,
        Func<SaveData>? saveDataFactory = null,
        int autoSaveSlot = 1)
    {
        _dialogueService = dialogueService ?? throw new ArgumentNullException(nameof(dialogueService));
        _saveService = saveService;
        _saveDataFactory = saveDataFactory;
        _autoSaveSlot = autoSaveSlot;
    }

    /// <inheritdoc/>
    public bool IsConversationActive { get; private set; }

    /// <inheritdoc/>
    public DialogueLine? CurrentLine { get; private set; }

    /// <inheritdoc/>
    public event Action? OnConversationStarted;

    /// <inheritdoc/>
    public event Action<DialogueLine>? OnLineChanged;

    /// <inheritdoc/>
    public event Action? OnConversationEnded;

    /// <inheritdoc/>
    public void StartConversation(string npcId)
    {
        _activeDialogue = _dialogueService.LoadDialogue(npcId);
        CurrentLine = _dialogueService.GetLine(_activeDialogue.FirstLineId);
        IsConversationActive = true;

        OnConversationStarted?.Invoke();
        OnLineChanged?.Invoke(CurrentLine);
    }

    /// <inheritdoc/>
    public bool Advance()
    {
        if (!IsConversationActive || CurrentLine is null)
            return false;

        var nextLine = _dialogueService.GetNextLine(CurrentLine.Id);
        if (nextLine is null)
        {
            EndConversation();
            return false;
        }

        CurrentLine = nextLine;
        OnLineChanged?.Invoke(nextLine);
        return true;
    }

    /// <inheritdoc/>
    public void EndConversation()
    {
        if (!IsConversationActive)
            return;

        var shouldAutoSave = _activeDialogue?.IsStoryCritical == true;
        IsConversationActive = false;
        CurrentLine = null;

        if (shouldAutoSave && _saveService is not null && _saveDataFactory is not null)
            _saveService.Save(_saveDataFactory(), _autoSaveSlot);

        _activeDialogue = null;
        OnConversationEnded?.Invoke();
    }
}