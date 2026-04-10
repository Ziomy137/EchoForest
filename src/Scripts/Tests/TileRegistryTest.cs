using System.Linq;
using NUnit.Framework;
using EchoForest.Core.Tiles;

namespace EchoForest.Tests;

/// <summary>
/// Tests for <see cref="TileRegistry"/> and <see cref="TileDefinition"/>.
/// Validates registry completeness, naming conventions, dimension spec,
/// and path correctness — without requiring the Godot runtime.
/// </summary>
[TestFixture]
public class TileRegistryTest
{
    // ── Registry completeness ─────────────────────────────────────────────────

    [Test]
    public void TileRegistry_HasElevenTiles()
    {
        Assert.That(TileRegistry.All, Has.Length.EqualTo(11));
    }

    [Test]
    public void TileRegistry_AllNames_AreUnique()
    {
        var names = TileRegistry.All.Select(t => t.Name).ToArray();
        Assert.That(names.Distinct().Count(), Is.EqualTo(names.Length));
    }

    [Test]
    public void TileRegistry_AllFilePaths_AreUnique()
    {
        var paths = TileRegistry.All.Select(t => t.FilePath).ToArray();
        Assert.That(paths.Distinct().Count(), Is.EqualTo(paths.Length));
    }

    // ── Path convention ───────────────────────────────────────────────────────

    [Test]
    public void TileRegistry_AllTiles_PathStartsWithResProtocol()
    {
        foreach (var tile in TileRegistry.All)
            Assert.That(tile.FilePath, Does.StartWith("res://"),
                $"{tile.Name}: path must start with 'res://'");
    }

    [Test]
    public void TileRegistry_AllTiles_PathEndsWithPng()
    {
        foreach (var tile in TileRegistry.All)
            Assert.That(tile.FilePath, Does.EndWith(".png"),
                $"{tile.Name}: path must end with '.png'");
    }

    [Test]
    public void TileRegistry_AllTiles_PathContainsTilesFolder()
    {
        foreach (var tile in TileRegistry.All)
            Assert.That(tile.FilePath, Does.Contain("/Sprites/Tiles/"),
                $"{tile.Name}: path must be under Sprites/Tiles/");
    }

    // ── Dimension spec (isometric diamond = 64×32) ────────────────────────────

    [Test]
    public void TileRegistry_AllTiles_AreIsometricDimensions()
    {
        foreach (var tile in TileRegistry.All)
        {
            Assert.That(tile.PixelWidth, Is.EqualTo(64),
                $"{tile.Name}: expected width 64");
            Assert.That(tile.PixelHeight, Is.EqualTo(32),
                $"{tile.Name}: expected height 32");
        }
    }

    // ── Palette colors not empty ──────────────────────────────────────────────

    [Test]
    public void TileRegistry_AllTiles_HaveAtLeastOnePaletteColor()
    {
        foreach (var tile in TileRegistry.All)
            Assert.That(tile.PaletteHexColors, Is.Not.Empty,
                $"{tile.Name}: must declare at least one palette color");
    }

    // ── Required tiles exist by name ──────────────────────────────────────────

    [TestCase("GrassGround")]
    [TestCase("GrassVariation")]
    [TestCase("DirtPath")]
    [TestCase("Farmland")]
    [TestCase("StoneFloor")]
    [TestCase("Water")]
    [TestCase("CottageWallFront")]
    [TestCase("CottageRoof")]
    [TestCase("FenceHorizontal")]
    [TestCase("FenceVertical")]
    [TestCase("Shadow")]
    public void TileRegistry_Contains_RequiredTileByName(string tileName)
    {
        Assert.That(TileRegistry.All.Select(t => t.Name), Does.Contain(tileName));
    }

    // ── Find() ────────────────────────────────────────────────────────────────

    [Test]
    public void TileRegistry_Find_ReturnsCorrectTile()
    {
        var tile = TileRegistry.Find("GrassGround");

        Assert.That(tile, Is.Not.Null);
        Assert.That(tile!.Name, Is.EqualTo("GrassGround"));
    }

    [Test]
    public void TileRegistry_Find_UnknownName_ReturnsNull()
    {
        var tile = TileRegistry.Find("DoesNotExist");

        Assert.That(tile, Is.Null);
    }

    // ── Static tile constant sanity ───────────────────────────────────────────

    [Test]
    public void TileRegistry_GrassGround_FilePath_IsCorrect()
    {
        Assert.That(TileRegistry.GrassGround.FilePath,
            Is.EqualTo("res://src/Assets/Sprites/Tiles/tile_grass.png"));
    }

    [Test]
    public void TileRegistry_Shadow_HasSingleDarkColor()
    {
        Assert.That(TileRegistry.Shadow.PaletteHexColors, Has.Length.EqualTo(1));
        Assert.That(TileRegistry.Shadow.PaletteHexColors[0], Is.EqualTo("1a1a1a"));
    }

    // ── TileDefinition record properties ──────────────────────────────────────

    [Test]
    public void TileDefinition_Properties_AreStoredCorrectly()
    {
        var colors = new[] { "1a1a1a" };
        var tile = new TileDefinition("Test", "res://test.png", 64, 32, colors);

        Assert.Multiple(() =>
        {
            Assert.That(tile.Name, Is.EqualTo("Test"));
            Assert.That(tile.FilePath, Is.EqualTo("res://test.png"));
            Assert.That(tile.PixelWidth, Is.EqualTo(64));
            Assert.That(tile.PixelHeight, Is.EqualTo(32));
            Assert.That(tile.PaletteHexColors, Is.EqualTo(colors));
        });
    }

    [Test]
    public void TileDefinition_DifferentNames_AreNotEqual()
    {
        var a = new TileDefinition("TileA", "res://test.png", 64, 32, new[] { "1a1a1a" });
        var b = new TileDefinition("TileB", "res://test.png", 64, 32, new[] { "1a1a1a" });

        Assert.That(a, Is.Not.EqualTo(b));
    }
}
