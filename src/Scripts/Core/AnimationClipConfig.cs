using System.Collections.Generic;
using System.Linq;

namespace EchoForest.Core;

/// <summary>
/// Immutable configuration for a single sprite animation clip inside
/// <c>player_spritesheet.png</c>.
///
/// Provides the data needed to slice the sprite sheet into
/// individual animation frames without any Godot runtime dependency.
///
/// Pure C# — no Godot runtime required. Testable with NUnit.
/// </summary>
public sealed class AnimationClipConfig
{
    /// <summary>Animation clip name as used in the SpriteFrames resource, e.g. "idle_down".</summary>
    public string Name { get; }

    /// <summary>Zero-based row index in the sprite sheet (0=Down, 1=Left, 2=Right, 3=Up).</summary>
    public int Row { get; }

    /// <summary>Zero-based column index of the first frame in this clip.</summary>
    public int StartColumn { get; }

    /// <summary>Number of frames in this clip.</summary>
    public int FrameCount { get; }

    /// <summary>Playback speed in frames per second.</summary>
    public float Fps { get; }

    private readonly IReadOnlyList<int> _frameColumns;

    public AnimationClipConfig(string name, int row, int startColumn, int frameCount, float fps)
    {
        Name = name;
        Row = row;
        StartColumn = startColumn;
        FrameCount = frameCount;
        Fps = fps;
        _frameColumns = Enumerable.Range(startColumn, frameCount).ToArray();
    }

    /// <summary>
    /// Returns the zero-based absolute column indices (across the full sheet) occupied
    /// by each frame in this clip. Computed once at construction; the same instance is
    /// returned on every access.
    /// </summary>
    public IReadOnlyList<int> FrameColumns => _frameColumns;
}
