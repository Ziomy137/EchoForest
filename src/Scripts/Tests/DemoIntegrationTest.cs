using Godot;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

/// <summary>
/// S4-03 Integration tests: verifies that the core pure-C# systems work together
/// correctly as they would in a running demo session.
///
/// Godot-specific checks (scene loading, collision, frame rate) are verified via
/// the manual QA checklist and GUT tests — they require the Godot engine scene tree
/// and cannot run in NUnit.
/// </summary>
[TestFixture]
public class DemoIntegrationTest
{
    // ─── Shared timing constants ───────────────────────────────────────────────

    private const int Fps = Constants.TargetFps;
    private const float Delta = 1f / Constants.TargetFps;

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private static PlayerController MakePlayer(MockInputHandler input) =>
        new(input, new PlayerStateMachine());

    // ─── Player moves right for one second (60 frames) ────────────────────────

    [Test]
    public void Demo_PlayerController_MovesRight_ForOneSecond()
    {
        var input = new MockInputHandler();
        var player = MakePlayer(input);
        input.SetPressed(InputActionNames.MoveRight, true);
        float startX = player.Position.X;
        for (int i = 0; i < Fps; i++)
            player.SimulatePhysicsFrame(Delta);
        Assert.That(player.Position.X, Is.GreaterThan(startX));
    }

    [Test]
    public void Demo_PlayerController_MovesLeft_ForOneSecond()
    {
        var input = new MockInputHandler();
        var player = MakePlayer(input);
        input.SetPressed(InputActionNames.MoveLeft, true);
        float startX = player.Position.X;
        for (int i = 0; i < Fps; i++)
            player.SimulatePhysicsFrame(Delta);
        Assert.That(player.Position.X, Is.LessThan(startX));
    }

    [Test]
    public void Demo_PlayerController_MovesDown_ForOneSecond()
    {
        var input = new MockInputHandler();
        var player = MakePlayer(input);
        input.SetPressed(InputActionNames.MoveDown, true);
        float startY = player.Position.Y;
        for (int i = 0; i < Fps; i++)
            player.SimulatePhysicsFrame(Delta);
        Assert.That(player.Position.Y, Is.GreaterThan(startY));
    }

    [Test]
    public void Demo_PlayerController_MovesUp_ForOneSecond()
    {
        var input = new MockInputHandler();
        var player = MakePlayer(input);
        input.SetPressed(InputActionNames.MoveUp, true);
        float startY = player.Position.Y;
        for (int i = 0; i < Fps; i++)
            player.SimulatePhysicsFrame(Delta);
        Assert.That(player.Position.Y, Is.LessThan(startY));
    }

    // ─── Running is visually and numerically faster ───────────────────────────

    [Test]
    public void Demo_Running_CoversMoreDistanceThanWalking_InSameTime()
    {
        var inputWalk = new MockInputHandler();
        var walk = MakePlayer(inputWalk);
        inputWalk.SetPressed(InputActionNames.MoveRight, true);

        var inputRun = new MockInputHandler();
        var run = MakePlayer(inputRun);
        inputRun.SetPressed(InputActionNames.MoveRight, true);
        inputRun.SetPressed(InputActionNames.Run, true);

        for (int i = 0; i < Fps; i++)
        {
            walk.SimulatePhysicsFrame(Delta);
            run.SimulatePhysicsFrame(Delta);
        }
        Assert.That(run.Position.X, Is.GreaterThan(walk.Position.X));
    }

    [Test]
    public void Demo_RunSpeed_IsExactlyDoubleWalkSpeed()
    {
        Assert.That(Constants.RunSpeed, Is.EqualTo(Constants.WalkSpeed * 2f));
    }

    // ─── State machine walkthrough: Idle → Walk → Run → Idle ─────────────────

