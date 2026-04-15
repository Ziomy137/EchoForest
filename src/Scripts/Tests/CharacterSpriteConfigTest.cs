using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

// ═══════════════════════════════════════════════════════════════════════════════
// AnimationClipConfig Tests
// ═══════════════════════════════════════════════════════════════════════════════

[TestFixture]
public class AnimationClipConfigTest
{
    private AnimationClipConfig _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _sut = new AnimationClipConfig("idle_down", row: 0, startColumn: 0, frameCount: 2, fps: 5.0f);
    }

    // ─── Constructor ──────────────────────────────────────────────────────────

    [Test]
    public void Constructor_SetsName() =>
        Assert.That(_sut.Name, Is.EqualTo("idle_down"));

    [Test]
    public void Constructor_SetsRow() =>
        Assert.That(_sut.Row, Is.EqualTo(0));

    [Test]
    public void Constructor_SetsStartColumn() =>
        Assert.That(_sut.StartColumn, Is.EqualTo(0));

    [Test]
    public void Constructor_SetsFrameCount() =>
        Assert.That(_sut.FrameCount, Is.EqualTo(2));

    [Test]
    public void Constructor_SetsFps() =>
        Assert.That(_sut.Fps, Is.EqualTo(5.0f));

    // ─── FrameColumns ─────────────────────────────────────────────────────────

    [Test]
    public void FrameColumns_HasCorrectCount()
    {
        var clip = new AnimationClipConfig("walk_down", 0, startColumn: 2, frameCount: 4, fps: 8.0f);
        Assert.That(clip.FrameColumns.Count, Is.EqualTo(4));
    }

    [Test]
    public void FrameColumns_IdleClip_Returns0And1()
    {
        Assert.That(_sut.FrameColumns, Is.EqualTo(new[] { 0, 1 }));
    }

    [Test]
    public void FrameColumns_WalkClip_Returns2Through5()
    {
        var clip = new AnimationClipConfig("walk_down", 0, startColumn: 2, frameCount: 4, fps: 8.0f);
        Assert.That(clip.FrameColumns, Is.EqualTo(new[] { 2, 3, 4, 5 }));
    }

    [Test]
    public void FrameColumns_RunClip_Returns6Through9()
    {
        var clip = new AnimationClipConfig("run_down", 0, startColumn: 6, frameCount: 4, fps: 10.0f);
        Assert.That(clip.FrameColumns, Is.EqualTo(new[] { 6, 7, 8, 9 }));
    }

    [Test]
    public void FrameColumns_CountMatchesFrameCount()
    {
        var clip = new AnimationClipConfig("x", 0, startColumn: 0, frameCount: 4, fps: 8.0f);
        Assert.That(clip.FrameColumns.Count, Is.EqualTo(clip.FrameCount));
    }

    [Test]
    public void FrameColumns_AreSequentialFromStartColumn()
    {
        var clip = new AnimationClipConfig("x", 1, startColumn: 3, frameCount: 3, fps: 8.0f);
        var expected = new[] { 3, 4, 5 };
        Assert.That(clip.FrameColumns, Is.EqualTo(expected));
    }

    // ─── Various rows ─────────────────────────────────────────────────────────

    [Test]
    public void Constructor_Row1_SetsRowToOne() =>
        Assert.That(new AnimationClipConfig("idle_left", 1, 0, 2, 5.0f).Row, Is.EqualTo(1));

    [Test]
    public void Constructor_Row3_SetsRowToThree() =>
        Assert.That(new AnimationClipConfig("idle_up", 3, 0, 2, 5.0f).Row, Is.EqualTo(3));
}

// ═══════════════════════════════════════════════════════════════════════════════
// CharacterSpriteConfig Tests
// ═══════════════════════════════════════════════════════════════════════════════

[TestFixture]
public class CharacterSpriteConfigTest
{
    // ─── Sheet geometry constants ─────────────────────────────────────────────

    [Test]
    public void FrameWidth_Is64() =>
        Assert.That(CharacterSpriteConfig.FrameWidth, Is.EqualTo(64));

    [Test]
    public void FrameHeight_Is24() =>
        Assert.That(CharacterSpriteConfig.FrameHeight, Is.EqualTo(24));

    [Test]
    public void Columns_Is10() =>
        Assert.That(CharacterSpriteConfig.Columns, Is.EqualTo(10));

