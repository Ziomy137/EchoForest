using System.Linq;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// Tests for <see cref="PhysicsLayers"/> constants. Validates that layer IDs
/// match the expected Godot project layer configuration.
/// </summary>
[TestFixture]
public class PhysicsLayersTest
{
    // ── Individual layer values ───────────────────────────────────────────────

    [Test]
    public void PhysicsLayers_WorldLayer_IsOne() =>
        Assert.That(PhysicsLayers.World, Is.EqualTo(1));

    [Test]
    public void PhysicsLayers_PlayerLayer_IsTwo() =>
        Assert.That(PhysicsLayers.Player, Is.EqualTo(2));

    [Test]
    public void PhysicsLayers_NpcsLayer_IsThree() =>
        Assert.That(PhysicsLayers.Npcs, Is.EqualTo(3));

    [Test]
    public void PhysicsLayers_InteractablesLayer_IsFour() =>
        Assert.That(PhysicsLayers.Interactables, Is.EqualTo(4));

    // ── All collection ────────────────────────────────────────────────────────

    [Test]
    public void PhysicsLayers_All_HasFourEntries() =>
        Assert.That(PhysicsLayers.All, Has.Length.EqualTo(4));

    [Test]
    public void PhysicsLayers_All_AreUnique() =>
        Assert.That(PhysicsLayers.All.Distinct().Count(), Is.EqualTo(PhysicsLayers.All.Length));

    [Test]
    public void PhysicsLayers_All_ContainsAllIndividualLayers()
    {
        Assert.That(PhysicsLayers.All, Does.Contain(PhysicsLayers.World));
        Assert.That(PhysicsLayers.All, Does.Contain(PhysicsLayers.Player));
        Assert.That(PhysicsLayers.All, Does.Contain(PhysicsLayers.Npcs));
        Assert.That(PhysicsLayers.All, Does.Contain(PhysicsLayers.Interactables));
    }

    // ── Consistency with Constants.Layers ────────────────────────────────────

    [Test]
    public void PhysicsLayers_MatchesConstantsLayers()
    {
        Assert.Multiple(() =>
        {
            Assert.That(PhysicsLayers.World, Is.EqualTo(Constants.Layers.World));
            Assert.That(PhysicsLayers.Player, Is.EqualTo(Constants.Layers.Player));
            Assert.That(PhysicsLayers.Npcs, Is.EqualTo(Constants.Layers.Npcs));
            Assert.That(PhysicsLayers.Interactables, Is.EqualTo(Constants.Layers.Interactables));
        });
    }
}
