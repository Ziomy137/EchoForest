using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>CanvasLayer</c> node for the full Game HUD.
///
/// Reads state from <see cref="GameHudController"/> during the initial
/// <see cref="_Ready"/> refresh and when explicit update methods are called,
/// then updates scene-tree child nodes: health bar, health label, quest
/// objective panel, interaction prompt, active weapon slot, and minimap
/// placeholder.
///
/// All HUD logic lives in the pure-C# <see cref="GameHudController"/> so it
/// can be unit-tested independently of the Godot runtime.
///
/// Excluded from NUnit code coverage — requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot CanvasLayer wrapper — requires scene tree")]
public partial class GameHudNode : CanvasLayer
{
    private GameHudController _ctrl = null!;

    // Cached node references resolved once in _Ready.
    private ProgressBar _healthBar = null!;
    private Label _healthLabel = null!;
    private Label _questNameLabel = null!;
    private Label _questObjectiveLabel = null!;
    private Label _questProgressLabel = null!;
    private Label _interactionPromptLabel = null!;
    private Label _weaponLabel = null!;

    public override void _Ready()
    {
        _ctrl = new GameHudController();

        _healthBar = GetNode<ProgressBar>("TopLeft/HealthBar");
        _healthLabel = GetNode<Label>("TopLeft/HealthLabel");
        _questNameLabel = GetNode<Label>("TopLeft/QuestPanel/QuestVBox/QuestNameLabel");
        _questObjectiveLabel = GetNode<Label>("TopLeft/QuestPanel/QuestVBox/QuestObjectiveLabel");
        _questProgressLabel = GetNode<Label>("TopLeft/QuestPanel/QuestVBox/QuestProgressLabel");
        _interactionPromptLabel = GetNode<Label>("BottomCenter/InteractionPromptLabel");
        _weaponLabel = GetNode<Label>("TopRight/WeaponLabel");

        RefreshHud();
    }

    // ── Public API — called by other Godot nodes ──────────────────────────────

    /// <summary>
    /// Updates health display. Call this from <c>PlayerControllerNode</c>
    /// whenever the player takes damage or heals.
    /// </summary>
    public void UpdateHealth(float current, float max)
    {
        _ctrl.UpdateHealth(current, max);
        RefreshHealthBar();
    }

    /// <summary>Updates the active weapon slot label.</summary>
    public void SetActiveWeapon(string? weaponId)
    {
        _ctrl.SetActiveWeapon(weaponId);
        RefreshWeapon();
    }

    /// <summary>Updates the quest objective panel.</summary>
    public void SetQuestObjective(string? questName, string? objectiveText, int current, int total)
    {
        _ctrl.SetQuestObjective(questName, objectiveText, current, total);
        RefreshQuestPanel();
    }

    /// <summary>Shows the context-sensitive interaction prompt.</summary>
    public void ShowInteractionPrompt(string? action)
    {
        _ctrl.ShowInteractionPrompt(action);
        RefreshInteractionPrompt();
    }

    /// <summary>Hides the interaction prompt.</summary>
    public void HideInteractionPrompt()
    {
        _ctrl.HideInteractionPrompt();
        RefreshInteractionPrompt();
    }

    /// <summary>Updates the minimap player dot position.</summary>
    public void UpdateMinimap(float playerX, float playerY, string? areaId)
    {
        _ctrl.UpdateMinimap(playerX, playerY, areaId);
        // Minimap rendering is placeholder — future sprint will add minimap nodes.
    }

    // ── Private refresh helpers ───────────────────────────────────────────────

    private void RefreshHud()
    {
        RefreshHealthBar();
        RefreshQuestPanel();
        RefreshInteractionPrompt();
        RefreshWeapon();
    }

    private void RefreshHealthBar()
    {
        _healthBar.Value = _ctrl.HealthFillRatio * _healthBar.MaxValue;
        _healthLabel.Text = $"{(int)_ctrl.CurrentHealth}/{(int)_ctrl.MaxHealth}";
    }

    private void RefreshQuestPanel()
    {
        _questNameLabel.Text = _ctrl.CurrentQuestName;
        _questObjectiveLabel.Text = _ctrl.CurrentObjectiveText;
        _questProgressLabel.Text = _ctrl.ObjectiveProgress;
    }

    private void RefreshInteractionPrompt()
    {
        _interactionPromptLabel.Visible = _ctrl.IsInteractionPromptVisible;
        _interactionPromptLabel.Text = _ctrl.IsInteractionPromptVisible
            ? $"[E] {_ctrl.InteractionPromptText}"
            : string.Empty;
    }

    private void RefreshWeapon()
    {
        _weaponLabel.Text = string.IsNullOrEmpty(_ctrl.ActiveWeaponId)
            ? "—"
            : _ctrl.ActiveWeaponId;
    }

    // ── Pause input ───────────────────────────────────────────────────────────

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("pause") && !GetTree().Paused)
            GetTree().Root.AddChild(
                GD.Load<PackedScene>(MainMenuConfig.PauseMenuScenePath).Instantiate());
    }
}
