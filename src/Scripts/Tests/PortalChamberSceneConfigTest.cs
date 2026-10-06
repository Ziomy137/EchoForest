using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class PortalChamberSceneConfigTest
{
    [Test]
    public void ScenePath_UsesRegisteredPortalChamberScene() =>
        Assert.That(PortalChamberSceneConfig.SceneResPath, Is.EqualTo(MainMenuConfig.PortalChamberScenePath));

    [Test]
    public void Layout_HasThirtyByThirtyGridAndCentralPlatform()
    {
        Assert.Multiple(() =>
        {
            Assert.That(PortalChamberSceneConfig.GridColumns, Is.EqualTo(30));
            Assert.That(PortalChamberSceneConfig.GridRows, Is.EqualTo(30));
            Assert.That(PortalChamberSceneConfig.CentralPlatform.Contains(
                PortalChamberSceneConfig.CentralPortalPosition.Col,
                PortalChamberSceneConfig.CentralPortalPosition.Row), Is.True);
            Assert.That(PortalChamberSceneConfig.NorthBossArena.Contains(
                PortalChamberSceneConfig.BossArenaPosition.Col,
                PortalChamberSceneConfig.BossArenaPosition.Row), Is.True);
        });
    }

    [Test]
    public void PortalPositions_IncludeCentralAndSouthEntrance()
    {
        Assert.That(PortalChamberSceneConfig.AnimatedPortalPositions, Is.EquivalentTo(new[]
        {
            PortalChamberSceneConfig.CentralPortalPosition,
            PortalChamberSceneConfig.EntrancePortalPosition,
        }));
    }

    [Test]
    public void RequiredProps_AreRegisteredAndPlaced()
    {
        var propFileNames = PortalChamberSceneConfig.Props.Select(prop => prop.FileName).ToHashSet();
        Assert.That(propFileNames, Is.SupersetOf(new[]
        {
            PropRegistry.MagicPillar.FileName,
            PropRegistry.Chains.FileName,
            PropRegistry.ChildCage.FileName,
        }));
    }

    [Test]
    public void GeneratedPropMetadata_IncludesAllDistinctSpriteColors()
    {
        Assert.Multiple(() =>
        {
            Assert.That(PropRegistry.MagicPillar.ExpectedColorHexCodes, Is.SupersetOf(new[] { "1a1a1a", "5c3d2e" }));
            Assert.That(PropRegistry.Chains.ExpectedColorHexCodes, Is.SupersetOf(new[] { "ffd700" }));
            Assert.That(PropRegistry.ChildCage.ExpectedColorHexCodes, Is.SupersetOf(new[] { "2a1a4a", "ffd700" }));
        });
    }

    [Test]
    public void PillarsAndCageBlockMovement_WhileChainsRemainDecorative()
    {
        Assert.Multiple(() =>
        {
            Assert.That(PortalChamberSceneConfig.IsWalkable(10, 11), Is.False);
            Assert.That(PortalChamberSceneConfig.IsWalkable(15, 5), Is.False);
            Assert.That(PortalChamberSceneConfig.IsWalkable(5, 10), Is.True);
        });
    }

    [Test]
    public void ExitTransition_ReturnsToTowerAtPortalExitSpawn()
    {
        var transition = PortalChamberSceneConfig.Transitions.Single();
        Assert.Multiple(() =>
        {
            Assert.That(transition.TargetArea, Is.EqualTo(MainMenuConfig.MagesTowerScenePath));
            Assert.That(transition.SpawnPointId, Is.EqualTo(MagesTowerSceneConfig.PortalExitSpawnPointId));
            Assert.That(transition.Position, Is.EqualTo(PortalChamberSceneConfig.TowerReturnTransitionPosition));
            var transitionWorldPosition = PortalChamberSceneConfig.GridToWorld(transition.Position);
            var spawnWorldPosition = PortalChamberSceneConfig.GridToWorld(PortalChamberSceneConfig.TowerEntranceSpawnPosition);
            Assert.That(System.Math.Abs(transitionWorldPosition.X - spawnWorldPosition.X), Is.GreaterThan(TileRegistry.TileWidth));
        });
    }

    [Test]
    public void Routes_ConnectSouthEntranceToPortalAndBossArena()
    {
        Assert.Multiple(() =>
        {
            Assert.That(IsReachable(PortalChamberSceneConfig.TowerEntranceSpawnPosition, PortalChamberSceneConfig.CentralPortalPosition), Is.True);
            Assert.That(IsReachable(PortalChamberSceneConfig.TowerEntranceSpawnPosition, PortalChamberSceneConfig.BossArenaPosition), Is.True);
        });
    }

    [Test]
    public void AllCells_ResolveToRegisteredTilesAndSourceIds()
    {
        var registeredTileFiles = TileRegistry.All.Select(tile => tile.FileName).ToHashSet();
        for (var row = 0; row < PortalChamberSceneConfig.GridRows; row++)
        {
            for (var col = 0; col < PortalChamberSceneConfig.GridColumns; col++)
            {
                var tileFileName = PortalChamberSceneConfig.GetTileFileName(col, row);
                Assert.That(registeredTileFiles.Contains(tileFileName), Is.True, $"Unknown tile at ({col}, {row})");
                Assert.That(PortalChamberSceneConfig.GetSourceId(tileFileName), Is.InRange(0, 27));
            }
        }
    }

    private static bool IsReachable(
        PortalChamberSceneConfig.GridPosition start,
        PortalChamberSceneConfig.GridPosition destination)
    {
        var visited = new HashSet<PortalChamberSceneConfig.GridPosition> { start };
        var pending = new Queue<PortalChamberSceneConfig.GridPosition>();
        pending.Enqueue(start);

        while (pending.Count > 0)
        {
            var current = pending.Dequeue();
            if (current == destination)
                return true;

            foreach (var next in new[]
            {
                new PortalChamberSceneConfig.GridPosition(current.Col - 1, current.Row),
                new PortalChamberSceneConfig.GridPosition(current.Col + 1, current.Row),
                new PortalChamberSceneConfig.GridPosition(current.Col, current.Row - 1),
                new PortalChamberSceneConfig.GridPosition(current.Col, current.Row + 1),
            })
            {
                if (next.Col < 0 || next.Col >= PortalChamberSceneConfig.GridColumns
                    || next.Row < 0 || next.Row >= PortalChamberSceneConfig.GridRows
                    || !PortalChamberSceneConfig.IsWalkable(next.Col, next.Row)
                    || !visited.Add(next))
                {
                    continue;
                }

                pending.Enqueue(next);
            }
        }

        return false;
    }
}