    [Test]
    public void Rows_Is4() =>
        Assert.That(CharacterSpriteConfig.Rows, Is.EqualTo(4));

    [Test]
    public void TotalFrames_Is40() =>
        Assert.That(CharacterSpriteConfig.TotalFrames, Is.EqualTo(40));

    [Test]
    public void SheetWidth_Is640() =>
        Assert.That(CharacterSpriteConfig.SheetWidth, Is.EqualTo(640));

    [Test]
    public void SheetHeight_Is96() =>
        Assert.That(CharacterSpriteConfig.SheetHeight, Is.EqualTo(96));

    [Test]
    public void SheetWidth_EqualsFrameWidthTimesColumns() =>
        Assert.That(CharacterSpriteConfig.SheetWidth,
            Is.EqualTo(CharacterSpriteConfig.FrameWidth * CharacterSpriteConfig.Columns));

    [Test]
    public void SheetHeight_EqualsFrameHeightTimesRows() =>
        Assert.That(CharacterSpriteConfig.SheetHeight,
            Is.EqualTo(CharacterSpriteConfig.FrameHeight * CharacterSpriteConfig.Rows));

    [Test]
    public void TotalFrames_EqualsColumnsTimesRows() =>
        Assert.That(CharacterSpriteConfig.TotalFrames,
            Is.EqualTo(CharacterSpriteConfig.Columns * CharacterSpriteConfig.Rows));

    // ─── Direction row indices ────────────────────────────────────────────────

    [Test]
    public void RowDown_Is0() =>
        Assert.That(CharacterSpriteConfig.RowDown, Is.EqualTo(0));

    [Test]
    public void RowLeft_Is1() =>
        Assert.That(CharacterSpriteConfig.RowLeft, Is.EqualTo(1));

    [Test]
    public void RowRight_Is2() =>
        Assert.That(CharacterSpriteConfig.RowRight, Is.EqualTo(2));

    [Test]
    public void RowUp_Is3() =>
        Assert.That(CharacterSpriteConfig.RowUp, Is.EqualTo(3));

    [Test]
    public void RowIndices_AreDistinct()
    {
        var rows = new[] {
            CharacterSpriteConfig.RowDown,
            CharacterSpriteConfig.RowLeft,
            CharacterSpriteConfig.RowRight,
            CharacterSpriteConfig.RowUp,
        };
        Assert.That(rows.Distinct().Count(), Is.EqualTo(4));
    }

    // ─── Animation column layout constants ───────────────────────────────────

    [Test]
    public void IdleStartColumn_Is0() =>
        Assert.That(CharacterSpriteConfig.IdleStartColumn, Is.EqualTo(0));

    [Test]
    public void IdleFrameCount_Is2() =>
        Assert.That(CharacterSpriteConfig.IdleFrameCount, Is.EqualTo(2));

    [Test]
    public void WalkStartColumn_Is2() =>
        Assert.That(CharacterSpriteConfig.WalkStartColumn, Is.EqualTo(2));

    [Test]
    public void WalkFrameCount_Is4() =>
        Assert.That(CharacterSpriteConfig.WalkFrameCount, Is.EqualTo(4));

    [Test]
    public void RunStartColumn_Is6() =>
        Assert.That(CharacterSpriteConfig.RunStartColumn, Is.EqualTo(6));

    [Test]
    public void RunFrameCount_Is4() =>
        Assert.That(CharacterSpriteConfig.RunFrameCount, Is.EqualTo(4));

    [Test]
    public void IdleWalkRun_FrameCounts_SumToColumns()
    {
        var total = CharacterSpriteConfig.IdleFrameCount
                  + CharacterSpriteConfig.WalkFrameCount
                  + CharacterSpriteConfig.RunFrameCount;
        Assert.That(total, Is.EqualTo(CharacterSpriteConfig.Columns));
    }

    // ─── FPS constants ────────────────────────────────────────────────────────

    [Test]
    public void IdleFps_Is5() =>
        Assert.That(CharacterSpriteConfig.IdleFps, Is.EqualTo(5.0f));

    [Test]
    public void WalkFps_Is8() =>
        Assert.That(CharacterSpriteConfig.WalkFps, Is.EqualTo(8.0f));

    [Test]
    public void RunFps_Is10() =>
        Assert.That(CharacterSpriteConfig.RunFps, Is.EqualTo(10.0f));

