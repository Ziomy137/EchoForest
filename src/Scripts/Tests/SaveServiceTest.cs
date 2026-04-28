using System;
using System.Collections.Generic;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// Unit tests for <see cref="SaveService"/>, <see cref="SaveData"/>,
/// <see cref="SaveSlotInfo"/>, and <see cref="SaveDataException"/>.
/// All tests run without the Godot runtime.
/// </summary>
[TestFixture]
public class SaveServiceTest
{
    // ── Helpers ───────────────────────────────────────────────────────────────

    private static SaveService Make(MockFileSystem fs) => new(fs);

    private static MockFileSystem EmptyFs() => new();                         // no file

    private static MockFileSystem CorruptFs()
    {
        var files = new Dictionary<string, string>();
        for (int i = 1; i <= Constants.SaveSlotCount; i++)
            files[$"user://save_slot_{i}.json"] = "{{corrupted}}";
        return new MockFileSystem(files);
    }

    // ── Save ──────────────────────────────────────────────────────────────────

    [Test]
    public void SaveService_Save_NullData_Throws()
    {
        var svc = Make(EmptyFs());
        Assert.Throws<ArgumentNullException>(() => svc.Save(null!, 1));
    }

    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(6)]
    public void SaveService_Save_InvalidSlot_Throws(int slot)
    {
        var svc = Make(EmptyFs());
        Assert.Throws<ArgumentOutOfRangeException>(() => svc.Save(new SaveData(), slot));
    }

    [Test]
    public void SaveService_Save_WritesJson()
    {
        var fs = EmptyFs();
        var svc = Make(fs);
        svc.Save(new SaveData { PlayerX = 100f, PlayerY = 200f }, 1);

        Assert.That(fs.GetContent("user://save_slot_1.json"), Is.Not.Null.And.Contains("100"));
    }

    // ── Load — round-trip ─────────────────────────────────────────────────────

    [Test]
    public void SaveService_SaveAndLoad_PreservesPlayerPosition()
    {
        var fs = EmptyFs();
        var svc = Make(fs);
        svc.Save(new SaveData { PlayerX = 320f, PlayerY = 240f }, 1);

        var loaded = svc.Load(1);

        Assert.Multiple(() =>
        {
            Assert.That(loaded.PlayerX, Is.EqualTo(320f).Within(0.001f));
            Assert.That(loaded.PlayerY, Is.EqualTo(240f).Within(0.001f));
        });
    }

    [Test]
    public void SaveService_SaveAndLoad_PreservesQuestState()
    {
        var fs = EmptyFs();
        var svc = Make(fs);
        var data = new SaveData();
        data.QuestStates["q_kidnapped"] = QuestState.Completed;
        svc.Save(data, 1);

        var loaded = svc.Load(1);

        Assert.That(loaded.QuestStates["q_kidnapped"], Is.EqualTo(QuestState.Completed));
    }

    [Test]
    public void SaveService_SaveAndLoad_PreservesInventory()
    {
        var fs = EmptyFs();
        var svc = Make(fs);
        var data = new SaveData();
        data.InventoryItems["sword_iron"] = 1;
        data.InventoryItems["potion_health"] = 3;
        svc.Save(data, 1);

        var loaded = svc.Load(1);

        Assert.Multiple(() =>
        {
            Assert.That(loaded.InventoryItems["sword_iron"], Is.EqualTo(1));
            Assert.That(loaded.InventoryItems["potion_health"], Is.EqualTo(3));
        });
    }

    [Test]
    public void SaveService_SaveAndLoad_PreservesPlaytime()
    {
        var fs = EmptyFs();
        var svc = Make(fs);
        svc.Save(new SaveData { PlaytimeTotalSeconds = 3661.0 }, 1);

        var loaded = svc.Load(1);

        Assert.That(loaded.PlaytimeTotalSeconds, Is.EqualTo(3661.0).Within(0.001));
    }

    // ── Load — missing file ───────────────────────────────────────────────────

    [Test]
    public void SaveService_Load_MissingSlot_ThrowsSaveDataException()
    {
        var svc = Make(EmptyFs());
        Assert.Throws<SaveDataException>(() => svc.Load(1));
    }

    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(6)]
    public void SaveService_Load_InvalidSlot_Throws(int slot)
    {
        var svc = Make(EmptyFs());
        Assert.Throws<ArgumentOutOfRangeException>(() => svc.Load(slot));
    }

    // ── Load — corrupt file ───────────────────────────────────────────────────

    [Test]
    public void SaveService_CorruptFile_ThrowsSaveDataException()
    {
        var svc = Make(CorruptFs());
        Assert.Throws<SaveDataException>(() => svc.Load(1));
    }

    // ── HasSave ───────────────────────────────────────────────────────────────

    [Test]
    public void SaveService_HasSave_FalseForEmptySlot()
    {
        var svc = Make(EmptyFs());
        Assert.That(svc.HasSave(1), Is.False);
    }

    [Test]
    public void SaveService_HasSave_TrueAfterSave()
    {
        var fs = EmptyFs();
        var svc = Make(fs);
        svc.Save(new SaveData(), 1);
        Assert.That(svc.HasSave(1), Is.True);
    }

    // ── Delete ────────────────────────────────────────────────────────────────

    [Test]
    public void SaveService_Delete_RemovesFile()
    {
        var fs = EmptyFs();
        var svc = Make(fs);
        svc.Save(new SaveData(), 1);
        svc.Delete(1);

        Assert.That(svc.HasSave(1), Is.False);
    }

    [Test]
    public void SaveService_Delete_NoopWhenSlotEmpty()
    {
        var svc = Make(EmptyFs());
        Assert.DoesNotThrow(() => svc.Delete(1));
    }

    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(6)]
    public void SaveService_Delete_InvalidSlot_Throws(int slot)
    {
        var svc = Make(EmptyFs());
        Assert.Throws<ArgumentOutOfRangeException>(() => svc.Delete(slot));
    }

    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(6)]
    public void SaveService_HasSave_InvalidSlot_Throws(int slot)
    {
        var svc = Make(EmptyFs());
        Assert.Throws<ArgumentOutOfRangeException>(() => svc.HasSave(slot));
    }

    // ── HasSaveFile (ISaveService compat) ─────────────────────────────────────

    [Test]
    public void SaveService_HasSaveFile_FalseWhenNoSlots()
    {
        var svc = Make(EmptyFs());
        Assert.That(svc.HasSaveFile(), Is.False);
    }

    [Test]
    public void SaveService_HasSaveFile_TrueWhenAnySlotExists()
    {
        var fs = EmptyFs();
        var svc = Make(fs);
        svc.Save(new SaveData(), 3);

        Assert.That(svc.HasSaveFile(), Is.True);

        var slots = svc.GetSaveSlots();
        Assert.That(slots[0].IsEmpty, Is.True, "Saving to slot 3 should not populate slot 1");
        Assert.That(slots[2].IsEmpty, Is.False, "Saving to slot 3 should populate slot 3");
    }

    // ── GetSaveSlots ──────────────────────────────────────────────────────────

    [Test]
    public void SaveService_GetSaveSlots_ReturnsAllFiveSlots()
    {
        var svc = Make(EmptyFs());
        var slots = svc.GetSaveSlots();
        Assert.That(slots, Has.Count.EqualTo(5));
    }

    [Test]
    public void SaveService_GetSaveSlots_EmptySlotsAreEmpty()
    {
        var svc = Make(EmptyFs());
        foreach (var slot in svc.GetSaveSlots())
            Assert.That(slot.IsEmpty, Is.True, $"Slot {slot.Slot} should be empty");
    }

    [Test]
    public void SaveService_GetSaveSlots_SavedSlotIsNotEmpty()
    {
        var fs = EmptyFs();
        var svc = Make(fs);
        svc.Save(new SaveData { CurrentArea = "res://Cottage.tscn", PlaytimeTotalSeconds = 120 }, 1);

        var slots = svc.GetSaveSlots();
        // Slot index 0 = Slot 1
        Assert.That(slots[0].IsEmpty, Is.False);
        Assert.That(slots[0].Area, Is.EqualTo("res://Cottage.tscn"));
    }

    [Test]
    public void SaveService_GetSaveSlots_CorruptSlot_IsReturnedAsEmpty()
    {
        // MockFileSystem with corrupt content: all 5 slots appear to exist but contain invalid JSON.
        var svc = Make(CorruptFs());
        var slots = svc.GetSaveSlots();
        Assert.That(slots, Has.Count.EqualTo(5));
        foreach (var slot in slots)
            Assert.That(slot.IsEmpty, Is.True, $"Corrupt slot {slot.Slot} should be surfaced as empty");
    }

    // ── SaveSlotInfo ──────────────────────────────────────────────────────────

    [Test]
    public void SaveSlotInfo_IsEmpty_TrueWhenSavedAtNull()
    {
        var info = new SaveSlotInfo(1, null, null, 0, null);
        Assert.That(info.IsEmpty, Is.True);
    }

    [Test]
    public void SaveSlotInfo_PlaytimeDisplay_EmptyWhenNoSave()
    {
        var info = new SaveSlotInfo(1, null, null, 0, null);
        Assert.That(info.PlaytimeDisplay, Is.EqualTo("—"));
    }

    [Test]
    public void SaveSlotInfo_PlaytimeDisplay_FormattedCorrectly()
    {
        var info = new SaveSlotInfo(1, null, null, 3661.0, DateTime.Now);
        Assert.That(info.PlaytimeDisplay, Is.EqualTo("1:01:01"));
    }

    [Test]
    public void SaveSlotInfo_SaveName_IsPreserved()
    {
        var info = new SaveSlotInfo(1, "Chapter 1", null, 0, DateTime.Now);
        Assert.That(info.SaveName, Is.EqualTo("Chapter 1"));
    }

    // ── SaveDataException ─────────────────────────────────────────────────────

    [Test]
    public void SaveDataException_Message_IsPreserved()
    {
        var ex = new SaveDataException("test error");
        Assert.That(ex.Message, Is.EqualTo("test error"));
    }

    [Test]
    public void SaveDataException_InnerException_IsPreserved()
    {
        var inner = new System.IO.IOException("disk full");
        var ex = new SaveDataException("wrapped", inner);
        Assert.That(ex.InnerException, Is.SameAs(inner));
    }
}
