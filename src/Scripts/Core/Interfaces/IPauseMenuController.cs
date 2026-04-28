namespace EchoForest.Core;

/// <summary>
/// Contract for the Pause Menu controller.
/// </summary>
public interface IPauseMenuController
{
    /// <summary>Whether the pause menu is currently open.</summary>
    bool IsPaused { get; }

    /// <summary>Whether a game save was triggered during this pause session.</summary>
    bool GameSaved { get; }

    /// <summary>Opens the pause menu and pauses the game.</summary>
    void Open();

    /// <summary>Closes the pause menu and resumes the game.</summary>
    void OnResume();

    /// <summary>Navigates to the Settings screen.</summary>
    void OnSettings();

    /// <summary>Saves to slot 1 and sets <see cref="GameSaved"/>.</summary>
    void OnSaveGame();

    /// <summary>Returns to the Main Menu.</summary>
    void OnMainMenu();
}
