using System;
using Godot;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// TDD RED: written before IsometricMath and MockTileMapService existed.
///
/// Strategy:
///   - IsometricMath (coordinate conversion) is pure math — tested directly in NUnit.
///     Godot's Vector2/Vector2I are value types available without the engine runtime.
///   - TileMapService wraps a Godot TileMap node — tested via GUT (test_tile_map_service.gd).
///   - MockTileMapService implements ITileMapService for use in all other NUnit tests.
/// </summary>
[TestFixture]
public class TileMapServiceTest
{
    // ── IsometricMath — coordinate round-trip ─────────────────────────────────

    [TestCase(3, 5)]
    [TestCase(0, 0)]
    [TestCase(1, 0)]
    [TestCase(0, 1)]
    [TestCase(-2, 4)]
    [TestCase(10, 10)]
    [TestCase(-5, -5)]
    public void TileToWorld_ThenWorldToTile_ReturnsSameTile(int col, int row)
    {
        var original = new Vector2I(col, row);
        var world = IsometricMath.TileToWorld(original);
        var back = IsometricMath.WorldToTile(world);
        Assert.That(back, Is.EqualTo(original));
    }

    [Test]
    public void TileToWorld_Origin_ReturnsWorldZero()
    {
        var world = IsometricMath.TileToWorld(new Vector2I(0, 0));
        Assert.That(world, Is.EqualTo(Vector2.Zero));
    }

    [Test]
    public void WorldToTile_WorldZero_ReturnsTileOrigin()
    {
        var tile = IsometricMath.WorldToTile(Vector2.Zero);
        Assert.That(tile, Is.EqualTo(Vector2I.Zero));
    }

    [Test]
    public void TileToWorld_IncreasingCol_IncreasesX()
    {
        // Moving right along columns: X increases, Y increases (isometric right)
        var w1 = IsometricMath.TileToWorld(new Vector2I(0, 0));
        var w2 = IsometricMath.TileToWorld(new Vector2I(1, 0));
        Assert.That(w2.X, Is.GreaterThan(w1.X));
    }

    [Test]
    public void TileToWorld_IncreasingRow_DecreasesX()
    {
        // Moving down along rows: X decreases (isometric left)
        var w1 = IsometricMath.TileToWorld(new Vector2I(0, 0));
        var w2 = IsometricMath.TileToWorld(new Vector2I(0, 1));
        Assert.That(w2.X, Is.LessThan(w1.X));
    }

    [Test]
    public void TileToWorld_UsesCorrectTileSize()
    {
        // Tile(1,0): x = (1-0)*halfW = 32, y = (1+0)*halfH = 16
        var w = IsometricMath.TileToWorld(new Vector2I(1, 0));
        Assert.That(w.X, Is.EqualTo(Constants.TileWidth / 2f));
        Assert.That(w.Y, Is.EqualTo(Constants.TileHeight / 2f));
    }

    [Test]
    public void TileToWorld_Tile_3_5_IsCorrect()
    {
        // col=3, row=5: x=(3-5)*32=-64, y=(3+5)*16=128
        var w = IsometricMath.TileToWorld(new Vector2I(3, 5));
        Assert.That(w.X, Is.EqualTo(-64f));
        Assert.That(w.Y, Is.EqualTo(128f));
    }

    // ── MockTileMapService — implements ITileMapService ───────────────────────

    [Test]
    public void MockTileMapService_ImplementsITileMapService() =>
        Assert.That(new MockTileMapService(), Is.InstanceOf<ITileMapService>());

    [Test]
    public void IsWalkable_TileSetAsWalkable_ReturnsTrue()
    {
        var mock = new MockTileMapService();
        mock.SetWalkable(new Vector2I(2, 3), true);
        Assert.That(mock.IsWalkable(new Vector2I(2, 3)), Is.True);
    }

    [Test]
    public void IsWalkable_TileSetAsNotWalkable_ReturnsFalse()
    {
        var mock = new MockTileMapService();
        mock.SetWalkable(new Vector2I(2, 3), false);
        Assert.That(mock.IsWalkable(new Vector2I(2, 3)), Is.False);
    }

    [Test]
    public void IsWalkable_UnregisteredTile_ReturnsFalse()
    {
        var mock = new MockTileMapService();
        Assert.That(mock.IsWalkable(new Vector2I(99, 99)), Is.False);
    }

    [Test]
    public void GetTileAtPosition_UnregisteredPosition_ReturnsNull()
    {
        var mock = new MockTileMapService();
        var result = mock.GetTileAtPosition(new Vector2(9999f, 9999f));
        Assert.That(result, Is.Null);
    }

    [Test]
    public void MockTileMapService_WorldToTile_DelegatesToIsometricMath()
    {
        var mock = new MockTileMapService();
        var tile = new Vector2I(3, 5);
        var world = IsometricMath.TileToWorld(tile);
        Assert.That(mock.WorldToTile(world), Is.EqualTo(tile));
    }

    [Test]
    public void MockTileMapService_TileToWorld_DelegatesToIsometricMath()
    {
        var mock = new MockTileMapService();
        var tile = new Vector2I(3, 5);
        Assert.That(mock.TileToWorld(tile), Is.EqualTo(IsometricMath.TileToWorld(tile)));
    }

    [Test]
    public void MockTileMapService_RegisterTile_ThenRemove_ReturnsNotWalkable()
    {
        var mock = new MockTileMapService();
        mock.SetWalkable(new Vector2I(1, 1), true);
        mock.SetWalkable(new Vector2I(1, 1), false);
        Assert.That(mock.IsWalkable(new Vector2I(1, 1)), Is.False);
    }

    // ── IsometricMath — CalculateZIndex ──────────────────────────────────────

    [Test]
    public void CalculateZIndex_HigherY_ReturnsHigherZIndex()
    {
        int z1 = IsometricMath.CalculateZIndex(new Vector2(0f, 100f));
        int z2 = IsometricMath.CalculateZIndex(new Vector2(0f, 200f));
        Assert.That(z2, Is.GreaterThan(z1));
    }

    [Test]
    public void CalculateZIndex_SameX_DifferentY_AreDifferent()
    {
        int z1 = IsometricMath.CalculateZIndex(new Vector2(50f, 50f));
        int z2 = IsometricMath.CalculateZIndex(new Vector2(50f, 51f));
        Assert.That(z1, Is.Not.EqualTo(z2));
    }

    [Test]
    public void CalculateZIndex_ReturnsRoundedY()
    {
        Assert.That(IsometricMath.CalculateZIndex(new Vector2(0f, 128.4f)), Is.EqualTo(128));
        Assert.That(IsometricMath.CalculateZIndex(new Vector2(0f, 128.6f)), Is.EqualTo(129));
    }
}

