namespace EchoForest.Core;

/// <summary>
/// Defines the collision shape and physics layer settings for the player
/// <c>CharacterBody2D</c> node. All values match what is configured in
/// <c>Player.tscn</c> so that tests can verify correctness without the
/// Godot editor.
///
/// Sprite size is 16×24 px at 1× scale.  The capsule is sized to the
/// lower portion of the sprite so the player's feet align with the
/// isometric tile surface.
/// </summary>
public static class PlayerCollisionConfig
{
    // ── CapsuleShape2D dimensions ─────────────────────────────────────────────

    /// <summary>
    /// Capsule collision shape radius in pixels.
    /// Half the sprite width (16 px / 2 = 8, divided by 2 for isometric
    /// compression = 4 px).
    /// </summary>
    public const float CapsuleRadius = 4f;

    /// <summary>
    /// Capsule collision height in pixels (distance between the two end
    /// centres, not total height).  Set so the full shape height
    /// (<c>CapsuleHeight + 2 × CapsuleRadius</c>) equals 16 px — the
    /// lower two-thirds of the 24 px sprite.
    /// </summary>
    public const float CapsuleHeight = 8f;

    /// <summary>Total height of the capsule shape in pixels (height + 2 × radius).</summary>
    public const float CapsuleTotalHeight = CapsuleHeight + 2f * CapsuleRadius;

    // ── CharacterBody2D motion mode ───────────────────────────────────────────

    /// <summary>
    /// Godot <c>CharacterBody2D.MotionModeEnum</c> value.
    /// 0 = <c>Grounded</c> — default; handles floor/ceiling/wall normals.
    /// </summary>
    public const int MotionMode = 0; // CharacterBody2D.MotionModeEnum.Grounded

    // ── Physics layer bitmasks (collision_layer / collision_mask) ─────────────
    //
    // Godot uses 1-based layer numbers; bitmask = 2^(layerNumber − 1).

    /// <summary>
    /// <c>collision_layer</c> bitmask: player occupies layer 2.
    /// Bitmask = 2^(2−1) = 2.
    /// </summary>
    public const uint LayerBitmask = 1u << (PhysicsLayers.Player - 1);

    /// <summary>
    /// <c>collision_mask</c> bitmask: player collides with layer 1 (World).
    /// Bitmask = 2^(1−1) = 1.
    /// </summary>
    public const uint MaskBitmask = 1u << (PhysicsLayers.World - 1);
}
