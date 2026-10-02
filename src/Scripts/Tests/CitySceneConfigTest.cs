using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class CitySceneConfigTest
{
    [Test]
    public void Grid_HasRequiredDimensions()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CitySceneConfig.GridColumns, Is.EqualTo(60));
            Assert.That(CitySceneConfig.GridRows, Is.EqualTo(60));
            Assert.That(CitySceneConfig.TotalCells, Is.EqualTo(3600));
        });
    }

    [Test]
    public void ScenePath_UsesRegisteredCityScene()
    {
        Assert.That(CitySceneConfig.SceneResPath, Is.EqualTo(MainMenuConfig.CityScenePath));
    }

    [Test]
    public void MainStreet_ConnectsTheSouthAndNorthEdges()
    {
        for (var row = 0; row < CitySceneConfig.GridRows; row++)
        {
            var tileFileName = CitySceneConfig.GetTileFileName(CitySceneConfig.MainStreet.Col, row);
            Assert.That(TileRegistry.GetByFileName(tileFileName)?.IsWalkable, Is.True, $"Main street blocked at row {row}.");
        }
    }

    [Test]
    public void AllCells_ReturnRegisteredTilesWithValidSourceIds()
    {
        var registeredTileNames = TileRegistry.All.Select(tile => tile.FileName).ToHashSet();

        for (var row = 0; row < CitySceneConfig.GridRows; row++)
        {
            for (var col = 0; col < CitySceneConfig.GridColumns; col++)
            {
                var tileFileName = CitySceneConfig.GetTileFileName(col, row);
                Assert.Multiple(() =>
                {
                    Assert.That(registeredTileNames.Contains(tileFileName), Is.True, $"Unregistered tile at ({col}, {row})");
                    Assert.That(CitySceneConfig.GetSourceId(tileFileName), Is.InRange(0, 25), $"Invalid source at ({col}, {row})");
                });
            }
        }
    }

    [Test]
    public void BuildingFootprints_DoNotOverlapMainStreets()
    {
        foreach (var building in CitySceneConfig.BuildingFootprints)
        {
            for (var row = building.Row; row < building.Row + building.Height; row++)
            {
                for (var col = building.Col; col < building.Col + building.Width; col++)
                {
                    Assert.That(CitySceneConfig.MainStreet.Contains(col, row), Is.False, $"Building blocks main street at ({col}, {row})");
                    Assert.That(CitySceneConfig.MarketCrossStreet.Contains(col, row), Is.False, $"Building blocks market cross street at ({col}, {row})");
                    Assert.That(CitySceneConfig.TowerAccessStreet.Contains(col, row), Is.False, $"Building blocks tower access at ({col}, {row})");
                }
            }
        }
    }

    [Test]
    public void City_IsNavigableFromSouthEntranceToTowerApproach()
    {
        var visited = new HashSet<CitySceneConfig.GridPosition>();
        var pending = new Queue<CitySceneConfig.GridPosition>();
        pending.Enqueue(CitySceneConfig.SouthEntranceSpawnGridPosition);
        visited.Add(CitySceneConfig.SouthEntranceSpawnGridPosition);

        while (pending.Count > 0)
        {
            var current = pending.Dequeue();
            foreach (var next in new[]
            {
                new CitySceneConfig.GridPosition(current.Col - 1, current.Row),
                new CitySceneConfig.GridPosition(current.Col + 1, current.Row),
                new CitySceneConfig.GridPosition(current.Col, current.Row - 1),
                new CitySceneConfig.GridPosition(current.Col, current.Row + 1),
            })
            {
                if (next.Col < 0 || next.Col >= CitySceneConfig.GridColumns || next.Row < 0 || next.Row >= CitySceneConfig.GridRows)
                    continue;
                if (!IsWalkable(next) || !visited.Add(next))
                    continue;

                pending.Enqueue(next);
            }
        }

        Assert.Multiple(() =>
        {
            Assert.That(visited, Does.Contain(CitySceneConfig.TowerTransitionGridPosition));
            Assert.That(visited, Does.Contain(CitySceneConfig.TowerExitSpawnGridPosition));
        });
    }

    [Test]
    public void CityBuildings_ResolveToBlockingTiles()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CitySceneConfig.GetTileFileName(CitySceneConfig.BuildingFootprints[0].Col, CitySceneConfig.BuildingFootprints[0].Row), Is.EqualTo(TileRegistry.CityRoof.FileName));
            Assert.That(TileRegistry.CityRoof.IsWalkable, Is.False);
            Assert.That(TileRegistry.CityWall.IsWalkable, Is.False);
        });
    }

    [Test]
    public void MarketDistrict_HasFourStallsAndCentralFountain()
    {
        var stalls = CitySceneConfig.Props.Where(prop => prop.FileName == PropRegistry.MarketStall.FileName).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(stalls, Has.Length.EqualTo(4));
            Assert.That(CitySceneConfig.Props.Count(prop => prop.FileName == PropRegistry.Fountain.FileName), Is.EqualTo(1));
            Assert.That(stalls.All(prop => !string.IsNullOrEmpty(prop.MarkerName)), Is.True);
        });
    }

    [Test]
    public void CityProps_ContainAllRequiredArtPackAssets()
    {
        var registeredProps = CitySceneConfig.Props.Select(prop => prop.FileName).ToHashSet();

        Assert.That(registeredProps, Is.SupersetOf(new[]
        {
            PropRegistry.LampPost.FileName,
            PropRegistry.Fountain.FileName,
            PropRegistry.MarketStall.FileName,
            PropRegistry.NoticeBoard.FileName,
            PropRegistry.CityGate.FileName,
            PropRegistry.TowerWall.FileName,
            PropRegistry.TowerDoor.FileName,
        }));
    }

    [Test]
    public void NpcAnchors_ContainMerchantGuardAndThreeTownspersons()
    {
        var anchorNames = CitySceneConfig.NpcAnchors.Select(anchor => anchor.Name).ToArray();

        Assert.That(anchorNames, Is.EquivalentTo(new[]
        {
            "MerchantAnchor",
            "CityGuardAnchor",
            "TownspersonAnchorOne",
            "TownspersonAnchorTwo",
            "TownspersonAnchorThree",
        }));
    }

    private static bool IsWalkable(CitySceneConfig.GridPosition position)
    {
        var tileFileName = CitySceneConfig.GetTileFileName(position.Col, position.Row);
        if (TileRegistry.GetByFileName(tileFileName)?.IsWalkable != true)
            return false;

        return !CitySceneConfig.Props.Any(prop =>
            prop.IsBlocking && prop.Col == position.Col && prop.Row == position.Row);
    }
}