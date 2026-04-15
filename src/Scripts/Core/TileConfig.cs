using System.Collections.Generic;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Immutable configuration for a single tile sprite asset.
///
/// Stores metadata that can be validated in pure C# tests without
/// requiring Godot runtime. Each tile tracks its file path, expected
/// dimensions, approved palette colors, and walkability flag.
///
/// Pure C# — no Godot runtime required. Testable with NUnit.
/// </summary>
public sealed class TileConfig
{
    /// <summary>Display name shown in tools and debug output.</summary>
    public string Name { get; }

    /// <summary>Filename without directory, e.g. "tile_grass.png".</summary>
    public string FileName { get; }

    /// <summary>Expected width in pixels (64 for isometric tiles).</summary>
    public int Width { get; }

    /// <summary>Expected height in pixels (32 for isometric tiles).</summary>
    public int Height { get; }

    /// <summary>Whether the tile should be marked walkable in the TileSet.</summary>
    public bool IsWalkable { get; }

    /// <summary>
    /// Approved palette hex codes (lowercase, no '#') that this tile is expected to use.
    /// Every opaque pixel must match one of these AND be in <see cref="Palette.All"/>.
    /// </summary>
    public IReadOnlyList<string> ExpectedColorHexCodes { get; }

    /// <summary>Godot resource path relative to the project root.</summary>
    public string ResourcePath => $"res://src/Assets/Sprites/Tiles/{FileName}";

    /// <summary>Relative filesystem path from the project root.</summary>
    public string FilePath => $"src/Assets/Sprites/Tiles/{FileName}";

    public TileConfig(
        string name,
        string fileName,
        int width,
        int height,
        bool isWalkable,
        IReadOnlyList<string> expectedColorHexCodes)
    {
        Name = name;
        FileName = fileName;
        Width = width;
        Height = height;
        IsWalkable = isWalkable;
        ExpectedColorHexCodes = expectedColorHexCodes;
    }
}
