using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

// ═══════════════════════════════════════════════════════════════════════════════
// TileConfig Tests
// ═══════════════════════════════════════════════════════════════════════════════

[TestFixture]
public class TileConfigTest
{
    private TileConfig _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _sut = new TileConfig(
            "Test Tile", "test_tile.png", 64, 32, true,
            new[] { "1a3a1a", "2d2416" });
    }

    // ─── Constructor ──────────────────────────────────────────────────────────

    [Test]
    public void Constructor_SetsName() =>
        Assert.That(_sut.Name, Is.EqualTo("Test Tile"));

    [Test]
    public void Constructor_SetsFileName() =>
        Assert.That(_sut.FileName, Is.EqualTo("test_tile.png"));

    [Test]
    public void Constructor_SetsWidth() =>
        Assert.That(_sut.Width, Is.EqualTo(64));

    [Test]
    public void Constructor_SetsHeight() =>
        Assert.That(_sut.Height, Is.EqualTo(32));

    [Test]
    public void Constructor_SetsIsWalkable() =>
        Assert.That(_sut.IsWalkable, Is.True);

    [Test]
    public void Constructor_SetsExpectedColorHexCodes() =>
        Assert.That(_sut.ExpectedColorHexCodes, Is.EquivalentTo(new[] { "1a3a1a", "2d2416" }));

    // ─── Computed paths ───────────────────────────────────────────────────────

    [Test]
    public void ResourcePath_ReturnsGodotResPath() =>
        Assert.That(_sut.ResourcePath, Is.EqualTo("res://src/Assets/Sprites/Tiles/test_tile.png"));

    [Test]
    public void FilePath_ReturnsRelativeFilePath() =>
        Assert.That(_sut.FilePath, Is.EqualTo("src/Assets/Sprites/Tiles/test_tile.png"));

    // ─── Non-walkable tile ────────────────────────────────────────────────────

    [Test]
    public void Constructor_NonWalkable_SetsIsWalkableFalse()
    {
        var blocking = new TileConfig("Wall", "wall.png", 64, 32, false, new[] { "5c3d2e" });
        Assert.That(blocking.IsWalkable, Is.False);
    }

    [Test]
    public void Constructor_SingleColor_HasOneExpectedHex()
    {
        var mono = new TileConfig("Mono", "mono.png", 64, 32, true, new[] { "1a1a1a" });
        Assert.That(mono.ExpectedColorHexCodes.Count, Is.EqualTo(1));
    }

    [Test]
    public void ExpectedColorHexCodes_IsImmutable_MutatingSourceArrayDoesNotAffectConfig()
    {
        var source = new[] { "1a3a1a", "2d2416" };
        var tile = new TileConfig("Test", "test.png", 64, 32, true, source);
        source[0] = "ffffff";
        Assert.That(tile.ExpectedColorHexCodes[0], Is.EqualTo("1a3a1a"));
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// TileRegistry Tests
// ═══════════════════════════════════════════════════════════════════════════════

[TestFixture]
public class TileRegistryTest
{
    // ─── Count ────────────────────────────────────────────────────────────────

    [Test]
    public void All_ContainsExactlyExpectedTileCount() =>
        Assert.That(TileRegistry.All.Length, Is.EqualTo(TileRegistry.ExpectedTileCount));

    [Test]
    public void ExpectedTileCount_IsEleven() =>
        Assert.That(TileRegistry.ExpectedTileCount, Is.EqualTo(11));

    // ─── Dimensions ───────────────────────────────────────────────────────────

    [Test]
    public void TileWidth_Is64() =>
        Assert.That(TileRegistry.TileWidth, Is.EqualTo(64));

    [Test]
    public void TileHeight_Is32() =>
        Assert.That(TileRegistry.TileHeight, Is.EqualTo(32));

    [Test]
    public void All_AllTilesHaveCorrectWidth()
    {
        foreach (var tile in TileRegistry.All)
            Assert.That(tile.Width, Is.EqualTo(TileRegistry.TileWidth),
                $"Tile '{tile.Name}' has wrong width");
    }

    [Test]
    public void All_AllTilesHaveCorrectHeight()
    {
        foreach (var tile in TileRegistry.All)
            Assert.That(tile.Height, Is.EqualTo(TileRegistry.TileHeight),
                $"Tile '{tile.Name}' has wrong height");
    }

    // ─── Uniqueness ───────────────────────────────────────────────────────────

    [Test]
    public void All_AllFileNamesAreUnique()
    {
        var seen = new HashSet<string>();
        foreach (var tile in TileRegistry.All)
            Assert.That(seen.Add(tile.FileName), Is.True,
                $"Duplicate filename: {tile.FileName}");
    }

    [Test]
    public void All_AllNamesAreUnique()
    {
        var seen = new HashSet<string>();
        foreach (var tile in TileRegistry.All)
            Assert.That(seen.Add(tile.Name), Is.True,
                $"Duplicate name: {tile.Name}");
    }

    [Test]
    public void All_AllResourcePathsAreUnique()
    {
        var paths = TileRegistry.All.Select(t => t.ResourcePath).ToList();
        Assert.That(paths.Distinct().Count(), Is.EqualTo(paths.Count));
    }

    // ─── Individual tiles exist ───────────────────────────────────────────────

    [Test] public void Grass_IsRegistered() => Assert.That(TileRegistry.Grass, Is.Not.Null);
    [Test] public void GrassVariation_IsRegistered() => Assert.That(TileRegistry.GrassVariation, Is.Not.Null);
    [Test] public void Dirt_IsRegistered() => Assert.That(TileRegistry.Dirt, Is.Not.Null);
    [Test] public void Farmland_IsRegistered() => Assert.That(TileRegistry.Farmland, Is.Not.Null);
    [Test] public void Stone_IsRegistered() => Assert.That(TileRegistry.Stone, Is.Not.Null);
    [Test] public void Water_IsRegistered() => Assert.That(TileRegistry.Water, Is.Not.Null);
    [Test] public void CottageWall_IsRegistered() => Assert.That(TileRegistry.CottageWall, Is.Not.Null);
    [Test] public void CottageRoof_IsRegistered() => Assert.That(TileRegistry.CottageRoof, Is.Not.Null);
    [Test] public void FenceH_IsRegistered() => Assert.That(TileRegistry.FenceHorizontal, Is.Not.Null);
    [Test] public void FenceV_IsRegistered() => Assert.That(TileRegistry.FenceVertical, Is.Not.Null);
    [Test] public void Shadow_IsRegistered() => Assert.That(TileRegistry.Shadow, Is.Not.Null);

    // ─── File names match spec ────────────────────────────────────────────────

    [TestCase("tile_grass.png")]
    [TestCase("tile_grass_var.png")]
    [TestCase("tile_dirt.png")]
    [TestCase("tile_farm.png")]
    [TestCase("tile_stone.png")]
    [TestCase("tile_water.png")]
    [TestCase("tile_wall_front.png")]
    [TestCase("tile_roof.png")]
    [TestCase("tile_fence_h.png")]
    [TestCase("tile_fence_v.png")]
    [TestCase("tile_shadow.png")]
    public void All_ContainsTileWithFileName(string fileName)
    {
        Assert.That(TileRegistry.All.Any(t => t.FileName == fileName), Is.True,
            $"Missing tile: {fileName}");
    }

    // ─── Walkability ──────────────────────────────────────────────────────────

    [Test]
    public void Grass_IsWalkable() => Assert.That(TileRegistry.Grass.IsWalkable, Is.True);

    [Test]
    public void GrassVariation_IsWalkable() => Assert.That(TileRegistry.GrassVariation.IsWalkable, Is.True);

    [Test]
    public void Dirt_IsWalkable() => Assert.That(TileRegistry.Dirt.IsWalkable, Is.True);

    [Test]
    public void Farmland_IsWalkable() => Assert.That(TileRegistry.Farmland.IsWalkable, Is.True);

    [Test]
    public void Stone_IsWalkable() => Assert.That(TileRegistry.Stone.IsWalkable, Is.True);

    [Test]
    public void Water_IsBlocking() => Assert.That(TileRegistry.Water.IsWalkable, Is.False);

    [Test]
    public void CottageWall_IsBlocking() => Assert.That(TileRegistry.CottageWall.IsWalkable, Is.False);

    [Test]
    public void CottageRoof_IsBlocking() => Assert.That(TileRegistry.CottageRoof.IsWalkable, Is.False);

    [Test]
    public void FenceH_IsBlocking() => Assert.That(TileRegistry.FenceHorizontal.IsWalkable, Is.False);

    [Test]
    public void FenceV_IsBlocking() => Assert.That(TileRegistry.FenceVertical.IsWalkable, Is.False);

    [Test]
    public void Shadow_IsWalkable() => Assert.That(TileRegistry.Shadow.IsWalkable, Is.True);

    [Test]
    public void Walkable_ReturnsOnlyWalkableTiles()
    {
        var walkable = TileRegistry.Walkable;
        Assert.That(walkable.All(t => t.IsWalkable), Is.True);
        Assert.That(walkable.Length, Is.EqualTo(6));
    }

    [Test]
    public void Blocking_ReturnsOnlyBlockingTiles()
    {
        var blocking = TileRegistry.Blocking;
        Assert.That(blocking.All(t => !t.IsWalkable), Is.True);
        Assert.That(blocking.Length, Is.EqualTo(5));
    }

    // ─── Color compliance — all tile colors are approved palette colors ───────

    [Test]
    public void All_ExpectedColors_AreApprovedPaletteColors()
    {
        var approvedHex = new HashSet<string>(
            Palette.All.Select(c => c.ToHtml(false)));

        foreach (var tile in TileRegistry.All)
            foreach (var hex in tile.ExpectedColorHexCodes)
                Assert.That(approvedHex.Contains(hex), Is.True,
                    $"Tile '{tile.Name}' expects non-palette color: #{hex}");
    }

    [Test]
    public void All_EachTileHasAtLeastOneExpectedColor()
    {
        foreach (var tile in TileRegistry.All)
            Assert.That(tile.ExpectedColorHexCodes.Count, Is.GreaterThan(0),
                $"Tile '{tile.Name}' has no expected colors");
    }

    // ─── Resource paths ───────────────────────────────────────────────────────

    [Test]
    public void All_ResourcePaths_StartWithResPrefix()
    {
        foreach (var tile in TileRegistry.All)
            Assert.That(tile.ResourcePath, Does.StartWith("res://"),
                $"Tile '{tile.Name}' resource path missing res:// prefix");
    }

    [Test]
    public void All_ResourcePaths_EndWithPng()
    {
        foreach (var tile in TileRegistry.All)
            Assert.That(tile.ResourcePath, Does.EndWith(".png"),
                $"Tile '{tile.Name}' resource path should end with .png");
    }

    [Test]
    public void All_FilePaths_ContainTilesDirectory()
    {
        foreach (var tile in TileRegistry.All)
            Assert.That(tile.FilePath, Does.Contain("Sprites/Tiles/"),
                $"Tile '{tile.Name}' not in Tiles directory");
    }

    // ─── Lookup methods ───────────────────────────────────────────────────────

    [Test]
    public void GetByFileName_ExistingTile_ReturnsTile()
    {
        var result = TileRegistry.GetByFileName("tile_grass.png");
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Grass Ground"));
    }

    [Test]
    public void GetByFileName_NonExistentTile_ReturnsNull()
    {
        Assert.That(TileRegistry.GetByFileName("nonexistent.png"), Is.Null);
    }

    [Test]
    public void GetByName_ExistingTile_ReturnsTile()
    {
        var result = TileRegistry.GetByName("Water");
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.FileName, Is.EqualTo("tile_water.png"));
    }

    [Test]
    public void GetByName_NonExistentTile_ReturnsNull()
    {
        Assert.That(TileRegistry.GetByName("Lava"), Is.Null);
    }

    [TestCase("tile_grass.png", "Grass Ground")]
    [TestCase("tile_dirt.png", "Dirt Path")]
    [TestCase("tile_water.png", "Water")]
    [TestCase("tile_roof.png", "Cottage Roof")]
    [TestCase("tile_shadow.png", "Shadow")]
    public void GetByFileName_ReturnsCorrectName(string fileName, string expectedName)
    {
        var result = TileRegistry.GetByFileName(fileName);
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo(expectedName));
    }

    // ─── Defensive copy ───────────────────────────────────────────────────────

    [Test]
    public void All_ReturnsDefensiveCopy()
    {
        var a = TileRegistry.All;
        var b = TileRegistry.All;
        Assert.That(ReferenceEquals(a, b), Is.False);
    }

    [Test]
    public void Walkable_ReturnsDefensiveCopy()
    {
        var a = TileRegistry.Walkable;
        var b = TileRegistry.Walkable;
        Assert.That(ReferenceEquals(a, b), Is.False);
    }

    [Test]
    public void Blocking_ReturnsDefensiveCopy()
    {
        var a = TileRegistry.Blocking;
        var b = TileRegistry.Blocking;
        Assert.That(ReferenceEquals(a, b), Is.False);
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// TileImportConfig Tests
// ═══════════════════════════════════════════════════════════════════════════════

[TestFixture]
public class TileImportConfigTest
{
    [Test]
    public void CompressMode_IsLossless() =>
        Assert.That(TileImportConfig.CompressMode, Is.EqualTo(0));

    [Test]
    public void MipmapsEnabled_IsFalse() =>
        Assert.That(TileImportConfig.MipmapsEnabled, Is.False);

    [Test]
    public void Detect3DCompressTo_IsZero() =>
        Assert.That(TileImportConfig.Detect3DCompressTo, Is.EqualTo(0));

    [Test]
    public void ImporterType_IsTexture() =>
        Assert.That(TileImportConfig.ImporterType, Is.EqualTo("texture"));

    [Test]
    public void ResourceType_IsCompressedTexture2D() =>
        Assert.That(TileImportConfig.ResourceType, Is.EqualTo("CompressedTexture2D"));

    [Test]
    public void TileSpritesDirectory_PointsToCorrectPath() =>
        Assert.That(TileImportConfig.TileSpritesDirectory, Is.EqualTo("src/Assets/Sprites/Tiles"));

    [Test]
    public void TileSpritesResPath_HasResPrefix() =>
        Assert.That(TileImportConfig.TileSpritesResPath, Does.StartWith("res://"));

    [Test]
    public void TileSpritesResPath_MatchesDirectory() =>
        Assert.That(TileImportConfig.TileSpritesResPath,
            Is.EqualTo("res://" + TileImportConfig.TileSpritesDirectory));
}
