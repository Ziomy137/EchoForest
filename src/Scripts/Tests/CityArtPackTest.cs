using System.Linq;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class CityArtPackTest
{
    [Test]
    public void CityTiles_RegistersAllSixRequiredFiles()
    {
        Assert.That(TileRegistry.All.Select(tile => tile.FileName), Does.Contain("tile_cobble.png"));
        Assert.That(TileRegistry.All.Select(tile => tile.FileName), Does.Contain("tile_cobble_var.png"));
        Assert.That(TileRegistry.All.Select(tile => tile.FileName), Does.Contain("tile_city_stone.png"));
        Assert.That(TileRegistry.All.Select(tile => tile.FileName), Does.Contain("tile_city_wall.png"));
        Assert.That(TileRegistry.All.Select(tile => tile.FileName), Does.Contain("tile_city_roof.png"));
        Assert.That(TileRegistry.All.Select(tile => tile.FileName), Does.Contain("tile_stall_top.png"));
    }

    [Test]
    public void CityProps_RegistersAllSevenRequiredFiles()
    {
        var fileNames = PropRegistry.All.Select(prop => prop.FileName).ToArray();

        Assert.That(fileNames, Is.SupersetOf(new[]
        {
            "prop_lamppost.png",
            "prop_fountain.png",
            "prop_market_stall.png",
            "prop_notice_board.png",
            "prop_city_gate.png",
            "prop_tower_wall.png",
            "prop_tower_door.png",
        }));
    }

    [Test]
    public void CityTileMetadata_UsesIsometricDimensionsAndApprovedColors()
    {
        var cityTiles = TileRegistry.All.Where(tile => tile.FileName.StartsWith("tile_cobble", System.StringComparison.Ordinal)
            || tile.FileName.StartsWith("tile_city_", System.StringComparison.Ordinal)
            || tile.FileName == "tile_stall_top.png").ToArray();
        var approvedColors = Palette.All.Select(color => color.ToHtml(false)).ToHashSet();

        Assert.That(cityTiles, Has.Length.EqualTo(6));
        foreach (var tile in cityTiles)
        {
            Assert.Multiple(() =>
            {
                Assert.That(tile.Width, Is.EqualTo(TileRegistry.TileWidth));
                Assert.That(tile.Height, Is.EqualTo(TileRegistry.TileHeight));
                Assert.That(tile.ExpectedColorHexCodes.All(approvedColors.Contains), Is.True, tile.Name);
            });
        }
    }

    [Test]
    public void CityProps_MetadataHasDimensionsAndApprovedColors()
    {
        var cityPropNames = new[]
        {
            "prop_lamppost.png",
            "prop_fountain.png",
            "prop_market_stall.png",
            "prop_notice_board.png",
            "prop_city_gate.png",
            "prop_tower_wall.png",
            "prop_tower_door.png",
        };
        var cityProps = PropRegistry.All.Where(prop => cityPropNames.Contains(prop.FileName)).ToArray();
        var approvedColors = Palette.All.Select(color => color.ToHtml(false).TrimStart('#').ToLowerInvariant()).ToHashSet();

        Assert.That(cityProps, Has.Length.EqualTo(7));
        foreach (var prop in cityProps)
        {
            Assert.Multiple(() =>
            {
                Assert.That(prop.Width, Is.GreaterThan(0), prop.Name);
                Assert.That(prop.Height, Is.GreaterThan(0), prop.Name);
                Assert.That(prop.ExpectedColorHexCodes.All(color => approvedColors.Contains(color.ToLowerInvariant())), Is.True, prop.Name);
            });
        }
    }

    [Test]
    public void CityGateAndTowerProps_AreLargerThanCottageArchitecture()
    {
        Assert.Multiple(() =>
        {
            Assert.That(PropRegistry.CityGate.Width, Is.GreaterThan(PropRegistry.Door.Width));
            Assert.That(PropRegistry.CityGate.Height, Is.GreaterThan(PropRegistry.Door.Height));
            Assert.That(PropRegistry.TowerWall.Width, Is.GreaterThanOrEqualTo(PropRegistry.Barn.Width));
            Assert.That(PropRegistry.TowerWall.Height, Is.GreaterThan(PropRegistry.Barn.Height));
        });
    }
}