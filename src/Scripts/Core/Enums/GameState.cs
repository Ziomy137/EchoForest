namespace EchoForest.Core;

/// <summary>
/// High-level game flow state used by menus, pause logic, and cutscene systems.
/// </summary>
public enum GameState
{
    Playing = 0,
    Paused = 1,
    Cutscene = 2,
}