    [Test]
    public void Demo_FullStateSequence_IdleWalkRunIdle()
    {
        var input = new MockInputHandler();
        var player = MakePlayer(input);

        // Idle
        player.SimulatePhysicsFrame(Delta);
        Assert.That(player.CurrentState, Is.EqualTo(PlayerState.Idle), "Expected Idle on no input");

        // Walk
        input.SetPressed(InputActionNames.MoveRight, true);
        player.SimulatePhysicsFrame(Delta);
        Assert.That(player.CurrentState, Is.EqualTo(PlayerState.Walking), "Expected Walking on move");

        // Run
        input.SetPressed(InputActionNames.Run, true);
        player.SimulatePhysicsFrame(Delta);
        Assert.That(player.CurrentState, Is.EqualTo(PlayerState.Running), "Expected Running with run held");

        // Back to Idle
        input.Reset();
        player.SimulatePhysicsFrame(Delta);
        Assert.That(player.CurrentState, Is.EqualTo(PlayerState.Idle), "Expected Idle after releasing all keys");
    }

    // ─── All four directions cycle correctly ──────────────────────────────────

    [Test]
    public void Demo_AllFourDirections_FacingUpdatesCorrectly()
    {
        var input = new MockInputHandler();
        var player = MakePlayer(input);

        input.SetPressed(InputActionNames.MoveRight, true);
        player.SimulatePhysicsFrame(Delta);
        Assert.That(player.FacingDirection, Is.EqualTo(Direction.Right));

        input.Reset();
        input.SetPressed(InputActionNames.MoveLeft, true);
        player.SimulatePhysicsFrame(Delta);
        Assert.That(player.FacingDirection, Is.EqualTo(Direction.Left));

        input.Reset();
        input.SetPressed(InputActionNames.MoveDown, true);
        player.SimulatePhysicsFrame(Delta);
        Assert.That(player.FacingDirection, Is.EqualTo(Direction.Down));

        input.Reset();
        input.SetPressed(InputActionNames.MoveUp, true);
        player.SimulatePhysicsFrame(Delta);
        Assert.That(player.FacingDirection, Is.EqualTo(Direction.Up));
    }

    // ─── Camera follows player smoothly ───────────────────────────────────────

    [Test]
    public void Demo_Camera_FollowsPlayerPosition_AfterUpdate()
    {
        var input = new MockInputHandler();
        var player = MakePlayer(input);
        var cam = new CameraController();
        cam.SetBounds(CottageSceneConfig.CameraBounds);
        cam.SnapToPixels = true;

        input.SetPressed(InputActionNames.MoveRight, true);

        for (int i = 0; i < Fps / 2; i++)
        {
            player.SimulatePhysicsFrame(Delta);
            cam.SetTarget(player.Position);
            cam.Update(Delta);
        }

        // Camera must have moved toward player
        Assert.That(cam.Position.X, Is.GreaterThan(0f));
    }

    [Test]
    public void Demo_Camera_DoesNotExceedBounds_WhenPlayerRunsToEdge()
    {
        var input = new MockInputHandler();
        var player = MakePlayer(input);
        var cam = new CameraController();
        cam.SetBounds(CottageSceneConfig.CameraBounds);

        // Simulate player running right for 10 seconds (would easily leave the cottage map)
        input.SetPressed(InputActionNames.MoveRight, true);
        input.SetPressed(InputActionNames.Run, true);
        for (int i = 0; i < Fps * 10; i++)
        {
            player.SimulatePhysicsFrame(Delta);
            cam.SetTarget(player.Position);
            cam.Update(Delta);
        }

        var bounds = CottageSceneConfig.CameraBounds;
        Assert.That(cam.Position.X, Is.LessThanOrEqualTo(bounds.Position.X + bounds.Size.X));
    }

    [Test]
    public void Demo_Camera_PixelSnapEnabled_PositionIsIntegerAligned()
    {
        var input = new MockInputHandler();
        var player = MakePlayer(input);
        var cam = new CameraController { SnapToPixels = true };
        cam.SetBounds(CottageSceneConfig.CameraBounds);

        input.SetPressed(InputActionNames.MoveRight, true);
        for (int i = 0; i < Fps / 2; i++)
        {
            player.SimulatePhysicsFrame(Delta);
            cam.SetTarget(player.Position);
            cam.Update(Delta);
        }

        Assert.That(cam.Position.X % 1f, Is.EqualTo(0f).Within(0.001f));
        Assert.That(cam.Position.Y % 1f, Is.EqualTo(0f).Within(0.001f));
    }

    // ─── All pure-C# systems have expected pre-init defaults ──────────────────

