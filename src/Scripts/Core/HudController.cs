namespace EchoForest.Core;

/// <summary>
/// Pure-C# HUD state controller. Tracks tutorial hint visibility and debug
/// label visibility independently of the Godot scene tree.
///
/// Call <see cref="Initialize"/> once when the scene starts. Call
/// <see cref="SimulateTimePassed"/> every process frame (or in tests) to
/// advance the hint timeout. Call <see cref="SetDebugMode"/> to control
/// whether the debug label is shown.
///
/// Pure C# — no Godot runtime required. Testable with NUnit.
/// </summary>
public sealed class HudController
{
    private readonly float _hintTimeoutSeconds;
    private float _elapsed;
    private bool _initialized;

    // ── Public state ──────────────────────────────────────────────────────────

    /// <summary>Whether the tutorial movement hint should currently be shown.</summary>
    public bool IsTutorialHintVisible { get; private set; }

    /// <summary>Whether the player-state debug label should currently be shown.</summary>
    public bool IsDebugLabelVisible { get; private set; }

    /// <summary>Current player state text for the debug label.</summary>
    public string PlayerStateText { get; private set; } = string.Empty;

    // ── Construction ──────────────────────────────────────────────────────────

    /// <param name="hintTimeoutSeconds">
    /// Seconds after which the tutorial hint fades out.
    /// Defaults to <see cref="Constants.TutorialHintTimeout"/> (10 s).
    /// </param>
    public HudController(float hintTimeoutSeconds = Constants.TutorialHintTimeout)
    {
        _hintTimeoutSeconds = hintTimeoutSeconds;
    }

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Call once when the scene loads. Shows the tutorial hint and resets the timer.
    /// </summary>
    public void Initialize()
    {
        _initialized = true;
        _elapsed = 0f;
        IsTutorialHintVisible = true;
    }

    /// <summary>
    /// Advance the hint timeout timer by <paramref name="delta"/> seconds.
    /// Hides the tutorial hint once the timeout elapses.
    /// No-op if <see cref="Initialize"/> has not been called.
    /// </summary>
    public void SimulateTimePassed(float delta)
    {
        if (!_initialized) return;

        _elapsed += delta;
        if (_elapsed >= _hintTimeoutSeconds)
            IsTutorialHintVisible = false;
    }

    // ── Debug mode ────────────────────────────────────────────────────────────

    /// <summary>
    /// Controls whether the debug label (player state) is visible.
    /// Pass <c>true</c> when running inside the Godot editor or a debug build.
    /// </summary>
    public void SetDebugMode(bool enabled)
    {
        IsDebugLabelVisible = enabled;
    }

    // ── Player state text ─────────────────────────────────────────────────────

    /// <summary>Updates the text shown in the debug player-state label.</summary>
    public void UpdatePlayerState(string stateName)
    {
        PlayerStateText = stateName ?? string.Empty;
    }
}
