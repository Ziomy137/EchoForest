using System;
using System.Collections.Generic;
using Godot;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class PaletteTest
{
    // ─── Palette.All count ───────────────────────────────────────────────────

    [Test]
    public void Palette_Contains_ExactlySixteenColors()
    {
        Assert.That(Palette.All.Length, Is.EqualTo(16));
    }

    [Test]
    public void Palette_All_ContainsNoNullEntries()
    {
        foreach (var color in Palette.All)
            Assert.That(color, Is.Not.EqualTo(default(Color)));
    }

    // ─── Individual hex values ────────────────────────────────────────────────

    // Godot Color.ToHtml(false) returns lowercase hex WITHOUT a '#' prefix.
    [TestCase("DeepBlack", "1a1a1a")]
    [TestCase("DarkBrown", "2d2416")]
    [TestCase("DarkGray", "3d3d3d")]
    [TestCase("MediumGray", "5a5a5a")]
    [TestCase("WarmBrown", "8b7355")]
    [TestCase("DarkLeather", "5c3d2e")]
    [TestCase("DarkRed", "8b0000")]
    [TestCase("DeepPurple", "2a1a4a")]
    [TestCase("DarkOrange", "ff6b00")]
    [TestCase("Gold", "ffd700")]
    [TestCase("DarkGreen", "1a3a1a")]
    [TestCase("DeepWater", "1a3a5c")]
    [TestCase("SkinTone", "8b6f47")]
    [TestCase("LightSkin", "a88860")]
    [TestCase("White", "ffffff")]
    [TestCase("LightGray", "cccccc")]
    public void Palette_Color_HasCorrectHexValue(string colorName, string expectedHex)
    {
        var color = colorName switch
        {
            "DeepBlack" => Palette.DeepBlack,
            "DarkBrown" => Palette.DarkBrown,
            "DarkGray" => Palette.DarkGray,
            "MediumGray" => Palette.MediumGray,
            "WarmBrown" => Palette.WarmBrown,
            "DarkLeather" => Palette.DarkLeather,
            "DarkRed" => Palette.DarkRed,
            "DeepPurple" => Palette.DeepPurple,
            "DarkOrange" => Palette.DarkOrange,
            "Gold" => Palette.Gold,
            "DarkGreen" => Palette.DarkGreen,
            "DeepWater" => Palette.DeepWater,
            "SkinTone" => Palette.SkinTone,
            "LightSkin" => Palette.LightSkin,
            "White" => Palette.White,
            "LightGray" => Palette.LightGray,
            _ => throw new ArgumentException($"Unknown color: {colorName}")
        };

        Assert.That(color.ToHtml(false), Is.EqualTo(expectedHex));
    }

    // ─── Palette.All contains every named color ───────────────────────────────

    [Test]
    public void Palette_All_ContainsDeepBlack() => Assert.That(Palette.All, Contains.Item(Palette.DeepBlack));
    [Test]
    public void Palette_All_ContainsWhite() => Assert.That(Palette.All, Contains.Item(Palette.White));
    [Test]
    public void Palette_All_ContainsGold() => Assert.That(Palette.All, Contains.Item(Palette.Gold));
    [Test]
    public void Palette_All_ContainsDarkRed() => Assert.That(Palette.All, Contains.Item(Palette.DarkRed));

    // ─── All colors in Palette.All are unique ────────────────────────────────

    [Test]
    public void Palette_All_AllColorsAreUnique()
    {
        var seen = new System.Collections.Generic.HashSet<string>();
        foreach (var c in Palette.All)
            Assert.That(seen.Add(c.ToHtml(false)), Is.True,
                $"Duplicate color in Palette.All: {c.ToHtml(false)}");
    }
}

[TestFixture]
public class PaletteValidatorTest
{
    // ─── IsApprovedColor — approved colors ───────────────────────────────────

    [TestCase("#1a1a1a")]
    [TestCase("#2d2416")]
    [TestCase("#3d3d3d")]
    [TestCase("#5a5a5a")]
    [TestCase("#8b7355")]
    [TestCase("#5c3d2e")]
    [TestCase("#8b0000")]
    [TestCase("#2a1a4a")]
    [TestCase("#ff6b00")]
    [TestCase("#ffd700")]
    [TestCase("#1a3a1a")]
    [TestCase("#1a3a5c")]
    [TestCase("#8b6f47")]
    [TestCase("#a88860")]
    [TestCase("#ffffff")]
    [TestCase("#cccccc")]
    public void IsApprovedColor_PaletteColor_ReturnsTrue(string hex)
    {
        Assert.That(PaletteValidator.IsApprovedColor(new Color(hex)), Is.True);
    }

    // ─── IsApprovedColor — non-palette colors ────────────────────────────────

    [TestCase("#ff0000")]   // pure red
    [TestCase("#00ff00")]   // pure green
    [TestCase("#0000ff")]   // pure blue
    [TestCase("#123456")]   // arbitrary hex
    [TestCase("#abcdef")]   // arbitrary hex
    public void IsApprovedColor_NonPaletteColor_ReturnsFalse(string hex)
    {
        Assert.That(PaletteValidator.IsApprovedColor(new Color(hex)), Is.False);
    }

    // ─── All colors in Palette.All are approved ───────────────────────────────

    [Test]
    public void IsApprovedColor_AllPaletteColors_ReturnTrue()
    {
        foreach (var color in Palette.All)
            Assert.That(PaletteValidator.IsApprovedColor(color), Is.True,
                $"Palette color {color.ToHtml(false)} should be approved");
    }

    // ─── Edge cases ───────────────────────────────────────────────────────────

    [Test]
    public void IsApprovedColor_TransparentVariantOfPaletteColor_ReturnsFalse()
    {
        // Semi-transparent variant — alpha strip means it doesn't match
        var semiTransparent = new Color(Palette.White.R, Palette.White.G, Palette.White.B, 0.5f);
        Assert.That(PaletteValidator.IsApprovedColor(semiTransparent), Is.False);
    }

    [Test]
    public void IsApprovedColor_FullyTransparentPixel_ReturnsFalse()
    {
        Assert.That(PaletteValidator.IsApprovedColor(new Color(0, 0, 0, 0)), Is.False);
    }
}
