using System.Linq;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class ForestPathSceneConfigTest
{
    [Test]
    public void Grid_HasRequiredDimensions()
    {
        Assert.Multiple(() =>
        {
            Assert.That(ForestPathSceneConfig.GridColumns, Is.EqualTo(35));
            Assert.That(ForestPathSceneConfig.GridRows, Is.EqualTo(50));
            Assert.That(ForestPathSceneConfig.TotalCells, Is.EqualTo(1750));
        });
    }

    [Test]
    public void ScenePath_UsesRegisteredForestPathScene()
    {
        Assert.That(ForestPathSceneConfig.SceneResPath, Is.EqualTo(MainMenuConfig.ForestPathScenePath));
    }

    [Test]
    public void WorldBounds_ContainTheEntireIsometricGrid()
    {
        var furthestGridY = (ForestPathSceneConfig.GridColumns - 1 + ForestPathSceneConfig.GridRows - 1) * TileRegistry.TileHeight / 2f;

        Assert.Multiple(() =>
        {
            Assert.That(ForestPathSceneConfig.WorldBoundaryTop, Is.LessThanOrEqualTo(0f));
            Assert.That(ForestPathSceneConfig.WorldBoundaryBottom, Is.GreaterThan(furthestGridY));
        });
    }

    [Test]
    public void MudPath_IsContinuousFromSouthToNorth()
    {
        for (var row = 0; row < ForestPathSceneConfig.GridRows; row++)
        {
            var pathColumn = ForestPathSceneConfig.GetPathColumn(row);
            Assert.That(pathColumn, Is.InRange(0, ForestPathSceneConfig.GridColumns - 1));
            Assert.That(ForestPathSceneConfig.GetTileFileName(pathColumn, row), Is.EqualTo(TileRegistry.Mud.FileName));
        }
    }

    [Test]
    public void MudPath_IsThreeTilesWideAtEveryRow()
    {
        for (var row = 0; row < ForestPathSceneConfig.GridRows; row++)
        {
            var centerColumn = ForestPathSceneConfig.GetPathColumn(row);
            Assert.Multiple(() =>
            {
                Assert.That(ForestPathSceneConfig.GetTileFileName(centerColumn - 1, row), Is.EqualTo(TileRegistry.Mud.FileName));
                Assert.That(ForestPathSceneConfig.GetTileFileName(centerColumn, row), Is.EqualTo(TileRegistry.Mud.FileName));
                Assert.That(ForestPathSceneConfig.GetTileFileName(centerColumn + 1, row), Is.EqualTo(TileRegistry.Mud.FileName));
            });
        }
    }

    [Test]
    public void StreamCrossing_DoesNotInterruptMudPath()
    {
        Assert.Multiple(() =>
        {
            Assert.That(ForestPathSceneConfig.StreamZone.Contains(ForestPathSceneConfig.GetPathColumn(ForestPathSceneConfig.StreamZone.Row), ForestPathSceneConfig.StreamZone.Row), Is.True);
            Assert.That(ForestPathSceneConfig.GetTileFileName(0, ForestPathSceneConfig.StreamZone.Row), Is.EqualTo(TileRegistry.Water.FileName));
            Assert.That(
                ForestPathSceneConfig.GetTileFileName(ForestPathSceneConfig.GetPathColumn(ForestPathSceneConfig.StreamZone.Row), ForestPathSceneConfig.StreamZone.Row),
                Is.EqualTo(TileRegistry.Mud.FileName));
        });
    }

    [Test]
    public void AllCells_ReturnRegisteredTilesWithValidSourceIds()
    {
        var registeredTileNames = TileRegistry.All.Select(tile => tile.FileName).ToHashSet();

        for (var row = 0; row < ForestPathSceneConfig.GridRows; row++)
        {
            for (var col = 0; col < ForestPathSceneConfig.GridColumns; col++)
            {
                var tileFileName = ForestPathSceneConfig.GetTileFileName(col, row);
                Assert.Multiple(() =>
                {
                    Assert.That(registeredTileNames.Contains(tileFileName), Is.True, $"Unregistered tile at ({col}, {row})");
                    Assert.That(ForestPathSceneConfig.GetSourceId(tileFileName), Is.InRange(0, 19), $"Invalid source at ({col}, {row})");
                });
            }
        }
    }

    [Test]
    public void Props_ContainRequiredForestFeatures()
    {
        Assert.That(ForestPathSceneConfig.Props.Select(prop => prop.FileName).Distinct(), Is.EquivalentTo(
        [
            PropRegistry.DenseTree.FileName,
            PropRegistry.FallenLog.FileName,
            PropRegistry.Mushrooms.FileName,
            PropRegistry.Boulder.FileName,
        ]));
    }

    [Test]
    public void BlockingProps_IncludeDenseTreesFallenLogAndBoulders()
    {
        var blockingFileNames = ForestPathSceneConfig.Props
            .Where(prop => prop.IsBlocking)
            .Select(prop => prop.FileName)
            .Distinct();

        Assert.That(blockingFileNames, Is.EquivalentTo(
        [
            PropRegistry.DenseTree.FileName,
            PropRegistry.FallenLog.FileName,
            PropRegistry.Boulder.FileName,
        ]));
    }

    [Test]
    public void BlockingProps_DoNotObstructTheMudPath()
    {
        foreach (var prop in ForestPathSceneConfig.Props.Where(prop => prop.IsBlocking))
        {
            var pathColumn = ForestPathSceneConfig.GetPathColumn(prop.Row);
            Assert.That(prop.Col, Is.Not.InRange(pathColumn - 1, pathColumn + 1),
                $"Blocking prop '{prop.FileName}' obstructs the mud path at row {prop.Row}.");
        }
    }

    [Test]
    public void DenseTreeColliders_CoverOneFullIsometricTileStep()
    {
        Assert.Multiple(() =>
        {
            Assert.That(ForestPathSceneConfig.DenseTreeColliderWidth, Is.EqualTo(TileRegistry.TileWidth));
            Assert.That(ForestPathSceneConfig.DenseTreeColliderHeight, Is.EqualTo(TileRegistry.TileHeight));
        });
    }

    [Test]
    public void HasTwoEnemyEncounterPositionsAndOneOptionalPickupPosition()
    {
        Assert.Multiple(() =>
        {
            Assert.That(ForestPathSceneConfig.EnemyEncounterPositions.Length, Is.EqualTo(2));
            Assert.That(ForestPathSceneConfig.OptionalItemPickupPosition, Is.Not.EqualTo(default(ForestPathSceneConfig.GridPosition)));
        });
    }

    [Test]
    public void NewForestAssets_HaveExpectedPixelDimensions()
    {
        Assert.Multiple(() =>
        {
            Assert.That(TileRegistry.ForestFloor.Width, Is.EqualTo(TileRegistry.TileWidth));
            Assert.That(TileRegistry.ForestShadow.Height, Is.EqualTo(TileRegistry.TileHeight));
            Assert.That(TileRegistry.Mud.Width, Is.EqualTo(TileRegistry.TileWidth));
            Assert.That(TileRegistry.RockSmall.Height, Is.EqualTo(TileRegistry.TileHeight));
            Assert.That(PropRegistry.DenseTree.Width, Is.EqualTo(64));
            Assert.That(PropRegistry.FallenLog.Height, Is.EqualTo(48));
            Assert.That(PropRegistry.Mushrooms.Width, Is.EqualTo(32));
            Assert.That(PropRegistry.Boulder.Height, Is.EqualTo(64));
        });
    }
}