    // ─── Resource paths ───────────────────────────────────────────────────────

    [Test]
    public void SpritesResPath_StartsWithResProtocol() =>
        Assert.That(CharacterSpriteConfig.SpritesResPath, Does.StartWith("res://"));

    [Test]
    public void SheetResPath_ContainsPngFileName() =>
        Assert.That(CharacterSpriteConfig.SheetResPath, Does.EndWith("player_spritesheet.png"));

    [Test]
    public void AnimationsResPath_ContainersTresExtension() =>
        Assert.That(CharacterSpriteConfig.AnimationsResPath, Does.EndWith(".tres"));

    [Test]
    public void SheetResPath_ContainsCharactersDirectory() =>
        Assert.That(CharacterSpriteConfig.SheetResPath, Does.Contain("Characters"));

    [Test]
    public void AnimationsResPath_ContainsAnimationsDirectory() =>
        Assert.That(CharacterSpriteConfig.AnimationsResPath, Does.Contain("Animations"));

    // ─── ClipCount ────────────────────────────────────────────────────────────

    [Test]
    public void ClipCount_Is12() =>
        Assert.That(CharacterSpriteConfig.ClipCount, Is.EqualTo(12));

    // ─── All() collection ─────────────────────────────────────────────────────

    [Test]
    public void All_HasExactlyClipCount_Items() =>
        Assert.That(CharacterSpriteConfig.All.Length, Is.EqualTo(CharacterSpriteConfig.ClipCount));

    [Test]
    public void All_ReturnsDefensiveCopy()
    {
        var first = CharacterSpriteConfig.All;
        var second = CharacterSpriteConfig.All;
        Assert.That(ReferenceEquals(first, second), Is.False);
    }

    [Test]
    public void All_AllClipNamesAreUnique()
    {
        var names = CharacterSpriteConfig.All.Select(c => c.Name).ToList();
        Assert.That(names.Distinct().Count(), Is.EqualTo(names.Count));
    }

    // ─── Individual clips exist ───────────────────────────────────────────────

    [Test] public void IdleDown_IsNotNull() => Assert.That(CharacterSpriteConfig.IdleDown, Is.Not.Null);
    [Test] public void WalkDown_IsNotNull() => Assert.That(CharacterSpriteConfig.WalkDown, Is.Not.Null);
    [Test] public void RunDown_IsNotNull() => Assert.That(CharacterSpriteConfig.RunDown, Is.Not.Null);
    [Test] public void IdleLeft_IsNotNull() => Assert.That(CharacterSpriteConfig.IdleLeft, Is.Not.Null);
    [Test] public void WalkLeft_IsNotNull() => Assert.That(CharacterSpriteConfig.WalkLeft, Is.Not.Null);
    [Test] public void RunLeft_IsNotNull() => Assert.That(CharacterSpriteConfig.RunLeft, Is.Not.Null);
    [Test] public void IdleRight_IsNotNull() => Assert.That(CharacterSpriteConfig.IdleRight, Is.Not.Null);
    [Test] public void WalkRight_IsNotNull() => Assert.That(CharacterSpriteConfig.WalkRight, Is.Not.Null);
    [Test] public void RunRight_IsNotNull() => Assert.That(CharacterSpriteConfig.RunRight, Is.Not.Null);
    [Test] public void IdleUp_IsNotNull() => Assert.That(CharacterSpriteConfig.IdleUp, Is.Not.Null);
    [Test] public void WalkUp_IsNotNull() => Assert.That(CharacterSpriteConfig.WalkUp, Is.Not.Null);
    [Test] public void RunUp_IsNotNull() => Assert.That(CharacterSpriteConfig.RunUp, Is.Not.Null);

    // ─── Clip names match AnimationNames constants ─────────────────────────────

    [Test]
    public void IdleDown_HasCorrectName() =>
        Assert.That(CharacterSpriteConfig.IdleDown.Name, Is.EqualTo(AnimationNames.IdleDown));

    [Test]
    public void WalkDown_HasCorrectName() =>
        Assert.That(CharacterSpriteConfig.WalkDown.Name, Is.EqualTo(AnimationNames.WalkDown));

    [Test]
    public void RunDown_HasCorrectName() =>
        Assert.That(CharacterSpriteConfig.RunDown.Name, Is.EqualTo(AnimationNames.RunDown));

