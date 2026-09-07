using System.Linq;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class FarmSceneConfigTest
{
    [Test]
    public void Grid_HasRequiredDimensions()
    {
        Assert.Multiple(() =>
        {
            Assert.That(FarmSceneConfig.GridColumns, Is.EqualTo(40));
            Assert.That(FarmSceneConfig.GridRows, Is.EqualTo(25));
            Assert.That(FarmSceneConfig.TotalCells, Is.EqualTo(1000));
        });
    }

    [Test]
    public void ScenePath_UsesRegisteredFarmScene()
    {
        Assert.That(FarmSceneConfig.SceneResPath, Is.EqualTo(MainMenuConfig.FarmScenePath));
    }

    [Test]
    public void MatureCropField_IsSixByFour()
    {
        Assert.Multiple(() =>
        {
            Assert.That(FarmSceneConfig.MatureCropField.Width, Is.EqualTo(6));
            Assert.That(FarmSceneConfig.MatureCropField.Height, Is.EqualTo(4));
            Assert.That(FarmSceneConfig.MatureCropField.TileCount, Is.EqualTo(24));
        });
    }

    [Test]
    public void GetTileFileName_UsesMatureCropsAndIrrigationDitch()
    {
        Assert.Multiple(() =>
        {
            Assert.That(
                FarmSceneConfig.GetTileFileName(FarmSceneConfig.MatureCropField.Col, FarmSceneConfig.MatureCropField.Row),
                Is.EqualTo(TileRegistry.CropMature.FileName));
            Assert.That(
                FarmSceneConfig.GetTileFileName(FarmSceneConfig.IrrigationDitch.Col, FarmSceneConfig.IrrigationDitch.Row),
                Is.EqualTo(TileRegistry.Irrigation.FileName));
        });
    }

    [Test]
    public void AllCells_ReturnRegisteredTilesWithValidSourceIds()
    {
        var registeredTileNames = TileRegistry.All.Select(tile => tile.FileName).ToHashSet();

        for (var row = 0; row < FarmSceneConfig.GridRows; row++)
        {
            for (var col = 0; col < FarmSceneConfig.GridColumns; col++)
            {
                var tileFileName = FarmSceneConfig.GetTileFileName(col, row);
                Assert.Multiple(() =>
                {
                    Assert.That(registeredTileNames.Contains(tileFileName), Is.True, $"Unregistered tile at ({col}, {row})");
                    Assert.That(FarmSceneConfig.GetSourceId(tileFileName), Is.InRange(0, 15), $"Invalid source at ({col}, {row})");
                });
            }
        }
    }

    [Test]
    public void Props_ContainBarnScarecrowAndPlow()
    {
        Assert.That(FarmSceneConfig.Props.Select(prop => prop.FileName), Is.EquivalentTo(
        [
            PropRegistry.Barn.FileName,
            PropRegistry.Scarecrow.FileName,
            PropRegistry.Plow.FileName,
        ]));
    }

    [Test]
    public void Props_AllHaveCollisionConfigured()
    {
        Assert.That(FarmSceneConfig.Props.All(prop => prop.IsBlocking), Is.True);
    }

    [Test]
    public void NewFarmTiles_AreRegisteredAndWalkable()
    {
        Assert.Multiple(() =>
        {
            Assert.That(TileRegistry.CropYoung.IsWalkable, Is.True);
            Assert.That(TileRegistry.CropMature.IsWalkable, Is.True);
            Assert.That(TileRegistry.SoilDry.IsWalkable, Is.True);
            Assert.That(TileRegistry.Irrigation.IsWalkable, Is.True);
            Assert.That(TileRegistry.Hay.IsWalkable, Is.True);
        });
    }

    [Test]
    public void NewFarmAssets_HaveExpectedPixelDimensions()
    {
        Assert.Multiple(() =>
        {
            Assert.That(TileRegistry.CropYoung.Width, Is.EqualTo(TileRegistry.TileWidth));
            Assert.That(TileRegistry.CropMature.Height, Is.EqualTo(TileRegistry.TileHeight));
            Assert.That(PropRegistry.Barn.Width, Is.EqualTo(192));
            Assert.That(PropRegistry.Barn.Height, Is.EqualTo(160));
            Assert.That(PropRegistry.Scarecrow.Width, Is.EqualTo(32));
            Assert.That(PropRegistry.Plow.Height, Is.EqualTo(32));
        });
    }
}