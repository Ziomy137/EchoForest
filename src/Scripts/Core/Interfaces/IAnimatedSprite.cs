namespace EchoForest.Core;

/// <summary>
/// Abstraction over <c>AnimatedSprite2D.Play()</c> that lets
/// <see cref="PlayerAnimationController"/> be tested without the Godot runtime.
/// </summary>
public interface IAnimatedSprite
{
    /// <summary>Name of the animation clip currently playing.</summary>
    string CurrentAnimation { get; }

    /// <summary>Starts playing the named animation clip.</summary>
    void Play(string animationName);
}
