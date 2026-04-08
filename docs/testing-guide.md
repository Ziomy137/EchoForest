# Testing Guide — EchoForest

**Version:** 1.0 | **Last Updated:** April 8, 2026

This guide explains how to run, write, and maintain tests for the EchoForest project.
We use **two complementary test frameworks**: NUnit for pure C# logic and GUT for Godot scene integration.

---

## Table of Contents

1. [Framework Overview](#framework-overview)
2. [Running NUnit Tests (CLI)](#running-nunit-tests)
3. [Running GUT Tests (Godot Editor)](#running-gut-tests)
4. [Writing New Tests](#writing-new-tests)
5. [Coverage Reports](#coverage-reports)
6. [CI Pipeline](#ci-pipeline)
7. [TDD Workflow Reminder](#tdd-workflow-reminder)

---

## Framework Overview

| Framework   | Scope                                                 | Runner                      | CI                   |
| ----------- | ----------------------------------------------------- | --------------------------- | -------------------- |
| **NUnit 4** | Pure C# classes — state machines, services, utilities | `dotnet test`               | ✅ Automatic         |
| **GUT 9.x** | Scene/node tests requiring the Godot scene tree       | Godot editor / `--headless` | Manual (editor only) |

**Rule of thumb:** If a class does not inherit from a Godot type (`Node`, `CharacterBody2D`, etc.), test it with **NUnit**. If it does, test it with **GUT**.

---

## Running NUnit Tests

### Prerequisites

- .NET SDK 8.0 — [download](https://dotnet.microsoft.com/download/dotnet/8.0)
- Godot 4.3 with .NET support installed (required to build `EchoForest.csproj`)

### Quick Run

```bash
# From repository root — runs all NUnit tests
dotnet test src/Scripts/Tests/EchoForest.Tests.csproj
```

### With Verbose Output

```bash
dotnet test src/Scripts/Tests/EchoForest.Tests.csproj --verbosity normal
```

### Run a Specific Test Class

```bash
dotnet test src/Scripts/Tests/EchoForest.Tests.csproj --filter "FullyQualifiedName~EngineBootTest"
```

### Run a Specific Test Method

```bash
dotnet test src/Scripts/Tests/EchoForest.Tests.csproj --filter "Name=CI_TestRunnerIsOperational"
```

### Build First (if sources changed)

```bash
dotnet build EchoForest.sln
dotnet test src/Scripts/Tests/EchoForest.Tests.csproj --no-build
```

### Expected Output (Green)

```
Test run for EchoForest.Tests.dll (.NETCoreApp,Version=v8.0)
Microsoft (R) Test Execution Command Line Tool
...
Passed!  - Failed: 0, Passed: 3, Skipped: 0, Total: 3
```

---

## Running GUT Tests

### Installation (One-Time Setup)

GUT is **not bundled** in this repository. Install it before running GUT tests:

1. Open the project in **Godot 4.3**
2. Click **AssetLib** in the top toolbar
3. Search for **GUT** and install version ≥ 9.3.0
4. Enable via **Project → Project Settings → Plugins → GUT → ✅ Enable**

Alternatively, install from GitHub (see `addons/gut/README.md`).

### Running in Godot Editor

1. Open Godot editor with the EchoForest project
2. In the bottom panel, click the **GUT** tab
3. Click **Run All** to execute all `test_*.gd` files under `src/Scripts/Tests/`

### Running Headless (CLI)

```bash
godot --headless -s addons/gut/gut_cmdln.gd
```

GUT reads `.gutconfig.json` at the project root for configuration (test directories, prefix, etc.).

### GUT Test Location

All GUT tests live in `src/Scripts/Tests/` and use the `test_` prefix:

```
src/Scripts/Tests/
├── test_engine_boot.gd       ← GUT bootstrap test
├── EngineBootTest.cs         ← NUnit bootstrap test
└── ...
```

---

## Writing New Tests

### NUnit — Pure C# Logic

Create a test file next to the class under test, following the `<ClassName>Test.cs` convention:

```
src/Scripts/Core/StateMachine.cs       ← production code
src/Scripts/Tests/StateMachineTest.cs  ← NUnit tests
```

**Minimal NUnit test class:**

```csharp
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class StateMachineTest
{
    [Test]
    public void InitialState_IsSetCorrectly()
    {
        var sm = new StateMachine<PlayerState>(PlayerState.Idle);
        Assert.That(sm.CurrentState, Is.EqualTo(PlayerState.Idle));
    }

    [Test]
    public void TransitionTo_InvalidState_ThrowsException()
    {
        var sm = new PlayerStateMachine(PlayerState.Jumping);
        Assert.That(
            () => sm.TransitionTo(PlayerState.Running),
            Throws.TypeOf<InvalidOperationException>()
        );
    }
}
```

**TDD order:**

1. Write the test first (it will fail — RED)
2. Write the minimum code to make it pass (GREEN)
3. Refactor if needed (keep GREEN)
4. Commit both test and implementation together

### GUT — Godot Scene Tests

```gdscript
extends GutTest

func test_player_spawns_at_correct_position() -> void:
    var scene = load("res://src/Scenes/TestArea_Cottage.tscn")
    var instance = scene.instantiate()
    add_child(instance)

    var spawn = instance.find_child("PlayerSpawnPoint")
    assert_not_null(spawn, "PlayerSpawnPoint node must exist")

    queue_free()
```

---

## Coverage Reports

### Generate Locally

```bash
# Run tests and collect coverage (Cobertura XML format)
dotnet test src/Scripts/Tests/EchoForest.Tests.csproj \
  --collect:"XPlat Code Coverage" \
  --results-directory ./TestResults \
  -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura

# Install ReportGenerator (one-time)
dotnet tool install --global dotnet-reportgenerator-globaltool

# Build HTML + text summary report
reportgenerator \
  "-reports:TestResults/**/coverage.cobertura.xml" \
  "-targetdir:coverage-report" \
  "-reporttypes:Html;TextSummary"

# Open the report
open coverage-report/index.html   # macOS
```

### Coverage Threshold

The CI pipeline fails if line coverage drops below **90%**. Check your coverage before opening a PR:

```bash
cat coverage-report/Summary.txt
# Look for:  Line coverage: XX.X%
```

### Coverage Exclusions

Classes that require the Godot engine at runtime (scene tests, editor-only code) are excluded from the NUnit coverage metric. They are covered by GUT instead. To exclude a class explicitly:

```csharp
[ExcludeFromCodeCoverage]
public partial class PlayerController : CharacterBody2D { ... }
```

---

## CI Pipeline

The GitHub Actions pipeline (`.github/workflows/ci.yml`) runs automatically on every:

- `push` to any branch
- `pull_request` targeting `main`

**Pipeline steps:**

1. Checkout → Setup .NET 8 → Setup Godot 4.3
2. `dotnet restore` → `dotnet build`
3. `dotnet test` with Coverlet
4. ReportGenerator → HTML + TextSummary
5. Enforce ≥ 90% coverage threshold
6. Upload test results & coverage as artifacts

**Artifacts** (available in GitHub Actions run summary for 14 days):

- `test-results/` — NUnit `.trx` report
- `coverage-report/` — HTML coverage report

---

## TDD Workflow Reminder

```
1. RED    → Write a failing test for the expected behavior
2. GREEN  → Write the minimum code to make the test pass
3. REFACTOR → Clean up without breaking tests
4. COMMIT → Commit test + implementation together (never code without tests)
```

**No PR merges if:**

- Any NUnit test fails
- Line coverage drops below 90%
- CI pipeline is red

---

## Quick Reference

| Task                     | Command                                                                                                             |
| ------------------------ | ------------------------------------------------------------------------------------------------------------------- |
| Run all NUnit tests      | `dotnet test src/Scripts/Tests/EchoForest.Tests.csproj`                                                             |
| Run with coverage        | `dotnet test ... --collect:"XPlat Code Coverage"`                                                                   |
| Run specific test        | `dotnet test ... --filter "Name=MyTestMethod"`                                                                      |
| Build solution           | `dotnet build EchoForest.sln`                                                                                       |
| Run GUT (headless)       | `godot --headless -s addons/gut/gut_cmdln.gd`                                                                       |
| Generate coverage report | `reportgenerator "-reports:TestResults/**/coverage.cobertura.xml" "-targetdir:coverage-report" "-reporttypes:Html"` |
