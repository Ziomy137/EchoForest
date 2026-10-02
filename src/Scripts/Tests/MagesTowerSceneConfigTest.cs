using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class MagesTowerSceneConfigTest
{
    [Test]
    public void ScenePath_UsesRegisteredTowerScene() =>
        Assert.That(MagesTowerSceneConfig.SceneResPath, Is.EqualTo(MainMenuConfig.MagesTowerScenePath));

    [Test]
    public void Layout_HasSeparateExteriorAndInteriorRooms()
    {
        Assert.Multiple(() =>
        {
            Assert.That(MagesTowerSceneConfig.GridColumns, Is.EqualTo(40));
            Assert.That(MagesTowerSceneConfig.GridRows, Is.EqualTo(24));
            Assert.That(MagesTowerSceneConfig.ExteriorRoom.TileCount, Is.GreaterThan(0));
            Assert.That(MagesTowerSceneConfig.InteriorRoom.TileCount, Is.GreaterThan(0));
            Assert.That(MagesTowerSceneConfig.ExteriorRoom.Contains(MagesTowerSceneConfig.ExteriorEntryPosition.Col, MagesTowerSceneConfig.ExteriorEntryPosition.Row), Is.True);
            Assert.That(MagesTowerSceneConfig.InteriorRoom.Contains(MagesTowerSceneConfig.InteriorEntryPosition.Col, MagesTowerSceneConfig.InteriorEntryPosition.Row), Is.True);
        });
    }

    [Test]
    public void Doorway_ConnectsTheExteriorAndInteriorRooms()
    {
        Assert.Multiple(() =>
        {
            Assert.That(MagesTowerSceneConfig.Doorway.Contains(MagesTowerSceneConfig.ExteriorDoorTransitionPosition.Col, MagesTowerSceneConfig.ExteriorDoorTransitionPosition.Row), Is.True);
            Assert.That(MagesTowerSceneConfig.Doorway.Contains(MagesTowerSceneConfig.InteriorDoorTransitionPosition.Col, MagesTowerSceneConfig.InteriorDoorTransitionPosition.Row), Is.True);
            Assert.That(MagesTowerSceneConfig.Doorway.Contains(MagesTowerSceneConfig.ExteriorEntryPosition.Col, MagesTowerSceneConfig.ExteriorEntryPosition.Row), Is.False);
            Assert.That(MagesTowerSceneConfig.Doorway.Contains(MagesTowerSceneConfig.InteriorEntryPosition.Col, MagesTowerSceneConfig.InteriorEntryPosition.Row), Is.False);
        });
    }

    [Test]
    public void RequiredTowerProps_AreRegisteredAndPlaced()
    {
        var propFileNames = MagesTowerSceneConfig.Props.Select(prop => prop.FileName).ToHashSet();
        Assert.That(propFileNames, Is.SupersetOf(new[]
        {
            PropRegistry.PortalInactive.FileName,
            PropRegistry.Bookshelf.FileName,
            PropRegistry.AlchemyTable.FileName,
            PropRegistry.Fireplace.FileName,
        }));
    }

    [Test]
    public void TransitionEndpoints_ContainDoorRoundTripAndCityExit()
    {
        Assert.That(MagesTowerSceneConfig.Transitions.Select(transition => transition.Type), Is.EquivalentTo(new[]
        {
            MagesTowerSceneConfig.TransitionType.DoorToInterior,
            MagesTowerSceneConfig.TransitionType.DoorToExterior,
            MagesTowerSceneConfig.TransitionType.ExitToCity,
        }));
    }

    [Test]
    public void MageAnchor_IsInsideTowerInterior() =>
        Assert.That(MagesTowerSceneConfig.InteriorRoom.Contains(MagesTowerSceneConfig.MageAnchor.Col, MagesTowerSceneConfig.MageAnchor.Row), Is.True);

    [Test]
    public void RequiredTowerProps_HaveExpectedDimensions()
    {
        Assert.Multiple(() =>
        {
            Assert.That(PropRegistry.PortalInactive.Width, Is.EqualTo(64));
            Assert.That(PropRegistry.PortalInactive.Height, Is.EqualTo(64));
            Assert.That(PropRegistry.Bookshelf.Height, Is.GreaterThan(PropRegistry.PortalInactive.Height));
            Assert.That(PropRegistry.AlchemyTable.Width, Is.EqualTo(64));
            Assert.That(PropRegistry.Fireplace.Height, Is.EqualTo(64));
        });
    }

    [Test]
    public void AllCells_ResolveToRegisteredTiles()
    {
        var registeredTileFiles = TileRegistry.All.Select(tile => tile.FileName).ToHashSet();
        for (var row = 0; row < MagesTowerSceneConfig.GridRows; row++)
        {
            for (var col = 0; col < MagesTowerSceneConfig.GridColumns; col++)
            {
                var tileFileName = MagesTowerSceneConfig.GetTileFileName(col, row);
                Assert.That(registeredTileFiles.Contains(tileFileName), Is.True, $"Unknown tile at ({col}, {row})");
                Assert.That(MagesTowerSceneConfig.GetSourceId(tileFileName), Is.InRange(0, 25));
            }
        }
    }

    [Test]
    public void ExteriorApproach_IsNavigableFromCityEntranceToDoor()
    {
        Assert.That(IsReachable(
            MagesTowerSceneConfig.ExteriorEntryPosition,
            MagesTowerSceneConfig.ExteriorDoorTransitionPosition), Is.True);
    }

    [Test]
    public void TowerInterior_IsNavigableFromDoorToCityExit()
    {
        Assert.That(IsReachable(
            MagesTowerSceneConfig.InteriorEntryPosition,
            MagesTowerSceneConfig.CityExitTransitionPosition), Is.True);
    }

    private static bool IsReachable(MagesTowerSceneConfig.GridPosition start, MagesTowerSceneConfig.GridPosition destination)
    {
        var visited = new HashSet<MagesTowerSceneConfig.GridPosition> { start };
        var pending = new Queue<MagesTowerSceneConfig.GridPosition>();
        pending.Enqueue(start);

        while (pending.Count > 0)
        {
            var current = pending.Dequeue();
            if (current == destination)
                return true;

            foreach (var next in new[]
            {
                new MagesTowerSceneConfig.GridPosition(current.Col - 1, current.Row),
                new MagesTowerSceneConfig.GridPosition(current.Col + 1, current.Row),
                new MagesTowerSceneConfig.GridPosition(current.Col, current.Row - 1),
                new MagesTowerSceneConfig.GridPosition(current.Col, current.Row + 1),
            })
            {
                if (next.Col < 0 || next.Col >= MagesTowerSceneConfig.GridColumns
                    || next.Row < 0 || next.Row >= MagesTowerSceneConfig.GridRows
                    || !MagesTowerSceneConfig.IsWalkable(next.Col, next.Row)
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