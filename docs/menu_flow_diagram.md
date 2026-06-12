# Menu Flow Diagram — EchoForest

**Document Version:** 1.0  
**Last Updated:** April 28, 2026  
**Source:** `ui-ux-spec.md` §3 (Main Menu) + §9 (Pause Menu)  
**Implementation reference:** `MainMenuConfig.cs` scene path constants

---

## Legend

| Symbol      | Meaning                                                  |
| ----------- | -------------------------------------------------------- |
| `[Screen]`  | Full scene swap (`ISceneLoader.LoadScene`)               |
| `[Overlay]` | `CanvasLayer` added on top of current scene (`AddChild`) |
| `[OS]`      | Application exits                                        |
| ✅          | Implemented                                              |
| 🔲          | Planned (not yet implemented)                            |

---

## 1. Main Menu Flow

```mermaid
flowchart TD
    START([Game Launch]) --> MAIN

    MAIN["🏠 MainMenu.tscn\n(MainMenuNode / MainMenuController)"]

    MAIN -->|"New Game\nIsContinueEnabled = n/a"| BOOT
    MAIN -->|"Continue\n[disabled if no save]"| COTTAGE
    MAIN -->|"Load Game"| LOAD
    MAIN -->|"Settings"| SETTINGS_MM
    MAIN -->|"Credits"| CREDITS
    MAIN -->|"Exit"| OS([OS / Quit])

    BOOT["⚙️ GameBootstrap.tscn\n(GameBootstrapNode)\nauto-advances immediately"]
    BOOT -->|"LoadScene on _Ready"| COTTAGE

    COTTAGE["🏡 TestArea_Cottage.tscn\n(CottageAreaNode)\n= active gameplay"]

    LOAD["📂 LoadGameScreen.tscn\n(LoadGameScreenNode)\nslots 1–5"]
    LOAD -->|"Load slot N"| COTTAGE
    LOAD -->|"Back"| MAIN

    SETTINGS_MM["⚙️ SettingsScreen.tscn\n(SettingsScreenNode)\nfrom Main Menu"]
    SETTINGS_MM -->|"Back / Apply"| MAIN

    CREDITS["📜 CreditsScreen.tscn\n(CreditsScreenNode)\nauto-scroll 20 s"]
    CREDITS -->|"Back"| MAIN
```

**Scene path constants** (`MainMenuConfig.cs`):

| Transition | Constant                 | Path                                     |
| ---------- | ------------------------ | ---------------------------------------- |
| Main Menu  | `SceneResPath`           | `res://src/Scenes/MainMenu.tscn`         |
| New Game   | `GameBootstrapScenePath` | `res://src/Scenes/GameBootstrap.tscn`    |
| Continue   | `ContinueScenePath`      | `res://src/Scenes/TestArea_Cottage.tscn` |
| Load Game  | `LoadGameScenePath`      | `res://src/Scenes/LoadGameScreen.tscn`   |
| Settings   | `SettingsScenePath`      | `res://src/Scenes/SettingsScreen.tscn`   |
| Credits    | `CreditsScenePath`       | `res://src/Scenes/CreditsScreen.tscn`    |

---

## 2. Pause Menu Flow

```mermaid
flowchart TD
    GAMEPLAY["🏡 Active Gameplay Scene\n(e.g. TestArea_Cottage.tscn)"]

    GAMEPLAY -->|"'pause' input action\nGameHudNode._UnhandledInput\nAddChild(PauseMenu)"| PAUSE

    PAUSE["⏸️ PauseMenu.tscn (overlay)\n(PauseMenuNode / PauseMenuController)\nCanvasLayer layer=100\nGetTree().Paused = true"]

    PAUSE -->|"Resume\nGetTree().Paused = false\nQueueFree"| GAMEPLAY
    PAUSE -->|"Save Game\nSaveService.Save slot 1\nstays open"| PAUSE
    PAUSE -->|"Settings"| SETTINGS_PM
    PAUSE -->|"Return to Main Menu"| MAIN

    SETTINGS_PM["⚙️ SettingsScreen.tscn\n(from Pause Menu)"]
    SETTINGS_PM -->|"Back / Apply\n⚠️ returns to Main Menu\nnot back to Pause"| MAIN

    MAIN["🏠 MainMenu.tscn"]
```

**Pause Menu scene path** (`MainMenuConfig.cs`):

| Constant             | Path                              |
| -------------------- | --------------------------------- |
| `PauseMenuScenePath` | `res://src/Scenes/PauseMenu.tscn` |

