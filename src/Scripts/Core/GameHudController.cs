using System;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# Full Game HUD controller. Tracks health bar fill, quest objective
/// text, interaction prompt visibility, active weapon, and minimap position
/// without depending on the Godot runtime.
///
/// The companion Godot node (<see cref="GameHudNode"/>) reads these properties
/// every frame and updates scene-tree nodes accordingly.
///
/// Fully testable with NUnit.
/// </summary>
public sealed class GameHudController : IGameHudController
{
    // ── Health ────────────────────────────────────────────────────────────────

    /// <inheritdoc/>
    public float HealthFillRatio { get; private set; }

    /// <inheritdoc/>
    public float CurrentHealth { get; private set; }

    /// <inheritdoc/>
    public float MaxHealth { get; private set; }

    // ── Interaction prompt ────────────────────────────────────────────────────

    /// <inheritdoc/>
    public bool IsInteractionPromptVisible { get; private set; }

    /// <inheritdoc/>
    public string InteractionPromptText { get; private set; } = string.Empty;

    // ── Quest objective ───────────────────────────────────────────────────────

    /// <inheritdoc/>
    public string CurrentQuestName { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public string CurrentObjectiveText { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public string ObjectiveProgress { get; private set; } = string.Empty;

    // ── Equipment ─────────────────────────────────────────────────────────────

    /// <inheritdoc/>
    public string ActiveWeaponId { get; private set; } = string.Empty;

    // ── Minimap ───────────────────────────────────────────────────────────────

    /// <summary>Player X position on the minimap (world units).</summary>
    public float MinimapPlayerX { get; private set; }

    /// <summary>Player Y position on the minimap (world units).</summary>
    public float MinimapPlayerY { get; private set; }

    /// <summary>Area identifier shown on the minimap.</summary>
    public string MinimapAreaId { get; private set; } = string.Empty;

    // ── Mutations ─────────────────────────────────────────────────────────────

    /// <inheritdoc/>
    /// <exception cref="ArgumentException">Thrown when <paramref name="max"/> ≤ 0.</exception>
    public void UpdateHealth(float current, float max)
    {
        if (max <= 0f)
            throw new ArgumentException("max health must be greater than zero.", nameof(max));

        float clampedCurrent = Math.Clamp(current, 0f, max);

        CurrentHealth = clampedCurrent;
        MaxHealth = max;
        HealthFillRatio = clampedCurrent / max;
    }

    /// <inheritdoc/>
    public void SetActiveWeapon(string? weaponId) =>
        ActiveWeaponId = weaponId ?? string.Empty;

    /// <inheritdoc/>
    public void SetQuestObjective(string questName, string objectiveText, int current, int total)
    {
        CurrentQuestName = questName ?? string.Empty;
        CurrentObjectiveText = objectiveText ?? string.Empty;
        ObjectiveProgress = $"{current}/{total}";
    }

    /// <inheritdoc/>
    public void ShowInteractionPrompt(string action)
    {
        IsInteractionPromptVisible = true;
        InteractionPromptText = action ?? string.Empty;
    }

    /// <inheritdoc/>
    public void HideInteractionPrompt()
    {
        IsInteractionPromptVisible = false;
        InteractionPromptText = string.Empty;
    }

    /// <inheritdoc/>
    public void UpdateMinimap(float playerX, float playerY, string areaId)
    {
        MinimapPlayerX = playerX;
        MinimapPlayerY = playerY;
        MinimapAreaId = areaId ?? string.Empty;
    }
}
