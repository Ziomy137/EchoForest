using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>CanvasLayer</c> node for the Credits screen.
///
/// Wires the Back button to <see cref="CreditsController.OnBack"/>.
///
/// All navigation logic lives in the pure-C# <see cref="CreditsController"/>
/// so it can be unit-tested independently of the Godot runtime.
///
/// Excluded from NUnit code coverage — requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot CanvasLayer wrapper — requires scene tree")]
public partial class CreditsScreenNode : CanvasLayer
{
    public override void _Ready()
    {
        PopulateCreditsText();
        WireBackButton();
    }

    private void PopulateCreditsText()
    {
        const string leftText =
            "[b][color=#ffd700]Studio[/color][/b]\n" +
            "Ziomy137 Studio\n\n" +
            "[b][color=#ffd700]Lead Developer[/color][/b]\n" +
            "Filip Klos\n\n" +
            "[b][color=#ffd700]Art & Assets[/color][/b]\n" +
            "Filip Klos\n\n" +
            "[b][color=#ffd700]Level Design[/color][/b]\n" +
            "Filip Klos";

        const string rightText =
            "[b][color=#ffd700]QA & Testing[/color][/b]\n" +
            "Automated NUnit Suite\n\n" +
            "[b][color=#ffd700]Tools & Technologies[/color][/b]\n" +
            "Godot 4  |  C# / .NET 10\n" +
            "GitHub Actions  |  ReportGenerator\n\n" +
            "[b][color=#ffd700]Special Thanks[/color][/b]\n" +
            "[i]Thank you for playing Echo Forest.[/i]";

        var leftLabel = GetNodeOrNull<RichTextLabel>("VBox/Columns/LeftPanel/LeftLabel");
        if (leftLabel != null)
        {
            leftLabel.Clear();
            leftLabel.AppendText(leftText);
        }

        var rightLabel = GetNodeOrNull<RichTextLabel>("VBox/Columns/RightPanel/RightLabel");
        if (rightLabel != null)
        {
            rightLabel.Clear();
            rightLabel.AppendText(rightText);
        }
    }

    // ── Button wiring ─────────────────────────────────────────────────────────

    private void WireBackButton()
    {
        var btn = GetNodeOrNull<Button>("VBox/BackButton");
        if (btn != null)
            btn.Pressed += OnBack;
    }

    private void OnBack()
    {
        GetTree().ChangeSceneToFile(MainMenuConfig.SceneResPath);
    }
}
