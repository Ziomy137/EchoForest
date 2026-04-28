using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>CanvasLayer</c> node for the Pause Menu overlay.
///
/// Wires Resume / Settings / Save Game / Return to Main Menu buttons to
/// <see cref="PauseMenuController"/> and applies <c>GetTree().Paused</c>
/// after each action.
///
/// All logic lives in the pure-C# <see cref="PauseMenuController"/> so it
/// can be unit-tested independently of the Godot runtime.
///
/// Excluded from NUnit code coverage — requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot CanvasLayer wrapper — requires scene tree")]
public partial class PauseMenuNode : CanvasLayer
{
    private PauseMenuController _ctrl = null!;

    public override void _Ready()
    {
        _ctrl = new PauseMenuController(
            new SaveService(new GodotFileSystem()),
            new GodotSceneLoader());

        _ctrl.Open();
        // Tree is already paused by CottageAreaNode before AddChild was called.
        WireButtons();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("pause"))
        {
            GetViewport().SetInputAsHandled();
            OnResume();
        }
    }

    // ── Button wiring ─────────────────────────────────────────────────────────

    private void WireButtons()
    {
        GetNode<Button>("Center/Panel/VBox/ResumeButton").Pressed += OnResume;

        GetNode<Button>("Center/Panel/VBox/SettingsButton").Pressed += () =>
        {
            GetTree().Paused = false;
            _ctrl.OnSettings();
            QueueFree();
        };

        GetNode<Button>("Center/Panel/VBox/SaveGameButton").Pressed += OnSaveGame;

        GetNode<Button>("Center/Panel/VBox/MainMenuButton").Pressed += () =>
        {
            GetTree().Paused = false;
            _ctrl.OnMainMenu();
            QueueFree();
        };
    }

    // ── Handlers that also update tree state ──────────────────────────────────

    private void OnResume()
    {
        _ctrl.OnResume();
        GetTree().Paused = false;
        QueueFree();
    }

    private void OnSaveGame()
    {
        _ctrl.OnSaveGame();
        // Pause state remains — player stays in pause menu after saving.
    }
}
