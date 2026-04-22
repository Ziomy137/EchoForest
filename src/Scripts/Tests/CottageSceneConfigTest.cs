using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

// ═══════════════════════════════════════════════════════════════════════════════
// TileZone record-struct Tests
// ═══════════════════════════════════════════════════════════════════════════════

[TestFixture]
public class TileZoneTest
{
    private static readonly CottageSceneConfig.TileZone _zone = new(5, 3, 4, 2);
    //   covers cols 5,6,7,8  /  rows 3,4

    // ─── Contains ─────────────────────────────────────────────────────────────

    [Test]
    public void Contains_TopLeftCorner_ReturnsTrue() =>
        Assert.That(_zone.Contains(5, 3), Is.True);

    [Test]
    public void Contains_BottomRightInterior_ReturnsTrue() =>
        Assert.That(_zone.Contains(8, 4), Is.True);

    [Test]
    public void Contains_JustOutsideRight_ReturnsFalse() =>
        Assert.That(_zone.Contains(9, 3), Is.False);

    [Test]
    public void Contains_JustOutsideBottom_ReturnsFalse() =>
        Assert.That(_zone.Contains(5, 5), Is.False);

    [Test]
    public void Contains_JustOutsideLeft_ReturnsFalse() =>
        Assert.That(_zone.Contains(4, 3), Is.False);

    [Test]
    public void Contains_JustOutsideTop_ReturnsFalse() =>
        Assert.That(_zone.Contains(5, 2), Is.False);

    [Test]
    public void Contains_Origin_ReturnsFalse() =>
        Assert.That(_zone.Contains(0, 0), Is.False);

    // ─── TileCount / EndCol / EndRow ──────────────────────────────────────────

    [Test]
    public void TileCount_EqualsWidthTimesHeight() =>
        Assert.That(_zone.TileCount, Is.EqualTo(8));   // 4×2

    [Test]
    public void EndCol_EqualsColPlusWidth() =>
        Assert.That(_zone.EndCol, Is.EqualTo(9));      // 5+4

    [Test]
    public void EndRow_EqualsRowPlusHeight() =>
        Assert.That(_zone.EndRow, Is.EqualTo(5));      // 3+2

    // ─── Record equality ──────────────────────────────────────────────────────

    [Test]
    public void TileZone_EqualValues_AreEqual() =>
        Assert.That(new CottageSceneConfig.TileZone(5, 3, 4, 2),
            Is.EqualTo(new CottageSceneConfig.TileZone(5, 3, 4, 2)));

    [Test]
    public void TileZone_DifferentCol_AreNotEqual() =>
        Assert.That(new CottageSceneConfig.TileZone(6, 3, 4, 2),
            Is.Not.EqualTo(new CottageSceneConfig.TileZone(5, 3, 4, 2)));

    // ─── SingleCell zone (width=1, height=1) ──────────────────────────────────

