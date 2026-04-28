using System;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// Unit tests for <see cref="SaveGameController"/> and <see cref="SaveSlot"/>.
/// All tests run without the Godot runtime.
/// </summary>
[TestFixture]
public class SaveGameControllerTest
{
    // ── Construction ──────────────────────────────────────────────────────────

    [Test]
    public void Constructor_DefaultSlotCount_IsFive()
    {
        var ctrl = new SaveGameController();
        Assert.That(ctrl.SlotCount, Is.EqualTo(5));
    }

    [Test]
    public void Constructor_CustomSlotCount_IsHonoured()
    {
        var ctrl = new SaveGameController(slotCount: 3);
        Assert.That(ctrl.SlotCount, Is.EqualTo(3));
    }

    [Test]
    public void Constructor_ZeroSlotCount_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = new SaveGameController(0));
    }

    [Test]
    public void Constructor_NegativeSlotCount_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = new SaveGameController(-1));
    }

    // ── GetSlots ──────────────────────────────────────────────────────────────

    [Test]
    public void GetSlots_ReturnsCorrectCount()
    {
        var ctrl = new SaveGameController(slotCount: 4);
        Assert.That(ctrl.GetSlots(), Has.Length.EqualTo(4));
    }

    [Test]
    public void GetSlots_AllSlotsStartEmpty()
    {
        var ctrl = new SaveGameController();
        foreach (var slot in ctrl.GetSlots())
            Assert.That(slot.IsEmpty, Is.True, $"Slot {slot.Index} should be empty initially");
    }

    [Test]
    public void GetSlots_SlotsHaveCorrectIndices()
    {
        var ctrl = new SaveGameController(slotCount: 3);
        var slots = ctrl.GetSlots();
        Assert.Multiple(() =>
        {
            Assert.That(slots[0].Index, Is.EqualTo(0));
            Assert.That(slots[1].Index, Is.EqualTo(1));
            Assert.That(slots[2].Index, Is.EqualTo(2));
        });
    }

    [Test]
    public void GetSlots_ReturnsSnapshot_NotLiveReference()
    {
        var ctrl = new SaveGameController();
        var snap1 = ctrl.GetSlots();
        ctrl.SaveToSlot(0, "Test Save");
        var snap2 = ctrl.GetSlots();

        Assert.That(snap1[0].IsEmpty, Is.True, "First snapshot should not be mutated");
        Assert.That(snap2[0].IsEmpty, Is.False, "Second snapshot reflects the save");
    }

    // ── SaveToSlot ────────────────────────────────────────────────────────────

    [Test]
    public void SaveToSlot_ValidArgs_SlotIsNoLongerEmpty()
    {
        var ctrl = new SaveGameController();
        ctrl.SaveToSlot(0, "My Save");

        Assert.That(ctrl.GetSlots()[0].IsEmpty, Is.False);
    }

    [Test]
    public void SaveToSlot_ValidArgs_StoresSaveName()
    {
        var ctrl = new SaveGameController();
        ctrl.SaveToSlot(2, "Forest Run");

        Assert.That(ctrl.GetSlots()[2].SaveName, Is.EqualTo("Forest Run"));
    }

    [Test]
    public void SaveToSlot_ValidArgs_StoresSavedAt()
    {
        var before = DateTime.Now;
        var ctrl = new SaveGameController();
        ctrl.SaveToSlot(1, "Quick Save");
        var after = DateTime.Now;

        var savedAt = ctrl.GetSlots()[1].SavedAt;
        Assert.That(savedAt, Is.InRange(before, after));
    }

    [Test]
    public void SaveToSlot_OutOfRangeIndex_Throws()
    {
        var ctrl = new SaveGameController(slotCount: 3);
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ctrl.SaveToSlot(3, "X"));
            Assert.Throws<ArgumentOutOfRangeException>(() => ctrl.SaveToSlot(-1, "X"));
        });
    }

    [Test]
    public void SaveToSlot_NullSaveName_Throws()
    {
        var ctrl = new SaveGameController();
        Assert.Throws<ArgumentNullException>(() => ctrl.SaveToSlot(0, null!));
    }

    [Test]
    public void SaveToSlot_Overwrite_UpdatesSaveName()
    {
        var ctrl = new SaveGameController();
        ctrl.SaveToSlot(0, "Old Name");
        ctrl.SaveToSlot(0, "New Name");

        Assert.That(ctrl.GetSlots()[0].SaveName, Is.EqualTo("New Name"));
    }

    // ── SaveSlot record ───────────────────────────────────────────────────────

    [Test]
    public void SaveSlot_IsEmpty_TrueWhenSavedAtIsNull()
    {
        var slot = new SaveSlot(Index: 0);
        Assert.That(slot.IsEmpty, Is.True);
    }

    [Test]
    public void SaveSlot_IsEmpty_FalseWhenSavedAtIsSet()
    {
        var slot = new SaveSlot(Index: 0, SavedAt: DateTime.Now);
        Assert.That(slot.IsEmpty, Is.False);
    }
}
