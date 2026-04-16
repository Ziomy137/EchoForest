namespace EchoForest.Core;

/// <summary>
/// Import configuration constants for character and prop sprite assets.
///
/// Godot auto-generates a <c>.import</c> sidecar file for every asset the first
/// time the project is opened in the editor. Those files are excluded from
/// version control via <c>*.import</c> in <c>.gitignore</c> (standard Godot practice).
/// This class documents the expected import settings so developers can verify
/// that locally-generated <c>.import</c> files match the intended configuration.
///
/// Pure C# — no Godot runtime required. Testable with NUnit.
/// </summary>
public static class CharacterImportConfig
{
    /// <summary>Godot compress mode 0 = Lossless (preserves pixel-perfect quality).</summary>
    public const int CompressMode = 0;

    /// <summary>Mipmaps disabled for 2D pixel-art sprites.</summary>
    public const bool MipmapsEnabled = false;

    /// <summary>No 3D detection — strictly 2D assets.</summary>
    public const int Detect3DCompressTo = 0;

    /// <summary>Standard import type for sprite textures.</summary>
    public const string ImporterType = "texture";

    /// <summary>Godot resource type after import.</summary>
    public const string ResourceType = "CompressedTexture2D";

    /// <summary>Base directory for character sprite assets (relative to project root).</summary>
    public const string CharacterSpritesDirectory = "src/Assets/Sprites/Characters";

    /// <summary>Base Godot resource path for character sprites.</summary>
    public const string CharacterSpritesResPath = "res://src/Assets/Sprites/Characters";

    /// <summary>Base directory for prop sprite assets (relative to project root).</summary>
    public const string PropSpritesDirectory = "src/Assets/Sprites/Props";

    /// <summary>Base Godot resource path for prop sprites.</summary>
    public const string PropSpritesResPath = "res://src/Assets/Sprites/Props";
}