    [Test]
    public void IdleLeft_HasCorrectName() =>
        Assert.That(CharacterSpriteConfig.IdleLeft.Name, Is.EqualTo(AnimationNames.IdleLeft));

    [Test]
    public void WalkLeft_HasCorrectName() =>
        Assert.That(CharacterSpriteConfig.WalkLeft.Name, Is.EqualTo(AnimationNames.WalkLeft));

    [Test]
    public void RunLeft_HasCorrectName() =>
        Assert.That(CharacterSpriteConfig.RunLeft.Name, Is.EqualTo(AnimationNames.RunLeft));

    [Test]
    public void IdleRight_HasCorrectName() =>
        Assert.That(CharacterSpriteConfig.IdleRight.Name, Is.EqualTo(AnimationNames.IdleRight));

    [Test]
    public void WalkRight_HasCorrectName() =>
        Assert.That(CharacterSpriteConfig.WalkRight.Name, Is.EqualTo(AnimationNames.WalkRight));

    [Test]
    public void RunRight_HasCorrectName() =>
        Assert.That(CharacterSpriteConfig.RunRight.Name, Is.EqualTo(AnimationNames.RunRight));

    [Test]
    public void IdleUp_HasCorrectName() =>
        Assert.That(CharacterSpriteConfig.IdleUp.Name, Is.EqualTo(AnimationNames.IdleUp));

    [Test]
    public void WalkUp_HasCorrectName() =>
        Assert.That(CharacterSpriteConfig.WalkUp.Name, Is.EqualTo(AnimationNames.WalkUp));

    [Test]
    public void RunUp_HasCorrectName() =>
        Assert.That(CharacterSpriteConfig.RunUp.Name, Is.EqualTo(AnimationNames.RunUp));

    // ─── Clip row assignments ─────────────────────────────────────────────────

    [TestCase("idle_down", 0)]
    [TestCase("walk_down", 0)]
    [TestCase("run_down", 0)]
    [TestCase("idle_left", 1)]
    [TestCase("walk_left", 1)]
    [TestCase("run_left", 1)]
    [TestCase("idle_right", 2)]
    [TestCase("walk_right", 2)]
    [TestCase("run_right", 2)]
    [TestCase("idle_up", 3)]
    [TestCase("walk_up", 3)]
    [TestCase("run_up", 3)]
    public void Clip_HasCorrectRow(string clipName, int expectedRow)
    {
        var clip = CharacterSpriteConfig.GetByName(clipName);
        Assert.That(clip, Is.Not.Null);
        Assert.That(clip!.Row, Is.EqualTo(expectedRow), $"Clip '{clipName}' has wrong row");
    }

    // ─── Clip start-column assignments ───────────────────────────────────────

    [TestCase("idle_down", 0)]
    [TestCase("idle_left", 0)]
    [TestCase("idle_right", 0)]
    [TestCase("idle_up", 0)]
    [TestCase("walk_down", 2)]
    [TestCase("walk_left", 2)]
    [TestCase("walk_right", 2)]
    [TestCase("walk_up", 2)]
    [TestCase("run_down", 6)]
    [TestCase("run_left", 6)]
    [TestCase("run_right", 6)]
    [TestCase("run_up", 6)]
    public void Clip_HasCorrectStartColumn(string clipName, int expectedStart)
    {
        var clip = CharacterSpriteConfig.GetByName(clipName);
        Assert.That(clip, Is.Not.Null);
        Assert.That(clip!.StartColumn, Is.EqualTo(expectedStart));
    }

    // ─── Clip frame-count assignments ─────────────────────────────────────────

    [TestCase("idle_down", 2)]
    [TestCase("idle_left", 2)]
    [TestCase("idle_right", 2)]
    [TestCase("idle_up", 2)]
    [TestCase("walk_down", 4)]
    [TestCase("walk_left", 4)]
    [TestCase("walk_right", 4)]
    [TestCase("walk_up", 4)]
    [TestCase("run_down", 4)]
    [TestCase("run_left", 4)]
    [TestCase("run_right", 4)]
    [TestCase("run_up", 4)]
    public void Clip_HasCorrectFrameCount(string clipName, int expectedCount)
    {
        var clip = CharacterSpriteConfig.GetByName(clipName);
        Assert.That(clip, Is.Not.Null);
        Assert.That(clip!.FrameCount, Is.EqualTo(expectedCount));
    }

