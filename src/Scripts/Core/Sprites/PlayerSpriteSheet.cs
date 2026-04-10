using System;
using System.Linq;

namespace EchoForest.Core.Sprites;

/// <summary>
/// Static definition of the player character sprite sheet.<br/>
/// Layout: <b>40 frames</b> across a single PNG using 10 columns × 4 rows.
/// Each frame is <b>16 × 24 px</b> (displayed 2× scaled in-game).<br/>
/// Sheet dimensions: <b>640 × 96 px</b>.
///
/// Frame sequence:
/// <list type="table">
///   <item><term>0–7</term>   <description>Idle: down (0–1), up (2–3), left (4–5), right (6–7) — 2 frames each</description></item>
///   <item><term>8–23</term>  <description>Walk: down (8–11), up (12–15), left (16–19), right (20–23) — 4 frames each</description></item>
///   <item><term>24–39</term> <description>Run : down (24–27), up (28–31), left (32–35), right (36–39) — 4 frames each</description></item>
/// </list>
/// </summary>
public static class PlayerSpriteSheet
{
    /// <summary>Godot resource path to the sprite sheet PNG.</summary>
    public const string Path = "res://src/Assets/Sprites/Characters/player_spritesheet.png";

    // ── Per-frame dimensions ──────────────────────────────────────────────────

    /// <summary>Width of a single frame in pixels.</summary>
    public const int FrameWidth = 16;

    /// <summary>Height of a single frame in pixels.</summary>
    public const int FrameHeight = 24;

    // ── Sheet layout ──────────────────────────────────────────────────────────

    /// <summary>Number of columns in the sprite sheet.</summary>
    public const int Columns = 40;

    /// <summary>Number of rows in the sprite sheet.</summary>
    public const int Rows = 4;

    /// <summary>Total number of animation frames.</summary>
    public const int TotalFrames = 40;

    /// <summary>Full pixel width of the sprite sheet PNG (Columns × FrameWidth).</summary>
    public const int SheetWidth = Columns * FrameWidth;   // 640

    /// <summary>Full pixel height of the sprite sheet PNG (Rows × FrameHeight).</summary>
    public const int SheetHeight = Rows * FrameHeight;    // 96

    // ── Playback speeds ───────────────────────────────────────────────────────

    private const float IdleFps = 4f;
    private const float WalkFps = 8f;
    private const float RunFps = 12f;

    // ── 12 animation clips ────────────────────────────────────────────────────

    public static readonly AnimationClipDefinition IdleDown = new(AnimationNames.IdleDown, 0, 2, IdleFps);
    public static readonly AnimationClipDefinition IdleUp = new(AnimationNames.IdleUp, 2, 2, IdleFps);
    public static readonly AnimationClipDefinition IdleLeft = new(AnimationNames.IdleLeft, 4, 2, IdleFps);
    public static readonly AnimationClipDefinition IdleRight = new(AnimationNames.IdleRight, 6, 2, IdleFps);

    public static readonly AnimationClipDefinition WalkDown = new(AnimationNames.WalkDown, 8, 4, WalkFps);
    public static readonly AnimationClipDefinition WalkUp = new(AnimationNames.WalkUp, 12, 4, WalkFps);
    public static readonly AnimationClipDefinition WalkLeft = new(AnimationNames.WalkLeft, 16, 4, WalkFps);
    public static readonly AnimationClipDefinition WalkRight = new(AnimationNames.WalkRight, 20, 4, WalkFps);

    public static readonly AnimationClipDefinition RunDown = new(AnimationNames.RunDown, 24, 4, RunFps);
    public static readonly AnimationClipDefinition RunUp = new(AnimationNames.RunUp, 28, 4, RunFps);
    public static readonly AnimationClipDefinition RunLeft = new(AnimationNames.RunLeft, 32, 4, RunFps);
    public static readonly AnimationClipDefinition RunRight = new(AnimationNames.RunRight, 36, 4, RunFps);

    // ── Clips collection ─────────────────────────────────────────────────────────

    /// <summary>All 12 animation clips in sequence order.</summary>
    public static readonly AnimationClipDefinition[] Clips =
    {
        IdleDown, IdleUp, IdleLeft, IdleRight,
        WalkDown, WalkUp, WalkLeft, WalkRight,
        RunDown,  RunUp,  RunLeft,  RunRight,
    };

    // ── Lookup ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the <see cref="AnimationClipDefinition"/> for the given
    /// <paramref name="clipName"/>, or <c>null</c> if not found.
    /// </summary>
    public static AnimationClipDefinition? FindClip(string clipName) =>
        Clips.FirstOrDefault(c => c.Name == clipName);
}
