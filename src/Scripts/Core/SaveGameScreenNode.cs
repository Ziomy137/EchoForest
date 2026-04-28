using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>CanvasLayer</c> node for the Save Game screen.
///
/// Populates the five save-slot panels from <see cref="SaveGameController"/>
/// and wires each "Save" button so clicking it writes the current session into
/// that slot.  The "Cancel" button returns to the Main Menu.
///
/// All slot-management logic lives in the pure-C# <see cref="SaveGameController"/>
/// so it can be unit-tested independently of the Godot runtime.
///
/// Excluded from NUnit code coverage — requires the Godot scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot CanvasLayer wrapper — requires scene tree")]
public partial class SaveGameScreenNode : CanvasLayer
{
    private ISaveGameController _ctrl = null!;

    public override void _Ready()
    {
        _ctrl = new SaveGameController();
        PopulateSlots();
        WireCancelButton();
    }

    // ── Slot population ───────────────────────────────────────────────────────

    private void PopulateSlots()
    {
        var slots = _ctrl.GetSlots();
        for (int i = 0; i < slots.Length; i++)
            PopulateSlot(i, slots[i]);
    }

    private void PopulateSlot(int index, SaveSlot slot)
    {
        var idx = index.ToString();

        if (FindChild($"NameEdit{idx}") is LineEdit nameEdit && !slot.IsEmpty)
            nameEdit.Text = slot.SaveName ?? string.Empty;

        if (FindChild($"LevelLabel{idx}") is Label lvl)
            lvl.Text = slot.CharacterLevel.HasValue
                ? $"Level: {slot.CharacterLevel}"
                : "Level: —";

        if (FindChild($"LocationLabel{idx}") is Label loc)
            loc.Text = slot.Location is { Length: > 0 } l
                ? $"Location: {l}"
                : "Location: —";

        if (FindChild($"PlayTimeLabel{idx}") is Label time)
            time.Text = slot.PlayTime.HasValue
                ? $"Time: {slot.PlayTime:hh\\:mm\\:ss}"
                : "Time: —";

        if (FindChild($"DateLabel{idx}") is Label date)
            date.Text = slot.SavedAt.HasValue
                ? slot.SavedAt.Value.ToString("yyyy-MM-dd HH:mm")
                : "—";

        WireSaveButton(index);
    }

    // ── Signal wiring ─────────────────────────────────────────────────────────

    private void WireSaveButton(int slotIndex)
    {
        if (FindChild($"SaveButton{slotIndex}") is not Button btn) return;

        btn.Pressed += () =>
        {
            var nameEdit = FindChild($"NameEdit{slotIndex}") as LineEdit;
            var saveName = nameEdit?.Text.Trim() is { Length: > 0 } name
                ? name
                : $"Save {slotIndex + 1}";

            _ctrl.SaveToSlot(slotIndex, saveName);

            // Refresh this slot's display to show the saved data.
            PopulateSlot(slotIndex, _ctrl.GetSlots()[slotIndex]);
        };
    }

    private void WireCancelButton()
    {
        if (FindChild("CancelButton") is not Button btn) return;
        var loader = new GodotSceneLoader();
        btn.Pressed += () => loader.LoadScene(MainMenuConfig.SceneResPath);
    }
}