    [Test]
    public void Demo_AllSystems_PreInit_HaveExpectedDefaults()
    {
        var input = new MockInputHandler();
        var sm = new PlayerStateMachine();
        var player = new PlayerController(input, sm);
        var cam = new CameraController();
        var hud = new HudController();

        // Verify pre-initialization state — distinct from AfterInit which calls hud.Initialize()
        Assert.That(player.FacingDirection, Is.EqualTo(Direction.Down));
        Assert.That(hud.IsTutorialHintVisible, Is.False);
    }

    [Test]
    public void Demo_AllSystems_AfterInit_HaveExpectedDefaults()
    {
        var input = new MockInputHandler();
        var player = MakePlayer(input);
        var cam = new CameraController();
        var hud = new HudController();
        hud.Initialize();

        Assert.That(player.CurrentState, Is.EqualTo(PlayerState.Idle));
        Assert.That(player.Velocity, Is.EqualTo(Vector2.Zero));
        Assert.That(cam.Position, Is.EqualTo(Vector2.Zero));
        Assert.That(hud.IsTutorialHintVisible, Is.True);
    }

    // ─── Scene config consistency ─────────────────────────────────────────────

    [Test]
    public void Demo_CottageSceneConfig_SpawnPointWithinBounds()
    {
        var bounds = CottageSceneConfig.CameraBounds;
        Assert.That(CottageSceneConfig.SpawnWorldX,
            Is.InRange(bounds.Position.X, bounds.Position.X + bounds.Size.X),
            "Spawn X must be within camera bounds");
        Assert.That(CottageSceneConfig.SpawnWorldY,
            Is.InRange(bounds.Position.Y, bounds.Position.Y + bounds.Size.Y),
            "Spawn Y must be within camera bounds");
    }

    [Test]
    public void Demo_CottageSceneConfig_CameraBoundsContainConfiguredWorldBoundaries()
    {
        var bounds = CottageSceneConfig.CameraBounds;
        var minX = bounds.Position.X;
        var maxX = bounds.Position.X + bounds.Size.X;

        Assert.That(bounds.Size.X, Is.GreaterThan(0f), "Camera bounds width must be positive");
        Assert.That(bounds.Size.Y, Is.GreaterThan(0f), "Camera bounds height must be positive");
        Assert.That(CottageSceneConfig.WorldBoundaryLeft,
            Is.InRange(minX, maxX),
            "Left world boundary must be within camera bounds");
        Assert.That(CottageSceneConfig.WorldBoundaryRight,
            Is.InRange(minX, maxX),
            "Right world boundary must be within camera bounds");
    }

    // ─── HUD lifecycle ────────────────────────────────────────────────────────

    [Test]
    public void Demo_HUD_IsVisible_OnStartup()
    {
        var hud = new HudController();
        hud.Initialize();
        Assert.That(hud.IsTutorialHintVisible, Is.True);
    }

    [Test]
    public void Demo_HUD_TutorialHint_FadesAfterTimeout()
    {
        var hud = new HudController(hintTimeoutSeconds: 1f);
        hud.Initialize();
        hud.SimulateTimePassed(2f);
        Assert.That(hud.IsTutorialHintVisible, Is.False);
    }

    [Test]
    public void Demo_HUD_DebugLabel_CanBeDisabled()
    {
        var hud = new HudController();
        hud.SetDebugMode(false);
        Assert.That(hud.IsDebugLabelVisible, Is.False);
    }

    // ─── Camera boundary guard (long run left) ────────────────────────────────

    [Test]
    public void Demo_Camera_BoundaryGuard_AfterLongRunLeft()
    {
        var input = new MockInputHandler();
        var player = MakePlayer(input);
        var cam = new CameraController();
        cam.SetBounds(CottageSceneConfig.CameraBounds);

        // Simulate player running left for 10 seconds (would easily leave the cottage map)
        input.SetPressed(InputActionNames.MoveLeft, true);
        input.SetPressed(InputActionNames.Run, true);
        for (int i = 0; i < Fps * 10; i++)
        {
            player.SimulatePhysicsFrame(Delta);
            cam.SetTarget(player.Position);
            cam.Update(Delta);
        }

        var bounds = CottageSceneConfig.CameraBounds;
        Assert.That(cam.Position.X, Is.GreaterThanOrEqualTo(bounds.Position.X));
    }

}
