using System;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# animation state controller. Maps a (<see cref="PlayerState"/>,
/// <see cref="Direction"/>) pair to an animation clip name and drives an
/// <see cref="IAnimatedSprite"/> via <see cref="UpdateAnimation"/>.
/// Avoids restarting a clip that is already playing.
/// </summary>
public sealed class PlayerAnimationController
{
    private readonly IAnimatedSprite _sprite;

    /// <summary>
    /// Name of the animation clip currently being played, reflected directly
    /// from the underlying <see cref="IAnimatedSprite"/>.
    /// </summary>
    public string CurrentAnimationName => _sprite.CurrentAnimation;

    public PlayerAnimationController(IAnimatedSprite sprite)
    {
        _sprite = sprite ?? throw new ArgumentNullException(nameof(sprite));
    }

    /// <summary>
    /// Resolves the correct animation name for <paramref name="state"/> and
    /// <paramref name="direction"/> and calls <see cref="IAnimatedSprite.Play"/>
    /// only when the clip would change.  Compares against
    /// <see cref="IAnimatedSprite.CurrentAnimation"/> so that a clip already
    /// playing on the sprite (e.g. set before this controller was constructed)
    /// is also skipped.
    /// </summary>
    public void UpdateAnimation(PlayerState state, Direction direction)
    {
        string animName = AnimationNames.Get(state, direction);
        if (animName == _sprite.CurrentAnimation)
            return;

        _sprite.Play(animName);
    }
}
