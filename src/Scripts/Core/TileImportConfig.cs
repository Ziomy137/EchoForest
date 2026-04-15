namespace EchoForest.Core;

/// <summary>
/// Import configuration constants for tile sprite assets.
///
/// Godot auto-generates a <c>.import</c> sidecar file for every asset the first
/// time the project is opened in the editor. Those files are excluded from
/// version control via <c>*.import</c> in <c>.gitignore</c> (standard practice
/// for Godot projects). This class documents the expected import settings so
/// that developers can verify their locally generated <c>.import</c> files match
/// the intended configuration (nearest-neighbor filtering, no mipmaps, lossless).
///
/// Pure C# — no Godot runtime required. Testable with NUnit.
/// </summary>
public static class TileImportConfig
{
    /// <summary>Godot compress mode 0 = Lossless (preserves pixel-perfect quality).</summary>
    public const int CompressMode = 0;

    /// <summary>Mipmaps disabled for 2D pixel art tiles.</summary>
    public const bool MipmapsEnabled = false;

    /// <summary>No 3D detection — these are strictly 2D assets.</summary>
    public const int Detect3DCompressTo = 0;

    /// <summary>Standard import type for tile sprites.</summary>
    public const string ImporterType = "texture";

    /// <summary>Godot resource type after import.</summary>
    public const string ResourceType = "CompressedTexture2D";

    /// <summary>Base directory for tile sprite assets (relative to project root).</summary>
    public const string TileSpritesDirectory = "src/Assets/Sprites/Tiles";

    /// <summary>Base Godot resource path for tile sprites.</summary>
    public const string TileSpritesResPath = "res://src/Assets/Sprites/Tiles";
}
