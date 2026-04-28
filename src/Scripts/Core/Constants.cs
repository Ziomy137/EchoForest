namespace EchoForest.Core;

/// <summary>
/// Project-wide constants. No magic numbers anywhere else in the codebase —
/// all numeric values must be referenced from here.
/// </summary>
public static class Constants
{
    // ── Movement ─────────────────────────────────────────────────────────────

    /// <summary>Base walk speed in pixels per second.</summary>
    public const float WalkSpeed = 80f;

    /// <summary>Run speed in pixels per second (hold <c>run</c> action).</summary>
    public const float RunSpeed = 160f;

    // ── Isometric Tiles ───────────────────────────────────────────────────────

    /// <summary>Isometric tile width in pixels (diamond horizontal span).</summary>
    public const int TileWidth = 64;

    /// <summary>Isometric tile height in pixels (diamond vertical span).</summary>
    public const int TileHeight = 32;

    // ── Rendering ─────────────────────────────────────────────────────────────

    /// <summary>Target frame rate for the demo build.</summary>
    public const int TargetFps = 60;

    /// <summary>Base pixels-per-unit ratio for sprite rendering.</summary>
    public const int PixelsPerUnit = 1;

    // ── Physics Layers ────────────────────────────────────────────────────────

    /// <summary>
    /// Godot physics layer IDs. Must match the layer names configured in
    /// Project Settings → Physics → 2D → Layer Names.
    /// </summary>
    public static class Layers
    {
        public const int World = 1;
        public const int Player = 2;
        public const int Npcs = 3;
        public const int Interactables = 4;

        public static readonly int[] All = { World, Player, Npcs, Interactables };
    }

    // ── HUD ───────────────────────────────────────────────────────────────────

    /// <summary>Seconds before the tutorial movement hint fades out.</summary>
    public const float TutorialHintTimeout = 10f;

    // ── Camera ────────────────────────────────────────────────────────────────

    /// <summary>Camera lerp speed for smooth follow behaviour.</summary>
    public const float CameraFollowSpeed = 5f;

    // ── Save ──────────────────────────────────────────────────────────────────

    /// <summary>Number of available save slots.</summary>
    public const int SaveSlotCount = 5;

    /// <summary>Default player health at the start of a new game or when loading a save without health data.</summary>
    public const float DefaultPlayerHealth = 100f;

    // ── Credits ───────────────────────────────────────────────────────────────

    /// <summary>Duration in seconds for the credits screen auto-scroll to complete.</summary>
    public const float CreditsScrollDuration = 20f;
}
