using System.Diagnostics.CodeAnalysis;

namespace EchoForest.Core;

/// <summary>
/// Production <see cref="ISaveService"/> that reports a save file as existing
/// whenever an in-process game session has been started (<see cref="GameSession.HasSession"/>).
///
/// This keeps the Main Menu "Continue" button enabled after the player
/// returns from gameplay, without requiring the full save system (S5-04).
///
/// Excluded from NUnit code coverage — reads static state; tested via GameSessionTest.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Thin wrapper over GameSession static state — tested indirectly")]
public sealed class GodotSessionSaveService : ISaveService
{
    /// <inheritdoc/>
    public bool HasSaveFile() => GameSession.HasSession;
}