    [Test]
    public void TileZone_SingleCell_ContainsOnlyThatCell()
    {
        var cell = new CottageSceneConfig.TileZone(7, 7, 1, 1);
        Assert.That(cell.Contains(7, 7), Is.True);
        Assert.That(cell.Contains(6, 7), Is.False);
        Assert.That(cell.Contains(8, 7), Is.False);
        Assert.That(cell.Contains(7, 6), Is.False);
        Assert.That(cell.Contains(7, 8), Is.False);
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// CottageSceneConfig — Constants Tests
// ═══════════════════════════════════════════════════════════════════════════════

[TestFixture]
public class CottageSceneConfigConstantsTest
{
    // ─── Grid ─────────────────────────────────────────────────────────────────

    [Test]
    public void GridColumns_Is30() =>
        Assert.That(CottageSceneConfig.GridColumns, Is.EqualTo(30));

    [Test]
    public void GridRows_Is20() =>
        Assert.That(CottageSceneConfig.GridRows, Is.EqualTo(20));

    [Test]
    public void TotalCells_Is600() =>
        Assert.That(CottageSceneConfig.TotalCells, Is.EqualTo(600));

    [Test]
    public void TotalCells_EqualsColumnsTimesRows() =>
        Assert.That(CottageSceneConfig.TotalCells,
            Is.EqualTo(CottageSceneConfig.GridColumns * CottageSceneConfig.GridRows));

    // ─── Scene resource paths ─────────────────────────────────────────────────

    [Test]
    public void SceneResPath_StartsWithResProtocol() =>
        Assert.That(CottageSceneConfig.SceneResPath, Does.StartWith("res://"));

    [Test]
    public void SceneResPath_EndsWithTscn() =>
        Assert.That(CottageSceneConfig.SceneResPath, Does.EndWith(".tscn"));

    [Test]
    public void SceneResPath_ContainsScenesDirectory() =>
        Assert.That(CottageSceneConfig.SceneResPath, Does.Contain("Scenes"));

    [Test]
    public void TileSetResPath_StartsWithResProtocol() =>
        Assert.That(CottageSceneConfig.TileSetResPath, Does.StartWith("res://"));

    [Test]
    public void TileSetResPath_EndsWithTres() =>
        Assert.That(CottageSceneConfig.TileSetResPath, Does.EndWith(".tres"));

    // ─── Spawn point ──────────────────────────────────────────────────────────

    [Test]
    public void SpawnTileCol_WithinGridColumns() =>
        Assert.That(CottageSceneConfig.SpawnTileCol, Is.InRange(0, CottageSceneConfig.GridColumns - 1));

    [Test]
    public void SpawnTileRow_WithinGridRows() =>
        Assert.That(CottageSceneConfig.SpawnTileRow, Is.InRange(0, CottageSceneConfig.GridRows - 1));

    [Test]
    public void SpawnWorldX_MatchesTileFormula() =>
        Assert.That(CottageSceneConfig.SpawnWorldX,
            Is.EqualTo(CottageSceneConfig.TileToWorldX(
                CottageSceneConfig.SpawnTileCol, CottageSceneConfig.SpawnTileRow)).Within(0.001f));

    [Test]
    public void SpawnWorldY_MatchesTileFormula() =>
        Assert.That(CottageSceneConfig.SpawnWorldY,
            Is.EqualTo(CottageSceneConfig.TileToWorldY(
                CottageSceneConfig.SpawnTileCol, CottageSceneConfig.SpawnTileRow)).Within(0.001f));

    // ─── Node name constants ──────────────────────────────────────────────────

    [Test]
    public void TileMapLayerName_IsNotEmpty() =>
        Assert.That(CottageSceneConfig.TileMapLayerName, Is.Not.Empty);

    [Test]
    public void PlayerSpawnName_IsNotEmpty() =>
        Assert.That(CottageSceneConfig.PlayerSpawnName, Is.Not.Empty);

    [Test]
    public void PropsNodeName_IsNotEmpty() =>
        Assert.That(CottageSceneConfig.PropsNodeName, Is.Not.Empty);

    [Test]
    public void BoundaryNodeName_IsNotEmpty() =>
        Assert.That(CottageSceneConfig.BoundaryNodeName, Is.Not.Empty);

    [Test]
    public void CameraNodeName_IsNotEmpty() =>
        Assert.That(CottageSceneConfig.CameraNodeName, Is.Not.Empty);

    [Test]
    public void NodeNames_AreAllDistinct()
    {
        var names = new[]
        {
            CottageSceneConfig.TileMapLayerName,
            CottageSceneConfig.PlayerSpawnName,
            CottageSceneConfig.PropsNodeName,
            CottageSceneConfig.BoundaryNodeName,
            CottageSceneConfig.CameraNodeName,
        };
        Assert.That(names.Distinct().Count(), Is.EqualTo(names.Length));
    }

    // ─── Tile file name constants ─────────────────────────────────────────────

    [Test]
    public void TileGrass_MatchesTileRegistry() =>
        Assert.That(CottageSceneConfig.TileGrass, Is.EqualTo(TileRegistry.Grass.FileName));

    [Test]
    public void TileGrassVar_MatchesTileRegistry() =>
        Assert.That(CottageSceneConfig.TileGrassVar, Is.EqualTo(TileRegistry.GrassVariation.FileName));

    [Test]
    public void TileDirt_MatchesTileRegistry() =>
        Assert.That(CottageSceneConfig.TileDirt, Is.EqualTo(TileRegistry.Dirt.FileName));

    [Test]
    public void TileFarm_MatchesTileRegistry() =>
        Assert.That(CottageSceneConfig.TileFarm, Is.EqualTo(TileRegistry.Farmland.FileName));

    [Test]
    public void TileStone_MatchesTileRegistry() =>
        Assert.That(CottageSceneConfig.TileStone, Is.EqualTo(TileRegistry.Stone.FileName));

    [Test]
    public void TileWall_MatchesTileRegistry() =>
        Assert.That(CottageSceneConfig.TileWall, Is.EqualTo(TileRegistry.CottageWall.FileName));

    [Test]
    public void TileRoof_MatchesTileRegistry() =>
        Assert.That(CottageSceneConfig.TileRoof, Is.EqualTo(TileRegistry.CottageRoof.FileName));

    [Test]
    public void TileFenceH_MatchesTileRegistry() =>
        Assert.That(CottageSceneConfig.TileFenceH, Is.EqualTo(TileRegistry.FenceHorizontal.FileName));

    [Test]
    public void TileFenceV_MatchesTileRegistry() =>
        Assert.That(CottageSceneConfig.TileFenceV, Is.EqualTo(TileRegistry.FenceVertical.FileName));

    [Test]
    public void TileShadow_MatchesTileRegistry() =>
        Assert.That(CottageSceneConfig.TileShadow, Is.EqualTo(TileRegistry.Shadow.FileName));

    // ─── Source IDs ───────────────────────────────────────────────────────────

    [Test]
    public void SourceIdGrass_IsZero() => Assert.That(CottageSceneConfig.SourceIdGrass, Is.EqualTo(0));
    [Test]
    public void SourceIdGrassVar_IsOne() => Assert.That(CottageSceneConfig.SourceIdGrassVar, Is.EqualTo(1));
    [Test]
    public void SourceIdDirt_IsTwo() => Assert.That(CottageSceneConfig.SourceIdDirt, Is.EqualTo(2));
    [Test]
    public void SourceIdFarm_IsThree() => Assert.That(CottageSceneConfig.SourceIdFarm, Is.EqualTo(3));
    [Test]
    public void SourceIdStone_IsFour() => Assert.That(CottageSceneConfig.SourceIdStone, Is.EqualTo(4));
    [Test]
    public void SourceIdWater_IsFive() => Assert.That(CottageSceneConfig.SourceIdWater, Is.EqualTo(5));
    [Test]
    public void SourceIdWall_IsSix() => Assert.That(CottageSceneConfig.SourceIdWall, Is.EqualTo(6));
    [Test]
    public void SourceIdRoof_IsSeven() => Assert.That(CottageSceneConfig.SourceIdRoof, Is.EqualTo(7));
    [Test]
    public void SourceIdFenceH_IsEight() => Assert.That(CottageSceneConfig.SourceIdFenceH, Is.EqualTo(8));
    [Test]
    public void SourceIdFenceV_IsNine() => Assert.That(CottageSceneConfig.SourceIdFenceV, Is.EqualTo(9));
    [Test]
    public void SourceIdShadow_IsTen() => Assert.That(CottageSceneConfig.SourceIdShadow, Is.EqualTo(10));

    [Test]
    public void SourceIds_AreAllDistinct()
    {
        var ids = new[]
        {
            CottageSceneConfig.SourceIdGrass,    CottageSceneConfig.SourceIdGrassVar,
            CottageSceneConfig.SourceIdDirt,     CottageSceneConfig.SourceIdFarm,
            CottageSceneConfig.SourceIdStone,    CottageSceneConfig.SourceIdWater,
            CottageSceneConfig.SourceIdWall,     CottageSceneConfig.SourceIdRoof,
            CottageSceneConfig.SourceIdFenceH,   CottageSceneConfig.SourceIdFenceV,
            CottageSceneConfig.SourceIdShadow,
        };
        Assert.That(ids.Distinct().Count(), Is.EqualTo(ids.Length));
    }

    // ─── World boundary ───────────────────────────────────────────────────────

    [Test]
    public void WorldBoundaryLeft_IsNegative() =>
        Assert.That(CottageSceneConfig.WorldBoundaryLeft, Is.LessThan(0f));

    [Test]
    public void WorldBoundaryRight_IsPositive() =>
        Assert.That(CottageSceneConfig.WorldBoundaryRight, Is.GreaterThan(0f));

    [Test]
    public void WorldBoundaryLeft_LessThanRight() =>
        Assert.That(CottageSceneConfig.WorldBoundaryLeft,
            Is.LessThan(CottageSceneConfig.WorldBoundaryRight));

    [Test]
    public void WorldBoundaryTop_LessThanBottom() =>
        Assert.That(CottageSceneConfig.WorldBoundaryTop,
            Is.LessThan(CottageSceneConfig.WorldBoundaryBottom));

    [Test]
    public void WorldBoundaryLeft_IsOutsideGridLeftCorner()
    {
        // tile(0,19) → world x = (0−19)×32 = −608; boundary must be further left
        float gridLeftX = CottageSceneConfig.TileToWorldX(0, 19);
        Assert.That(CottageSceneConfig.WorldBoundaryLeft, Is.LessThan(gridLeftX));
    }

    [Test]
    public void WorldBoundaryRight_IsOutsideGridRightCorner()
    {
        // tile(29,0) → world x = (29−0)×32 = 928; boundary must be further right
        float gridRightX = CottageSceneConfig.TileToWorldX(29, 0);
        Assert.That(CottageSceneConfig.WorldBoundaryRight, Is.GreaterThan(gridRightX));
    }

    [Test]
    public void WorldBoundaryBottom_IsOutsideGridBottomCorner()
    {
        // tile(29,19) → world y = (29+19)×16 = 768; boundary must be further down
        float gridBottomY = CottageSceneConfig.TileToWorldY(29, 19);
        Assert.That(CottageSceneConfig.WorldBoundaryBottom, Is.GreaterThan(gridBottomY));
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// CottageSceneConfig — Zone Definition Tests
// ═══════════════════════════════════════════════════════════════════════════════

[TestFixture]
public class CottageSceneZoneTest
{
    // ─── TileCounts ───────────────────────────────────────────────────────────

    [Test]
    public void RoofZone_TileCount_Is4() =>
        Assert.That(CottageSceneConfig.RoofZone.TileCount, Is.EqualTo(4));

    [Test]
    public void WallZone_TileCount_Is4() =>
        Assert.That(CottageSceneConfig.WallZone.TileCount, Is.EqualTo(4));

    [Test]
    public void InteriorZone_TileCount_Is12() =>
        Assert.That(CottageSceneConfig.InteriorZone.TileCount, Is.EqualTo(12));

    [Test]
    public void FarmZone_TileCount_Is25() =>
        Assert.That(CottageSceneConfig.FarmZone.TileCount, Is.EqualTo(25));

    [Test]
    public void FenceNorth_TileCount_Is5() =>
        Assert.That(CottageSceneConfig.FenceNorth.TileCount, Is.EqualTo(5));

    [Test]
    public void FenceSouth_TileCount_Is5() =>
        Assert.That(CottageSceneConfig.FenceSouth.TileCount, Is.EqualTo(5));

    [Test]
    public void FenceWest_TileCount_Is5() =>
        Assert.That(CottageSceneConfig.FenceWest.TileCount, Is.EqualTo(5));

    [Test]
    public void FenceEast_TileCount_Is5() =>
        Assert.That(CottageSceneConfig.FenceEast.TileCount, Is.EqualTo(5));

    [Test]
    public void DirtPath_TileCount_Is18() =>
        Assert.That(CottageSceneConfig.DirtPath.TileCount, Is.EqualTo(18));

    // ─── All zones within grid bounds ─────────────────────────────────────────

    [Test]
    public void AllZones_StartWithinGrid()
    {
        var zones = new[]
        {
            CottageSceneConfig.RoofZone,     CottageSceneConfig.WallZone,
            CottageSceneConfig.InteriorZone, CottageSceneConfig.FarmZone,
            CottageSceneConfig.FenceNorth,   CottageSceneConfig.FenceSouth,
            CottageSceneConfig.FenceWest,    CottageSceneConfig.FenceEast,
            CottageSceneConfig.DirtPath,
        };
        foreach (var z in zones)
        {
            Assert.That(z.Col, Is.GreaterThanOrEqualTo(0), $"Zone Col < 0");
            Assert.That(z.Row, Is.GreaterThanOrEqualTo(0), $"Zone Row < 0");
        }
    }

    [Test]
    public void AllZones_EndWithinGrid()
    {
        var zones = new[]
        {
            CottageSceneConfig.RoofZone,     CottageSceneConfig.WallZone,
            CottageSceneConfig.InteriorZone, CottageSceneConfig.FarmZone,
            CottageSceneConfig.FenceNorth,   CottageSceneConfig.FenceSouth,
            CottageSceneConfig.FenceWest,    CottageSceneConfig.FenceEast,
            CottageSceneConfig.DirtPath,
        };
        foreach (var z in zones)
        {
            Assert.That(z.EndCol, Is.LessThanOrEqualTo(CottageSceneConfig.GridColumns),
                $"Zone EndCol {z.EndCol} exceeds GridColumns");
            Assert.That(z.EndRow, Is.LessThanOrEqualTo(CottageSceneConfig.GridRows),
                $"Zone EndRow {z.EndRow} exceeds GridRows");
        }
    }

    // ─── Zone non-overlap ─────────────────────────────────────────────────────

    [Test]
    public void CottageZones_DoNotOverlapFarmZone()
    {
        var cottageZones = new[]
        {
            CottageSceneConfig.RoofZone, CottageSceneConfig.WallZone, CottageSceneConfig.InteriorZone,
        };
        foreach (var cottage in cottageZones)
            for (int c = cottage.Col; c < cottage.EndCol; c++)
                for (int r = cottage.Row; r < cottage.EndRow; r++)
                    Assert.That(CottageSceneConfig.FarmZone.Contains(c, r), Is.False,
                        $"Cottage zone overlaps farm at ({c},{r})");
    }

    [Test]
    public void FarmZone_DoesNotOverlapCottageZones()
    {
        for (int c = CottageSceneConfig.FarmZone.Col; c < CottageSceneConfig.FarmZone.EndCol; c++)
            for (int r = CottageSceneConfig.FarmZone.Row; r < CottageSceneConfig.FarmZone.EndRow; r++)
            {
                Assert.That(CottageSceneConfig.RoofZone.Contains(c, r), Is.False);
                Assert.That(CottageSceneConfig.WallZone.Contains(c, r), Is.False);
                Assert.That(CottageSceneConfig.InteriorZone.Contains(c, r), Is.False);
            }
    }

    [Test]
    public void DirtPath_DoesNotOverlapCottageOrFarm()
    {
        for (int c = CottageSceneConfig.DirtPath.Col; c < CottageSceneConfig.DirtPath.EndCol; c++)
            for (int r = CottageSceneConfig.DirtPath.Row; r < CottageSceneConfig.DirtPath.EndRow; r++)
            {
                Assert.That(CottageSceneConfig.RoofZone.Contains(c, r), Is.False, $"DirtPath overlaps RoofZone at ({c},{r})");
                Assert.That(CottageSceneConfig.WallZone.Contains(c, r), Is.False, $"DirtPath overlaps WallZone at ({c},{r})");
                Assert.That(CottageSceneConfig.InteriorZone.Contains(c, r), Is.False, $"DirtPath overlaps InteriorZone at ({c},{r})");
                Assert.That(CottageSceneConfig.FarmZone.Contains(c, r), Is.False, $"DirtPath overlaps FarmZone at ({c},{r})");
            }
    }

    [Test]
    public void FenceZones_DoNotOverlapFarmZone()
    {
        var fences = new[]
        {
            CottageSceneConfig.FenceNorth, CottageSceneConfig.FenceSouth,
            CottageSceneConfig.FenceWest,  CottageSceneConfig.FenceEast,
        };
        foreach (var fence in fences)
            for (int c = fence.Col; c < fence.EndCol; c++)
                for (int r = fence.Row; r < fence.EndRow; r++)
                    Assert.That(CottageSceneConfig.FarmZone.Contains(c, r), Is.False,
                        $"Fence zone overlaps farm at ({c},{r})");
    }

    [Test]
    public void RoofWallInterior_DoNotOverlapEachOther()
    {
        var zones = new[]
        {
            CottageSceneConfig.RoofZone, CottageSceneConfig.WallZone, CottageSceneConfig.InteriorZone,
        };
        for (int i = 0; i < zones.Length; i++)
            for (int j = i + 1; j < zones.Length; j++)
            {
                var a = zones[i];
                var b = zones[j];
                for (int c = a.Col; c < a.EndCol; c++)
                    for (int r = a.Row; r < a.EndRow; r++)
                        Assert.That(b.Contains(c, r), Is.False,
                            $"Zones {i} and {j} overlap at ({c},{r})");
            }
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// CottageSceneConfig — GetTileFileName Tests
// ═══════════════════════════════════════════════════════════════════════════════

[TestFixture]
public class CottageSceneGetTileTest
{
    private static readonly string Roof = CottageSceneConfig.TileRoof;
    private static readonly string Wall = CottageSceneConfig.TileWall;
    private static readonly string Stone = CottageSceneConfig.TileStone;
    private static readonly string Farm = CottageSceneConfig.TileFarm;
    private static readonly string FenceH = CottageSceneConfig.TileFenceH;
    private static readonly string FenceV = CottageSceneConfig.TileFenceV;
    private static readonly string Dirt = CottageSceneConfig.TileDirt;
    private static readonly string Grass = CottageSceneConfig.TileGrass;
    private static readonly string GrassV = CottageSceneConfig.TileGrassVar;

    // ─── Roof zone (row 9, cols 12-15) ───────────────────────────────────────

    [TestCase(12, 9)]
    [TestCase(13, 9)]
    [TestCase(14, 9)]
    [TestCase(15, 9)]
    public void GetTileFileName_RoofZone_ReturnsRoof(int col, int row) =>
        Assert.That(CottageSceneConfig.GetTileFileName(col, row), Is.EqualTo(Roof));

    // ─── Wall zone (row 10, cols 12-15) ───────────────────────────────────────

    [TestCase(12, 10)]
    [TestCase(13, 10)]
    [TestCase(14, 10)]
    [TestCase(15, 10)]
    public void GetTileFileName_WallZone_ReturnsWall(int col, int row) =>
        Assert.That(CottageSceneConfig.GetTileFileName(col, row), Is.EqualTo(Wall));

    // ─── Interior zone (rows 11-13, cols 12-15) ───────────────────────────────

    [TestCase(12, 11)]
    [TestCase(15, 11)]
    [TestCase(12, 13)]
    [TestCase(15, 13)]
    public void GetTileFileName_InteriorZone_ReturnsStone(int col, int row) =>
        Assert.That(CottageSceneConfig.GetTileFileName(col, row), Is.EqualTo(Stone));

    // ─── Farm zone (rows 1-5, cols 23-27) ────────────────────────────────────

    [TestCase(23, 1)]
    [TestCase(27, 1)]
    [TestCase(23, 5)]
    [TestCase(27, 5)]
    [TestCase(25, 3)]
    public void GetTileFileName_FarmZone_ReturnsFarm(int col, int row) =>
        Assert.That(CottageSceneConfig.GetTileFileName(col, row), Is.EqualTo(Farm));

    // ─── Fence North (row 0, cols 23-27) ─────────────────────────────────────

    [TestCase(23, 0)]
    [TestCase(25, 0)]
    [TestCase(27, 0)]
    public void GetTileFileName_FenceNorth_ReturnsFenceH(int col, int row) =>
        Assert.That(CottageSceneConfig.GetTileFileName(col, row), Is.EqualTo(FenceH));

    // ─── Fence South (row 6, cols 23-27) ─────────────────────────────────────

    [TestCase(23, 6)]
    [TestCase(25, 6)]
    [TestCase(27, 6)]
    public void GetTileFileName_FenceSouth_ReturnsFenceH(int col, int row) =>
        Assert.That(CottageSceneConfig.GetTileFileName(col, row), Is.EqualTo(FenceH));

    // ─── Fence West (col 22, rows 1-5) ────────────────────────────────────────

    [TestCase(22, 1)]
    [TestCase(22, 3)]
    [TestCase(22, 5)]
    public void GetTileFileName_FenceWest_ReturnsFenceV(int col, int row) =>
        Assert.That(CottageSceneConfig.GetTileFileName(col, row), Is.EqualTo(FenceV));

    // ─── Fence East (col 28, rows 1-5) ────────────────────────────────────────

    [TestCase(28, 1)]
    [TestCase(28, 3)]
    [TestCase(28, 5)]
    public void GetTileFileName_FenceEast_ReturnsFenceV(int col, int row) =>
        Assert.That(CottageSceneConfig.GetTileFileName(col, row), Is.EqualTo(FenceV));

    // ─── Dirt path (rows 14-19, cols 13-15) ──────────────────────────────────

    [TestCase(13, 14)]
    [TestCase(15, 14)]
    [TestCase(14, 17)]
    [TestCase(13, 19)]
    public void GetTileFileName_DirtPath_ReturnsDirt(int col, int row) =>
        Assert.That(CottageSceneConfig.GetTileFileName(col, row), Is.EqualTo(Dirt));

    // ─── Outside special zones ────────────────────────────────────────────────

    [Test]
    public void GetTileFileName_Origin_ReturnsGrassOrGrassVar() =>
        Assert.That(CottageSceneConfig.GetTileFileName(0, 0),
            Is.AnyOf(Grass, GrassV));

    [Test]
    public void GetTileFileName_BottomRight_ReturnsGrassOrGrassVar() =>
        Assert.That(CottageSceneConfig.GetTileFileName(29, 19),
            Is.AnyOf(Grass, GrassV));

    // ─── GetTileFileName is deterministic ─────────────────────────────────────

    [Test]
    public void GetTileFileName_CalledTwice_ReturnsSameResult()
    {
        for (int col = 0; col < 10; col++)
            for (int row = 0; row < 10; row++)
            {
                var first = CottageSceneConfig.GetTileFileName(col, row);
                var second = CottageSceneConfig.GetTileFileName(col, row);
                Assert.That(first, Is.EqualTo(second), $"Non-deterministic at ({col},{row})");
            }
    }

    // ─── All 600 cells return a known tile ────────────────────────────────────

    [Test]
    public void GetTileFileName_AllCells_ReturnValidTileFileName()
    {
        var valid = new HashSet<string>
        {
            Roof, Wall, Stone, Farm, FenceH, FenceV, Dirt, Grass, GrassV,
        };
        for (int row = 0; row < CottageSceneConfig.GridRows; row++)
            for (int col = 0; col < CottageSceneConfig.GridColumns; col++)
            {
                var tile = CottageSceneConfig.GetTileFileName(col, row);
                Assert.That(valid.Contains(tile), Is.True, $"Unknown tile at ({col},{row}): '{tile}'");
            }
    }

    // ─── All 600 cells return a tile that exists in TileRegistry ─────────────

    [Test]
    public void GetTileFileName_AllCells_ReturnTileRegisteredInTileRegistry()
    {
        var registeredNames = TileRegistry.All.Select(t => t.FileName).ToHashSet();
        for (int row = 0; row < CottageSceneConfig.GridRows; row++)
            for (int col = 0; col < CottageSceneConfig.GridColumns; col++)
            {
                var tile = CottageSceneConfig.GetTileFileName(col, row);
                Assert.That(registeredNames.Contains(tile), Is.True,
                    $"Tile '{tile}' at ({col},{row}) is not in TileRegistry");
            }
    }

    // ─── Adjacent rows show correct zone boundary transitions ─────────────────

    [Test]
    public void Row8_Col14_IsNotRoof() =>
        Assert.That(CottageSceneConfig.GetTileFileName(14, 8), Is.Not.EqualTo(Roof));

    [Test]
    public void Row9_Col14_IsRoof() =>
        Assert.That(CottageSceneConfig.GetTileFileName(14, 9), Is.EqualTo(Roof));

    [Test]
    public void Row10_Col14_IsWall() =>
        Assert.That(CottageSceneConfig.GetTileFileName(14, 10), Is.EqualTo(Wall));

    [Test]
    public void Row11_Col14_IsStone() =>
        Assert.That(CottageSceneConfig.GetTileFileName(14, 11), Is.EqualTo(Stone));

    [Test]
    public void Row14_Col14_IsDirt() =>
        Assert.That(CottageSceneConfig.GetTileFileName(14, 14), Is.EqualTo(Dirt));

    [Test]
    public void Col11_Row9_IsBeyondRoof() =>
        Assert.That(CottageSceneConfig.GetTileFileName(11, 9), Is.Not.EqualTo(Roof));

    [Test]
    public void Col16_Row10_IsBeyondWall() =>
        Assert.That(CottageSceneConfig.GetTileFileName(16, 10), Is.Not.EqualTo(Wall));
}

// ═══════════════════════════════════════════════════════════════════════════════
// CottageSceneConfig — GetSourceId Tests
// ═══════════════════════════════════════════════════════════════════════════════

[TestFixture]
public class CottageSceneGetSourceIdTest
{
    [Test]
    public void GetSourceId_Grass_Returns0() =>
        Assert.That(CottageSceneConfig.GetSourceId(CottageSceneConfig.TileGrass),
            Is.EqualTo(CottageSceneConfig.SourceIdGrass));

    [Test]
    public void GetSourceId_GrassVar_Returns1() =>
        Assert.That(CottageSceneConfig.GetSourceId(CottageSceneConfig.TileGrassVar),
            Is.EqualTo(CottageSceneConfig.SourceIdGrassVar));

    [Test]
    public void GetSourceId_Dirt_Returns2() =>
        Assert.That(CottageSceneConfig.GetSourceId(CottageSceneConfig.TileDirt),
            Is.EqualTo(CottageSceneConfig.SourceIdDirt));

    [Test]
    public void GetSourceId_Farm_Returns3() =>
        Assert.That(CottageSceneConfig.GetSourceId(CottageSceneConfig.TileFarm),
            Is.EqualTo(CottageSceneConfig.SourceIdFarm));

    [Test]
    public void GetSourceId_Stone_Returns4() =>
        Assert.That(CottageSceneConfig.GetSourceId(CottageSceneConfig.TileStone),
            Is.EqualTo(CottageSceneConfig.SourceIdStone));

    [Test]
    public void GetSourceId_Wall_Returns6() =>
        Assert.That(CottageSceneConfig.GetSourceId(CottageSceneConfig.TileWall),
            Is.EqualTo(CottageSceneConfig.SourceIdWall));

    [Test]
    public void GetSourceId_Roof_Returns7() =>
        Assert.That(CottageSceneConfig.GetSourceId(CottageSceneConfig.TileRoof),
            Is.EqualTo(CottageSceneConfig.SourceIdRoof));

    [Test]
    public void GetSourceId_FenceH_Returns8() =>
        Assert.That(CottageSceneConfig.GetSourceId(CottageSceneConfig.TileFenceH),
            Is.EqualTo(CottageSceneConfig.SourceIdFenceH));

    [Test]
    public void GetSourceId_FenceV_Returns9() =>
        Assert.That(CottageSceneConfig.GetSourceId(CottageSceneConfig.TileFenceV),
            Is.EqualTo(CottageSceneConfig.SourceIdFenceV));

    [Test]
    public void GetSourceId_Shadow_Returns10() =>
        Assert.That(CottageSceneConfig.GetSourceId(CottageSceneConfig.TileShadow),
            Is.EqualTo(CottageSceneConfig.SourceIdShadow));

    [Test]
    public void GetSourceId_UnknownTile_ThrowsArgumentException() =>
        Assert.Throws<System.ArgumentException>(() =>
            CottageSceneConfig.GetSourceId("unknown_tile.png"));

    [Test]
    public void GetSourceId_EmptyString_ThrowsArgumentException() =>
        Assert.Throws<System.ArgumentException>(() =>
            CottageSceneConfig.GetSourceId(""));

    // ─── Round-trip: all 600 cells produce valid source IDs ──────────────────

    [Test]
    public void GetSourceId_AllCells_ReturnNonNegativeId()
    {
        for (int row = 0; row < CottageSceneConfig.GridRows; row++)
            for (int col = 0; col < CottageSceneConfig.GridColumns; col++)
            {
                var tile = CottageSceneConfig.GetTileFileName(col, row);
                var id = CottageSceneConfig.GetSourceId(tile);
                Assert.That(id, Is.GreaterThanOrEqualTo(0), $"Negative source ID at ({col},{row})");
            }
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// CottageSceneConfig — IsGrassVariation Tests
// ═══════════════════════════════════════════════════════════════════════════════

[TestFixture]
public class CottageSceneGrassVariationTest
{
    [Test]
    public void IsGrassVariation_IsDeterministic()
    {
        for (int c = 0; c < 5; c++)
            for (int r = 0; r < 5; r++)
            {
                var a = CottageSceneConfig.IsGrassVariation(c, r);
                var b = CottageSceneConfig.IsGrassVariation(c, r);
                Assert.That(a, Is.EqualTo(b));
            }
    }

    [Test]
    public void IsGrassVariation_ReturnsTrueForSomeCells()
    {
        bool any = false;
        for (int c = 0; c < CottageSceneConfig.GridColumns; c++)
            for (int r = 0; r < CottageSceneConfig.GridRows; r++)
                if (CottageSceneConfig.IsGrassVariation(c, r)) { any = true; break; }
        Assert.That(any, Is.True, "IsGrassVariation never returns true");
    }

    [Test]
    public void IsGrassVariation_ReturnsFalseForSomeCells()
    {
        bool any = false;
        for (int c = 0; c < CottageSceneConfig.GridColumns; c++)
            for (int r = 0; r < CottageSceneConfig.GridRows; r++)
                if (!CottageSceneConfig.IsGrassVariation(c, r)) { any = true; break; }
        Assert.That(any, Is.True, "IsGrassVariation always returns true");
    }

    // col=0,row=0: (0*3+0*7)%5 = 0 → true
    [Test]
    public void IsGrassVariation_0_0_ReturnsTrue() =>
        Assert.That(CottageSceneConfig.IsGrassVariation(0, 0), Is.True);

    // col=1,row=0: (3+0)%5 = 3 → false
    [Test]
    public void IsGrassVariation_1_0_ReturnsFalse() =>
        Assert.That(CottageSceneConfig.IsGrassVariation(1, 0), Is.False);

    // col=0,row=1: (0+7)%5 = 2 → false
    [Test]
    public void IsGrassVariation_0_1_ReturnsFalse() =>
        Assert.That(CottageSceneConfig.IsGrassVariation(0, 1), Is.False);
}

// ═══════════════════════════════════════════════════════════════════════════════
// CottageSceneConfig — Coordinate Helper Tests
// ═══════════════════════════════════════════════════════════════════════════════

[TestFixture]
public class CottageSceneCoordinateTest
{
    // ─── TileToWorldX ─────────────────────────────────────────────────────────

    [Test]
    public void TileToWorldX_Origin_IsZero() =>
        Assert.That(CottageSceneConfig.TileToWorldX(0, 0), Is.EqualTo(0f));

    [Test]
    public void TileToWorldX_SpawnTile_EqualsSpawnWorldX() =>
        Assert.That(
            CottageSceneConfig.TileToWorldX(CottageSceneConfig.SpawnTileCol, CottageSceneConfig.SpawnTileRow),
            Is.EqualTo(CottageSceneConfig.SpawnWorldX).Within(0.001f));

    [Test]
    public void TileToWorldX_Col1_Row0_Is32() =>
        Assert.That(CottageSceneConfig.TileToWorldX(1, 0), Is.EqualTo(32f));

    [Test]
    public void TileToWorldX_Col0_Row1_IsNegative32() =>
        Assert.That(CottageSceneConfig.TileToWorldX(0, 1), Is.EqualTo(-32f));

    [Test]
    public void TileToWorldX_SymmetricAroundDiagonal()
    {
        // TileToWorldX(n, 0) = -TileToWorldX(0, n) for any n
        for (int n = 1; n <= 5; n++)
            Assert.That(CottageSceneConfig.TileToWorldX(n, 0),
                Is.EqualTo(-CottageSceneConfig.TileToWorldX(0, n)).Within(0.001f));
    }

    // ─── TileToWorldY ─────────────────────────────────────────────────────────

    [Test]
    public void TileToWorldY_Origin_IsZero() =>
        Assert.That(CottageSceneConfig.TileToWorldY(0, 0), Is.EqualTo(0f));

    [Test]
    public void TileToWorldY_SpawnTile_EqualsSpawnWorldY() =>
        Assert.That(
            CottageSceneConfig.TileToWorldY(CottageSceneConfig.SpawnTileCol, CottageSceneConfig.SpawnTileRow),
            Is.EqualTo(CottageSceneConfig.SpawnWorldY).Within(0.001f));

    [Test]
    public void TileToWorldY_Col1_Row0_Is16() =>
        Assert.That(CottageSceneConfig.TileToWorldY(1, 0), Is.EqualTo(16f));

    [Test]
    public void TileToWorldY_Col0_Row1_Is16() =>
        Assert.That(CottageSceneConfig.TileToWorldY(0, 1), Is.EqualTo(16f));

    [Test]
    public void TileToWorldY_AlwaysNonNegativeForNonNegativeTiles()
    {
        for (int c = 0; c < 5; c++)
            for (int r = 0; r < 5; r++)
                Assert.That(CottageSceneConfig.TileToWorldY(c, r), Is.GreaterThanOrEqualTo(0f));
    }

    // ─── Grid corner world positions ──────────────────────────────────────────

    [Test]
    public void RightCorner_World_Is928_464()
    {
        Assert.That(CottageSceneConfig.TileToWorldX(29, 0), Is.EqualTo(928f));
        Assert.That(CottageSceneConfig.TileToWorldY(29, 0), Is.EqualTo(464f));
    }

    [Test]
    public void BottomCorner_World_Is320_768()
    {
        Assert.That(CottageSceneConfig.TileToWorldX(29, 19), Is.EqualTo(320f));
        Assert.That(CottageSceneConfig.TileToWorldY(29, 19), Is.EqualTo(768f));
    }

    [Test]
    public void LeftCorner_World_IsNeg608_304()
    {
        Assert.That(CottageSceneConfig.TileToWorldX(0, 19), Is.EqualTo(-608f));
        Assert.That(CottageSceneConfig.TileToWorldY(0, 19), Is.EqualTo(304f));
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// CottageSceneConfig — PropPlacement Tests
// ═══════════════════════════════════════════════════════════════════════════════

[TestFixture]
public class CottageScenePropTest
{
    // ─── Count ────────────────────────────────────────────────────────────────

    [Test]
    public void Props_LengthMatchesPropCount() =>
        Assert.That(CottageSceneConfig.Props.Length, Is.EqualTo(CottageSceneConfig.PropCount));

    [Test]
    public void PropCount_Is12() =>
        Assert.That(CottageSceneConfig.PropCount, Is.EqualTo(12));

    // ─── All positions are within grid ────────────────────────────────────────

    [Test]
    public void Props_AllColumnsWithinGrid()
    {
        foreach (var p in CottageSceneConfig.Props)
            Assert.That(p.Col, Is.InRange(0, CottageSceneConfig.GridColumns - 1),
                $"Prop '{p.FileName}' col {p.Col} is out of bounds");
    }

    [Test]
    public void Props_AllRowsWithinGrid()
    {
        foreach (var p in CottageSceneConfig.Props)
            Assert.That(p.Row, Is.InRange(0, CottageSceneConfig.GridRows - 1),
                $"Prop '{p.FileName}' row {p.Row} is out of bounds");
    }

    // ─── Required prop types present ──────────────────────────────────────────

    [Test]
    public void Props_ContainsWell()
    {
        Assert.That(CottageSceneConfig.Props.Any(p => p.FileName == PropRegistry.Well.FileName), Is.True);
    }

    [Test]
    public void Props_ContainsAtLeastFourTrees()
    {
        int count = CottageSceneConfig.Props.Count(p => p.FileName == PropRegistry.Tree.FileName);
        Assert.That(count, Is.GreaterThanOrEqualTo(4));
    }

    [Test]
    public void Props_ContainsTwoHayBales()
    {
        int count = CottageSceneConfig.Props.Count(p => p.FileName == PropRegistry.HayBale.FileName);
        Assert.That(count, Is.EqualTo(2));
    }

    [Test]
    public void Props_ContainsFourFencePosts()
    {
        int count = CottageSceneConfig.Props.Count(p => p.FileName == PropRegistry.FencePost.FileName);
        Assert.That(count, Is.EqualTo(4));
    }

    [Test]
    public void Props_ContainsDoor()
    {
        Assert.That(CottageSceneConfig.Props.Any(p => p.FileName == PropRegistry.Door.FileName), Is.True);
    }

    // ─── All file names in PropRegistry ───────────────────────────────────────

    [Test]
    public void Props_AllFileNames_AreInPropRegistry()
    {
        var known = PropRegistry.All.Select(p => p.FileName).ToHashSet();
        foreach (var prop in CottageSceneConfig.Props)
            Assert.That(known.Contains(prop.FileName), Is.True,
                $"Prop '{prop.FileName}' is not registered in PropRegistry");
    }

    // ─── No duplicate positions ────────────────────────────────────────────────

    [Test]
    public void Props_NoDuplicatePositions()
    {
        var positions = CottageSceneConfig.Props.Select(p => (p.Col, p.Row)).ToList();
        Assert.That(positions.Distinct().Count(), Is.EqualTo(positions.Count),
            "Two or more props share the same tile position");
    }

    // ─── PropPlacement record equality ────────────────────────────────────────

    [Test]
    public void PropPlacement_EqualValues_AreEqual() =>
        Assert.That(
            new CottageSceneConfig.PropPlacement("prop_well.png", 10, 11),
            Is.EqualTo(new CottageSceneConfig.PropPlacement("prop_well.png", 10, 11)));

    [Test]
    public void PropPlacement_DifferentFileName_AreNotEqual() =>
        Assert.That(
            new CottageSceneConfig.PropPlacement("prop_door.png", 10, 11),
            Is.Not.EqualTo(new CottageSceneConfig.PropPlacement("prop_well.png", 10, 11)));

    [Test]
    public void PropPlacement_DifferentCol_AreNotEqual() =>
        Assert.That(
            new CottageSceneConfig.PropPlacement("prop_well.png", 9, 11),
            Is.Not.EqualTo(new CottageSceneConfig.PropPlacement("prop_well.png", 10, 11)));
}
