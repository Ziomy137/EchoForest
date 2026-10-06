using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>CanvasLayer</c> node for the full Game HUD.
///
/// Reads state from <see cref="GameHudController"/> during the initial
/// <see cref="_Ready"/> refresh and when explicit update methods are called,
/// then updates scene-tree child nodes: health bar, quest panel, interaction
/// prompt, active weapon slot, and area minimap.
///
/// All HUD logic lives in the pure-C# <see cref="GameHudController"/> so it
/// can be unit-tested independently of the Godot runtime.
///
/// Excluded from NUnit code coverage — requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot CanvasLayer wrapper — requires scene tree")]
public partial class GameHudNode : CanvasLayer, IGameHudController
{
    private GameHudController _ctrl = null!;
    private IEventBus _eventBus = null!;
    private Tween? _damageFlashTween;
    private Tween? _questPanelTween;
    private float _lastKnownHealth;
    private MinimapController _minimap = null!;
    private string _lastRenderedMinimapArea = string.Empty;

    // Cached node references resolved once in _Ready.
    private ProgressBar _healthBar = null!;
    private PanelContainer _questPanel = null!;
    private Label _healthLabel = null!;
    private Label _questNameLabel = null!;
    private Label _questObjectiveLabel = null!;
    private Label _questProgressLabel = null!;
    private Label _interactionPromptLabel = null!;
    private Label _weaponLabel = null!;
    private Control _minimapContainer = null!;
    private TextureRect _minimapTexture = null!;
    private Label _minimapAreaLabel = null!;
    private ColorRect _minimapPlayerDot = null!;

    public float HealthFillRatio => _ctrl.HealthFillRatio;
    public float CurrentHealth => _ctrl.CurrentHealth;
    public float MaxHealth => _ctrl.MaxHealth;
    public bool IsInteractionPromptVisible => _ctrl.IsInteractionPromptVisible;
    public string InteractionPromptText => _ctrl.InteractionPromptText;
    public string CurrentQuestName => _ctrl.CurrentQuestName;
    public string CurrentObjectiveText => _ctrl.CurrentObjectiveText;
    public string ObjectiveProgress => _ctrl.ObjectiveProgress;
    public string ActiveWeaponId => _ctrl.ActiveWeaponId;

    public override void _Ready()
    {
        var currentScene = GetTree().CurrentScene;
        _eventBus = (currentScene as IAreaSceneContext)?.EventBus ?? new EventBus();
        _eventBus.Subscribe<PlayerHealthChangedEvent>(OnPlayerHealthChanged);
        _ctrl = new GameHudController(_eventBus);
        _minimap = new MinimapController(_eventBus, currentScene?.SceneFilePath ?? string.Empty);

        _healthBar = GetNode<ProgressBar>("TopLeft/HealthBar");
        _questPanel = GetNode<PanelContainer>("TopLeft/QuestPanel");
        _healthLabel = GetNode<Label>("TopLeft/HealthLabel");
        _questNameLabel = GetNode<Label>("TopLeft/QuestPanel/QuestVBox/QuestNameLabel");
        _questObjectiveLabel = GetNode<Label>("TopLeft/QuestPanel/QuestVBox/QuestObjectiveLabel");
        _questProgressLabel = GetNode<Label>("TopLeft/QuestPanel/QuestVBox/QuestProgressLabel");
        _interactionPromptLabel = GetNode<Label>("BottomCenter/InteractionPromptLabel");
        _weaponLabel = GetNode<Label>("TopRight/WeaponLabel");
        _minimapContainer = GetNode<Control>("MinimapContainer");
        _minimapTexture = GetNode<TextureRect>("MinimapContainer/MinimapPanel/MinimapTexture");
        _minimapAreaLabel = GetNode<Label>("MinimapContainer/AreaNameLabel");
        _minimapPlayerDot = GetNode<ColorRect>("MinimapContainer/MinimapPanel/PlayerDot");

        RefreshHud();
        RefreshMinimapArea();
        _lastKnownHealth = _ctrl.CurrentHealth;
    }

    public override void _Process(double delta)
    {
        if (_minimap is null)
            return;

        var player = GetTree().CurrentScene?.GetNodeOrNull<Node2D>("Player");
        if (player is not null)
        {
            _minimap.UpdatePlayerPosition(player.GlobalPosition.X, player.GlobalPosition.Y);
            _ctrl.UpdateMinimap(player.GlobalPosition.X, player.GlobalPosition.Y, _minimap.CurrentAreaId);
            RefreshMinimapPlayerDot();
        }

        RefreshMinimapArea();
    }

    public override void _Input(InputEvent @event)
    {
        if (_minimap is null || !@event.IsActionPressed(InputActionNames.ToggleMinimap))
            return;

        _minimap.ToggleVisibility();
        _minimapContainer.Visible = _minimap.IsVisible;
        GetViewport().SetInputAsHandled();
    }

    // ── Public API — called by other Godot nodes ──────────────────────────────

    /// <summary>
    /// Updates health display. Call this from <c>PlayerControllerNode</c>
    /// whenever the player takes damage or heals.
    /// </summary>
    public void UpdateHealth(float current, float max)
    {
        _ctrl.UpdateHealth(current, max);
    }

    public override void _ExitTree()
    {
        _eventBus?.Unsubscribe<PlayerHealthChangedEvent>(OnPlayerHealthChanged);
        _questPanelTween?.Kill();
        _minimap?.Dispose();
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
        PlayQuestPanelFade();
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

    private void OnPlayerHealthChanged(PlayerHealthChangedEvent healthEvent)
    {
        var tookDamage = healthEvent.NewHealth < _lastKnownHealth;
        _lastKnownHealth = healthEvent.NewHealth;
        RefreshHealthBar();

        if (tookDamage)
            PlayDamageFlash();
    }

    private void PlayDamageFlash()
    {
        _damageFlashTween?.Kill();
        _healthBar.Modulate = new Color(1f, 0.35f, 0.35f, 1f);
        _damageFlashTween = CreateTween();
        _damageFlashTween.TweenProperty(_healthBar, "modulate", Colors.White, 0.2d);
    }

    private void PlayQuestPanelFade()
    {
        _questPanelTween?.Kill();
        _questPanel.Modulate = new Color(1f, 1f, 1f, 0f);
        _questPanelTween = CreateTween();
        _questPanelTween.TweenProperty(_questPanel, "modulate", Colors.White, 0.2d);
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

    private void RefreshMinimapArea()
    {
        if (_lastRenderedMinimapArea == _minimap.CurrentAreaId)
            return;

        _lastRenderedMinimapArea = _minimap.CurrentAreaId;
        _minimapAreaLabel.Text = _minimap.CurrentAreaName;
        _minimapTexture.Texture = GD.Load<Texture2D>(_minimap.CurrentMapTexturePath);
        RefreshMinimapPlayerDot();
    }

    private void RefreshMinimapPlayerDot()
    {
        var point = _minimap.PlayerDotPosition;
        var mapSize = _minimapTexture.Size;
        const float dotSize = 5f;
        _minimapPlayerDot.Position = new Vector2(
            4f + point.X / (MinimapController.MapWidth - 1f) * mapSize.X - dotSize / 2f,
            4f + point.Y / (MinimapController.MapHeight - 1f) * mapSize.Y - dotSize / 2f);
    }

    // Pause input is handled by CottageAreaNode (root Node2D) — CanvasLayer
    // nodes do not reliably receive _UnhandledInput in Godot 4.
}
