using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>CanvasLayer</c> node that drives the Main Menu UI.
///
/// Wires each Godot button's <c>pressed</c> signal to the corresponding
/// <see cref="MainMenuController"/> action. The controller contains all
/// navigation logic and is unit-tested independently of the Godot runtime.
///
/// Excluded from NUnit code coverage — requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot CanvasLayer wrapper — requires scene tree")]
public partial class MainMenuNode : CanvasLayer
{
    private MainMenuController _controller = null!;

    public override void _Ready()
    {
        var sceneLoader = new GodotSceneLoader();
        var appCtrl = new GodotApplicationController();
        var saveService = new NullSaveService();
        _controller = new MainMenuController(saveService, sceneLoader, appCtrl);

        WireButton("NewGameButton", _controller.OnNewGame);
        WireButton("ContinueButton", _controller.OnContinue);
        WireButton("LoadGameButton", _controller.OnLoadGame);
        WireButton("SettingsButton", _controller.OnSettings);
        WireButton("CreditsButton", _controller.OnCredits);
        WireButton("ExitButton", _controller.OnExit);

        // Reflect initial state of Continue button
        if (FindChild("ContinueButton") is Button continueBtn)
            continueBtn.Disabled = !_controller.IsContinueEnabled;
    }

    private void WireButton(string nodeName, System.Action action)
    {
        if (FindChild(nodeName) is not Button btn) return;
        btn.Pressed += action;
    }
}
