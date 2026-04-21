using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot-coupled adapter that wraps <c>AnimatedSprite2D</c> and implements
/// <see cref="IAnimatedSprite"/> so <see cref="PlayerAnimationController"/> can
/// drive it without depending on the Godot runtime directly.
///
/// Excluded from NUnit code coverage because it requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot AnimatedSprite2D adapter — requires scene tree")]
public sealed class GodotAnimatedSprite : IAnimatedSprite
{
    private readonly AnimatedSprite2D _sprite;

    public GodotAnimatedSprite(AnimatedSprite2D sprite) =>
        _sprite = sprite;

    public string CurrentAnimation => _sprite.Animation;

    public void Play(string animationName) => _sprite.Play(animationName);
}
