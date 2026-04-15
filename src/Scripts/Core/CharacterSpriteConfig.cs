using System.Linq;

namespace EchoForest.Core;

/// <summary>
/// Central configuration for the player character sprite sheet and all
/// animation clips defined in S3-02.
///
/// Sprite sheet layout (<c>player_spritesheet.png</c>, 640×96):
/// <list type="bullet">
///   <item>10 columns × 4 rows, 64×24 pixels per frame</item>
///   <item>Row 0 = Down, Row 1 = Left, Row 2 = Right, Row 3 = Up</item>
///   <item>Cols 0-1 = Idle (2 frames), Cols 2-5 = Walk (4 frames), Cols 6-9 = Run (4 frames)</item>
/// </list>
///
/// Pure C# — no Godot runtime required. Testable with NUnit.
/// </summary>
public static class CharacterSpriteConfig
{
    // ─── Sheet geometry ───────────────────────────────────────────────────────

    /// <summary>Width of each individual animation frame in pixels.</summary>
    public const int FrameWidth = 64;

    /// <summary>Height of each individual animation frame in pixels.</summary>
    public const int FrameHeight = 24;

    /// <summary>Total columns in the sprite sheet (2 idle + 4 walk + 4 run).</summary>
    public const int Columns = 10;

    /// <summary>Total rows in the sprite sheet (one per direction).</summary>
    public const int Rows = 4;

    /// <summary>Total number of frames in the sprite sheet.</summary>
    public const int TotalFrames = Columns * Rows;   // 40

    /// <summary>Full width of the sprite sheet in pixels (FrameWidth × Columns).</summary>
    public const int SheetWidth = FrameWidth * Columns;   // 640

    /// <summary>Full height of the sprite sheet in pixels (FrameHeight × Rows).</summary>
    public const int SheetHeight = FrameHeight * Rows;    // 96

    // ─── Directions (row indices) ─────────────────────────────────────────────

    /// <summary>Row index for Down-facing frames.</summary>
    public const int RowDown = 0;
    /// <summary>Row index for Left-facing frames.</summary>
    public const int RowLeft = 1;
    /// <summary>Row index for Right-facing frames.</summary>
    public const int RowRight = 2;
    /// <summary>Row index for Up-facing frames.</summary>
    public const int RowUp = 3;

    // ─── Animation column layout ──────────────────────────────────────────────

    /// <summary>Start column for Idle frames.</summary>
    public const int IdleStartColumn = 0;
    /// <summary>Number of frames in the Idle animation.</summary>
    public const int IdleFrameCount = 2;

    /// <summary>Start column for Walk frames.</summary>
    public const int WalkStartColumn = 2;
    /// <summary>Number of frames in the Walk animation.</summary>
    public const int WalkFrameCount = 4;

    /// <summary>Start column for Run frames.</summary>
    public const int RunStartColumn = 6;
    /// <summary>Number of frames in the Run animation.</summary>
    public const int RunFrameCount = 4;

    // ─── Playback speeds (fps) ────────────────────────────────────────────────

    /// <summary>Playback speed for idle animations (frames per second).</summary>
    public const float IdleFps = 5.0f;
    /// <summary>Playback speed for walk animations (frames per second).</summary>
    public const float WalkFps = 8.0f;
    /// <summary>Playback speed for run animations (frames per second).</summary>
    public const float RunFps = 10.0f;

    // ─── Resource paths ───────────────────────────────────────────────────────

    /// <summary>Godot resource path for the player sprite sheet.</summary>
    public const string SpritesResPath = "res://src/Assets/Sprites/Characters";
    /// <summary>Godot resource path for the player sprite sheet PNG.</summary>
    public const string SheetResPath = $"{SpritesResPath}/player_spritesheet.png";
    /// <summary>Godot resource path for the SpriteFrames animation library.</summary>
    public const string AnimationsResPath = "res://src/Assets/Animations/player_animations.tres";

    // ─── Animation clip definitions ───────────────────────────────────────────

    public static readonly AnimationClipConfig IdleDown = new(AnimationNames.IdleDown, RowDown, IdleStartColumn, IdleFrameCount, IdleFps);
    public static readonly AnimationClipConfig WalkDown = new(AnimationNames.WalkDown, RowDown, WalkStartColumn, WalkFrameCount, WalkFps);
    public static readonly AnimationClipConfig RunDown = new(AnimationNames.RunDown, RowDown, RunStartColumn, RunFrameCount, RunFps);

    public static readonly AnimationClipConfig IdleLeft = new(AnimationNames.IdleLeft, RowLeft, IdleStartColumn, IdleFrameCount, IdleFps);
    public static readonly AnimationClipConfig WalkLeft = new(AnimationNames.WalkLeft, RowLeft, WalkStartColumn, WalkFrameCount, WalkFps);
    public static readonly AnimationClipConfig RunLeft = new(AnimationNames.RunLeft, RowLeft, RunStartColumn, RunFrameCount, RunFps);

    public static readonly AnimationClipConfig IdleRight = new(AnimationNames.IdleRight, RowRight, IdleStartColumn, IdleFrameCount, IdleFps);
    public static readonly AnimationClipConfig WalkRight = new(AnimationNames.WalkRight, RowRight, WalkStartColumn, WalkFrameCount, WalkFps);
    public static readonly AnimationClipConfig RunRight = new(AnimationNames.RunRight, RowRight, RunStartColumn, RunFrameCount, RunFps);

    public static readonly AnimationClipConfig IdleUp = new(AnimationNames.IdleUp, RowUp, IdleStartColumn, IdleFrameCount, IdleFps);
    public static readonly AnimationClipConfig WalkUp = new(AnimationNames.WalkUp, RowUp, WalkStartColumn, WalkFrameCount, WalkFps);
    public static readonly AnimationClipConfig RunUp = new(AnimationNames.RunUp, RowUp, RunStartColumn, RunFrameCount, RunFps);

    // ─── Collection access ────────────────────────────────────────────────────

    /// <summary>Total number of animation clips (4 directions × 3 states).</summary>
    public const int ClipCount = 12;

    private static readonly AnimationClipConfig[] _all =
    {
        IdleDown,  WalkDown,  RunDown,
        IdleLeft,  WalkLeft,  RunLeft,
        IdleRight, WalkRight, RunRight,
        IdleUp,    WalkUp,    RunUp,
    };

    /// <summary>
    /// Returns all 12 animation clip configurations.
    /// A defensive copy is returned so callers cannot mutate the array.
    /// </summary>
    public static AnimationClipConfig[] All => (AnimationClipConfig[])_all.Clone();

    /// <summary>
    /// Looks up a clip by its animation name. Returns null if not found.
    /// </summary>
    public static AnimationClipConfig? GetByName(string name) =>
        _all.FirstOrDefault(c => c.Name == name);
}
