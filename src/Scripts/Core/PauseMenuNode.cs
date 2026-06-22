using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>CanvasLayer</c> node for the Pause Menu overlay.
///
/// Button signals are connected in <c>PauseMenu.tscn</c> via
/// <c>[connection]</c> declarations — the most reliable approach for scenes
/// added dynamically via <c>GetTree().Root.AddChild()</c>. C# delegate wiring
/// in <c>_Ready()</c> can silently fail for such scenes.
///
/// Escape / "pause" input is handled here via <c>_Input</c> (fires before GUI,
/// so no GUI element can swallow it). <see cref="CottageAreaNode"/> also
/// handles Escape as a fallback.
///
/// Excluded from NUnit code coverage — requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot CanvasLayer wrapper — requires scene tree")]
public partial class PauseMenuNode : CanvasLayer
{
    private PauseMenuController _ctrl = null!;

    public override void _Ready()
    {
        _ctrl = new PauseMenuController(new SaveService(new GodotFileSystem()));
        _ctrl.Open();
    }

    // _Input fires for ALL input events before GUI processing —
    // guarantees Escape is received even when a button has keyboard focus.
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("pause"))
        {
            GetViewport().SetInputAsHandled();
            OnResume();
        }
    }

    // ── Signal receivers (connected from PauseMenu.tscn [connection] blocks) ──

    /// <summary>Resumes gameplay and closes the pause menu.</summary>
    public void OnResume()
    {
        _ctrl.OnResume();
        QueueFree();
    }

    /// <summary>Opens Settings screen and closes the pause menu.</summary>
    public void OnSettingsPressed()
    {
        // Unpause before navigation so the incoming scene runs normally.
        // GetTree() must be captured before RemoveChild invalidates it.
        var tree = GetTree();
        tree.Paused = false;
        tree.Root.RemoveChild(this);
        QueueFree();
        tree.ChangeSceneToFile(MainMenuConfig.SettingsScenePath);
    }

    /// <summary>Saves the game to slot 1 (menu stays open).</summary>
    public void OnSaveGame()
    {
        _ctrl.OnSaveGame();
    }

    /// <summary>Returns to the Main Menu scene.</summary>
    public void OnMainMenuPressed()
    {
        var tree = GetTree();
        tree.Paused = false;
        tree.Root.RemoveChild(this);
        QueueFree();
        tree.ChangeSceneToFile(MainMenuConfig.SceneResPath);
    }
}