> **⚠️ Known limitation (S6-02):** `SettingsScreen` always navigates back to `MainMenu` via `MainMenuConfig.SceneResPath`. When opened from the Pause Menu, pressing Back/Apply sends the player to the Main Menu rather than returning to the Pause Menu. Fixing this requires a caller-context parameter on `SettingsController` — deferred to a later sprint.

---

## 3. Combined Overview

```mermaid
flowchart LR
    subgraph "Out-of-Game Screens"
        MAIN["MainMenu"]
        LOAD["LoadGameScreen"]
        SETTINGS["SettingsScreen"]
        CREDITS["CreditsScreen"]
        BOOT["GameBootstrap"]
    end

    subgraph "In-Game"
        GAMEPLAY["Gameplay Scene\n(TestArea_Cottage)"]
        PAUSE["PauseMenu\n(overlay)"]
        HUD["GameHUD\n(overlay, layer 10)"]
    end

    MAIN -- "New Game" --> BOOT
    BOOT -- "auto" --> GAMEPLAY
    MAIN -- "Continue" --> GAMEPLAY
    MAIN -- "Load Game" --> LOAD
    LOAD -- "slot selected" --> GAMEPLAY
    MAIN -- "Settings" --> SETTINGS
    SETTINGS -- "Back" --> MAIN
    MAIN -- "Credits" --> CREDITS
    CREDITS -- "Back" --> MAIN

    GAMEPLAY -- "pause input" --> PAUSE
    PAUSE -- "Resume" --> GAMEPLAY
    PAUSE -- "Settings" --> SETTINGS
    SETTINGS -. "Back ⚠️" .-> MAIN
    PAUSE -- "Main Menu" --> MAIN
    LOAD -- "Back" --> MAIN
```

---

## 4. Controller ↔ Scene Mapping

| Screen                  | Controller class      | Node class           | Layer                      |
| ----------------------- | --------------------- | -------------------- | -------------------------- |
| `MainMenu.tscn`         | `MainMenuController`  | `MainMenuNode`       | CanvasLayer (default)      |
| `GameBootstrap.tscn`    | —                     | `GameBootstrapNode`  | Node2D                     |
| `LoadGameScreen.tscn`   | —                     | `LoadGameScreenNode` | CanvasLayer                |
| `SettingsScreen.tscn`   | `SettingsController`  | `SettingsScreenNode` | CanvasLayer                |
| `CreditsScreen.tscn`    | `CreditsController`   | `CreditsScreenNode`  | CanvasLayer                |
| `GameHUD.tscn`          | `GameHudController`   | `GameHudNode`        | CanvasLayer, `layer = 10`  |
| `PauseMenu.tscn`        | `PauseMenuController` | `PauseMenuNode`      | CanvasLayer, `layer = 100` |
| `TestArea_Cottage.tscn` | —                     | `CottageAreaNode`    | Node2D (root)              |

---

## 5. Transition Implementation Notes

- **Full scene swap:** `ISceneLoader.LoadScene(path)` → calls `GetTree().ChangeSceneToFile(path)`. Previous scene is destroyed.
- **Overlay (Pause Menu):** `GD.Load<PackedScene>(path).Instantiate()` added via `GetTree().Root.AddChild(...)` in `CottageAreaNode._UnhandledInput`. Gameplay scene stays in tree; only the `Player` node has its `ProcessMode` set to `Disabled` to freeze movement.
- **Why NOT `GetTree().Paused`:** In Godot 4, `SceneTree.Paused = true` blocks `_gui_input` on **all** nodes — including those with `ProcessMode.Always` — preventing button `Pressed` signals from firing. `CottageAreaNode` deliberately avoids this.
- **Overlay visual blocking:** The `Overlay` ColorRect must have `mouse_filter = IGNORE (2)` so mouse events pass through it to the buttons. Default `mouse_filter = STOP (0)` on a full-screen overlay can intercept clicks before they reach the button controls.
- **Button wiring pattern:** All menus (including `PauseMenuNode`) use `FindChild(name)` for button lookup — recursive, null-safe, and forgiving of scene hierarchy changes. Hard-coded `GetNode("path")` calls are fragile.
- **Back from overlay:** `QueueFree()` on the overlay node + `player.ProcessMode = ProcessModeEnum.Inherit` restored via `TreeExiting` signal.

---

## 6. Revision History

| Version | Date           | Changes                                                                              |
| ------- | -------------- | ------------------------------------------------------------------------------------ |
| 1.0     | April 28, 2026 | Initial diagram — Main Menu + Pause Menu flows                                       |
| 1.1     | April 28, 2026 | Fixed implementation notes: no `GetTree().Paused`, Overlay `mouse_filter`, FindChild |
