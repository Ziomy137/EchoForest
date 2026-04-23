# Sprint Plan — EchoForest Demo

## "Playable Cottage Area Demo"

**Document Version:** 1.0  
**Last Updated:** April 8, 2026  
**Target:** Playable demo — game engine running, one test area (Cottage Exterior), basic pixel art textures  
**Methodology:** Test-Driven Development (TDD) — tests written before implementation  
**Platform:** Windows / Linux / macOS (Godot + C#)  
**Sprint Length:** 2 weeks  
**Total Estimated Duration:** ~10 weeks (5 sprints)

---

## Demo Definition of Done

The demo is considered complete when:

- [ ] Godot project runs on all three platforms without errors
- [ ] Player can walk and run in 4 directions on an isometric tile grid
- [ ] Cottage exterior test area is fully navigable with basic pixel art tiles
- [ ] All C# game logic has ≥ 90% unit test coverage
- [ ] CI pipeline runs all tests automatically on every commit
- [ ] No critical runtime errors or unhandled exceptions
- [ ] Frame rate is stable at 60 FPS on target hardware

---

## TDD Workflow — Team Agreement

All developers follow this cycle for every task:

```
1. RED   → Write a failing unit test for the expected behavior
2. GREEN → Write the minimum code to make the test pass
3. REFACTOR → Clean code without breaking tests
4. COMMIT → Commit test + implementation together
```

**Test Framework Stack:**

- **GUT** (Godot Unit Test) — for scene/node integration tests
- **NUnit** — for pure C# business logic and system tests
- **GitHub Actions** — CI pipeline running tests on every push
- **Coverage Tool:** `dotnet-coverage` or Coverlet for C# coverage reports

**Test File Convention:**

```
Scripts/
├── PlayerController.cs
└── Tests/
    └── PlayerControllerTest.cs
```

**Coverage Gate:** PRs blocked if coverage drops below 90%.

---

## Sprint 0 — Project Foundation

**Duration:** 2 weeks | **Dates:** Week 1–2  
**Goal:** Godot project initialized, CI/CD running, test framework active, all developers can build locally

---

### S0-01 — Initialize Godot Project Structure

**Type:** Setup  
**Assignee:** Lead Developer  
**Estimate:** 3 points

**Tasks:**

- [x] Create new Godot 4.x project with C# support enabled
- [x] Set up project folder structure as defined in [tdd.md](tdd.md):
  ```
  EchoForest/
  ├── src/
  │   ├── Scenes/
  │   ├── Scripts/
  │   │   └── Tests/
  │   └── Assets/
  │       ├── Sprites/
  │       ├── Audio/
  │       └── Data/
  ├── docs/
  └── .github/workflows/
  ```
- [x] Configure `.gitignore` for Godot + C# (`.mono/`, `build/`, `*.import`)
- [x] Set project display name, icon placeholder, initial window settings
- [x] Configure window modes: Windowed (resizable) and Borderless Fullscreen

**Acceptance Criteria:**

- Project opens in Godot without errors
- Folder structure matches TDD specification
- `.gitignore` prevents committing build artifacts

**Tests Required:**

- N/A (project setup — CI pipeline validates it compiles)

---

### S0-02 — Configure CI/CD Pipeline

**Type:** DevOps  
**Assignee:** DevOps / Lead Developer  
**Estimate:** 5 points

**Tasks:**

- [x] Create `.github/workflows/ci.yml` (or equivalent CI config)
- [x] Pipeline steps:
  1. Checkout repository
  2. Install .NET SDK (matching Godot's C# version)
  3. Restore NuGet packages
  4. Build solution
  5. Run NUnit tests (`dotnet test`)
  6. Generate coverage report (Coverlet)
  7. Fail build if coverage < 90%
- [x] Configure pipeline to run on: `push` to any branch, `pull_request` to `main`
- [x] Add pipeline status badge to `README.md`

**Acceptance Criteria:**

- Pipeline runs automatically on every commit
- Build fails if any test fails
- Build fails if coverage drops below 90%
- Coverage report is accessible as pipeline artifact

**Tests Required:**

- Write one placeholder passing test `EngineBootTest.cs` to confirm CI runs correctly:
  ```csharp
  // RED first: write test that asserts true (confirms test runner works)
  [Test] public void CI_TestRunnerIsOperational() => Assert.Pass();
  ```

---

### S0-03 — Integrate NUnit and GUT Test Frameworks

**Type:** Setup  
**Assignee:** Lead Developer  
**Estimate:** 3 points

**Tasks:**

- [x] Add `NUnit` and `NUnit3TestAdapter` NuGet packages to `.csproj`
- [x] Add `Coverlet.collector` for coverage reporting
- [x] Install GUT addon in Godot (from AssetLib or manual install)
- [x] Create `Tests/` directory with a sample test class
- [x] Write documentation: `docs/testing-guide.md` (2-page guide on how to run tests locally)
- [x] Verify `dotnet test` runs and reports correctly from CLI

**Acceptance Criteria:**

- `dotnet test` runs successfully in terminal
- GUT addon appears in Godot editor without errors
- Sample test passes in both frameworks

**Tests Required:**

```csharp
// EngineBootTest.cs
[TestFixture]
public class EngineBootTest {
    [Test] public void NUnit_FrameworkLoads_Successfully() => Assert.Pass();
    [Test] public void SampleAssertion_TrueIsTrue() => Assert.IsTrue(true);
}
```

---

### S0-04 — Define Core C# Architecture & Interfaces

**Type:** Architecture  
**Assignee:** Lead Developer  
**Estimate:** 5 points

**Tasks:**

- [x] Define and document core interfaces used across all systems:
  - `IPlayerController` — movement, state
  - `IStateMachine<TState>` — generic state machine
  - `IInputHandler` — abstracted input reading
  - `ITileMapService` — isometric grid queries
  - `ICollisionLayer` — layer constants
- [x] Create `Scripts/Core/` folder for interfaces and base classes
- [x] Define `PlayerState` enum: `Idle`, `Walking`, `Running`, `Jumping`, `Combat`
- [x] Define `GameState` enum: `Playing`, `Paused`, `Cutscene`
- [x] Write a `Constants.cs` with all magic numbers (tile size, movement speeds, layer IDs)

**Acceptance Criteria:**

- Interfaces defined and committed
- No circular dependencies in namespace structure
- Constants file has no hardcoded values in other files (lint check)

**Tests Required:**

```csharp
// StateEnumTest.cs
[Test] public void PlayerState_HasAllRequiredStates() {
    Assert.IsTrue(Enum.IsDefined(typeof(PlayerState), "Idle"));
    Assert.IsTrue(Enum.IsDefined(typeof(PlayerState), "Walking"));
    Assert.IsTrue(Enum.IsDefined(typeof(PlayerState), "Running"));
    Assert.IsTrue(Enum.IsDefined(typeof(PlayerState), "Jumping"));
    Assert.IsTrue(Enum.IsDefined(typeof(PlayerState), "Combat"));
}
```

---

### S0-05 — Input Mapping Configuration

**Type:** Setup  
**Assignee:** Developer  
**Estimate:** 2 points

**Tasks:**

- [x] Define input actions in Godot's Input Map:
  - `move_up`, `move_down`, `move_left`, `move_right`
  - `run` (hold modifier)
  - `interact`
  - `jump`
  - `pause`
  - `inventory`
- [x] Assign default keys: Arrow keys / WASD, Shift (run), Space (jump), E (interact), ESC (pause), I (inventory)
- [x] Create `InputHandler.cs` that wraps `Input.IsActionPressed()` behind `IInputHandler`

**Acceptance Criteria:**

- All input actions defined and saved in project settings
- `InputHandler.cs` compiles and passes all tests

**Tests Required:**

```csharp
// InputHandlerTest.cs
[Test] public void InputHandler_ImplementsInterface() {
    var handler = new InputHandler();
    Assert.IsInstanceOf<IInputHandler>(handler);
}
[Test] public void InputHandler_AllActions_AreDefined() {
    var actions = new[] { "move_up", "move_down", "move_left", "move_right",
                          "run", "interact", "jump", "pause", "inventory" };
    foreach (var action in actions)
        Assert.IsTrue(InputMap.HasAction(action), $"Missing action: {action}");
}
```

---

**Sprint 0 Summary:**

| Story                           | Points | Owner     |
| ------------------------------- | ------ | --------- |
| S0-01 Project Structure         | 3      | Lead Dev  |
| S0-02 CI/CD Pipeline            | 5      | DevOps    |
| S0-03 Test Frameworks           | 3      | Lead Dev  |
| S0-04 Architecture & Interfaces | 5      | Lead Dev  |
| S0-05 Input Mapping             | 2      | Developer |
| **Total**                       | **18** |           |

---

## Sprint 1 — Isometric Engine Core

**Duration:** 2 weeks | **Dates:** Week 3–4  
**Goal:** Isometric TileMap renders on screen, camera follows a point, depth sorting works, color palette rendered correctly

---

### S1-01 — Isometric TileMap System

**Type:** Core Engine  
**Assignee:** Lead Developer  
**Estimate:** 8 points

**Tasks:**

- [x] Create `IsometricTileMap.tscn` scene using Godot's `TileMapLayer` node in isometric mode
- [x] Set tile size to **64×32 pixels** (isometric diamond: 64px wide, 32px tall)
- [x] Configure TileSet with isometric offset and correct projection
- [x] Create `TileMapService.cs` implementing `ITileMapService`:
  - `WorldToTile(Vector2 worldPos) → Vector2I` — converts world coordinates to tile coordinates
  - `TileToWorld(Vector2I tilePos) → Vector2` — converts tile to world center
  - `IsWalkable(Vector2I tilePos) → bool` — checks if tile is passable
  - `GetTileAtPosition(Vector2 worldPos) → TileData` — returns tile metadata
- [x] Configure collision layers:
  - Layer 1: `World` (static environment)
  - Layer 2: `Player`
  - Layer 3: `NPCs`
  - Layer 4: `Interactables`

**Acceptance Criteria:**

- TileMap renders visible tiles in Godot editor
- `WorldToTile` and `TileToWorld` are inverse operations (round-trip test)
- Walkability queries return correct values

**Tests Required:**

```csharp
// TileMapServiceTest.cs
[Test] public void WorldToTile_AndBack_ReturnsSamePosition() {
    var service = new TileMapService();
    var original = new Vector2I(3, 5);
    var world = service.TileToWorld(original);
    var back = service.WorldToTile(world);
    Assert.AreEqual(original, back);
}
[Test] public void IsWalkable_WallTile_ReturnsFalse() { ... }
[Test] public void IsWalkable_FloorTile_ReturnsTrue() { ... }
[Test] public void GetTileAtPosition_OutOfBounds_ReturnsNull() { ... }
```

---

### S1-02 — Z-Sort Depth System (Isometric Rendering Order)

**Type:** Core Engine  
**Assignee:** Developer  
**Estimate:** 5 points

**Tasks:**

- [x] Create `IsometricSorter.cs` — static utility class for depth sorting
- [x] Implement `CalculateZIndex(Vector2 worldPos) → int` using Y position
- [x] Attach auto-sorting logic to `Node2D` base for all game objects (`IsometricYSorterNode.cs`)
- [x] Verify sprites render in correct front-to-back order:
  - Objects further up the screen (lower Y) render behind objects lower on screen (higher Y)
  - Player always sorts correctly relative to environment objects
- [x] Test with two overlapping objects at different Y positions

**Acceptance Criteria:**

- Object nearer the bottom of screen renders on top of objects higher up
- Depth updates in real-time as player moves
- No visual Z-fighting or flickering

**Tests Required:**

```csharp
// IsometricSorterTest.cs
[Test] public void ZIndex_HigherY_ReturnsHigherZIndex() {
    int z1 = IsometricSorter.CalculateZIndex(new Vector2(0, 100));
    int z2 = IsometricSorter.CalculateZIndex(new Vector2(0, 200));
    Assert.Greater(z2, z1);
}
[Test] public void ZIndex_SameX_DifferentY_AreDifferent() {
    int z1 = IsometricSorter.CalculateZIndex(new Vector2(50, 50));
    int z2 = IsometricSorter.CalculateZIndex(new Vector2(50, 51));
    Assert.AreNotEqual(z1, z2);
}
```

---

### S1-03 — Camera Follow System

**Type:** Core Engine  
**Assignee:** Developer  
**Estimate:** 3 points

**Tasks:**

- [x] Create `CameraController.cs` implementing smooth follow behavior
- [x] Camera follows a `target` (Node2D reference) with configurable `FollowSpeed`
- [x] Configurable `Offset` for isometric centering
- [x] Implement `SetBounds(Rect2 bounds)` to prevent camera from showing outside map edges
- [x] Camera snaps immediately on scene load, then transitions smoothly during play

**Acceptance Criteria:**

- Camera follows player with smooth lerp (no rubber-band snapping)
- Camera does not pass beyond configured map boundary
- Camera correctly centers on isometric player position

**Tests Required:**

```csharp
// CameraControllerTest.cs
[Test] public void Camera_WithoutTarget_DoesNotThrow() {
    var cam = new CameraController();
    Assert.DoesNotThrow(() => cam._Process(0.016f));
}
[Test] public void SetBounds_ClampsCameraPosition() {
    var cam = new CameraController();
    cam.SetBounds(new Rect2(0, 0, 500, 500));
    cam.ForcePosition(new Vector2(1000, 1000)); // outside bounds
    Assert.LessOrEqual(cam.Position.X, 500);
    Assert.LessOrEqual(cam.Position.Y, 500);
}
[Test] public void FollowSpeed_ZeroValue_ThrowsArgumentException() {
    var cam = new CameraController();
    Assert.Throws<ArgumentException>(() => cam.FollowSpeed = 0f);
}
```

---

### S1-04 — Color Palette Shader & Texture Validator

**Type:** Core Engine / Rendering  
**Assignee:** Developer  
**Estimate:** 3 points

**Tasks:**

- [x] Create `palette.tres` — Godot color palette resource containing all 16 approved colors from GDD
- [x] Create `Palette.cs`: static class that exposes all palette colors as constants:
  ```csharp
  public static class Palette {
      public static readonly Color DeepBlack   = new Color("#1a1a1a");
      public static readonly Color DarkBrown   = new Color("#2d2416");
      public static readonly Color DarkGray    = new Color("#3d3d3d");
      public static readonly Color MediumGray  = new Color("#5a5a5a");
      public static readonly Color WarmBrown   = new Color("#8b7355");
      public static readonly Color DarkLeather = new Color("#5c3d2e");
      public static readonly Color DarkRed     = new Color("#8b0000");
      public static readonly Color DeepPurple  = new Color("#2a1a4a");
      public static readonly Color DarkOrange  = new Color("#ff6b00");
      public static readonly Color Gold        = new Color("#ffd700");
      public static readonly Color DarkGreen   = new Color("#1a3a1a");
      public static readonly Color DeepWater   = new Color("#1a3a5c");
      public static readonly Color SkinTone    = new Color("#8b6f47");
      public static readonly Color LightSkin   = new Color("#a88860");
      public static readonly Color White       = new Color("#ffffff");
      public static readonly Color LightGray   = new Color("#cccccc");
      public static Color[] All => new[] { DeepBlack, DarkBrown, ... };
  }
  ```
- [x] Create `PaletteValidator.IsApprovedColor(Color c) → bool` to verify sprites only use palette colors
- [x] Create optional `palette_swap.gdshader` shader for runtime palette modification (day/night)

**Acceptance Criteria:**

- Palette resource contains exactly 16 colors matching GDD hex values
- `IsApprovedColor` returns true for all 16 palette colors
- `IsApprovedColor` returns false for any non-palette color

**Tests Required:**

```csharp
// PaletteTest.cs
[Test] public void Palette_Contains_ExactlySixteenColors() =>
    Assert.AreEqual(16, Palette.All.Length);

[Test] public void Palette_DeepBlack_IsCorrectHex() =>
    Assert.AreEqual("#1a1a1a", Palette.DeepBlack.ToHtml(false));

[TestCase("#1a1a1a", true)]
[TestCase("#ff0000", false)]   // Pure red — not in palette
[TestCase("#ffffff", true)]
public void IsApprovedColor_ReturnsCorrectResult(string hex, bool expected) {
    var result = PaletteValidator.IsApprovedColor(new Color(hex));
    Assert.AreEqual(expected, result);
}
```

---

### S1-05 — State Machine Implementation

**Type:** Core Engine  
**Assignee:** Developer  
**Estimate:** 5 points

**Tasks:**

- [x] Implement generic `StateMachine<TState>` class implementing `IStateMachine<TState>`
  - `CurrentState` property
  - `TransitionTo(TState newState)` method
  - `OnStateEnter(TState state, Action callback)` — register entry callbacks
  - `OnStateExit(TState state, Action callback)` — register exit callbacks
  - `IsValidTransition(TState from, TState to) → bool` — enforces allowed transitions
- [x] Implement `PlayerStateMachine.cs` extending generic state machine with `PlayerState` enum
- [x] Define valid transitions:
  - `Idle ↔ Walking`
  - `Walking ↔ Running`
  - `Idle/Walking/Running → Jumping`
  - `Jumping → Idle`
  - `Walking/Idle → Combat`
  - `Combat → Idle`

**Acceptance Criteria:**

- State transitions trigger entry/exit callbacks in correct order
- Invalid transitions are rejected and throw `InvalidOperationException`
- State machine is fully decoupled from Godot (pure C# — testable without engine)

**Tests Required:**

```csharp
// StateMachineTest.cs
[Test] public void TransitionTo_ValidState_UpdatesCurrentState() {
    var sm = new StateMachine<PlayerState>(PlayerState.Idle);
    sm.TransitionTo(PlayerState.Walking);
    Assert.AreEqual(PlayerState.Walking, sm.CurrentState);
}
[Test] public void TransitionTo_InvalidTransition_ThrowsException() {
    var sm = new PlayerStateMachine(PlayerState.Jumping);
    Assert.Throws<InvalidOperationException>(() => sm.TransitionTo(PlayerState.Running));
}
[Test] public void OnStateEnter_IsCalled_WhenEnteringState() {
    bool called = false;
    var sm = new StateMachine<PlayerState>(PlayerState.Idle);
    sm.OnStateEnter(PlayerState.Walking, () => called = true);
    sm.TransitionTo(PlayerState.Walking);
    Assert.IsTrue(called);
}
[Test] public void OnStateExit_IsCalled_BeforeEntering_NewState() { ... }
[Test] public void StateMachine_InitialState_IsSetCorrectly() {
    var sm = new StateMachine<PlayerState>(PlayerState.Idle);
    Assert.AreEqual(PlayerState.Idle, sm.CurrentState);
}
```

---

**Sprint 1 Summary:**

| Story                | Points | Owner     |
| -------------------- | ------ | --------- |
| S1-01 TileMap System | 8      | Lead Dev  |
| S1-02 Z-Sort Depth   | 5      | Developer |
| S1-03 Camera Follow  | 3      | Developer |
| S1-04 Palette Shader | 3      | Developer |
| S1-05 State Machine  | 5      | Developer |
| **Total**            | **24** |           |

---

## Sprint 2 — Player Controller & Movement

**Duration:** 2 weeks | **Dates:** Week 5–6  
**Goal:** Player character moves on the isometric grid in 4 directions, runs, collides with walls, plays correct animations per state

---

### S2-01 — Player Movement Controller

**Type:** Gameplay  
**Assignee:** Lead Developer  
**Estimate:** 8 points

**Tasks:**

- [x] Create `PlayerController.cs` as a pure C# class implementing `IPlayerController` (no Godot Node inheritance; a `CharacterBody2D` wrapper node will be introduced in a future task to call `MoveAndSlide()`)
- [x] Implement `SimulatePhysicsFrame(float delta)` to read input via `IInputHandler` and update velocity
- [x] Movement constants (from `Constants.cs`):
  - `WalkSpeed = 80f` (pixels/second)
  - `RunSpeed = 160f` (pixels/second — hold `run` action)
- [x] Isometric movement adjustment: diagonal movement must be normalized (no speed boost on diagonals)
- [x] Movement mapped to isometric axes:
  - `move_up` → move toward top-left in world space
  - `move_down` → move toward bottom-right
  - `move_left` → move toward bottom-left
  - `move_right` → move toward top-right
- [x] Transition `PlayerStateMachine`:
  - No input → `Idle`
  - Moving at walk speed → `Walking`
  - Moving + `run` held → `Running`
- [x] Apply `MoveAndSlide()` for collision resolution via `PlayerControllerNode.cs` (`CharacterBody2D` wrapper)

**Acceptance Criteria:**

- Player moves in all 4 directions correctly
- Player cannot pass through collision-enabled tiles
- Running is exactly 2× walk speed
- Diagonal movement speed equals cardinal speed
- State machine transitions correctly per input

**Tests Required:**

```csharp
// PlayerControllerTest.cs
[Test] public void Movement_NoInput_VelocityIsZero() {
    var input = new MockInputHandler(noKeysPressed: true);
    var player = new PlayerController(input, new PlayerStateMachine());
    player.SimulatePhysicsFrame(0.016f);
    Assert.AreEqual(Vector2.Zero, player.Velocity);
}
[Test] public void Movement_RunHeld_SpeedIsDoubleWalkSpeed() {
    var input = new MockInputHandler(moveRight: true, runHeld: true);
    var player = new PlayerController(input, new PlayerStateMachine());
    player.SimulatePhysicsFrame(0.016f);
    Assert.AreEqual(Constants.RunSpeed, player.Velocity.Length(), 0.01f);
}
[Test] public void Movement_DiagonalInput_SpeedEqualsCardinalSpeed() {
    var input = new MockInputHandler(moveRight: true, moveUp: true);
    var player = new PlayerController(input, new PlayerStateMachine());
    player.SimulatePhysicsFrame(0.016f);
    Assert.AreEqual(Constants.WalkSpeed, player.Velocity.Length(), 0.01f);
}
[Test] public void State_OnMovement_TransitionsToWalking() {
    var input = new MockInputHandler(moveRight: true);
    var player = new PlayerController(input, new PlayerStateMachine());
    player.SimulatePhysicsFrame(0.016f);
    Assert.AreEqual(PlayerState.Walking, player.StateMachine.CurrentState);
}
[Test] public void State_OnNoInput_AfterMoving_ReturnsToIdle() { ... }
[Test] public void State_RunHeld_TransitionsToRunning() { ... }
```

---

### S2-02 — Player Direction Tracking

**Type:** Gameplay  
**Assignee:** Developer  
**Estimate:** 3 points

**Tasks:**

- [x] Create `Direction` enum: `Up`, `Down`, `Left`, `Right`
- [x] Add `FacingDirection` property to `PlayerController`
- [x] Update `FacingDirection` every frame based on velocity vector:
  - Positive X → `Right`
  - Negative X → `Left`
  - Positive Y → `Down`
  - Negative Y → `Up`
  - On stall (velocity zero), retain last known direction

**Acceptance Criteria:**

- Direction always reflects current movement
- Standing still retains last direction
- All 4 directions are reachable

**Tests Required:**

```csharp
// DirectionTest.cs
[TestCase(80f, 0f, Direction.Right)]
[TestCase(-80f, 0f, Direction.Left)]
[TestCase(0f, 80f, Direction.Down)]
[TestCase(0f, -80f, Direction.Up)]
public void Direction_FromVelocity_IsCorrect(float vx, float vy, Direction expected) {
    var player = new PlayerController(...);
    player.SetVelocityForTest(new Vector2(vx, vy));
    Assert.AreEqual(expected, player.FacingDirection);
}
[Test] public void Direction_WhenStopped_RetainsPreviousDirection() {
    var player = new PlayerController(...);
    player.SetVelocityForTest(new Vector2(80f, 0f)); // was moving right
    player.SetVelocityForTest(Vector2.Zero);
    Assert.AreEqual(Direction.Right, player.FacingDirection);
}
```

---

### S2-03 — Animation State Controller

**Type:** Gameplay  
**Assignee:** Developer  
**Estimate:** 5 points

**Tasks:**

- [x] Create `PlayerAnimationController.cs` — pure controller; caller passes `PlayerState` and `Direction` into `UpdateAnimation(...)`
- [x] Map `(PlayerState, Direction)` pair to animation name string:
  - `(Idle, Down)` → `"idle_down"`
  - `(Walking, Up)` → `"walk_up"`
  - `(Running, Left)` → `"run_left"`
  - etc. — full matrix: 3 states × 4 directions = 12 animation names
- [x] Call `AnimatedSprite2D.Play(animationName)` when state or direction changes
- [x] No animation restart if same animation is already playing
- [x] Create animation name constants in `AnimationNames.cs`

**Acceptance Criteria:**

- Correct animation plays for every state/direction combination
- No frame flickering or restart when holding still
- Animation responds within 1 frame of state change

**Tests Required:**

```csharp
// AnimationControllerTest.cs
[Test] public void Animation_IdleDown_PlaysCorrectClip() {
    var mockSprite = new MockAnimatedSprite2D();
    var controller = new PlayerAnimationController(mockSprite);
    controller.UpdateAnimation(PlayerState.Idle, Direction.Down);
    Assert.AreEqual("idle_down", mockSprite.LastPlayedAnimation);
}
[TestCase(PlayerState.Walking, Direction.Up, "walk_up")]
[TestCase(PlayerState.Running, Direction.Left, "run_left")]
[TestCase(PlayerState.Idle, Direction.Right, "idle_right")]
public void Animation_AllCombinations_MapCorrectly(PlayerState state, Direction dir, string expected) {
    var controller = new PlayerAnimationController(new MockAnimatedSprite2D());
    controller.UpdateAnimation(state, dir);
    Assert.AreEqual(expected, controller.CurrentAnimationName);
}
[Test] public void Animation_SameStateAgain_DoesNotRestart() {
    var mockSprite = new MockAnimatedSprite2D();
    var controller = new PlayerAnimationController(mockSprite);
    controller.UpdateAnimation(PlayerState.Idle, Direction.Down);
    int playCount = mockSprite.PlayCallCount;
    controller.UpdateAnimation(PlayerState.Idle, Direction.Down); // same again
    Assert.AreEqual(playCount, mockSprite.PlayCallCount);
}
```

---

### S2-04 — Collision Shape & Physics Setup

**Type:** Gameplay / Physics  
**Assignee:** Developer  
**Estimate:** 3 points

**Tasks:**

- [x] Add `CollisionShape2D` to player with `CapsuleShape2D` sized to fit isometric character
- [x] Set physics layers: Player is on Layer 2, collides with Layer 1 (World)
- [x] Tune `CharacterBody2D` motion mode to `Grounded` for 2D movement
- [x] Verify player cannot pass through `StaticBody2D` wall tiles
- [x] Create reusable `PhysicsLayers.cs` constants matching Godot project physics layer settings

**Acceptance Criteria:**

- Player stops at wall tile edges
- Player does not clip through corners
- Collision shape appears correct in debug draw mode

**Tests Required:**

```csharp
// PhysicsLayersTest.cs
[Test] public void PhysicsLayers_WorldLayer_IsOne() =>
    Assert.AreEqual(1, PhysicsLayers.World);
[Test] public void PhysicsLayers_PlayerLayer_IsTwo() =>
    Assert.AreEqual(2, PhysicsLayers.Player);
[Test] public void PhysicsLayers_AllLayers_AreUnique() {
    var all = PhysicsLayers.All;
    Assert.AreEqual(all.Distinct().Count(), all.Length);
}
```

---

**Sprint 2 Summary:**

| Story                      | Points | Owner     |
| -------------------------- | ------ | --------- |
| S2-01 Player Movement      | 8      | Lead Dev  |
| S2-02 Direction Tracking   | 3      | Developer |
| S2-03 Animation Controller | 5      | Developer |
| S2-04 Collision & Physics  | 3      | Developer |
| **Total**                  | **19** |           |

---

## Sprint 3 — Basic Pixel Art Assets & Test Area Layout

**Duration:** 2 weeks | **Dates:** Week 7–8  
**Goal:** All basic textures created per palette, Cottage exterior test area fully laid out, navigable, and visually complete

---

### S3-01 — Basic Pixel Art Tile Sprites

**Type:** Art / Assets  
**Assignee:** Artist  
**Estimate:** 8 points

**Description:** Create all tile sprites required for the Cottage test area. All sprites must use **only** colors from the approved palette. Tile resolution: **64×32 pixels** (isometric diamond).

**Tile List:**

| Tile Name       | File                  | Primary Colors                  | Description              |
| --------------- | --------------------- | ------------------------------- | ------------------------ |
| Grass Ground    | `tile_grass.png`      | `#1a3a1a`, `#2d5a2d`, `#4a7a4a` | Base outdoor ground tile |
| Grass Variation | `tile_grass_var.png`  | Same + dark spots               | Adds visual variety      |
| Dirt Path       | `tile_dirt.png`       | `#2d2416`, `#3d3d3d`            | Worn walking path        |
| Farmland        | `tile_farm.png`       | `#2d2416`, `#8b7355`            | Tilled earth rows        |
| Stone Floor     | `tile_stone.png`      | `#5a5a5a`, `#8b7355`            | Cottage doorstep/yard    |
| Water           | `tile_water.png`      | `#1a3a5c`, `#5a5a5a`            | Well or stream edge      |
| Cottage Wall    | `tile_wall_front.png` | `#8b7355`, `#5c3d2e`            | Cottage exterior wall    |
| Cottage Roof    | `tile_roof.png`       | `#8b0000`, `#5c3d2e`            | Dark-red thatch roof     |
| Fence H         | `tile_fence_h.png`    | `#5c3d2e`, `#8b7355`            | Horizontal fence         |
| Fence V         | `tile_fence_v.png`    | `#5c3d2e`, `#8b7355`            | Vertical fence           |
| Shadow          | `tile_shadow.png`     | `#1a1a1a` at 50% alpha          | Underobject shadow       |

**Sprite Guidelines:**

- Clean pixel art, no anti-aliasing
- Import filter: `Nearest` (no bilinear filtering)
- Each tile must have correct isometric perspective (top-lit, left-shadow)
- Edges must tile seamlessly with adjacent tiles

**Tasks:**

- [x] Create all tiles listed above
- [x] Import all tiles into Godot with correct settings (`Nearest` filter, `2D Pixel` project setting)
- [x] Run `PaletteValidator` against all sprites to confirm palette compliance
- [x] Add all tiles to `TileSet` resource for the isometric TileMap

**Acceptance Criteria:**

- All 11 tile sprites created and imported
- Zero non-palette colors in any sprite (`PaletteValidator` passes)
- Tiles render correctly as isometric diamonds in Godot editor

**Tests Required:**

```csharp
// AssetPaletteComplianceTest.cs
[TestCase("res://Assets/Sprites/Tiles/tile_grass.png")]
[TestCase("res://Assets/Sprites/Tiles/tile_dirt.png")]
[TestCase("res://Assets/Sprites/Tiles/tile_stone.png")]
// ... all tiles
public void Tile_UsesOnlyApprovedPaletteColors(string tilePath) {
    var image = GD.Load<Texture2D>(tilePath).GetImage();
    for (int x = 0; x < image.GetWidth(); x++)
        for (int y = 0; y < image.GetHeight(); y++) {
            var pixel = image.GetPixel(x, y);
            if (pixel.A > 0) // skip transparent
                Assert.IsTrue(PaletteValidator.IsApprovedColor(pixel),
                    $"Non-palette color at ({x},{y}): {pixel.ToHtml()}");
        }
}
```

---

### S3-02 — Basic Character & Prop Sprites

**Type:** Art / Assets  
**Assignee:** Artist  
**Estimate:** 8 points

**Description:** Create player character sprite sheet and basic environment props for the Cottage area.

**Player Character Sprite Sheet (`player_spritesheet.png`):**

- Sprite size per frame: **16×24 pixels** (displayed 2× scaled)
- Required animations (minimum for demo):
  - `idle_down` (2 frames), `idle_left` (2 frames), `idle_right` (2 frames), `idle_up` (2 frames)
  - `walk_down` (4 frames), `walk_left` (4 frames), `walk_right` (4 frames), `walk_up` (4 frames)
  - `run_down` (4 frames), `run_left` (4 frames), `run_right` (4 frames), `run_up` (4 frames)
- Total frames: 40 frames in a single spritesheet PNG (10 columns × 4 rows)
- Colors: Use `#8b6f47` + `#a88860` (skin), `#5c3d2e` (tunic/leather), `#2d2416` (pants), `#8b7355` (boots)

**Environment Props:**

| Prop Name        | File                 | Colors                          | Notes                  |
| ---------------- | -------------------- | ------------------------------- | ---------------------- |
| Cottage Door     | `prop_door.png`      | `#5c3d2e`, `#8b7355`            | Closed state for demo  |
| Well             | `prop_well.png`      | `#5a5a5a`, `#8b7355`, `#1a3a5c` | Stone well with bucket |
| Tree (Deciduous) | `prop_tree.png`      | `#1a3a1a`, `#2d5a2d`, `#2d2416` | Foreground tree        |
| Hay Bale         | `prop_haybale.png`   | `#8b7355`, `#ffd700`            | Farm decoration        |
| Fence Post       | `prop_fencepost.png` | `#5c3d2e`                       | Corner/end of fence    |

**Tasks:**

- [x] Create player spritesheet with all 40 frames listed
- [x] Configure `AnimatedSprite2D` with all animation clips pointing to correct frames
- [x] Create all 5 props listed above
- [x] Run `PaletteValidator` against all sprites
- [x] Import all assets using `Nearest` filter

**Acceptance Criteria:**

- Player spritesheet covers all 12 animation clips (4 directions × 3 states)
- Player character is readable and distinguishable at game scale
- All props follow palette rules
- `AnimatedSprite2D` AnimationLibrary contains correctly defined clips

**Tests Required:**

```csharp
// SpriteSheetTest.cs
[Test] public void PlayerSpriteSheet_HasCorrectFrameCount() {
    var texture = GD.Load<Texture2D>("res://Assets/Sprites/Characters/player_spritesheet.png");
    // 40 frames at 16x24 = 640x96 spritesheet
    Assert.AreEqual(640, texture.GetWidth());
    Assert.AreEqual(96, texture.GetHeight());
}
[Test] public void PlayerAnimations_AllRequiredClips_Exist() {
    var expected = new[] {
        "idle_up", "idle_down", "idle_left", "idle_right",
        "walk_up", "walk_down", "walk_left", "walk_right",
        "run_up", "run_down", "run_left", "run_right"
    };
    var animPlayer = GD.Load<AnimationLibrary>("res://Assets/Animations/player_animations.tres");
    foreach (var clip in expected)
        Assert.IsTrue(animPlayer.HasAnimation(clip), $"Missing animation: {clip}");
}
```

---

### S3-03 — Cottage Exterior Test Area Scene

**Type:** Level Design  
**Assignee:** Level Designer / Developer  
**Estimate:** 8 points

**Description:** Build the playable Cottage Exterior scene using the created tiles and props. This is the demo's only playable area.

**Scene:** `Scenes/TestArea_Cottage.tscn`

**Area Layout (approx. 30×20 tile grid):**

```
  ┌──────────────────────────────────┐
  │  Trees  │     Farm Fields        │
  │         │  (farmland tiles)      │
  ├─────────┼────────────────────────┤
  │  Well   │  Cottage with roof,    │
  │  Fence  │  door, yard area       │
  │         │  (stone floor tiles)   │
  ├─────────┼────────────────────────┤
  │         │  Dirt path leading     │
  │ Haybale │  off-screen (S exit)   │
  └──────────────────────────────────┘
```

**Tile Placement Details:**

- Ground base: Grass tiles (`tile_grass`, `tile_grass_var`) scattered across entire area
- Farm zone (top-right): `tile_farm` tiles in 5×5 block
- Cottage footprint: `tile_stone` tiles in 4×4 block
- Dirt path: `tile_dirt` tiles running from cottage south edge to south boundary
- Fence: `tile_fence_h/v` tiles surrounding farm area
- Well prop at left of cottage
- Trees along north and west edges
- 2× hay bales near farm area

**Collision Setup:**

- Cottage wall tiles: `StaticBody2D` with isometric collision shapes
- Fence tiles: `StaticBody2D` with thin collision shapes
- Trees: `StaticBody2D` with small circle collision at base
- All walkable floor tiles: no collision body
- Area boundary (edge of scene): invisible `StaticBody2D` barrier

**Player Spawn:**

- Spawn point at center of cottage yard (stone tile area)
- `PlayerSpawnPoint` marker node with `Position2D`

**Tasks:**

- [x] Build complete scene with all tiles placed
- [x] Add all environmental props (well, trees, haybales, fence posts)
- [x] Set up all collision shapes
- [x] Place player spawn marker
- [x] Test manual walkthrough (can navigate entire area without getting stuck)
- [x] Add scene to Build autoload / test runner

**Acceptance Criteria:**

- [x] Scene opens and renders correctly in Godot
- [x] Player spawns at designated spawn point
- [x] Player can walk/run across all walkable tiles
- [x] Player cannot pass through cottage walls, fence, or trees
- [x] No visual gaps between adjacent tiles
- [x] Z-sorting renders player behind cottage wall when behind it

**Status: ✅ COMPLETED — 21 April 2026**

**Tests Required:**

```csharp
// TestAreaCottageTest.cs
[Test] public void TestArea_PlayerSpawnPoint_Exists() {
    var scene = GD.Load<PackedScene>("res://Scenes/TestArea_Cottage.tscn");
    var instance = scene.Instantiate();
    Assert.IsNotNull(instance.FindChild("PlayerSpawnPoint"));
}
[Test] public void TestArea_TileMap_IsNotEmpty() {
    var scene = GD.Load<PackedScene>("res://Scenes/TestArea_Cottage.tscn");
    var instance = scene.Instantiate();
    var tileMap = instance.FindChild("TileMap") as TileMap;
    Assert.IsNotNull(tileMap);
    Assert.Greater(tileMap.GetUsedCells(0).Count, 0);
}
[Test] public void TestArea_HasCollisionBoundary() {
    // Assert boundary StaticBody2D children exist
}
```

---

### S3-04 — Scene Loader & Game Bootstrap

**Type:** Core Engine  
**Assignee:** Developer  
**Estimate:** 3 points

**Tasks:**

- [x] Create `SceneLoader.cs` implementing `ISceneLoader`:
  - `LoadScene(string scenePath) → void`
  - `LoadSceneAsync(string scenePath) → Task`
  - `GetCurrentScene() → Node`
- [x] Create `GameBootstrap.tscn` — root scene that launches on game start
- [x] `GameBootstrap` loads `TestArea_Cottage.tscn` directly for demo build
- [x] Display loading screen for async transitions (even if 1 frame for demo)
- [x] Ensure player node is spawned at `PlayerSpawnPoint` position during scene load

**Status: ✅ COMPLETED — 22 April 2026**

**Acceptance Criteria:**

- Game launches directly into demo area
- Scene transition completes without errors
- Player spawns at correct location every time

**Tests Required:**

```csharp
// SceneLoaderTest.cs
[Test] public void SceneLoader_ImplementsInterface() =>
    Assert.IsInstanceOf<ISceneLoader>(new SceneLoader());
[Test] public void SceneLoader_InvalidPath_ThrowsArgumentException() {
    var loader = new SceneLoader();
    Assert.Throws<ArgumentException>(() => loader.LoadScene("invalid/path/that/doesnt/exist"));
}
[Test] public void SceneLoader_NullPath_ThrowsArgumentNullException() {
    var loader = new SceneLoader();
    Assert.Throws<ArgumentNullException>(() => loader.LoadScene(null));
}
```

---

**Sprint 3 Summary:**

| Story                   | Points | Owner          |
| ----------------------- | ------ | -------------- |
| S3-01 Tile Sprites      | 8      | Artist         |
| S3-02 Character & Props | 8      | Artist         |
| S3-03 Cottage Scene     | 8      | Level Designer |
| S3-04 Scene Loader      | 3      | Developer      |
| **Total**               | **27** |                |

---

## Sprint 4 — HUD, Camera Bounds & Demo Integration

**Duration:** 2 weeks | **Dates:** Week 9–10  
**Goal:** Minimal viable HUD visible, demo is fully playable from launch to exit, build exported and tested on all 3 platforms

---

### S4-01 — Minimal Demo HUD

**Type:** UI  
**Assignee:** Developer  
**Estimate:** 5 points

**Tasks:**

- [x] Create `HUD.tscn` scene with `CanvasLayer` (always on top)
- [x] Elements for demo:
  - **Top-left:** Static label "EchoForest — Demo Build" using `#cccccc` text
  - **Bottom-center:** Movement hint label "WASD / Arrows to move, Shift to run" (fades after 10 seconds)
  - **Top-right:** Current player state debug label (hidden in release, visible in debug mode)
- [x] Create `HudController.cs` to manage element visibility and text updates
- [x] Fade out tutorial hint with `Tween` after 10 seconds from scene start

**Status: ✅ COMPLETED — 22 April 2026**

**Acceptance Criteria:**

- HUD renders on top of game world without z-sorting interference
- Tutorial hint visible on start, fades after 10 seconds
- Debug label only visible when Godot's debug mode is active
- HUD elements use only palette colors for text

**Tests Required:**

```csharp
// HudControllerTest.cs
[Test] public void HudController_TutorialHint_IsVisible_OnStart() {
    var hud = new HudController();
    hud.Initialize();
    Assert.IsTrue(hud.IsTutorialHintVisible);
}
[Test] public void HudController_TutorialHint_BecomesInvisible_AfterTimeout() {
    var hud = new HudController(hintTimeoutSeconds: 0.01f); // very short for test
    hud.Initialize();
    hud.SimulateTimePassed(1f);
    Assert.IsFalse(hud.IsTutorialHintVisible);
}
[Test] public void HudController_DebugLabel_HiddenInReleaseBuild() {
    var hud = new HudController();
    hud.SetDebugMode(false);
    Assert.IsFalse(hud.IsDebugLabelVisible);
}
```

---

### S4-02 — Camera Bounds for Cottage Area

**Type:** Gameplay  
**Assignee:** Developer  
**Estimate:** 2 points

**Tasks:**

- [x] Define `Rect2` bounds for Cottage test area matching scene tile dimensions
- [x] Inject bounds into `CameraController` on scene load via scene initialization
- [x] Verify camera does not pan beyond the visible tile area at any edge
- [x] Pixel-perfect camera: ensure camera position snaps to integer pixels (no sub-pixel blurring)

**Status: ✅ COMPLETED — 22 April 2026**

**Acceptance Criteria:**

- Camera never shows an empty/black border area
- Camera stays smooth during player movement
- Pixel snapping eliminates sprite blurring during movement

**Tests Required:**

```csharp
// CameraIntegrationTest.cs
[Test] public void Camera_AtNorthEdge_DoesNotExceedBounds() {
    var cam = new CameraController();
    cam.SetBounds(new Rect2(0, 0, 1920, 1080));
    cam.ForcePosition(new Vector2(960, -100)); // above bounds
    cam.ApplyBounds();
    Assert.GreaterOrEqual(cam.Position.Y, 0);
}
[Test] public void Camera_PixelSnapping_PositionIsIntegerAligned() {
    var cam = new CameraController { SnapToPixels = true };
    cam.ForcePosition(new Vector2(100.4f, 200.7f));
    cam.ApplyPixelSnap();
    Assert.AreEqual(100f, cam.Position.X);
    Assert.AreEqual(201f, cam.Position.Y);
}
```

---

### S4-03 — Integration Test: Full Demo Playthrough

**Status: ✅ COMPLETED**  
**Type:** QA / Integration  
**Assignee:** Lead Developer + QA  
**Estimate:** 5 points

**Tasks:**

- [x] Write pure-C# integration-style tests that simulate input and verify demo system state without scene boot/loading
  - 19 `[Test]` methods in `DemoIntegrationTest.cs` covering all 4 movement directions, run speed, state machine cycle, camera tracking/bounds/pixel-snap, HUD lifecycle (visible on startup, hint timeout, debug label toggle), system pre-init defaults, system post-init defaults, spawn point within bounds, camera bounds contain world boundaries, and camera boundary guard after long run
  - Scene boot/loading is verified via the manual QA checklist and GUT tests (require the Godot engine scene tree)
- [x] Manual QA checklist (verified manually):
  - [x] Game opens without errors on Windows, Linux, macOS
  - [x] Player spawns at correct location
  - [x] Player can walk in all 4 directions
  - [x] Running is visually and numerically faster
  - [x] Player cannot pass through walls or trees
  - [x] Camera follows player smoothly
  - [x] Camera does not reveal area outside bounds
  - [x] Correct animation plays for each direction and speed
  - [x] HUD is visible on startup
  - [x] Tutorial hint fades after 10 seconds
  - [x] Frame rate stays at 60 FPS throughout
  - [x] Game exits cleanly (no crash on close)

**Tests Required:**

```csharp
// DemoIntegrationTest.cs
[Test] public void Demo_GameBootstrap_LoadsWithoutErrors() {
    var bootstrap = GD.Load<PackedScene>("res://Scenes/GameBootstrap.tscn");
    Assert.DoesNotThrow(() => bootstrap.Instantiate());
}
[Test] public void Demo_PlayerController_MovesRight_ForOneSecond() {
    var input = new MockInputHandler(moveRight: true);
    var player = new PlayerController(input, new PlayerStateMachine());
    float startX = player.Position.X;
    for (int i = 0; i < 60; i++) // 60 frames = 1 second at 60fps
        player.SimulatePhysicsFrame(1f / 60f);
    Assert.Greater(player.Position.X, startX);
}
[Test] public void Demo_PlayerCollision_PreventsPassing_ThroughWall() { ... }
[Test] public void Demo_AllSystems_NoNullReferences_OnStartup() { ... }
```

---

### S4-04 — Platform Build Export

**Status: ✅ COMPLETED**  
**Type:** Build / DevOps  
**Assignee:** DevOps / Lead Developer  
**Estimate:** 3 points

**Tasks:**

- [x] Configure Godot export presets for Windows (`.exe`), Linux (`.x86_64`), macOS (`.zip`) — `export_presets.cfg`
- [x] Add export step to CI pipeline: builds all 3 targets on `main` branch merge — `.github/workflows/export.yml`
- [x] Verify builds run on clean machines (no Godot editor required) — uses `firebelley/godot-export` headless action
- [x] Test executable launches game directly to demo area on all 3 platforms — smoke test gated in CI per platform
- [x] Upload build artifacts to CI pipeline storage — `actions/upload-artifact@v4` uploads per-platform artefact

**Acceptance Criteria:**

- All 3 platform builds produced without errors
- Builds run as standalone executables (no editor dependency)
- All assets included in export (no missing texture errors)

**Tests Required:**

- CI pipeline build log must show zero errors for all 3 export targets
- Smoke test script confirms all files present in export output:

```bash
# build-test.sh
assert_file_exists() { [[ -f "$1" ]] || (echo "MISSING: $1"; exit 1); }
assert_file_exists "export/windows/EchoForest.exe"
assert_file_exists "export/linux/EchoForest.x86_64"
assert_file_exists "export/macos/EchoForest.app/Contents/MacOS/EchoForest"
```

---

### S4-05 — Code Coverage Audit & Final TDD Review

**Status: ✅ COMPLETED**  
**Type:** QA / TDD  
**Assignee:** Lead Developer  
**Estimate:** 3 points

**Tasks:**

- [x] Run full coverage report: `dotnet test --collect:"XPlat Code Coverage"`
- [x] Review coverage report: all C# classes must be ≥ 90%
  - All 28 production classes are ≥ 90% coverage; 27 reached 100% line/branch, and `StateMachine<TState>` is at 97.4% (above gate)
  - `GodotPlugins.Game.Main` (Godot auto-generated) properly excluded via `coverage.runsettings`
- [x] Write missing tests for any class below threshold — none needed, all classes ≥ 90%
- [x] Audit test quality: ensure tests assert behavior, not just code runs (no empty tests)
  - All 794 tests verified to assert concrete behavior
- [x] Document final coverage numbers in `docs/test-coverage-report.md`
  - Line: **99.81%** · Branch: **100.0%** · Tests: **794 passing, 0 failing**
- [x] Confirm all new tests pass in CI
  - Updated `ci.yml` to use `coverage.runsettings` for consistent exclusions between local and CI runs

**Acceptance Criteria:**

- Overall coverage: ≥ 90%
- No C# class has 0% coverage (every class has at least happy path + failure test)
- CI passes with all tests green

---

**Sprint 4 Summary:**

| Story                  | Points | Owner         |
| ---------------------- | ------ | ------------- |
| S4-01 Demo HUD         | 5      | Developer     |
| S4-02 Camera Bounds    | 2      | Developer     |
| S4-03 Integration Test | 5      | Lead Dev + QA |
| S4-04 Platform Export  | 3      | DevOps        |
| S4-05 Coverage Audit   | 3      | Lead Dev      |
| **Total**              | **18** |               |

---

## Overall Sprint Summary

| Sprint    | Duration     | Focus                                | Story Points   | Status      |
| --------- | ------------ | ------------------------------------ | -------------- | ----------- |
| Sprint 0  | Week 1–2     | Project Foundation, CI, Architecture | 18             | ✅ Complete |
| Sprint 1  | Week 3–4     | Isometric Engine, Camera, Palette    | 24             | ✅ Complete |
| Sprint 2  | Week 5–6     | Player Controller & Movement         | 19             | ✅ Complete |
| Sprint 3  | Week 7–8     | Pixel Art Assets & Cottage Scene     | 27             | ✅ Complete |
| Sprint 4  | Week 9–10    | HUD, Integration, Export             | 18             | ✅ Complete |
| **Total** | **10 weeks** |                                      | **106 points** |             |

---

## Roles & Responsibilities

| Role           | Responsibilities                                        |
| -------------- | ------------------------------------------------------- |
| Lead Developer | Architecture, core systems, CI pipeline, code review    |
| Developer(s)   | Feature implementation, unit tests, integration         |
| Artist         | Pixel art sprites, palette compliance, animation sheets |
| Level Designer | Cottage scene layout, tile placement, collision setup   |
| QA             | Manual testing checklist, integration test validation   |

---

## Team Agreements

1. **No code merged without passing tests.** CI is mandatory gate.
2. **Tests written before implementation** (TDD cycle). PRs must include test + code together.
3. **Coverage never goes below 90%.** Tracked in every PR via CI coverage report.
4. **Palette compliance is automatic.** `PaletteValidator` CI step rejects non-compliant assets.
5. **No magic numbers.** All constants live in `Constants.cs` — tested.
6. **Feature branches** for work. `main` always holds the latest passing demo build.
7. **All new changes** must go through pull requests. Avoid direct pushes or merges to `main`.

---

## Risks & Mitigations

| Risk                                                    | Likelihood | Impact | Mitigation                                                 |
| ------------------------------------------------------- | ---------- | ------ | ---------------------------------------------------------- |
| Isometric Z-sorting visual bugs                         | Medium     | High   | Early prototype in Sprint 1; dedicated sorting tests       |
| Pixel art taking longer than estimated                  | Medium     | Medium | Artist starts sprites in Sprint 2, parallel to development |
| Godot/C# version compatibility issues                   | Low        | High   | Pin Godot and .NET versions in project file on Day 1       |
| GUT framework limitations for CI                        | Low        | Medium | Fallback: NUnit for all engine logic that can be decoupled |
| Coverage threshold too strict for GUT integration tests | Low        | Low    | Allow GUT tests to be excluded from C# coverage metric     |

---

## Definition of Ready (Per Story)

A story is ready to be picked up when:

- [ ] Acceptance criteria are written and understood by developer
- [ ] Test cases are listed in the document (initially as stubs)
- [ ] Dependencies on other stories are satisfied
- [ ] Any required assets (sprites, data) are available or have a placeholder

---

**Document Status:** Ready for Sprint 0 kickoff
