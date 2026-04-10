namespace EchoForest.Core.Tiles;

/// <summary>
/// Describes a single tile sprite asset required by the game.
/// Used by <see cref="TileRegistry"/> to track expected tile files and
/// by the palette-compliance test infrastructure to verify imports.
/// </summary>
/// <param name="Name">Human-readable tile name (PascalCase), e.g. "GrassGround".</param>
/// <param name="FilePath">Godot resource path, e.g. "res://src/Assets/Sprites/Tiles/tile_grass.png".</param>
/// <param name="PixelWidth">Tile width in pixels (isometric diamond = 64).</param>
/// <param name="PixelHeight">Tile height in pixels (isometric diamond = 32).</param>
/// <param name="PaletteHexColors">
/// Array of approved hex color codes (lowercase, no '#') used by this tile.
/// Must be a subset of <see cref="Palette.All"/>.
/// </param>
public sealed record TileDefinition(
    string Name,
    string FilePath,
    int PixelWidth,
    int PixelHeight,
    string[] PaletteHexColors);
