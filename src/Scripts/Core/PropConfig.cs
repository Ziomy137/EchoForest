using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EchoForest.Core;

/// <summary>
/// Immutable configuration for a single environment prop sprite asset.
///
/// Stores metadata that can be validated in pure C# tests without requiring
/// the Godot runtime. Each prop tracks its file path, expected pixel dimensions,
/// and approved palette colors.
///
/// Pure C# — no Godot runtime required. Testable with NUnit.
/// </summary>
public sealed class PropConfig
{
    /// <summary>Display name shown in tools and debug output, e.g. "Cottage Door".</summary>
    public string Name { get; }

    /// <summary>Filename without directory, e.g. "prop_door.png".</summary>
    public string FileName { get; }

    /// <summary>Expected width in pixels.</summary>
    public int Width { get; }

    /// <summary>Expected height in pixels.</summary>
    public int Height { get; }

    /// <summary>
    /// Approved palette hex codes (lowercase, no '#') that this prop is expected to use.
    /// Every opaque pixel must match one of these AND be in <see cref="Palette.All"/>.
    /// </summary>
    public IReadOnlyList<string> ExpectedColorHexCodes { get; }

    /// <summary>Godot resource path relative to the project root.</summary>
    public string ResourcePath => $"{CharacterImportConfig.PropSpritesResPath}/{FileName}";

    /// <summary>Relative filesystem path from the project root.</summary>
    public string FilePath => $"{CharacterImportConfig.PropSpritesDirectory}/{FileName}";

    public PropConfig(
        string name,
        string fileName,
        int width,
        int height,
        IReadOnlyList<string> expectedColorHexCodes)
    {
        Name = name;
        FileName = fileName;
        Width = width;
        Height = height;
        ExpectedColorHexCodes = new ReadOnlyCollection<string>(new List<string>(expectedColorHexCodes));
    }
}