    // ─── Clip FPS assignments ─────────────────────────────────────────────────

    [TestCase("idle_down", 5.0f)]
    [TestCase("idle_left", 5.0f)]
    [TestCase("idle_right", 5.0f)]
    [TestCase("idle_up", 5.0f)]
    [TestCase("walk_down", 8.0f)]
    [TestCase("walk_left", 8.0f)]
    [TestCase("walk_right", 8.0f)]
    [TestCase("walk_up", 8.0f)]
    [TestCase("run_down", 10.0f)]
    [TestCase("run_left", 10.0f)]
    [TestCase("run_right", 10.0f)]
    [TestCase("run_up", 10.0f)]
    public void Clip_HasCorrectFps(string clipName, float expectedFps)
    {
        var clip = CharacterSpriteConfig.GetByName(clipName);
        Assert.That(clip, Is.Not.Null);
        Assert.That(clip!.Fps, Is.EqualTo(expectedFps).Within(0.001f));
    }

    // ─── GetByName ────────────────────────────────────────────────────────────

    [Test]
    public void GetByName_ExistingClip_ReturnsClip() =>
        Assert.That(CharacterSpriteConfig.GetByName("idle_down"), Is.Not.Null);

    [Test]
    public void GetByName_UnknownName_ReturnsNull() =>
        Assert.That(CharacterSpriteConfig.GetByName("nonexistent"), Is.Null);

    [Test]
    public void GetByName_EmptyString_ReturnsNull() =>
        Assert.That(CharacterSpriteConfig.GetByName(""), Is.Null);

    [TestCase("idle_down")]
    [TestCase("walk_down")]
    [TestCase("run_down")]
    [TestCase("idle_left")]
    [TestCase("walk_left")]
    [TestCase("run_left")]
    [TestCase("idle_right")]
    [TestCase("walk_right")]
    [TestCase("run_right")]
    [TestCase("idle_up")]
    [TestCase("walk_up")]
    [TestCase("run_up")]
    public void GetByName_AllClipNames_FindsClip(string name)
    {
        Assert.That(CharacterSpriteConfig.GetByName(name), Is.Not.Null,
            $"GetByName could not find clip '{name}'");
    }

    // ─── All clips fit within sheet bounds ────────────────────────────────────

    [Test]
    public void AllClips_LastFrameColumn_IsWithinSheetColumns()
    {
        foreach (var clip in CharacterSpriteConfig.All)
        {
            int lastCol = clip.StartColumn + clip.FrameCount - 1;
            Assert.That(lastCol, Is.LessThan(CharacterSpriteConfig.Columns),
                $"Clip '{clip.Name}' exceeds sheet columns");
        }
    }

