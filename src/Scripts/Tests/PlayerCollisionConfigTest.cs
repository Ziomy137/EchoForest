using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// Tests for <see cref="PlayerCollisionConfig"/>. Validates that the
/// capsule shape dimensions and physics layer bitmasks are correct and
/// consistent with the values configured in <c>Player.tscn</c>.
/// </summary>
[TestFixture]
public class PlayerCollisionConfigTest
{
    // ── CapsuleShape2D dimensions ─────────────────────────────────────────────

    [Test]
    public void CapsuleRadius_IsPositive()
    {
        Assert.That(PlayerCollisionConfig.CapsuleRadius, Is.GreaterThan(0f));
    }

    [Test]
    public void CapsuleHeight_IsPositive()
    {
        Assert.That(PlayerCollisionConfig.CapsuleHeight, Is.GreaterThan(0f));
    }

    [Test]
    public void CapsuleTotalHeight_EqualsCapsuleHeightPlusTwoRadii()
    {
        float expected = PlayerCollisionConfig.CapsuleHeight
                         + 2f * PlayerCollisionConfig.CapsuleRadius;
        Assert.That(PlayerCollisionConfig.CapsuleTotalHeight, Is.EqualTo(expected));
    }

    [Test]
    public void CapsuleTotalHeight_IsGreaterThanRadius()
    {
        Assert.That(PlayerCollisionConfig.CapsuleTotalHeight,
            Is.GreaterThan(PlayerCollisionConfig.CapsuleRadius));
    }

    [Test]
    public void CapsuleRadius_IsLessThanHalfTileWidth()
    {
        // Capsule must fit within the isometric tile (64 px wide).
        Assert.That(PlayerCollisionConfig.CapsuleRadius,
            Is.LessThan(Constants.TileWidth / 2f));
    }

    [Test]
    public void CapsuleTotalHeight_FitsWithinSpriteBounds()
    {
        // Sprite is 24 px tall; shape must not exceed that.
        const float spriteHeight = 24f;
        Assert.That(PlayerCollisionConfig.CapsuleTotalHeight,
            Is.LessThanOrEqualTo(spriteHeight));
    }

    [Test]
    public void CapsuleRadius_IsEqualTo_4()
    {
        Assert.That(PlayerCollisionConfig.CapsuleRadius, Is.EqualTo(4f));
    }

    [Test]
    public void CapsuleHeight_IsEqualTo_8()
    {
        Assert.That(PlayerCollisionConfig.CapsuleHeight, Is.EqualTo(8f));
    }

    // ── Motion mode ───────────────────────────────────────────────────────────

    [Test]
    public void MotionMode_IsGrounded()
    {
        // 0 = CharacterBody2D.MotionModeEnum.Grounded
        Assert.That(PlayerCollisionConfig.MotionMode, Is.EqualTo(0));
    }

    // ── Layer bitmasks ────────────────────────────────────────────────────────

    [Test]
    public void LayerBitmask_CorrespondsToPlayerLayer()
    {
        // Player is on layer 2 → bitmask = 2^(2-1) = 2
        uint expected = 1u << (PhysicsLayers.Player - 1);
        Assert.That(PlayerCollisionConfig.LayerBitmask, Is.EqualTo(expected));
    }

    [Test]
    public void LayerBitmask_IsEqualTo_2()
    {
        Assert.That(PlayerCollisionConfig.LayerBitmask, Is.EqualTo(2u));
    }

    [Test]
    public void MaskBitmask_CorrespondsToWorldLayer()
    {
        // World is layer 1 → bitmask = 2^(1-1) = 1
        uint expected = 1u << (PhysicsLayers.World - 1);
        Assert.That(PlayerCollisionConfig.MaskBitmask, Is.EqualTo(expected));
    }

    [Test]
    public void MaskBitmask_IsEqualTo_1()
    {
        Assert.That(PlayerCollisionConfig.MaskBitmask, Is.EqualTo(1u));
    }

    [Test]
    public void LayerBitmask_AndMaskBitmask_AreDifferent()
    {
        Assert.That(PlayerCollisionConfig.LayerBitmask,
            Is.Not.EqualTo(PlayerCollisionConfig.MaskBitmask));
    }

    // ── Cross-checks with PhysicsLayers ──────────────────────────────────────

    [Test]
    public void LayerBitmask_IsConsistentWith_PhysicsLayersPlayer()
    {
        // LayerBitmask encodes the same layer number as PhysicsLayers.Player
        int layerNumber = PhysicsLayers.Player;
        uint expectedBitmask = 1u << (layerNumber - 1);
        Assert.That(PlayerCollisionConfig.LayerBitmask, Is.EqualTo(expectedBitmask));
    }

    [Test]
    public void MaskBitmask_IsConsistentWith_PhysicsLayersWorld()
    {
        int layerNumber = PhysicsLayers.World;
        uint expectedBitmask = 1u << (layerNumber - 1);
        Assert.That(PlayerCollisionConfig.MaskBitmask, Is.EqualTo(expectedBitmask));
    }
}
