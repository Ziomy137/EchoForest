using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>CanvasLayer</c> node for the Load Game screen.
///
/// Populates up to 5 save-slot panels from <see cref="SaveService"/> and wires
/// each "Load" button to start a session from the saved state.
/// The "Cancel" button returns to the Main Menu.
///
/// Excluded from NUnit code coverage — requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot CanvasLayer wrapper — requires scene tree")]
public partial class LoadGameScreenNode : CanvasLayer
{
    private ISaveDataService _svc = null!;

    public override void _Ready()
    {
        _svc = new SaveService(new GodotFileSystem());
        PopulateSlots();
        WireCancelButton();
    }

    // ── Slot population ───────────────────────────────────────────────────────

    private void PopulateSlots()
    {
        var slots = _svc.GetSaveSlots();
        for (int i = 0; i < slots.Count; i++)
            PopulateSlot(i + 1, slots[i]);
    }

    private void PopulateSlot(int displayNum, SaveSlotInfo info)
    {
        var idx = displayNum.ToString();

        if (FindChild($"AreaLabel{idx}") is Label area)
            area.Text = string.IsNullOrEmpty(info.Area) ? "—" : info.Area;

        if (FindChild($"PlayTimeLabel{idx}") is Label time)
            time.Text = info.PlaytimeDisplay;

        if (FindChild($"DateLabel{idx}") is Label date)
            date.Text = info.SavedAt.HasValue
                ? info.SavedAt.Value.ToString("yyyy-MM-dd HH:mm")
                : "—";

        if (FindChild($"LoadButton{idx}") is Button btn)
        {
            btn.Disabled = info.IsEmpty;
            if (!info.IsEmpty)
                WireLoadButton(displayNum, info);
        }
    }

    // ── Signal wiring ─────────────────────────────────────────────────────────

    private void WireLoadButton(int slot, SaveSlotInfo info)
    {
        if (FindChild($"LoadButton{slot}") is not Button btn) return;
        var loader = new GodotSceneLoader();
        btn.Pressed += () =>
        {
            // Apply saved position to GameSession so the scene can restore it.
            var data = _svc.Load(slot);
            GameSession.Start();
            GameSession.SavePlayerPosition(data.PlayerX, data.PlayerY);

            var scene = string.IsNullOrEmpty(data.CurrentArea)
                ? MainMenuConfig.ContinueScenePath
                : data.CurrentArea;
            loader.LoadScene(scene);
        };
    }

    private void WireCancelButton()
    {
        if (FindChild("CancelButton") is not Button btn) return;
        var loader = new GodotSceneLoader();
        btn.Pressed += () => loader.LoadScene(MainMenuConfig.SceneResPath);
    }
}
