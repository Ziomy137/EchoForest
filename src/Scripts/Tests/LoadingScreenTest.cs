using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// Unit tests for <see cref="LoadingScreen"/>.
/// All tests run without the Godot runtime.
/// </summary>
[TestFixture]
public class LoadingScreenTest
{
    // ── Initial state ─────────────────────────────────────────────────────────

    [Test]
    public void LoadingScreen_InitiallyNotVisible()
    {
        var screen = new LoadingScreen();
        Assert.That(screen.IsVisible, Is.False);
    }

    // ── Show ──────────────────────────────────────────────────────────────────

    [Test]
    public void Show_SetsIsVisibleTrue()
    {
        var screen = new LoadingScreen();
        screen.Show();
        Assert.That(screen.IsVisible, Is.True);
    }

    [Test]
    public void Show_CalledTwice_RemainsVisible()
    {
        var screen = new LoadingScreen();
        screen.Show();
        screen.Show();
        Assert.That(screen.IsVisible, Is.True);
    }

    // ── Hide ──────────────────────────────────────────────────────────────────

    [Test]
    public void Hide_WhenVisible_SetsIsVisibleFalse()
    {
        var screen = new LoadingScreen();
        screen.Show();
        screen.Hide();
        Assert.That(screen.IsVisible, Is.False);
    }

    [Test]
    public void Hide_WhenAlreadyHidden_RemainsHidden()
    {
        var screen = new LoadingScreen();
        screen.Hide();
        Assert.That(screen.IsVisible, Is.False);
    }

    [Test]
    public void Hide_CalledTwice_RemainsHidden()
    {
        var screen = new LoadingScreen();
        screen.Show();
        screen.Hide();
        screen.Hide();
        Assert.That(screen.IsVisible, Is.False);
    }

    // ── Transitions ───────────────────────────────────────────────────────────

    [Test]
    public void ShowThenHide_IsNotVisible()
    {
        var screen = new LoadingScreen();
        screen.Show();
        screen.Hide();
        Assert.That(screen.IsVisible, Is.False);
    }

    [Test]
    public void ShowHideThenShow_IsVisible()
    {
        var screen = new LoadingScreen();
        screen.Show();
        screen.Hide();
        screen.Show();
        Assert.That(screen.IsVisible, Is.True);
    }
}
