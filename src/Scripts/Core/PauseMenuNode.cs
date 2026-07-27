using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>CanvasLayer</c> node for the Pause Menu overlay.
///
/// Button signals are wired in <c>_Ready()</c> after the dynamically added
/// scene has entered the tree.
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

        WireNavButton("ResumeButton", () => OnResume());
        WireNavButton("SettingsButton", OpenSettings);
        WireNavButton("SaveGameButton", () => OnSaveGame());
        WireNavButton("MainMenuButton", () =>
        {
            var tree = GetTree();
            tree.Root.RemoveChild(this);
            QueueFree();
            tree.ChangeSceneToFile(MainMenuConfig.SceneResPath);
        });
    }

    private void WireNavButton(string nodeName, System.Action action)
    {
        var button = FindChild(nodeName) as Button;
        if (button != null)
            button.Pressed += action;
    }

    private void OpenSettings()
    {
        Visible = false;
        ProcessMode = ProcessModeEnum.Disabled;
        GetTree().Root.AddChild(
            GD.Load<PackedScene>(MainMenuConfig.SettingsScenePath).Instantiate<SettingsScreenNode>());
    }

    // _Input fires for ALL input events before GUI processing —
    // guarantees Escape is received even when a button has keyboard focus.
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed(InputActionNames.Pause))
        {
            GetViewport().SetInputAsHandled();
            OnResume();
        }
    }

    // ── Button actions ───────────────────────────────────────────────────────

    /// <summary>Resumes gameplay and closes the pause menu.</summary>
    public void OnResume()
    {
        _ctrl.OnResume();
        QueueFree();
    }

    /// <summary>Saves the game to slot 1 (menu stays open).</summary>
    public void OnSaveGame()
    {
        var player = GetTree().CurrentScene?.GetNodeOrNull<Node2D>("Player");
        var currentArea = GetTree().CurrentScene?.SceneFilePath ?? string.Empty;

        _ctrl.OnSaveGame(new SaveData
        {
            CurrentArea = currentArea,
            PlayerX = player?.GlobalPosition.X ?? 0f,
            PlayerY = player?.GlobalPosition.Y ?? 0f,
        });
    }
}
