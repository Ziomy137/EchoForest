namespace EchoForest.Core;

/// <summary>
/// In-process session state. Tracks whether the player has started (or
/// returned to) a game session during this application run.
///
/// Set via <see cref="Start"/> when <c>GameBootstrapNode</c> loads.
/// Read by <see cref="ISaveService"/> implementations to decide whether
/// the "Continue" button on the Main Menu should be enabled.
///
/// Pure C# — no Godot runtime required, fully testable with NUnit.
/// </summary>
public static class GameSession
{
    /// <summary><c>true</c> when the player has started at least one session.</summary>
    public static bool HasSession { get; private set; }

    /// <summary>Marks a session as active. Call when the game world loads.</summary>
    public static void Start() => HasSession = true;

    /// <summary>Clears the active session flag. Useful in tests and on new install.</summary>
    public static void Clear() => HasSession = false;
}
