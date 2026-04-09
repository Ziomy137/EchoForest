using Godot;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// TDD RED: written before IsometricSorter existed.
///
/// IsometricSorter is the public depth-sorting API that game objects use.
/// It is pure C# (no Godot runtime needed) and fully covered by NUnit.
///
/// The Godot-coupled counterpart (IsometricYSorterNode) is excluded from coverage
/// and tested via GUT.
/// </summary>
[TestFixture]
public class IsometricSorterTest
{
    [Test]
    public void ZIndex_HigherY_ReturnsHigherZIndex()
    {
        int z1 = IsometricSorter.CalculateZIndex(new Vector2(0f, 100f));
        int z2 = IsometricSorter.CalculateZIndex(new Vector2(0f, 200f));
        Assert.That(z2, Is.GreaterThan(z1));
    }

    [Test]
    public void ZIndex_SameX_DifferentY_AreDifferent()
    {
        int z1 = IsometricSorter.CalculateZIndex(new Vector2(50f, 50f));
        int z2 = IsometricSorter.CalculateZIndex(new Vector2(50f, 51f));
        Assert.That(z1, Is.Not.EqualTo(z2));
    }

    [Test]
    public void ZIndex_Origin_ReturnsZero()
    {
        Assert.That(IsometricSorter.CalculateZIndex(Vector2.Zero), Is.EqualTo(0));
    }

    [Test]
    public void ZIndex_NegativeY_ReturnsNegativeOrZero()
    {
        int z = IsometricSorter.CalculateZIndex(new Vector2(0f, -100f));
        Assert.That(z, Is.LessThanOrEqualTo(0));
    }

    [Test]
    public void ZIndex_XAxisDoesNotAffectResult()
    {
        // Objects at same Y but different X must have same Z index
        int z1 = IsometricSorter.CalculateZIndex(new Vector2(0f, 128f));
        int z2 = IsometricSorter.CalculateZIndex(new Vector2(999f, 128f));
        Assert.That(z1, Is.EqualTo(z2));
    }

    [Test]
    public void ZIndex_LowerY_ReturnsLowerZIndex()
    {
        // Objects higher on screen (lower Y) should render behind (lower Z)
        int z1 = IsometricSorter.CalculateZIndex(new Vector2(0f, 50f));
        int z2 = IsometricSorter.CalculateZIndex(new Vector2(0f, 100f));
        Assert.That(z1, Is.LessThan(z2));
    }

    [TestCase(0f, 0, TestName = "Y=0 -> Z=0")]
    [TestCase(100f, 100, TestName = "Y=100 -> Z=100")]
    [TestCase(200f, 200, TestName = "Y=200 -> Z=200")]
    [TestCase(-50f, -50, TestName = "Y=-50 -> Z=-50")]
    public void ZIndex_IntegerY_ReturnsExactY(float y, int expected)
    {
        Assert.That(IsometricSorter.CalculateZIndex(new Vector2(0f, y)), Is.EqualTo(expected));
    }
}
