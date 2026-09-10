namespace EchoForest.Core;

/// <summary>
/// Scene path constants for the Main Menu and its sub-screens.
/// </summary>
public static class MainMenuConfig
{
    /// <summary>Main Menu scene resource path.</summary>
    public const string SceneResPath = "res://src/Scenes/MainMenu.tscn";

    /// <summary>Game bootstrap scene loaded when starting a NEW playthrough.</summary>
    public const string GameBootstrapScenePath = "res://src/Scenes/GameBootstrap.tscn";

    /// <summary>Game scene loaded directly when continuing an existing session (skips bootstrap).</summary>
    public const string ContinueScenePath = "res://src/Scenes/TestArea_Cottage.tscn";

    /// <summary>Farm scene resource path.</summary>
    public const string FarmScenePath = "res://src/Scenes/Scene_Farm.tscn";

    /// <summary>Forest Path scene resource path.</summary>
    public const string ForestPathScenePath = "res://src/Scenes/Scene_ForestPath.tscn";

    /// <summary>City scene resource path.</summary>
    public const string CityScenePath = "res://src/Scenes/Scene_City.tscn";

    /// <summary>Load Game screen scene resource path.</summary>
    public const string LoadGameScenePath = "res://src/Scenes/LoadGameScreen.tscn";

    /// <summary>Settings screen scene resource path.</summary>
    public const string SettingsScenePath = "res://src/Scenes/SettingsScreen.tscn";

    /// <summary>Credits screen scene resource path.</summary>
    public const string CreditsScenePath = "res://src/Scenes/CreditsScreen.tscn";

    /// <summary>Pause menu overlay scene resource path.</summary>
    public const string PauseMenuScenePath = "res://src/Scenes/PauseMenu.tscn";

    /// <summary>Dialogue overlay scene resource path.</summary>
    public const string DialogueBoxScenePath = "res://src/Scenes/DialogueBox.tscn";

    /// <summary>Quest journal overlay scene resource path.</summary>
    public const string QuestJournalScenePath = "res://src/Scenes/QuestJournal.tscn";
}
