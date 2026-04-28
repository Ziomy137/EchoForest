namespace EchoForest.Core;

/// <summary>
/// Contract for the full Game HUD controller.
/// Tracks health, quest objective, interaction prompt, active weapon
/// and minimap state — all without the Godot runtime.
/// </summary>
public interface IGameHudController
{
    // ── Observable state ──────────────────────────────────────────────────────

    /// <summary>Health fill ratio in [0, 1]. 1 = full health, 0 = dead.</summary>
    float HealthFillRatio { get; }

    /// <summary>Raw current health value from the last <see cref="UpdateHealth"/> call.</summary>
    float CurrentHealth { get; }

    /// <summary>Raw max health value from the last <see cref="UpdateHealth"/> call.</summary>
    float MaxHealth { get; }

    /// <summary>Whether the interaction prompt label should be visible.</summary>
    bool IsInteractionPromptVisible { get; }

    /// <summary>Text shown in the interaction prompt (e.g. "Talk", "Open").</summary>
    string InteractionPromptText { get; }

    /// <summary>Name of the active quest shown in the HUD.</summary>
    string CurrentQuestName { get; }

    /// <summary>Current objective description text.</summary>
    string CurrentObjectiveText { get; }

    /// <summary>Formatted progress string, e.g. "1/3".</summary>
    string ObjectiveProgress { get; }

    /// <summary>Item ID of the currently equipped weapon, or empty string if none.</summary>
    string ActiveWeaponId { get; }

    // ── Mutations ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Updates the health bar. <paramref name="current"/> is clamped to [0, <paramref name="max"/>].
    /// </summary>
    /// <exception cref="System.ArgumentException">Thrown when <paramref name="max"/> is ≤ 0.</exception>
    void UpdateHealth(float current, float max);

    /// <summary>Records the currently equipped weapon for the equipment display.</summary>
    void SetActiveWeapon(string? weaponId);

    /// <summary>
    /// Updates the quest objective panel. <paramref name="questName"/> and
    /// <paramref name="objectiveText"/> may be <see langword="null"/> and are
    /// treated as empty strings for HUD display/storage.
    /// </summary>
    void SetQuestObjective(string? questName, string? objectiveText, int current, int total);

    /// <summary>
    /// Shows the interaction prompt with the given action label.
    /// <paramref name="action"/> may be <see langword="null"/>, in which case it is treated as an empty string.
    /// </summary>
    void ShowInteractionPrompt(string? action);

    /// <summary>Hides the interaction prompt and clears its text.</summary>
    void HideInteractionPrompt();

    /// <summary>
    /// Records the player's minimap position and current area identifier.
    /// <paramref name="areaId"/> may be <see langword="null"/> and is treated as an empty string.
    /// </summary>
    void UpdateMinimap(float playerX, float playerY, string? areaId);
}
