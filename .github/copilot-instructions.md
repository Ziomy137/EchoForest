# Copilot Agent Instructions — EchoForest

**Trust these instructions. Only search the codebase if something here is incomplete or wrong.**

---

## What This Repository Is

2D isometric pixel-art RPG built with **Godot 4.6.2 + C# (net10.0)**. The game engine is Godot; all game logic is implemented in pure C# classes inside `src/Scripts/Core/`. Design documents (GDD, TDD, sprint plans) live in `docs/`.

---

## Build & Test — Exact Commands

**Runtime requirements:** .NET SDK 10.0, Godot 4.6.2 with .NET support.

### Step 1 — Restore (always run first after any `.csproj` change)

```bash
dotnet restore EchoForest.sln
```

### Step 2 — Build the test project

```bash
dotnet build src/Scripts/Tests/EchoForest.Tests.csproj --configuration Debug --no-restore
```

> Do **not** `dotnet build EchoForest.csproj` directly for tests — the main project uses `Godot.NET.Sdk` which requires the Godot binary. The test project uses `Microsoft.NET.Sdk` and references the main project with `ExcludeAssets="analyzers"` to avoid this.

### Step 3 — Run tests

```bash
dotnet test src/Scripts/Tests/EchoForest.Tests.csproj --configuration Debug --no-restore --no-build
```

Expected: **all tests pass, 0 failures**. ~980+ tests, runs in < 1 s.

### Step 4 — Run tests with coverage (CI-equivalent)

```bash
dotnet test src/Scripts/Tests/EchoForest.Tests.csproj \
  --configuration Debug \
  --settings src/Scripts/Tests/coverage.runsettings \
  --results-directory ./TestResults \
  --collect:"XPlat Code Coverage"
```

Coverage gate: **≥ 90% line coverage** (enforced by CI).

### Step 5 - Update the sprint plan

Update sprint-plan-full.md with the new feature, bug fix, or refactor. Add a new row to the "Test Suite Breakdown" table with the test file name, number of tests, sprint, and notes. Move deferred tasks to adequate sections.

### Step 6 - Push changes to the repository

Push changes to the repository to trigger CI pipeline. CI will run the same commands as above, and will reject the PR if any test fails or if line coverage drops below 90%. Create a PR to `main` with a descriptive title and summary of changes.

### Quick combined sequence (validated working)

```bash
dotnet restore EchoForest.sln && \
dotnet build src/Scripts/Tests/EchoForest.Tests.csproj --configuration Debug --no-restore && \
dotnet test src/Scripts/Tests/EchoForest.Tests.csproj --configuration Debug --no-restore --no-build
```

---

## CI Pipeline

**File:** `.github/workflows/ci.yml`  
Triggers on every push and every PR to `main`. Steps:

1. Setup .NET 10 + Godot 4.6.2 (via `chickensoft-games/setup-godot`)
2. `dotnet restore EchoForest.sln`
3. `dotnet build EchoForest.sln --configuration ExportRelease` (full solution)
4. `dotnet test src/Scripts/Tests/EchoForest.Tests.csproj` with coverage
5. Enforce ≥ 90% line coverage via ReportGenerator summary

**A PR will be rejected if:** build fails, any test fails, or line coverage drops below 90%.

---

## Architecture & Key Patterns

### Two-layer architecture (critical)

Every feature follows this split — **never mix the two layers**:

| Layer                  | Location                    | Rule                                                                             |
| ---------------------- | --------------------------- | -------------------------------------------------------------------------------- |
| **Pure C# logic**      | `src/Scripts/Core/*.cs`     | No Godot imports. Testable with NUnit.                                           |
| **Godot node wrapper** | `src/Scripts/Core/*Node.cs` | Inherits Godot type. Decorated `[ExcludeFromCodeCoverage]`. Not tested by NUnit. |

