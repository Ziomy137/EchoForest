namespace EchoForest.Core;

/// <summary>
/// Represents the movement and action state of the player character.
/// Default value is <see cref="Idle"/> (value = 0).
/// </summary>
public enum PlayerState
{
    Idle = 0,
    Walking = 1,
    Running = 2,
    Jumping = 3,
    Combat = 4,
}
