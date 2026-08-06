using System;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot UI adapter that renders <see cref="DialogueController"/> state and
/// reveals each line with a typewriter effect.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot CanvasLayer wrapper — requires scene tree")]
public partial class DialogueBoxNode : CanvasLayer
{
    [Export]
    public float CharactersPerSecond { get; set; } = 48f;

    private DialogueController _controller = null!;
    private Label _speakerLabel = null!;
    private Label _dialogueLabel = null!;
    private Label _continueLabel = null!;
    private string _currentText = string.Empty;
    private float _revealedCharacters;

    /// <summary>Whether a dialogue currently owns the interact input.</summary>
    public bool IsConversationActive => _controller.IsConversationActive;

    public override void _Ready()
    {
        _speakerLabel = GetNode<Label>("Margin/Panel/VBox/SpeakerLabel");
        _dialogueLabel = GetNode<Label>("Margin/Panel/VBox/DialogueLabel");
        _continueLabel = GetNode<Label>("Margin/Panel/VBox/ContinueLabel");

        var saveService = new SaveService(new GodotFileSystem());
        _controller = new DialogueController(
            new DialogueService(new GodotFileSystem()),
            saveService,
            CreateSaveData);
        _controller.OnConversationStarted += ShowDialogue;
        _controller.OnLineChanged += DisplayLine;
        _controller.OnConversationEnded += HideDialogue;

        Visible = false;
    }

    public override void _Process(double delta)
    {
        if (!IsConversationActive || IsLineFullyRevealed)
            return;

        _revealedCharacters = Math.Min(_currentText.Length, _revealedCharacters + CharactersPerSecond * (float)delta);
        _dialogueLabel.VisibleCharacters = (int)_revealedCharacters;
        UpdateContinuePrompt();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!IsConversationActive || !@event.IsActionPressed(InputActionNames.Interact))
            return;

        if (!IsLineFullyRevealed)
            RevealLineImmediately();
        else
            _controller.Advance();

        GetViewport().SetInputAsHandled();
    }

    /// <summary>Starts the configured dialogue for one NPC.</summary>
    public void StartConversation(string npcId)
    {
        if (!IsConversationActive)
            _controller.StartConversation(npcId);
    }

    /// <summary>Starts a cutscene dialogue from one configured line and signals when it ends.</summary>
    public void PlayCutsceneLine(string npcId, string lineId, Action onComplete)
    {
        ArgumentNullException.ThrowIfNull(onComplete);
        if (IsConversationActive)
            throw new InvalidOperationException("Cannot start a cutscene dialogue while another conversation is active.");

        void Complete()
        {
            _controller.OnConversationEnded -= Complete;
            onComplete();
        }

        _controller.OnConversationEnded += Complete;
        _controller.StartConversationAtLine(npcId, lineId);
    }

    private bool IsLineFullyRevealed => _revealedCharacters >= _currentText.Length;

    private void ShowDialogue()
    {
        Visible = true;
    }

    private void DisplayLine(DialogueLine line)
    {
        _speakerLabel.Text = line.Speaker;
        _currentText = line.Text;
        _dialogueLabel.Text = _currentText;
        _revealedCharacters = 0f;
        _dialogueLabel.VisibleCharacters = 0;
        UpdateContinuePrompt();
    }

    private void HideDialogue()
    {
        Visible = false;
        _currentText = string.Empty;
        _dialogueLabel.Text = string.Empty;
        _continueLabel.Text = string.Empty;
    }

    private void RevealLineImmediately()
    {
        _revealedCharacters = _currentText.Length;
        _dialogueLabel.VisibleCharacters = _currentText.Length;
        UpdateContinuePrompt();
    }

    private void UpdateContinuePrompt()
    {
        _continueLabel.Text = IsLineFullyRevealed ? "[E] Continue" : "[E] Skip";
    }

    private SaveData CreateSaveData()
    {
        var scene = GetTree().CurrentScene;
        var player = scene.GetNodeOrNull<PlayerControllerNode>("Player");

        return new SaveData
        {
            CurrentArea = scene.SceneFilePath,
            PlayerX = player?.GlobalPosition.X ?? 0f,
            PlayerY = player?.GlobalPosition.Y ?? 0f,
        };
    }
}