**Pattern:** Create `FooController.cs` (pure C#) + `IFooController.cs` (interface) + `FooNode.cs` (Godot wrapper). Test only `FooController`.

### Dependency injection via interfaces

All external dependencies are injected via interfaces. For each Godot service there is:

- `IFileSystem` → `GodotFileSystem` (prod) / `MockFileSystem` (tests)
- `ISceneLoader` → `GodotSceneLoader` / `MockSceneLoader`
- `IApplicationController` → `GodotApplicationController` / `MockApplicationController`
- `IDisplayServer` → `GodotDisplayServer` / `MockDisplayServer`

**Always use the mock** in NUnit tests. Mocks live in `src/Scripts/Core/Mock*.cs`.

### Test doubles naming

All test doubles are named `Mock*` and live alongside production code in `src/Scripts/Core/`. Tests are in `src/Scripts/Tests/`.

### `[ExcludeFromCodeCoverage]` rule

Any class that inherits from a Godot type (`Node`, `CanvasLayer`, `CharacterBody2D`, etc.) **must** be decorated with `[ExcludeFromCodeCoverage(Justification = "...")]`. Without this, CI coverage will fail because these classes can't be exercised without the Godot runtime.

### Scene path constants

All `res://` scene paths are defined as `const string` in `src/Scripts/Core/MainMenuConfig.cs`. Never hardcode scene paths in controllers.

---

## Repository Layout

```
EchoForest.csproj          # Main game project (Godot.NET.Sdk/4.6.2, net10.0)
EchoForest.sln             # Solution (main + test project)
project.godot              # Godot project file
src/
  Assets/                  # Sprites, tilesets, audio, fonts
  Scenes/                  # .tscn scene files (one per screen/area)
  Scripts/
    Core/                  # ALL C# source — controllers, interfaces, mocks, nodes
      Interfaces/          # All I*.cs interfaces
      Enums/               # QuestState, etc.
    Tests/                 # NUnit test files (*Test.cs)
      EchoForest.Tests.csproj
      coverage.runsettings
docs/                      # GDD, TDD, sprint-plan-full.md, ui-ux-spec.md
addons/gut/                # GUT test framework (Godot scene tests — NOT run by dotnet test)
.github/workflows/
  ci.yml                   # CI: build + NUnit + coverage gate
  export.yml               # Manual: multi-platform Godot export
```

---

## Coding Conventions

- **Namespace:** `EchoForest.Core` for all production code; `EchoForest.Tests` for tests.
- **Nullable:** enabled — use `?` types and null guards (`?? throw new ArgumentNullException`).
- **New feature checklist:**
  1. Define `IFoo.cs` interface in `Interfaces/`
  2. Implement `Foo.cs` (pure C#, no Godot imports)
  3. Create `FooNode.cs` (Godot wrapper, `[ExcludeFromCodeCoverage]`)
  4. Create `FooTest.cs` in `Tests/` with NUnit, using `Mock*` doubles
  5. Run tests — all must pass and coverage must stay ≥ 90%

- **Scene wiring:** when adding a new screen scene, add its `res://` path as a `const` in `MainMenuConfig.cs`.
- **Save files:** `SaveService` writes to `user://save_slot_{N}.json` (slots 1–5) via `IFileSystem`.
- **Serialization:** `System.Text.Json` with `JsonStringEnumConverter`. No Newtonsoft.

## Known Pitfalls

- Running `dotnet build EchoForest.csproj` without Godot installed will fail — always use the test project path for local builds.
- `dotnet test` without `--no-restore` is safe but slower; add `--no-restore --no-build` when the project is already built.
- The `addons/gut/` GUT tests require the Godot editor to run — they are **not** part of the `dotnet test` pipeline.
- `InternalsVisibleTo("EchoForest.Tests")` is set in `EchoForest.csproj`, so `internal` members are accessible in tests.

---

## Agent Behavioral Guidelines

> Sourced from `andrej-karpathy-skills/CLAUDE.md`. These guidelines reduce common LLM coding mistakes.

**Tradeoff:** These guidelines bias toward caution over speed. For trivial tasks, use judgment.

### 1. Think Before Coding

**Don't assume. Don't hide confusion. Surface tradeoffs.**

Before implementing:

- State your assumptions explicitly. If uncertain, ask.
- If multiple interpretations exist, present them — don't pick silently.
- If a simpler approach exists, say so. Push back when warranted.
- If something is unclear, stop. Name what's confusing. Ask.

### 2. Simplicity First

**Minimum code that solves the problem. Nothing speculative.**

- No features beyond what was asked.
- No abstractions for single-use code.
- No "flexibility" or "configurability" that wasn't requested.
- No error handling for impossible scenarios.
- If you write 200 lines and it could be 50, rewrite it.

Ask yourself: "Would a senior engineer say this is overcomplicated?" If yes, simplify.

### 3. Surgical Changes

**Touch only what you must. Clean up only your own mess.**

When editing existing code:

- Don't "improve" adjacent code, comments, or formatting.
- Don't refactor things that aren't broken.
- Match existing style, even if you'd do it differently.
- If you notice unrelated dead code, mention it — don't delete it.

When your changes create orphans:

- Remove imports/variables/functions that YOUR changes made unused.
- Don't remove pre-existing dead code unless asked.

The test: Every changed line should trace directly to the user's request.

### 4. Goal-Driven Execution

**Define success criteria. Loop until verified.**

Transform tasks into verifiable goals:

- "Add validation" → "Write tests for invalid inputs, then make them pass"
- "Fix the bug" → "Write a test that reproduces it, then make it pass"
- "Refactor X" → "Ensure tests pass before and after"

For multi-step tasks, state a brief plan:

```
1. [Step] → verify: [check]
2. [Step] → verify: [check]
3. [Step] → verify: [check]
```

Strong success criteria let you loop independently. Weak criteria ("make it work") require constant clarification.
