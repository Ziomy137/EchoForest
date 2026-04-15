namespace EchoForest.Core;

/// <summary>
/// Import configuration constants for tile sprite assets.
///
/// Ensures all tile PNGs are imported with consistent settings:
/// nearest-neighbor filtering (no bilinear), no mipmaps, uncompressed.
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
