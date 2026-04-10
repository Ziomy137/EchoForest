namespace EchoForest.Core;

/// <summary>
/// NUnit test double for <see cref="IAnimatedSprite"/>.
/// Records every <see cref="Play"/> call so tests can assert on animation
/// name and invocation count without requiring the Godot runtime.
/// </summary>
public sealed class MockAnimatedSprite : IAnimatedSprite
{
    /// <inheritdoc/>
    public string CurrentAnimation { get; private set; } = string.Empty;

    /// <summary>Animation name from the most recent <see cref="Play"/> call.</summary>
    public string LastPlayedAnimation { get; private set; } = string.Empty;

    /// <summary>Total number of times <see cref="Play"/> has been called.</summary>
    public int PlayCallCount { get; private set; }

    /// <inheritdoc/>
    public void Play(string animationName)
    {
        LastPlayedAnimation = animationName;
        CurrentAnimation = animationName;
        PlayCallCount++;
    }
}
