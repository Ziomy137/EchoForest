using System.Linq;
using NUnit.Framework;
using EchoForest.Core.Scenes;

namespace EchoForest.Tests;

/// <summary>
/// Tests for <see cref="TestAreaConfig"/>. Validates configuration constants
/// for the Cottage exterior test area without requiring the Godot runtime.
/// </summary>
[TestFixture]
public class TestAreaConfigTest
{
    // ── Scene path ────────────────────────────────────────────────────────────

    [Test]
    public void TestAreaConfig_ScenePath_StartsWithResProtocol()
    {
        Assert.That(TestAreaConfig.ScenePath, Does.StartWith("res://"));
    }

    [Test]
    public void TestAreaConfig_ScenePath_EndsWithTscn()
    {
        Assert.That(TestAreaConfig.ScenePath, Does.EndWith(".tscn"));
    }

    [Test]
    public void TestAreaConfig_ScenePath_ContainsCottageName()
    {
        Assert.That(TestAreaConfig.ScenePath, Does.Contain("Cottage"));
    }

    // ── Grid dimensions ───────────────────────────────────────────────────────

    [Test]
    public void TestAreaConfig_GridWidth_Is30()
    {
        Assert.That(TestAreaConfig.GridWidth, Is.EqualTo(30));
    }

    [Test]
    public void TestAreaConfig_GridHeight_Is20()
    {
        Assert.That(TestAreaConfig.GridHeight, Is.EqualTo(20));
    }

    [Test]
    public void TestAreaConfig_GridArea_Is600Tiles()
    {
        Assert.That(TestAreaConfig.GridWidth * TestAreaConfig.GridHeight, Is.EqualTo(600));
    }

    // ── Spawn point ───────────────────────────────────────────────────────────

    [Test]
    public void TestAreaConfig_SpawnTileX_IsWithinGrid()
    {
        Assert.That(TestAreaConfig.SpawnTileX,
            Is.InRange(0, TestAreaConfig.GridWidth - 1));
    }

    [Test]
    public void TestAreaConfig_SpawnTileY_IsWithinGrid()
    {
        Assert.That(TestAreaConfig.SpawnTileY,
            Is.InRange(0, TestAreaConfig.GridHeight - 1));
    }

    // ── Required node names ───────────────────────────────────────────────────

    [Test]
    public void TestAreaConfig_RequiredNodeNames_HasThreeEntries()
    {
        Assert.That(TestAreaConfig.RequiredNodeNames, Has.Length.EqualTo(3));
    }

    [Test]
    public void TestAreaConfig_RequiredNodeNames_AreUnique()
    {
        var names = TestAreaConfig.RequiredNodeNames;
        Assert.That(names.Distinct().Count(), Is.EqualTo(names.Length));
    }

    [TestCase(TestAreaConfig.TileMapLayerNodeName)]
    [TestCase(TestAreaConfig.PlayerSpawnPointNodeName)]
    [TestCase(TestAreaConfig.CollisionBoundaryNodeName)]
    public void TestAreaConfig_RequiredNodeNames_ContainsExpectedNode(string nodeName)
    {
        Assert.That(TestAreaConfig.RequiredNodeNames, Does.Contain(nodeName));
    }

    [Test]
    public void TestAreaConfig_NodeNameConstants_AreNonEmpty()
    {
        Assert.Multiple(() =>
        {
            Assert.That(TestAreaConfig.TileMapLayerNodeName, Is.Not.Empty);
            Assert.That(TestAreaConfig.PlayerSpawnPointNodeName, Is.Not.Empty);
            Assert.That(TestAreaConfig.CollisionBoundaryNodeName, Is.Not.Empty);
        });
    }

    // ── Zone tile counts ──────────────────────────────────────────────────────

    [Test]
    public void TestAreaConfig_FarmZoneTileCount_IsTwentyFive()
    {
        Assert.That(TestAreaConfig.FarmZoneTileCount, Is.EqualTo(25));
    }

    [Test]
    public void TestAreaConfig_CottageFloorTileCount_IsSixteen()
    {
        Assert.That(TestAreaConfig.CottageFloorTileCount, Is.EqualTo(16));
    }

    [Test]
    public void TestAreaConfig_ZoneTileCounts_AreLessThanTotalGrid()
    {
        int total = TestAreaConfig.GridWidth * TestAreaConfig.GridHeight;
        Assert.That(TestAreaConfig.FarmZoneTileCount, Is.LessThan(total));
        Assert.That(TestAreaConfig.CottageFloorTileCount, Is.LessThan(total));
    }
}
