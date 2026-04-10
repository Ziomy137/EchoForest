namespace EchoForest.Core;

/// <summary>
/// Named 1-based physics layer IDs for use with APIs that accept layer numbers,
/// such as <c>CollisionObject2D.SetCollisionLayerValue</c> and
/// <c>CollisionObject2D.SetCollisionMaskValue</c>.<br/>
/// These values delegate to <see cref="Constants.Layers"/> so there is a single
/// source of truth. They must match the layer names in
/// <em>Project Settings → Physics → 2D → Layer Names</em>.<br/>
/// Do not assign these IDs directly to <c>CollisionObject2D.CollisionLayer</c>
/// or <c>CollisionObject2D.CollisionMask</c>, because those properties use
/// bitmasks rather than 1-based layer numbers.
/// </summary>
public static class PhysicsLayers
{
    /// <summary>Static world geometry (walls, floors).</summary>
    public const int World = Constants.Layers.World;

    /// <summary>Player character.</summary>
    public const int Player = Constants.Layers.Player;

    /// <summary>Non-player characters.</summary>
    public const int Npcs = Constants.Layers.Npcs;

    /// <summary>Interactable objects (chests, doors, etc.).</summary>
    public const int Interactables = Constants.Layers.Interactables;

    /// <summary>All defined layer IDs in ascending order.</summary>
    public static readonly int[] All = Constants.Layers.All;
}
