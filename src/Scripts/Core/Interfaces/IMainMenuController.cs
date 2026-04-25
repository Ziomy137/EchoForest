namespace EchoForest.Core;

/// <summary>
/// Contract for the Main Menu navigation controller.
/// Pure C# — no Godot runtime required.
/// </summary>
public interface IMainMenuController
{
    /// <summary>
    /// Whether the "Continue" button should be enabled.
    /// <c>true</c> when at least one save file exists.
    /// </summary>
    bool IsContinueEnabled { get; }

    /// <summary>Starts a new game from the beginning.</summary>
    void OnNewGame();

    /// <summary>Loads the most recent save file and resumes the game.</summary>
    void OnContinue();

    /// <summary>Opens the Load Game screen.</summary>
    void OnLoadGame();

    /// <summary>Opens the Settings screen.</summary>
    void OnSettings();

    /// <summary>Opens the Credits screen.</summary>
    void OnCredits();

    /// <summary>Exits the application.</summary>
    void OnExit();
}
