namespace EchoForest.Core.Sprites;

/// <summary>
/// Describes a single animation clip within a sprite sheet.
/// Used by <see cref="PlayerSpriteSheet"/> to map clip names to frame ranges
/// and by <see cref="AnimationNames"/> constants to verify clip availability.
/// </summary>
/// <param name="Name">
/// Clip name matching <see cref="AnimationNames"/> constants,
/// e.g. "idle_down", "walk_up", "run_left".
/// </param>
/// <param name="StartFrame">Zero-based index of the first frame in the sheet.</param>
/// <param name="FrameCount">Number of frames in this clip.</param>
/// <param name="Fps">Playback speed in frames-per-second.</param>
public sealed record AnimationClipDefinition(
    string Name,
    int StartFrame,
    int FrameCount,
    float Fps);