    [Test]
    public void AllClips_RowIndex_IsWithinSheetRows()
    {
        foreach (var clip in CharacterSpriteConfig.All)
        {
            Assert.That(clip.Row, Is.GreaterThanOrEqualTo(0));
            Assert.That(clip.Row, Is.LessThan(CharacterSpriteConfig.Rows),
                $"Clip '{clip.Name}' has row out of range");
        }
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// CharacterImportConfig Tests
// ═══════════════════════════════════════════════════════════════════════════════

[TestFixture]
public class CharacterImportConfigTest
{
    [Test]
    public void CompressMode_IsZero() =>
        Assert.That(CharacterImportConfig.CompressMode, Is.EqualTo(0));

    [Test]
    public void MipmapsEnabled_IsFalse() =>
        Assert.That(CharacterImportConfig.MipmapsEnabled, Is.False);

    [Test]
    public void Detect3DCompressTo_IsZero() =>
        Assert.That(CharacterImportConfig.Detect3DCompressTo, Is.EqualTo(0));

    [Test]
    public void ImporterType_IsTexture() =>
        Assert.That(CharacterImportConfig.ImporterType, Is.EqualTo("texture"));

    [Test]
    public void ResourceType_IsCompressedTexture2D() =>
        Assert.That(CharacterImportConfig.ResourceType, Is.EqualTo("CompressedTexture2D"));

    [Test]
    public void CharacterSpritesResPath_StartsWithResProtocol() =>
        Assert.That(CharacterImportConfig.CharacterSpritesResPath, Does.StartWith("res://"));

    [Test]
    public void PropSpritesResPath_StartsWithResProtocol() =>
        Assert.That(CharacterImportConfig.PropSpritesResPath, Does.StartWith("res://"));

    [Test]
    public void CharacterSpritesDirectory_ContainsCharacters() =>
        Assert.That(CharacterImportConfig.CharacterSpritesDirectory, Does.Contain("Characters"));

    [Test]
    public void PropSpritesDirectory_ContainsProps() =>
        Assert.That(CharacterImportConfig.PropSpritesDirectory, Does.Contain("Props"));

    [Test]
    public void CharacterSpritesResPath_ContainsCharacters() =>
        Assert.That(CharacterImportConfig.CharacterSpritesResPath, Does.Contain("Characters"));

    [Test]
    public void PropSpritesResPath_ContainsProps() =>
        Assert.That(CharacterImportConfig.PropSpritesResPath, Does.Contain("Props"));
}

// ═══════════════════════════════════════════════════════════════════════════════
// PropConfig Tests
// ═══════════════════════════════════════════════════════════════════════════════

[TestFixture]
public class PropConfigTest
{
    private PropConfig _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _sut = new PropConfig(
            "Test Prop", "prop_test.png",
            width: 48, height: 64,
            expectedColorHexCodes: new[] { "5c3d2e", "8b7355" });
    }

    // ─── Constructor ──────────────────────────────────────────────────────────

    [Test]
    public void Constructor_SetsName() =>
        Assert.That(_sut.Name, Is.EqualTo("Test Prop"));

    [Test]
    public void Constructor_SetsFileName() =>
        Assert.That(_sut.FileName, Is.EqualTo("prop_test.png"));

    [Test]
    public void Constructor_SetsWidth() =>
        Assert.That(_sut.Width, Is.EqualTo(48));

    [Test]
    public void Constructor_SetsHeight() =>
        Assert.That(_sut.Height, Is.EqualTo(64));

    [Test]
    public void Constructor_SetsExpectedColorHexCodes() =>
        Assert.That(_sut.ExpectedColorHexCodes, Is.EquivalentTo(new[] { "5c3d2e", "8b7355" }));

    // ─── Computed paths ───────────────────────────────────────────────────────

    [Test]
    public void ResourcePath_HasResProtocol() =>
        Assert.That(_sut.ResourcePath, Does.StartWith("res://"));

    [Test]
    public void ResourcePath_EndsWithFileName() =>
        Assert.That(_sut.ResourcePath, Does.EndWith("prop_test.png"));

    [Test]
    public void ResourcePath_ContainsPropDirectory() =>
        Assert.That(_sut.ResourcePath, Does.Contain("Props"));

    [Test]
    public void FilePath_EndsWithFileName() =>
        Assert.That(_sut.FilePath, Does.EndWith("prop_test.png"));

    [Test]
    public void FilePath_ContainsPropDirectory() =>
        Assert.That(_sut.FilePath, Does.Contain("Props"));

    [Test]
    public void FilePath_DoesNotHaveResProtocol() =>
        Assert.That(_sut.FilePath, Does.Not.StartWith("res://"));

    // ─── Immutability ─────────────────────────────────────────────────────────

    [Test]
    public void ExpectedColorHexCodes_IsImmutable_MutatingSourceArrayDoesNotAffectConfig()
    {
        var source = new[] { "5c3d2e", "8b7355" };
        var prop = new PropConfig("P", "p.png", 16, 16, source);
        source[0] = "ffffff";
        Assert.That(prop.ExpectedColorHexCodes[0], Is.EqualTo("5c3d2e"));
    }

    [Test]
    public void Constructor_SingleColor_HasOneExpectedHex()
    {
        var prop = new PropConfig("P", "p.png", 16, 32, new[] { "5c3d2e" });
        Assert.That(prop.ExpectedColorHexCodes.Count, Is.EqualTo(1));
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// PropRegistry Tests
// ═══════════════════════════════════════════════════════════════════════════════

[TestFixture]
public class PropRegistryTest
{
    // ─── Count ────────────────────────────────────────────────────────────────

    [Test]
    public void All_ContainsExactlyExpectedPropCount() =>
        Assert.That(PropRegistry.All.Length, Is.EqualTo(PropRegistry.ExpectedPropCount));

    [Test]
    public void ExpectedPropCount_IsFive() =>
        Assert.That(PropRegistry.ExpectedPropCount, Is.EqualTo(5));

    // ─── Individual props exist ───────────────────────────────────────────────

    [Test] public void Door_IsRegistered() => Assert.That(PropRegistry.Door, Is.Not.Null);
    [Test] public void Well_IsRegistered() => Assert.That(PropRegistry.Well, Is.Not.Null);
    [Test] public void Tree_IsRegistered() => Assert.That(PropRegistry.Tree, Is.Not.Null);
    [Test] public void HayBale_IsRegistered() => Assert.That(PropRegistry.HayBale, Is.Not.Null);
    [Test] public void FencePost_IsRegistered() => Assert.That(PropRegistry.FencePost, Is.Not.Null);

    // ─── File names match spec ────────────────────────────────────────────────

    [TestCase("prop_door.png")]
    [TestCase("prop_well.png")]
    [TestCase("prop_tree.png")]
    [TestCase("prop_haybale.png")]
    [TestCase("prop_fencepost.png")]
    public void All_ContainsPropWithFileName(string fileName)
    {
        Assert.That(PropRegistry.All.Any(p => p.FileName == fileName), Is.True,
            $"Missing prop: {fileName}");
    }

    // ─── Prop dimensions ──────────────────────────────────────────────────────

    [Test]
    public void Door_HasCorrectDimensions()
    {
        Assert.That(PropRegistry.Door.Width, Is.EqualTo(48));
        Assert.That(PropRegistry.Door.Height, Is.EqualTo(64));
    }

    [Test]
    public void Well_HasCorrectDimensions()
    {
        Assert.That(PropRegistry.Well.Width, Is.EqualTo(48));
        Assert.That(PropRegistry.Well.Height, Is.EqualTo(48));
    }

    [Test]
    public void Tree_HasCorrectDimensions()
    {
        Assert.That(PropRegistry.Tree.Width, Is.EqualTo(48));
        Assert.That(PropRegistry.Tree.Height, Is.EqualTo(64));
    }

    [Test]
    public void HayBale_HasCorrectDimensions()
    {
        Assert.That(PropRegistry.HayBale.Width, Is.EqualTo(48));
        Assert.That(PropRegistry.HayBale.Height, Is.EqualTo(32));
    }

    [Test]
    public void FencePost_HasCorrectDimensions()
    {
        Assert.That(PropRegistry.FencePost.Width, Is.EqualTo(16));
        Assert.That(PropRegistry.FencePost.Height, Is.EqualTo(32));
    }

    // ─── Uniqueness ───────────────────────────────────────────────────────────

    [Test]
    public void All_AllFileNamesAreUnique()
    {
        var seen = new HashSet<string>();
        foreach (var prop in PropRegistry.All)
            Assert.That(seen.Add(prop.FileName), Is.True,
                $"Duplicate filename: {prop.FileName}");
    }

    [Test]
    public void All_AllNamesAreUnique()
    {
        var seen = new HashSet<string>();
        foreach (var prop in PropRegistry.All)
            Assert.That(seen.Add(prop.Name), Is.True,
                $"Duplicate name: {prop.Name}");
    }

    [Test]
    public void All_AllResourcePathsAreUnique()
    {
        var paths = PropRegistry.All.Select(p => p.ResourcePath).ToList();
        Assert.That(paths.Distinct().Count(), Is.EqualTo(paths.Count));
    }

    // ─── Defensive copy ───────────────────────────────────────────────────────

    [Test]
    public void All_ReturnsDefensiveCopy()
    {
        var first = PropRegistry.All;
        var second = PropRegistry.All;
        Assert.That(ReferenceEquals(first, second), Is.False);
    }

    // ─── Lookup: GetByFileName ────────────────────────────────────────────────

    [Test]
    public void GetByFileName_ExistingFile_ReturnsProp() =>
        Assert.That(PropRegistry.GetByFileName("prop_door.png"), Is.Not.Null);

    [Test]
    public void GetByFileName_Door_ReturnsExactInstance() =>
        Assert.That(PropRegistry.GetByFileName("prop_door.png"),
            Is.SameAs(PropRegistry.Door));

    [Test]
    public void GetByFileName_Well_ReturnsExactInstance() =>
        Assert.That(PropRegistry.GetByFileName("prop_well.png"),
            Is.SameAs(PropRegistry.Well));

    [Test]
    public void GetByFileName_Tree_ReturnsExactInstance() =>
        Assert.That(PropRegistry.GetByFileName("prop_tree.png"),
            Is.SameAs(PropRegistry.Tree));

    [Test]
    public void GetByFileName_HayBale_ReturnsExactInstance() =>
        Assert.That(PropRegistry.GetByFileName("prop_haybale.png"),
            Is.SameAs(PropRegistry.HayBale));

    [Test]
    public void GetByFileName_FencePost_ReturnsExactInstance() =>
        Assert.That(PropRegistry.GetByFileName("prop_fencepost.png"),
            Is.SameAs(PropRegistry.FencePost));

    [Test]
    public void GetByFileName_Unknown_ReturnsNull() =>
        Assert.That(PropRegistry.GetByFileName("nonexistent.png"), Is.Null);

    [Test]
    public void GetByFileName_EmptyString_ReturnsNull() =>
        Assert.That(PropRegistry.GetByFileName(""), Is.Null);

    // ─── Lookup: GetByName ────────────────────────────────────────────────────

    [Test]
    public void GetByName_ExistingName_ReturnsProp() =>
        Assert.That(PropRegistry.GetByName("Cottage Door"), Is.Not.Null);

    [Test]
    public void GetByName_Door_ReturnsExactInstance() =>
        Assert.That(PropRegistry.GetByName("Cottage Door"),
            Is.SameAs(PropRegistry.Door));

    [Test]
    public void GetByName_Well_ReturnsExactInstance() =>
        Assert.That(PropRegistry.GetByName("Well"),
            Is.SameAs(PropRegistry.Well));

    [Test]
    public void GetByName_Tree_ReturnsExactInstance() =>
        Assert.That(PropRegistry.GetByName("Deciduous Tree"),
            Is.SameAs(PropRegistry.Tree));

    [Test]
    public void GetByName_HayBale_ReturnsExactInstance() =>
        Assert.That(PropRegistry.GetByName("Hay Bale"),
            Is.SameAs(PropRegistry.HayBale));

    [Test]
    public void GetByName_FencePost_ReturnsExactInstance() =>
        Assert.That(PropRegistry.GetByName("Fence Post"),
            Is.SameAs(PropRegistry.FencePost));

    [Test]
    public void GetByName_Unknown_ReturnsNull() =>
        Assert.That(PropRegistry.GetByName("nonexistent"), Is.Null);

    [Test]
    public void GetByName_EmptyString_ReturnsNull() =>
        Assert.That(PropRegistry.GetByName(""), Is.Null);

    // ─── All have expected colors ──────────────────────────────────────────────

    [Test]
    public void All_AllPropsHaveAtLeastOneExpectedColor()
    {
        foreach (var prop in PropRegistry.All)
            Assert.That(prop.ExpectedColorHexCodes.Count, Is.GreaterThan(0),
                $"Prop '{prop.Name}' has no expected colors");
    }

    [Test]
    public void All_AllExpectedColors_AreApprovedPaletteColors()
    {
        var approvedHex = Palette.All
            .Select(c => c.ToHtml(includeAlpha: false).TrimStart('#').ToLowerInvariant())
            .ToHashSet();

        foreach (var prop in PropRegistry.All)
        {
            foreach (var hex in prop.ExpectedColorHexCodes)
            {
                Assert.That(approvedHex.Contains(hex.ToLowerInvariant()),
                    $"Prop '{prop.Name}' uses non-palette color '#{hex}'");
            }
        }
    }

    // ─── Resource and file paths ──────────────────────────────────────────────

    [Test]
    public void All_AllResourcePaths_StartWithResProtocol()
    {
        foreach (var prop in PropRegistry.All)
            Assert.That(prop.ResourcePath, Does.StartWith("res://"),
                $"Prop '{prop.Name}' ResourcePath missing res:// prefix");
    }

    [Test]
    public void All_AllFilePaths_EndWithFileName()
    {
        foreach (var prop in PropRegistry.All)
            Assert.That(prop.FilePath, Does.EndWith(prop.FileName),
                $"Prop '{prop.Name}' FilePath doesn't end with FileName");
    }

    // ─── Tree prop: no illegal color ─────────────────────────────────────────

    [Test]
    public void Tree_DoesNotContainNonPaletteColor_2d5a2d()
    {
        Assert.That(PropRegistry.Tree.ExpectedColorHexCodes, Does.Not.Contain("2d5a2d"),
            "prop_tree.png must not list #2d5a2d — it is not in the approved palette");
    }
}
