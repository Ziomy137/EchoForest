namespace EchoForest.Core;

/// <summary>
/// Named physics layer IDs for use with <c>CollisionObject2D.CollisionLayer</c>
/// and <c>CollisionObject2D.CollisionMask</c>.<br/>
/// Values delegate to <see cref="Constants.Layers"/> so there is a single source
/// of truth. They must match the layer names in
/// <em>Project Settings → Physics → 2D → Layer Names</em>.
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
    public static readonly int[] All = { World, Player, Npcs, Interactables };
}
