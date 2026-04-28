# Sprint Plan — EchoForest Full Game

## Post-Demo Development Plan

**Document Version:** 1.0  
**Last Updated:** April 23, 2026  
**Baseline:** Demo complete (Sprint 0–4, 106 story points, 794 passing tests, 99.81% coverage)  
**Target:** Full playable game — all areas, all systems, complete narrative  
**Methodology:** Test-Driven Development (TDD) — tests written before implementation  
**Platform:** Windows / Linux / macOS (Godot + C#)  
**Sprint Length:** 2 weeks  
**Total Estimated Duration:** ~24 weeks (12 sprints, 6 phases)

---

## What the Demo Delivered

| System                      | Status      | Notes                                     |
| --------------------------- | ----------- | ----------------------------------------- |
| Project foundation & CI/CD  | ✅ Complete | GitHub Actions, 90% coverage gate         |
| Isometric TileMap + Z-sort  | ✅ Complete | 64×32 px tiles, depth ordering works      |
| Camera follow + bounds      | ✅ Complete | Smooth lerp, pixel snap                   |
| Color palette + shader      | ✅ Complete | 16 palette colors, PaletteValidator       |
| Generic state machine       | ✅ Complete | PlayerStateMachine with valid transitions |
| Player movement (4-dir)     | ✅ Complete | Walk/run, diagonal normalization          |
| Player animation controller | ✅ Complete | 12 animation clips, no-restart logic      |
| Collision & physics         | ✅ Complete | CharacterBody2D, layer constants          |
| Cottage exterior scene      | ✅ Complete | 30×20 tile grid, all props placed         |
| Scene loader & bootstrap    | ✅ Complete | Async-capable, spawn point integration    |
| Minimal demo HUD            | ✅ Complete | Build label, tutorial hint, debug state   |
| Platform export (all 3 OS)  | ✅ Complete | CI exports Windows / Linux / macOS        |

---

## Full Game Definition of Done

The game is considered shippable when:

- [ ] All 6 game areas are fully playable and connected
- [ ] Complete main quest chain (5 quests) is implementable and finishable
- [ ] Player can reach the end credits from a fresh start
- [ ] NPC dialogue is implemented for Wife, Local Mage, and key city NPCs
- [ ] Combat system functional (sword melee + bow ranged)
- [ ] Inventory and equipment system fully working with visual updates
- [ ] Save/Load system persists all critical game state
- [ ] Main Menu with all screens (New Game, Load, Settings, Credits, Exit)
- [ ] Audio: background music per area + key SFX implemented
- [ ] Day/night color cycle functional
- [ ] All C# game logic maintains ≥ 90% unit test coverage
- [ ] CI pipeline green on `main` at all times
- [ ] Stable 60 FPS across all areas on target hardware
- [ ] No critical runtime errors or unhandled exceptions

---

## TDD Workflow — Unchanged

```
1. RED   → Write a failing unit test for the expected behavior
2. GREEN → Write the minimum code to make the test pass
3. REFACTOR → Clean code without breaking tests
4. COMMIT → Commit test + implementation together
```

Coverage gate: PRs blocked if coverage drops below 90%.

---

## Phase 1 — Menus, Settings & Persistence

> **Goal:** Player can start, save, load and quit the game through a fully functional Main Menu.
> These are prerequisite systems for everything else — without save/load no other system can
> persist between sessions.

---

## Sprint 5 — Main Menu, Settings & Save/Load System

**Duration:** 2 weeks | **Dates:** Week 11–12  
**Goal:** Full Main Menu with all screens functional, game settings persisted to disk, save/load game state

---

### S5-01 — Main Menu Scene

**Status: ✅ COMPLETED**

**Type:** UI  
**Assignee:** Developer  
**Estimate:** 5 points

**Tasks:**

- [x] Create `MainMenu.tscn` with `CanvasLayer` root
- [x] Implement all 6 buttons from UI/UX spec using palette colors:
  - **New Game** → transitions to game bootstrap (new playthrough)
  - **Continue** → loads most recent save file (grayed out if none exists)
  - **Load Game** → opens `LoadGameScreen.tscn`
  - **Settings** → opens `SettingsScreen.tscn`
  - **Credits** → opens `CreditsScreen.tscn`
  - **Exit** → calls `get_tree().quit()`
- [x] Create `MainMenuController.cs` implementing `IMainMenuController`:
  - `OnNewGame()`, `OnContinue()`, `OnLoadGame()`, `OnSettings()`, `OnCredits()`, `OnExit()`
  - `IsContinueEnabled → bool` (true when save file exists)
- [x] Create `MainMenuConfig.cs` with scene path constants
- [x] Create `ISaveService`, `IApplicationController`, `IMainMenuController` interfaces
- [x] Create `MockSaveService`, `MockApplicationController`, `MockSceneLoader` test doubles
- [x] Create `NullSaveService` (placeholder until S5-04 save system)
- [x] Create `GodotApplicationController` (Godot quit wrapper, excluded from coverage)
- [x] Create `MainMenuNode.cs` Godot `CanvasLayer` wrapper (excluded from coverage)
- [x] Background: static image — cottage exterior scene as panoramic (placeholder — Deep Black #1a1a1a ColorRect)
- [x] Button hover, press, disabled visual states (StyleBoxFlat per palette; Gold #ffd700 highlight on hover/press)
- [x] Main menu background music placeholder (`AudioStreamPlayer` node — no stream, wired to Master bus)

**Acceptance Criteria:**

- All 6 buttons navigate to correct screens or perform correct action
- "Continue" is disabled when no save file exists
- "Continue" loads last session correctly when save exists
- Palette colors used for all UI elements

**Tests Required:**

```csharp
// MainMenuControllerTest.cs
[Test] public void MainMenu_ContinueButton_DisabledWhenNoSaveFile() {
    var saveSvc = new MockSaveService(hasSave: false);
    var ctrl = new MainMenuController(saveSvc);
    Assert.IsFalse(ctrl.IsContinueEnabled);
}
[Test] public void MainMenu_ContinueButton_EnabledWhenSaveExists() {
    var saveSvc = new MockSaveService(hasSave: true);
    var ctrl = new MainMenuController(saveSvc);
    Assert.IsTrue(ctrl.IsContinueEnabled);
}
[Test] public void MainMenu_OnNewGame_RequestsSceneLoad() {
    var sceneLoader = new MockSceneLoader();
    var ctrl = new MainMenuController(new MockSaveService(), sceneLoader);
    ctrl.OnNewGame();
    Assert.IsTrue(sceneLoader.WasLoadRequested);
}
[Test] public void MainMenu_OnExit_CallsQuit() {
    var appCtrl = new MockApplicationController();
    var ctrl = new MainMenuController(new MockSaveService(), appCtrl: appCtrl);
    ctrl.OnExit();
    Assert.IsTrue(appCtrl.QuitWasCalled);
}
```

---

### S5-02 — Settings Screen

**Status: ✅ COMPLETED**

**Type:** UI / Engine  
**Assignee:** Developer  
**Estimate:** 8 points

**Tasks:**

- [x] Create `SettingsScreen.tscn` with all options from TDD §3.4:
  - **Window Mode:** Windowed / Borderless Fullscreen (default: Windowed)
  - **Monitor:** dropdown (visible only in Borderless Fullscreen mode)
  - **FPS Limit:** 30 / 60 / 120 / 144 / Unlimited (grayed out when VSync active)
  - **VSync:** toggle (when enabled, FPS Limit is grayed out)
  - **Brightness:** slider 0–200% (default 100%)
  - **Gamma:** slider 0–200% (default 100%)
- [x] Create `WindowMode.cs` enum (`Windowed`, `BorderlessFullscreen`)
- [x] Create `IDisplayServer` + `ISettingsController` interfaces
- [x] Create `SettingsController.cs` implementing `ISettingsController`:
  - `SetWindowMode`, `SetVSync`, `SetFpsLimit`, `SetMonitor`, `SetBrightness`, `SetGamma`
  - `IsFpsLimitEnabled` — false when VSync on
  - `IsMonitorDropdownVisible` — true when Borderless Fullscreen
  - `Apply()` commits pending changes via `IDisplayServer`
  - `Cancel()` reverts to last committed snapshot
- [x] Create `MockDisplayServer` test double
- [x] Create `GodotDisplayServer` Godot wrapper (excluded from coverage)
- [x] Create `SettingsScreenNode.cs` Godot CanvasLayer wrapper (excluded from coverage)
- [x] "Apply" button applies all pending changes; "Cancel" reverts to saved values
- [x] "Back" button returns to Main Menu
- [x] Brightness/Gamma sliders apply post-process in real-time via `GodotDisplayServer`

**Acceptance Criteria:**

- Window mode change applies instantly without restart
- Monitor dropdown only visible in Borderless Fullscreen mode
- FPS Limit control grayed out when VSync is active
- Brightness/Gamma sliders apply visually in real-time
- Pressing "Cancel" reverts uncommitted changes

**Tests Required:**

```csharp
// SettingsControllerTest.cs
[Test] public void Settings_VSync_Enabled_FpsLimitIsDisabled() {
    var ctrl = new SettingsController(new MockDisplayServer());
    ctrl.SetVSync(true);
    Assert.IsFalse(ctrl.IsFpsLimitEnabled);
}
[Test] public void Settings_VSync_Disabled_FpsLimitIsEnabled() {
    var ctrl = new SettingsController(new MockDisplayServer());
    ctrl.SetVSync(false);
    Assert.IsTrue(ctrl.IsFpsLimitEnabled);
}
[Test] public void Settings_WindowMode_Borderless_ShowsMonitorDropdown() {
    var ctrl = new SettingsController(new MockDisplayServer());
    ctrl.SetWindowMode(WindowMode.BorderlessFullscreen);
    Assert.IsTrue(ctrl.IsMonitorDropdownVisible);
}
[Test] public void Settings_WindowMode_Windowed_HidesMonitorDropdown() {
    var ctrl = new SettingsController(new MockDisplayServer());
    ctrl.SetWindowMode(WindowMode.Windowed);
    Assert.IsFalse(ctrl.IsMonitorDropdownVisible);
}
[Test] public void Settings_Brightness_OutOfRange_ClampedTo0To200() {
    var ctrl = new SettingsController(new MockDisplayServer());
    ctrl.SetBrightness(250f);
    Assert.AreEqual(200f, ctrl.Brightness);
    ctrl.SetBrightness(-10f);
    Assert.AreEqual(0f, ctrl.Brightness);
}
[Test] public void Settings_Cancel_RevertsUncommittedChanges() {
    var ctrl = new SettingsController(new MockDisplayServer());
    ctrl.SetBrightness(50f);
    ctrl.Cancel();
    Assert.AreEqual(100f, ctrl.Brightness); // default
}
```

---

### S5-03 — User Config Persistence

**Status: ✅ COMPLETED**

**Type:** System  
**Assignee:** Developer  
**Estimate:** 3 points

**Tasks:**

- [x] Create `UserConfig.cs` — plain data class holding all settings:
  - `WindowMode`, `MonitorIndex`, `FpsLimit`, `VSync`, `Brightness`, `Gamma`
- [x] Create `ConfigService.cs` implementing `IConfigService`:
  - `Save(UserConfig config)` — serializes to JSON at `user://settings.json`
  - `Load() → UserConfig` — deserializes from disk; returns defaults if file absent
  - `GetDefaults() → UserConfig` — returns factory defaults
- [x] Load config on game start; apply before first frame renders
- [x] Settings screen calls `ConfigService.Save()` on "Apply"

**Acceptance Criteria:**

- Settings persist across game restarts
- Missing/corrupt config file falls back to defaults gracefully
- No personally identifiable information written to config

**Tests Required:**

```csharp
// ConfigServiceTest.cs
[Test] public void ConfigService_SaveAndLoad_RoundTrip() {
    var svc = new ConfigService(new MockFileSystem());
    var config = new UserConfig { Brightness = 120f, VSync = true };
    svc.Save(config);
    var loaded = svc.Load();
    Assert.AreEqual(120f, loaded.Brightness);
    Assert.IsTrue(loaded.VSync);
}
[Test] public void ConfigService_MissingFile_ReturnsDefaults() {
    var svc = new ConfigService(new MockFileSystem(fileExists: false));
    var config = svc.Load();
    Assert.AreEqual(100f, config.Brightness);
    Assert.AreEqual(100f, config.Gamma);
}
[Test] public void ConfigService_CorruptFile_ReturnsDefaults() {
    var fs = new MockFileSystem(content: "not valid json{{{{");
    var svc = new ConfigService(fs);
    var config = svc.Load();
    Assert.IsNotNull(config);
}
```

---

### S5-04 — Save/Load Game System

**Type:** System  
**Assignee:** Developer  
**Estimate:** 8 points  
**Status: ✅ COMPLETED** (save triggers at area transitions deferred to gameplay sprint)

**Tasks:**

- [x] Define `SaveData.cs` — serializable data class containing:
  - `PlayerPosition` (Vector2 as float pair)
  - `CurrentArea` (enum or string scene path)
  - `QuestStates` (dictionary: quest ID → `QuestState` enum)
  - `InventoryItems` (list of item IDs + quantities)
  - `EquippedItems` (equipment slot → item ID)
  - `PlayerHealth` (float)
  - `SaveTimestamp` (DateTime, UTC)
  - `PlaytimeTotalSeconds` (double)
- [x] Create `SaveService.cs` implementing `ISaveDataService` (extends `ISaveService`):
  - `Save(SaveData data, int slot)` — writes to `user://save_slot_{slot}.json`
  - `Load(int slot) → SaveData` — reads from disk
  - `Delete(int slot)` — removes save file
  - `GetSaveSlots() → List<SaveSlotInfo>` — returns metadata for all slots
  - `HasSave(int slot) → bool`
  - `HasSaveFile() → bool` — scans slots 1–5; satisfies `ISaveService` compat for Continue button
- [x] Create `LoadGameScreen.tscn` — displays 5 save slots with area, playtime, date; Load buttons disabled for empty slots
- [x] `QuestState` enum, `SaveSlotInfo` metadata record, `SaveDataException` custom exception
- [x] `IFileSystem.Delete()` added; `MainMenuNode` wired to real `SaveService`

**Acceptance Criteria:**

- Save and load preserves player position, quest state, inventory
- Loading a save file puts player in the correct area at the correct position
- Corrupt or missing save slot is handled gracefully (no crash)
- Playtime accumulates correctly across sessions

**Tests Required:**

```csharp
// SaveServiceTest.cs
[Test] public void SaveService_SaveAndLoad_PreservesPlayerPosition() {
    var svc = new SaveService(new MockFileSystem());
    var data = new SaveData { PlayerPosition = new Vector2(320f, 240f) };
    svc.Save(data, slot: 1);
    var loaded = svc.Load(1);
    Assert.AreEqual(320f, loaded.PlayerPosition.X, 0.001f);
    Assert.AreEqual(240f, loaded.PlayerPosition.Y, 0.001f);
}
[Test] public void SaveService_SaveAndLoad_PreservesQuestState() {
    var svc = new SaveService(new MockFileSystem());
    var data = new SaveData();
    data.QuestStates["q_kidnapped"] = QuestState.Completed;
    svc.Save(data, slot: 1);
    var loaded = svc.Load(1);
    Assert.AreEqual(QuestState.Completed, loaded.QuestStates["q_kidnapped"]);
}
[Test] public void SaveService_HasSave_FalseForEmptySlot() {
    var svc = new SaveService(new MockFileSystem(fileExists: false));
    Assert.IsFalse(svc.HasSave(1));
}
[Test] public void SaveService_Delete_RemovesFile() {
    var fs = new MockFileSystem();
    var svc = new SaveService(fs);
    svc.Save(new SaveData(), slot: 1);
    svc.Delete(1);
    Assert.IsFalse(svc.HasSave(1));
}
[Test] public void SaveService_CorruptFile_ThrowsSaveDataException() {
    var fs = new MockFileSystem(content: "{{corrupted}}");
    var svc = new SaveService(fs);
    Assert.Throws<SaveDataException>(() => svc.Load(1));
}
```

---

### S5-05 — Credits Screen

**Type:** UI  
**Assignee:** Developer  
**Estimate:** 2 points

**Tasks:**

- [ ] Create `CreditsScreen.tscn` — scrolling credits using `RichTextLabel` or animated labels
- [ ] Credit sections: Studio name, Lead Developer, Developer(s), Artist, Level Designer, QA, Tools used (Godot, NUnit, GUT, GitHub Actions)
- [ ] Auto-scrolls from bottom to top; "Back" button returns to main menu at any point
- [ ] Uses palette-compliant colors

**Acceptance Criteria:**

- Credits scroll at a readable pace
- "Back" works at any point in the scroll
- All team roles are credited

**Tests Required:**

```csharp
// CreditsControllerTest.cs
[Test] public void Credits_OnBack_NavigatesTo_MainMenu() {
    var nav = new MockNavigationService();
    var ctrl = new CreditsController(nav);
    ctrl.OnBack();
    Assert.AreEqual("MainMenu", nav.LastRequestedScreen);
}
```

---

**Sprint 5 Summary:**

| Story                     | Points | Owner     |
| ------------------------- | ------ | --------- |
| S5-01 Main Menu ✅        | 5      | Developer |
| S5-02 Settings Screen ✅  | 8      | Developer |
| S5-03 Config Persistence  | 3      | Developer |
| S5-04 Save/Load System ✅ | 8      | Developer |
| S5-05 Credits Screen      | 2      | Developer |
| **Total**                 | **26** |           |

---

## Sprint 6 — Full HUD, Dialogue System & NPC Framework

**Duration:** 2 weeks | **Dates:** Week 13–14  
**Goal:** Complete in-game HUD, NPC interaction system, and dialogue trees functional.
These are prerequisites for all quest and story content.

---

### S6-01 — Full Game HUD

**Type:** UI  
**Assignee:** Developer  
**Estimate:** 8 points

**Tasks:**

- [ ] Replace demo HUD with full `GameHUD.tscn` containing all elements from UI/UX spec §4:
  - **Health Bar** (top-left): visual fill bar + numerical `X/Y` display + damage flash
  - **Equipment Display** (top-right): active weapon icon + armor icon slots
  - **Quest Objective** (top-left, below health): current quest name + objective text + progress `1/3`
  - **Minimap** (bottom-right): placeholder 2D top-down overview of current area
  - **Interaction Prompt** (bottom-center): context-sensitive `[E] Interact` label
- [ ] Create `GameHudController.cs` implementing `IGameHudController`:
  - `UpdateHealth(float current, float max)`
  - `SetActiveWeapon(ItemId weaponId)`
  - `SetQuestObjective(string questName, string objectiveText, int current, int total)`
  - `ShowInteractionPrompt(string action)` / `HideInteractionPrompt()`
  - `UpdateMinimap(Vector2 playerPos, string areaId)`
- [ ] Health bar flashes red on damage (Tween)
- [ ] Quest objective fades in on update, stays persistent (no auto-fade like demo)
- [ ] Pause menu accessible from HUD via `pause` input action

**Acceptance Criteria:**

- Health bar visually fills/depletes proportionally
- Quest objective always shows current active objective
- Interaction prompt appears only when player is in range of an interactable
- Equipment slots update within 1 frame of equipment change

**Tests Required:**

```csharp
// GameHudControllerTest.cs
[Test] public void HUD_HealthBar_ReflectsCurrentHealth() {
    var ctrl = new GameHudController();
    ctrl.UpdateHealth(50f, 100f);
    Assert.AreEqual(0.5f, ctrl.HealthFillRatio, 0.001f);
}
[Test] public void HUD_HealthBar_ClampedBetween0And1() {
    var ctrl = new GameHudController();
    ctrl.UpdateHealth(150f, 100f);
    Assert.LessOrEqual(ctrl.HealthFillRatio, 1f);
    ctrl.UpdateHealth(-10f, 100f);
    Assert.GreaterOrEqual(ctrl.HealthFillRatio, 0f);
}
[Test] public void HUD_InteractionPrompt_Visible_WhenShown() {
    var ctrl = new GameHudController();
    ctrl.ShowInteractionPrompt("Talk");
    Assert.IsTrue(ctrl.IsInteractionPromptVisible);
}
[Test] public void HUD_InteractionPrompt_Hidden_WhenHidden() {
    var ctrl = new GameHudController();
    ctrl.ShowInteractionPrompt("Talk");
    ctrl.HideInteractionPrompt();
    Assert.IsFalse(ctrl.IsInteractionPromptVisible);
}
[Test] public void HUD_QuestObjective_UpdatesText() {
    var ctrl = new GameHudController();
    ctrl.SetQuestObjective("Kidnapped", "Wake up", 1, 3);
    Assert.AreEqual("Kidnapped", ctrl.CurrentQuestName);
    Assert.AreEqual("Wake up", ctrl.CurrentObjectiveText);
    Assert.AreEqual("1/3", ctrl.ObjectiveProgress);
}
```

---

### S6-02 — Pause Menu

**Type:** UI  
**Assignee:** Developer  
**Estimate:** 3 points

**Tasks:**

- [ ] Create `PauseMenu.tscn` — overlay on `CanvasLayer` (highest Z-order)
- [ ] Pause menu options: **Resume**, **Settings**, **Save Game**, **Return to Main Menu**
- [ ] Toggle on `pause` input action; `get_tree().paused = true` halts game loop
- [ ] Create `PauseMenuController.cs`:
  - `OnResume()` — unpauses
  - `OnSettings()` — opens Settings screen (shared with main menu)
  - `OnSaveGame()` — triggers save + shows "Game Saved" confirmation toast
  - `OnMainMenu()` — prompts unsaved changes warning → return to main menu

**Acceptance Criteria:**

- Game world freezes while pause menu is visible
- Settings opened from pause menu returns back to pause menu (not main menu)
- "Return to Main Menu" shows confirmation dialog if unsaved progress exists

**Tests Required:**

```csharp
// PauseMenuControllerTest.cs
[Test] public void PauseMenu_OnResume_UnpausesGame() {
    var gameState = new MockGameState();
    var ctrl = new PauseMenuController(gameState);
    ctrl.Open();
    ctrl.OnResume();
    Assert.IsFalse(gameState.IsPaused);
}
[Test] public void PauseMenu_OnSave_TriggersSave() {
    var saveService = new MockSaveService();
    var ctrl = new PauseMenuController(saveService: saveService);
    ctrl.OnSaveGame();
    Assert.IsTrue(saveService.SaveWasCalled);
}
```

---

### S6-03 — NPC Framework & Interaction System

**Type:** Gameplay / System  
**Assignee:** Lead Developer  
**Estimate:** 8 points

**Tasks:**

- [ ] Create `INpc` interface:
  - `NpcId` (string)
  - `DisplayName` (string)
  - `InteractionRadius` (float)
  - `Interact(IPlayerController player)`
  - `IsInteractable → bool`
- [ ] Create `NpcController.cs` as `CharacterBody2D` wrapper implementing `INpc`
- [ ] Create `InteractionDetector.cs` — `Area2D` child of player; detects when player enters NPC interaction radius
  - Raises `InteractableEntered(INpc npc)` and `InteractableExited(INpc npc)` events
  - Calls `GameHudController.ShowInteractionPrompt("Talk")` on enter
  - On `interact` input action: calls `npc.Interact(player)`
- [ ] NPCs have `Idle` animation loop (2-frame sprite cycle)
- [ ] NPCs face toward player when interaction starts
- [ ] Add Wife NPC placeholder to Cottage scene (non-functional dialogue for now)

**Acceptance Criteria:**

- Interaction prompt shows when player enters NPC radius
- Prompt hides when player leaves radius
- Pressing `interact` while in range calls `Interact()` on the NPC
- Only nearest NPC is targeted when multiple are in range

**Tests Required:**

```csharp
// InteractionDetectorTest.cs
[Test] public void Detector_WithinRadius_RaisesEnteredEvent() {
    bool raised = false;
    var detector = new InteractionDetector();
    detector.InteractableEntered += _ => raised = true;
    var mockNpc = new MockNpc { InteractionRadius = 100f };
    detector.SimulateNpcEnter(mockNpc, distance: 50f);
    Assert.IsTrue(raised);
}
[Test] public void Detector_OutsideRadius_DoesNotRaiseEvent() {
    bool raised = false;
    var detector = new InteractionDetector();
    detector.InteractableEntered += _ => raised = true;
    var mockNpc = new MockNpc { InteractionRadius = 50f };
    detector.SimulateNpcEnter(mockNpc, distance: 100f);
    Assert.IsFalse(raised);
}
[Test] public void Detector_MultipleNpcs_TargetsNearest() {
    var detector = new InteractionDetector();
    var npc1 = new MockNpc { NpcId = "wife", InteractionRadius = 100f };
    var npc2 = new MockNpc { NpcId = "mage", InteractionRadius = 100f };
    detector.SimulateNpcEnter(npc1, distance: 80f);
    detector.SimulateNpcEnter(npc2, distance: 40f);
    Assert.AreEqual("mage", detector.NearestInteractable.NpcId);
}
```

---

### S6-04 — Dialogue System

**Type:** Gameplay / System  
**Assignee:** Developer  
**Estimate:** 8 points

**Tasks:**

- [ ] Define dialogue data format in JSON:
  ```json
  {
    "npc_id": "wife",
    "lines": [
      {
        "id": "wife_01",
        "speaker": "Wife",
        "text": "Please, find our child!",
        "next": "wife_02"
      },
      {
        "id": "wife_02",
        "speaker": "Wife",
        "text": "Head to the city and find the local mage.",
        "next": null
      }
    ]
  }
  ```
- [ ] Create `DialogueLine.cs`, `DialogueTree.cs`, `DialogueOption.cs` data classes
- [ ] Create `DialogueService.cs` implementing `IDialogueService`:
  - `LoadDialogue(string npcId) → DialogueTree`
  - `GetLine(string lineId) → DialogueLine`
  - `GetNextLine(string currentLineId) → DialogueLine?`
- [ ] Create `DialogueController.cs` — manages conversation flow:
  - `StartConversation(string npcId)`
  - `Advance()` — moves to next line or ends conversation
  - `EndConversation()`
  - Event: `OnConversationStarted`, `OnLineChanged(DialogueLine)`, `OnConversationEnded`
- [ ] Create `DialogueBox.tscn` — UI panel with NPC name, dialogue text (typewriter effect), `[E] Continue` prompt
- [ ] Typewriter effect: characters appear one-by-one at configurable speed; pressing `interact` skips to full line
- [ ] Dialogue boxes use approved palette colors
- [ ] Game world does NOT pause during dialogue (unless quest cutscene)
- [ ] Trigger auto-save on `OnConversationEnded` for story-critical NPC dialogues (calls `ISaveDataService.Save`)

**Acceptance Criteria:**

- Dialogue box appears centered-bottom of screen
- NPC name displayed above dialogue text
- Typewriter effect plays; skippable on button press
- Conversation advances one line per `interact` press
- Conversation ends correctly on final line (no null-reference)

**Tests Required:**

```csharp
// DialogueServiceTest.cs
[Test] public void DialogueService_GetLine_ReturnsCorrectText() {
    var svc = new DialogueService(new MockFileSystem(dialogueJson: WifeDialogueJson));
    svc.LoadDialogue("wife");
    var line = svc.GetLine("wife_01");
    Assert.AreEqual("Please, find our child!", line.Text);
}
[Test] public void DialogueService_GetNextLine_FollowsChain() {
    var svc = new DialogueService(new MockFileSystem(dialogueJson: WifeDialogueJson));
    svc.LoadDialogue("wife");
    var next = svc.GetNextLine("wife_01");
    Assert.AreEqual("wife_02", next.Id);
}
[Test] public void DialogueService_GetNextLine_ReturnsNull_AtEnd() {
    var svc = new DialogueService(new MockFileSystem(dialogueJson: WifeDialogueJson));
    svc.LoadDialogue("wife");
    var next = svc.GetNextLine("wife_02");
    Assert.IsNull(next);
}
// DialogueControllerTest.cs
[Test] public void DialogueController_StartConversation_RaisesStartedEvent() {
    bool started = false;
    var ctrl = new DialogueController(new MockDialogueService());
    ctrl.OnConversationStarted += () => started = true;
    ctrl.StartConversation("wife");
    Assert.IsTrue(started);
}
[Test] public void DialogueController_Advance_AtLastLine_EndsConversation() {
    bool ended = false;
    var ctrl = new DialogueController(new MockDialogueService(lineCount: 1));
    ctrl.OnConversationEnded += () => ended = true;
    ctrl.StartConversation("wife");
    ctrl.Advance();
    Assert.IsTrue(ended);
}
```

---

**Sprint 6 Summary:**

| Story                 | Points | Owner     |
| --------------------- | ------ | --------- |
| S6-01 Full Game HUD   | 8      | Developer |
| S6-02 Pause Menu      | 3      | Developer |
| S6-03 NPC Framework   | 8      | Lead Dev  |
| S6-04 Dialogue System | 8      | Developer |
| **Total**             | **27** |           |

---

## Phase 2 — Quest System & Story Framework

---

## Sprint 7 — Quest System, Event Bus & Cutscene Framework

**Duration:** 2 weeks | **Dates:** Week 15–16  
**Goal:** Quest journal tracks player progress; global event bus drives all narrative hooks;
cutscene system can trigger story sequences. These form the backbone of all story content.

---

### S7-01 — Global Event Bus

**Type:** System / Architecture  
**Assignee:** Lead Developer  
**Estimate:** 5 points

**Tasks:**

- [ ] Create `EventBus.cs` as a pure C# singleton (injected via DI, not Godot autoload):
  - Typed `Publish<TEvent>(TEvent e)` — broadcasts event to all subscribers
  - Typed `Subscribe<TEvent>(Action<TEvent> handler)` — registers listener
  - `Unsubscribe<TEvent>(Action<TEvent> handler)` — deregisters listener
  - `Clear()` — removes all subscriptions (for test teardown)
- [ ] Define initial game events:
  - `PlayerHealthChangedEvent(float newHealth, float maxHealth)`
  - `QuestStartedEvent(string questId)`
  - `QuestObjectiveCompletedEvent(string questId, string objectiveId)`
  - `QuestCompletedEvent(string questId)`
  - `NpcInteractionStartedEvent(string npcId)`
  - `AreaTransitionEvent(string fromArea, string toArea)`
  - `PlayerDiedEvent`
  - `ItemPickedUpEvent(string itemId, int quantity)`
- [ ] All game systems communicate exclusively through EventBus (no direct references between systems)

**Acceptance Criteria:**

- Publisher does not need a reference to subscriber
- Multiple subscribers receive the same event independently
- Unsubscribing prevents further delivery
- EventBus is fully testable without Godot engine

**Tests Required:**

```csharp
// EventBusTest.cs
[Test] public void EventBus_Subscribe_ReceivesPublishedEvent() {
    var bus = new EventBus();
    int received = 0;
    bus.Subscribe<QuestStartedEvent>(_ => received++);
    bus.Publish(new QuestStartedEvent("q_kidnapped"));
    Assert.AreEqual(1, received);
}
[Test] public void EventBus_MultipleSubscribers_AllReceive() {
    var bus = new EventBus();
    int a = 0, b = 0;
    bus.Subscribe<QuestStartedEvent>(_ => a++);
    bus.Subscribe<QuestStartedEvent>(_ => b++);
    bus.Publish(new QuestStartedEvent("q_seek_mage"));
    Assert.AreEqual(1, a);
    Assert.AreEqual(1, b);
}
[Test] public void EventBus_Unsubscribe_StopsDelivery() {
    var bus = new EventBus();
    int count = 0;
    Action<QuestStartedEvent> handler = _ => count++;
    bus.Subscribe<QuestStartedEvent>(handler);
    bus.Unsubscribe<QuestStartedEvent>(handler);
    bus.Publish(new QuestStartedEvent("q_kidnapped"));
    Assert.AreEqual(0, count);
}
```

---

### S7-02 — Quest Data & Quest Service

**Type:** System  
**Assignee:** Developer  
**Estimate:** 8 points

**Tasks:**

- [ ] Define quest data format in JSON:
  ```json
  {
    "id": "q_kidnapped",
    "title": "Kidnapped",
    "objectives": [
      { "id": "wake_up", "text": "Wake up", "required": true },
      {
        "id": "find_portal",
        "text": "Find where the portal was",
        "required": true
      },
      { "id": "talk_to_wife", "text": "Talk to your wife", "required": true }
    ],
    "rewards": [],
    "triggers_quest": "q_seek_mage"
  }
  ```
- [ ] Create data classes: `QuestData.cs`, `QuestObjective.cs`, `QuestReward.cs`
- [ ] Create `QuestState` enum: `NotStarted`, `Active`, `Completed`, `Failed`
- [ ] Create `QuestService.cs` implementing `IQuestService`:
  - `StartQuest(string questId)`
  - `CompleteObjective(string questId, string objectiveId)`
  - `CompleteQuest(string questId)`
  - `GetQuestState(string questId) → QuestState`
  - `GetActiveQuests() → List<QuestData>`
  - `GetActiveObjectives(string questId) → List<QuestObjective>`
  - Publishes appropriate `EventBus` events on state changes
- [ ] Create `QuestDatabase.cs` — loads all quest JSON files from `res://Assets/Data/Quests/`
- [ ] Implement all 5 main quests as JSON data files (stubs — full dialogue content in Sprint 14)
- [ ] Apply loaded `SaveData.QuestStates` to `QuestService` on game load (load-game integration)

**Acceptance Criteria:**

- Starting a quest makes it appear in active quests list
- Completing all objectives automatically completes the quest
- Completed quest triggers the next quest in chain
- Quest state is serializable (integrates with SaveService)

**Tests Required:**

```csharp
// QuestServiceTest.cs
[Test] public void QuestService_StartQuest_ChangesStateTo_Active() {
    var svc = new QuestService(new MockQuestDatabase(), new EventBus());
    svc.StartQuest("q_kidnapped");
    Assert.AreEqual(QuestState.Active, svc.GetQuestState("q_kidnapped"));
}
[Test] public void QuestService_StartQuest_PublishesStartedEvent() {
    var bus = new EventBus();
    string publishedId = null;
    bus.Subscribe<QuestStartedEvent>(e => publishedId = e.QuestId);
    var svc = new QuestService(new MockQuestDatabase(), bus);
    svc.StartQuest("q_kidnapped");
    Assert.AreEqual("q_kidnapped", publishedId);
}
[Test] public void QuestService_AllObjectivesComplete_CompletesQuest() {
    var svc = new QuestService(new MockQuestDatabase(), new EventBus());
    svc.StartQuest("q_kidnapped");
    svc.CompleteObjective("q_kidnapped", "wake_up");
    svc.CompleteObjective("q_kidnapped", "find_portal");
    svc.CompleteObjective("q_kidnapped", "talk_to_wife");
    Assert.AreEqual(QuestState.Completed, svc.GetQuestState("q_kidnapped"));
}
[Test] public void QuestService_CompletedQuest_TriggersNextQuest() {
    var bus = new EventBus();
    string nextStarted = null;
    bus.Subscribe<QuestStartedEvent>(e => nextStarted = e.QuestId);
    var svc = new QuestService(new MockQuestDatabase(), bus);
    svc.StartQuest("q_kidnapped");
    // ... complete all objectives
    Assert.AreEqual("q_seek_mage", nextStarted);
}
```

---

### S7-03 — Quest Journal UI

**Type:** UI  
**Assignee:** Developer  
**Estimate:** 5 points

**Tasks:**

- [ ] Create `QuestJournal.tscn` — opened via `inventory` input action (or separate `journal` key)
- [ ] Two-panel layout: left = quest list, right = selected quest details + objectives
- [ ] Quest list shows: Active quests (gold color), Completed quests (grayed out)
- [ ] Detail panel shows: quest title, description, objective list with checkboxes
- [ ] Create `QuestJournalController.cs`:
  - Subscribes to `QuestStartedEvent`, `QuestObjectiveCompletedEvent`, `QuestCompletedEvent`
  - Refreshes display automatically when events arrive
- [ ] Pause game while journal is open

**Acceptance Criteria:**

- New quests appear automatically without restart
- Completed objectives show a checkmark
- Completed quests move to a separate "Completed" section
- Journal can be closed with `inventory` key or `ESC`

**Tests Required:**

```csharp
// QuestJournalControllerTest.cs
[Test] public void Journal_OnQuestStarted_AddedToActiveList() {
    var bus = new EventBus();
    var ctrl = new QuestJournalController(bus);
    bus.Publish(new QuestStartedEvent("q_kidnapped"));
    Assert.IsTrue(ctrl.ActiveQuestIds.Contains("q_kidnapped"));
}
[Test] public void Journal_OnQuestCompleted_MovesToCompletedList() {
    var bus = new EventBus();
    var ctrl = new QuestJournalController(bus);
    bus.Publish(new QuestStartedEvent("q_kidnapped"));
    bus.Publish(new QuestCompletedEvent("q_kidnapped"));
    Assert.IsFalse(ctrl.ActiveQuestIds.Contains("q_kidnapped"));
    Assert.IsTrue(ctrl.CompletedQuestIds.Contains("q_kidnapped"));
}
```

---

### S7-04 — Cutscene Framework

**Type:** System / Gameplay  
**Assignee:** Developer  
**Estimate:** 5 points

**Tasks:**

- [ ] Create `ICutscene` interface with `Play(Action onComplete)` method
- [ ] Create `CutsceneSequencer.cs`:
  - Accepts list of `ICutsceneStep` items (pan camera, show dialogue, fade, wait, trigger event)
  - Plays each step in sequence, waits for completion
  - Publishes `CutsceneStartedEvent` and `CutsceneEndedEvent` via EventBus
  - Locks player input during cutscene (`IInputHandler.IsBlocked = true`)
- [ ] Implement concrete steps:
  - `FadeStep(float duration, Color targetColor)` — screen fade in/out
  - `WaitStep(float seconds)` — simple delay
  - `DialogueStep(string npcId, string lineId)` — plays one dialogue line
  - `CameraPanStep(Vector2 target, float duration)` — pans camera to position
  - `PublishEventStep<TEvent>(TEvent e)` — fires an EventBus event at a point in sequence
- [ ] Intro cutscene (mage attack scene) implemented as first use case — plays on new game start

**Acceptance Criteria:**

- Cutscene steps play in order and wait for each to finish before the next
- Player cannot move during a cutscene
- Fade in/out completes correctly without rendering artifact
- EventBus events fire at the correct point in sequence

**Tests Required:**

```csharp
// CutsceneSequencerTest.cs
[Test] public void Sequencer_PlaysStepsInOrder() {
    var order = new List<int>();
    var steps = new List<ICutsceneStep> {
        new MockStep(() => order.Add(1)),
        new MockStep(() => order.Add(2)),
        new MockStep(() => order.Add(3))
    };
    var seq = new CutsceneSequencer(steps, new MockInputHandler());
    seq.Play(() => {});
    Assert.AreEqual(new[] { 1, 2, 3 }, order);
}
[Test] public void Sequencer_BlocksInput_DuringPlay() {
    var input = new MockInputHandler();
    var seq = new CutsceneSequencer(new List<ICutsceneStep>(), input);
    seq.Play(() => {});
    Assert.IsTrue(input.IsBlocked);
}
[Test] public void Sequencer_UnblocksInput_OnComplete() {
    var input = new MockInputHandler();
    var step = new MockInstantStep();
    var seq = new CutsceneSequencer(new List<ICutsceneStep> { step }, input);
    bool done = false;
    seq.Play(() => done = true);
    step.Complete();
    Assert.IsTrue(done);
    Assert.IsFalse(input.IsBlocked);
}
```

---

**Sprint 7 Summary:**

| Story                    | Points | Owner     |
| ------------------------ | ------ | --------- |
| S7-01 Event Bus          | 5      | Lead Dev  |
| S7-02 Quest System       | 8      | Developer |
| S7-03 Quest Journal UI   | 5      | Developer |
| S7-04 Cutscene Framework | 5      | Developer |
| **Total**                | **23** |           |

---

## Phase 3 — World Expansion

> **Goal:** Build all 6 game areas (cottage already done). Connect them with transitions.
> Artist works on area-specific tiles and props in parallel with developer area construction.

---

## Sprint 8 — Farm, Forest Path & Area Transitions

**Duration:** 2 weeks | **Dates:** Week 17–18  
**Goal:** Farm area and Forest Path area playable; area transition system implemented;
player can walk from Cottage → Farm → Forest Path.

---

### S8-01 — Area Transition System

**Type:** System / Gameplay  
**Assignee:** Lead Developer  
**Estimate:** 5 points

**Tasks:**

- [ ] Create `AreaTransition.cs` — `Area2D`-based trigger zone placed at scene edges:
  - `TargetArea` (string scene path)
  - `SpawnPointId` (string) — named spawn point in target scene
  - `TransitionType`: `Instant`, `FadeToBlack`, `FadeToWhite`
- [ ] Create `AreaTransitionService.cs` implementing `IAreaTransitionService`:
  - On trigger: fade out → unload current scene → load target scene → spawn at named point → fade in
  - Publishes `AreaTransitionEvent` to EventBus
  - Saves current game state before transition
- [ ] Add `SpawnPoint` nodes (named `Area2D` markers) to all scenes
- [ ] Wire Cottage scene southern boundary → Forest Path scene northern entrance
- [ ] Wire Cottage scene eastern boundary → Farm scene western entrance
- [ ] Apply `SaveData.PlayerX` / `SaveData.PlayerY` to override default spawn point when starting from a loaded save

**Acceptance Criteria:**

- Fade-to-black transition hides tile-pop during scene load
- Player spawns at correct entry point in new area
- Game state is auto-saved on transition
- No null-reference errors during transition

**Tests Required:**

```csharp
// AreaTransitionServiceTest.cs
[Test] public void Transition_PublishesEvent_OnTrigger() {
    var bus = new EventBus();
    string fromArea = null, toArea = null;
    bus.Subscribe<AreaTransitionEvent>(e => { fromArea = e.FromArea; toArea = e.ToArea; });
    var svc = new AreaTransitionService(bus, new MockSceneLoader(), new MockSaveService());
    svc.TriggerTransition("cottage", "forest_path", spawnId: "north_entrance");
    Assert.AreEqual("cottage", fromArea);
    Assert.AreEqual("forest_path", toArea);
}
[Test] public void Transition_SavesGameState_BeforeTransition() {
    var saveSvc = new MockSaveService();
    var svc = new AreaTransitionService(new EventBus(), new MockSceneLoader(), saveSvc);
    svc.TriggerTransition("cottage", "forest_path", spawnId: "north_entrance");
    Assert.IsTrue(saveSvc.SaveWasCalled);
}
```

---

### S8-02 — Farm Area Scene

**Type:** Level Design / Art  
**Assignee:** Level Designer + Artist  
**Estimate:** 8 points

**Tasks:**

- [ ] Create 5 new tile sprites for farm area (Artist):
  - `tile_crop_young.png` — early crop growth
  - `tile_crop_mature.png` — full grown crop
  - `tile_soil_dry.png` — untended earth
  - `tile_irrigation.png` — water channel edge
  - `tile_hay.png` — straw ground cover
- [ ] Create 3 new props (Artist):
  - `prop_barn.png` — large barn structure (multi-tile)
  - `prop_scarecrow.png` — field scarecrow
  - `prop_plow.png` — wooden plow tool
- [ ] Build `Scene_Farm.tscn` — 40×25 tile grid:
  - Crop fields (6×4 blocks of `tile_crop_mature`)
  - Irrigation ditch (`tile_irrigation` running east-west)
  - Barn in northeast corner
  - Scarecrow at field center
  - Plow near barn entrance
  - Dirt path connecting to Cottage (west) and Forest Path (north)
- [ ] All collision shapes configured
- [ ] Transition zones to Cottage (west) and Forest Path (north)
- [ ] Run `PaletteValidator` against all new sprites

**Acceptance Criteria:**

- Farm scene renders correctly, no tile gaps
- Player can navigate full farm without getting stuck
- Palette compliance passes for all new assets
- Transition zones lead to correct destinations

---

### S8-03 — Forest Path Area Scene

**Type:** Level Design / Art  
**Assignee:** Level Designer + Artist  
**Estimate:** 8 points

**Tasks:**

- [ ] Create 4 new tile sprites (Artist):
  - `tile_forest_floor.png` — moss/leaves ground
  - `tile_forest_shadow.png` — canopy shadow overlay
  - `tile_mud.png` — wet earth path
  - `tile_rock_small.png` — rocky ground
- [ ] Create 4 new props (Artist):
  - `prop_dense_tree.png` — solid oak (impassable)
  - `prop_fallen_log.png` — fallen tree (partial obstacle)
  - `prop_mushrooms.png` — decorative cluster
  - `prop_boulder.png` — large rock (impassable)
- [ ] Build `Scene_ForestPath.tscn` — 35×50 tile grid (taller than wide — north-south corridor):
  - Winding dirt path (`tile_dirt`) running north-south
  - Dense trees flanking path (impassable wall of `prop_dense_tree`)
  - Stream crossing at midpoint (decorative — no swimming)
  - Fallen log as partial obstacle (walk-around or vault placeholder)
  - 2 enemy encounter positions (empty for now — enemies added Sprint 13)
  - Optional item pickup location marker
- [ ] Transition zones: south → Farm, north → City outskirts

**Acceptance Criteria:**

- Forest path is navigable north-to-south without getting stuck
- Dense tree walls are fully impassable
- PaletteValidator passes all new sprites
- Scene renders dense atmospheric forest feel

---

**Sprint 8 Summary:**

| Story                     | Points | Owner                |
| ------------------------- | ------ | -------------------- |
| S8-01 Area Transition Sys | 5      | Lead Dev             |
| S8-02 Farm Area           | 8      | Level Designer + Art |
| S8-03 Forest Path Area    | 8      | Level Designer + Art |
| **Total**                 | **21** |                      |

---

## Sprint 9 — City Area & Mage's Tower

**Duration:** 2 weeks | **Dates:** Week 19–20  
**Goal:** City hub area fully playable with key NPCs and buildings; Mage's Tower interior accessible.

---

### S9-01 — City Tile & Prop Art Pack

**Type:** Art  
**Assignee:** Artist  
**Estimate:** 8 points

**Description:** Create all city-specific visual assets. City must feel architecturally distinct from the cottage area — larger stone buildings, cobblestone streets, market stalls.

**New Tiles:**

| Tile Name         | File                  | Primary Colors       |
| ----------------- | --------------------- | -------------------- |
| Cobblestone       | `tile_cobble.png`     | `#5a5a5a`, `#8b7355` |
| Cobble variation  | `tile_cobble_var.png` | Same + cracks        |
| City stone floor  | `tile_city_stone.png` | `#5a5a5a`, `#3d3d3d` |
| City wall (front) | `tile_city_wall.png`  | `#5a5a5a`, `#8b7355` |
| City roof tile    | `tile_city_roof.png`  | `#3d3d3d`, `#5c3d2e` |
| Market stall top  | `tile_stall_top.png`  | `#8b7355`, `#ffd700` |

**New Props:**

| Prop Name          | File                    |
| ------------------ | ----------------------- |
| Lamp post (lit)    | `prop_lamppost.png`     |
| City fountain      | `prop_fountain.png`     |
| Market stall       | `prop_market_stall.png` |
| Notice board       | `prop_notice_board.png` |
| City gate          | `prop_city_gate.png`    |
| Tower wall section | `prop_tower_wall.png`   |
| Tower door         | `prop_tower_door.png`   |

**Acceptance Criteria:**

- All tiles render correctly as isometric diamonds
- PaletteValidator passes all assets
- City props are clearly larger / more ornate than cottage props

---

### S9-02 — City Area Scene

**Type:** Level Design  
**Assignee:** Level Designer  
**Estimate:** 13 points

**Description:** The city is the largest area — hub for NPCs, shops, and quest information. Build in two sub-scenes loaded together: city exterior + market district.

**Tasks:**

- [ ] Build `Scene_City.tscn` — 60×60 tile grid:
  - **South entrance** — city gate from forest road
  - **Market District** (south-central): 3–4 market stalls, `prop_fountain` at center square
  - **Residential District** (east) — tightly packed buildings with narrow streets
  - **Mage's District** (north) — quieter, tower visible in distance
  - **Guard posts** at gate and key intersections (NPC placeholders)
  - Lamp posts along main street (decorative, no lighting system yet)
  - Notice board at entrance square
- [ ] Transition zones: south → Forest Path, north-inner → Mage's Tower entrance
- [ ] Place NPC anchor points: Merchant (market stall), City Guard (gate), Townsperson × 3
- [ ] All buildings impassable; doors are marked but non-functional (full interiors in future scope)
- [ ] Mage's Tower visually dominates north quarter (multi-tile `prop_tower_wall`)

**Acceptance Criteria:**

- City is fully navigable without getting stuck between props
- Area feels distinct from forest and cottage
- Transition to Forest Path (south) and Mage's Tower (north) work correctly
- NPC anchor nodes present for Sprint 14 dialogue implementation

---

### S9-03 — Mage's Tower Scene

**Type:** Level Design  
**Assignee:** Level Designer  
**Estimate:** 5 points

**Tasks:**

- [ ] Build `Scene_MagesTower.tscn` — two-room layout (exterior + interior):
  - **Exterior approach:** narrow cobblestone path, tower facade, guarded entrance
  - **Interior (ground floor):** bookshelves, alchemy table, fireplace, portal circle on floor
  - `prop_portal_inactive.png` — stone circle with faint glowing runes (placeholder sprite)
  - `prop_bookshelf.png` — prop for tower walls
  - `prop_alchemy_table.png` — prop for tower interior
- [ ] Create required art props (Artist): `prop_portal_inactive.png`, `prop_bookshelf.png`, `prop_alchemy_table.png`
- [ ] Tower interior collision: narrow walkable path between furniture
- [ ] Local Mage NPC anchor point inside tower
- [ ] Transition: exterior ↔ interior (door), interior → City (exit)

**Acceptance Criteria:**

- Tower exterior and interior both render correctly
- Player can navigate interior around obstacles
- Local Mage NPC anchor positioned for Sprint 14

---

**Sprint 9 Summary:**

| Story               | Points | Owner          |
| ------------------- | ------ | -------------- |
| S9-01 City Art Pack | 8      | Artist         |
| S9-02 City Area     | 13     | Level Designer |
| S9-03 Mage's Tower  | 5      | Level Designer |
| **Total**           | **26** |                |

---

## Sprint 10 — Portal Chamber & Full World Navigation

**Duration:** 2 weeks | **Dates:** Week 21–22  
**Goal:** Final game area built; player can walk uninterrupted from Cottage to Portal Chamber;
area transition chain is fully tested end-to-end.

---

### S10-01 — Portal Chamber Scene

**Type:** Level Design / Art  
**Assignee:** Level Designer + Artist  
**Estimate:** 8 points

**Tasks:**

- [ ] Create unique tile set for Portal Chamber (otherworldly aesthetic):
  - `tile_void_floor.png` — dark purple/black cracked stone (`#2a1a4a`, `#1a1a1a`)
  - `tile_rune_floor.png` — glowing rune inlay (`#2a1a4a`, `#9d4edd`)
- [ ] Create props:
  - `prop_portal_active.png` — swirling active portal (4-frame animated sprite)
  - `prop_magic_pillar.png` — ornate dark pillar
  - `prop_chains.png` — decorative hanging chains
  - `prop_child_cage.png` — cage where child is held (boss arena prop)
- [ ] Build `Scene_PortalChamber.tscn` — 30×30 grid:
  - Central raised platform with active portal
  - Surrounding ring of magic pillars
  - Child cage at far end (north)
  - Boss arena: open space for combat
  - Entrance portal (south — where player arrives)
- [ ] Animate `prop_portal_active.png` (AnimatedSprite2D, 4 frames, loop)
- [ ] PaletteValidator pass for all new assets

**Acceptance Criteria:**

- Portal Chamber has a visually distinct, otherworldly feel
- Active portal animation loops smoothly
- Arena is open enough for future combat (Sprint 12/13)
- Transition from Mage's Tower (via portal activation) functional

---

### S10-02 — World Navigation End-to-End Test

**Type:** QA / Integration  
**Assignee:** Lead Developer  
**Estimate:** 5 points

**Tasks:**

- [ ] Write end-to-end navigation test simulating player traversing all areas in sequence:
  ```
  Cottage → Farm → Forest Path → City → Mage's Tower → Portal Chamber
  ```
- [ ] Verify all transition zones fire correct events
- [ ] Verify player spawns at correct point in each area
- [ ] Verify save state updates correctly at each transition
- [ ] Test reverse navigation (walk back through areas)
- [ ] Performance: verify 60 FPS maintained in all areas (profiler check)

**Tests Required:**

```csharp
// WorldNavigationTest.cs
[Test] public void Navigation_Cottage_To_Farm_TransitionWorks() {
    // Simulate player reaching transition zone
    // Assert correct scene loaded + spawn point
}
[Test] public void Navigation_AllAreas_HaveSpawnPoints() {
    var areas = new[] { "cottage", "farm", "forest_path", "city",
                        "mages_tower", "portal_chamber" };
    foreach (var area in areas)
        Assert.IsNotNull(SceneRegistry.GetSpawnPoint(area, "default"),
            $"Missing spawn in: {area}");
}
[Test] public void Navigation_TransitionEvent_ContainsCorrectAreaNames() { ... }
```

---

### S10-03 — Minimap System

**Type:** UI / System  
**Assignee:** Developer  
**Estimate:** 5 points

**Tasks:**

- [ ] Create `MinimapController.cs`:
  - Renders simplified top-down overview of current area (sub-viewport or pre-rendered texture)
  - Player dot (white pixel) shows current position, updating every frame
  - Area name displayed above minimap
  - Toggle show/hide with a keybind (default: `M`)
- [ ] Each area has a `minimap_texture` resource (hand-drawn 64×64 px overview PNG per area) — Artist provides
- [ ] Minimap does NOT show enemies or NPCs (static map only for this scope)
- [ ] Connect to `AreaTransitionEvent` to switch minimap texture on area change

**Acceptance Criteria:**

- Player dot moves in sync with player world position
- Minimap switches texture on area transition
- Minimap can be toggled on/off

**Tests Required:**

```csharp
// MinimapControllerTest.cs
[Test] public void Minimap_PlayerDot_UpdatesOnPositionChange() {
    var ctrl = new MinimapController(mapSize: new Vector2I(64, 64),
                                     worldSize: new Vector2(1920, 1500));
    ctrl.UpdatePlayerPosition(new Vector2(960, 750)); // center of world
    Assert.AreEqual(new Vector2(32, 32), ctrl.PlayerDotPosition);
}
[Test] public void Minimap_OnAreaTransition_SwitchesTexture() {
    var bus = new EventBus();
    var ctrl = new MinimapController(bus);
    bus.Publish(new AreaTransitionEvent("cottage", "farm"));
    Assert.AreEqual("farm", ctrl.CurrentAreaId);
}
```

---

**Sprint 10 Summary:**

| Story                        | Points | Owner                |
| ---------------------------- | ------ | -------------------- |
| S10-01 Portal Chamber        | 8      | Level Designer + Art |
| S10-02 World Navigation Test | 5      | Lead Dev             |
| S10-03 Minimap System        | 5      | Developer            |
| **Total**                    | **18** |                      |

---

## Phase 4 — Gameplay Systems

> **Goal:** Equip the player, give them items, and let them fight.
> These three sprints implement the full gameplay loop beyond exploration.

---

## Sprint 11 — Inventory, Equipment & Items

**Duration:** 2 weeks | **Dates:** Week 23–24  
**Goal:** Player can pick up items, equip weapons and clothing, and see visual changes
on the character sprite. Tinctures (potions) restore health.

---

### S11-01 — Item Data System

**Type:** System  
**Assignee:** Developer  
**Estimate:** 5 points

**Tasks:**

- [ ] Define `ItemDefinition.cs` (pure data, no Godot dependency):
  - `Id` (string), `DisplayName`, `Description`, `ItemType` (enum), `MaxStackSize`, `IconTexturePath`
  - `ItemType` enum: `Weapon`, `Armor`, `Consumable`, `Artifact`, `Key`
- [ ] Create `ItemDatabase.cs` — loads all item definitions from `res://Assets/Data/Items/items.json`
- [ ] Create `InventoryItem.cs` — runtime instance: `ItemDefinition + Quantity`
- [ ] Define initial item catalog (JSON):
  - Weapons: `item_sword_basic`, `item_bow_basic`, `item_shield_basic`
  - Armor: `item_tunic_peasant`, `item_tunic_leather`
  - Consumables: `item_tincture_small` (restores 25 HP), `item_tincture_large` (restores 75 HP)
  - Artifacts: `item_portal_key` (quest item)

**Tests Required:**

```csharp
// ItemDatabaseTest.cs
[Test] public void ItemDatabase_LoadsCorrectly_HasExpectedItems() {
    var db = new ItemDatabase(new MockFileSystem(itemJson: SampleItemJson));
    Assert.IsNotNull(db.GetItem("item_sword_basic"));
    Assert.IsNotNull(db.GetItem("item_tincture_small"));
}
[Test] public void Item_Sword_HasCorrectType() {
    var db = new ItemDatabase(new MockFileSystem(itemJson: SampleItemJson));
    Assert.AreEqual(ItemType.Weapon, db.GetItem("item_sword_basic").ItemType);
}
```

---

### S11-02 — Inventory Manager

**Type:** System  
**Assignee:** Developer  
**Estimate:** 5 points

**Tasks:**

- [ ] Create `InventoryManager.cs` implementing `IInventoryManager`:
  - `AddItem(string itemId, int quantity = 1) → bool` — returns false if full
  - `RemoveItem(string itemId, int quantity = 1) → bool`
  - `HasItem(string itemId, int minQuantity = 1) → bool`
  - `GetItem(string itemId) → InventoryItem?`
  - `GetAllItems() → List<InventoryItem>`
  - `MaxSlots = 20` (configurable via Constants)
  - Publishes `ItemPickedUpEvent` on add, `ItemRemovedEvent` on remove
- [ ] Create `InventoryScreen.tscn` — 4×5 grid of item slots; item icon + quantity overlay
- [ ] Item tooltip on hover: name, description, type
- [ ] "Use" button on consumables, "Equip" button on equippable items
- [ ] Accessible via `inventory` input (pauses game)

**Acceptance Criteria:**

- Adding an item to a full inventory returns false and does not add the item
- Stacking: adding to an existing item stack increments quantity
- Removing more than available quantity returns false

**Tests Required:**

```csharp
// InventoryManagerTest.cs
[Test] public void Inventory_AddItem_Succeeds_WhenSpaceAvailable() {
    var inv = new InventoryManager(maxSlots: 5, new MockItemDatabase());
    bool result = inv.AddItem("item_tincture_small", 1);
    Assert.IsTrue(result);
    Assert.IsTrue(inv.HasItem("item_tincture_small"));
}
[Test] public void Inventory_AddItem_Fails_WhenFull() {
    var inv = new InventoryManager(maxSlots: 1, new MockItemDatabase());
    inv.AddItem("item_sword_basic", 1);
    bool result = inv.AddItem("item_bow_basic", 1); // different item
    Assert.IsFalse(result);
}
[Test] public void Inventory_AddItem_StacksExistingItem() {
    var inv = new InventoryManager(maxSlots: 5, new MockItemDatabase());
    inv.AddItem("item_tincture_small", 1);
    inv.AddItem("item_tincture_small", 2);
    Assert.AreEqual(3, inv.GetItem("item_tincture_small").Quantity);
}
[Test] public void Inventory_RemoveItem_MoreThanAvailable_ReturnsFalse() {
    var inv = new InventoryManager(maxSlots: 5, new MockItemDatabase());
    inv.AddItem("item_tincture_small", 1);
    bool result = inv.RemoveItem("item_tincture_small", 5);
    Assert.IsFalse(result);
}
```

---

### S11-03 — Equipment System

**Type:** System / Gameplay  
**Assignee:** Developer  
**Estimate:** 8 points

**Tasks:**

- [ ] Create `EquipmentSlot` enum: `Weapon`, `OffHand`, `Armor`, `Head`
- [ ] Create `EquipmentManager.cs` implementing `IEquipmentManager`:
  - `Equip(string itemId, EquipmentSlot slot) → bool`
  - `Unequip(EquipmentSlot slot) → InventoryItem?`
  - `GetEquipped(EquipmentSlot slot) → InventoryItem?`
  - `IsSlotOccupied(EquipmentSlot slot) → bool`
  - Validates item type matches slot (sword → Weapon, shield → OffHand, etc.)
  - Publishes `EquipmentChangedEvent(EquipmentSlot, string? newItemId)`
- [ ] Create `PlayerHealthManager.cs` implementing `IPlayerHealthManager`:
  - `MaxHealth`, `CurrentHealth`
  - `TakeDamage(float amount)`
  - `Heal(float amount)`
  - `IsDead → bool`
  - Publishes `PlayerHealthChangedEvent` and `PlayerDiedEvent` via EventBus
- [ ] On `EquipmentChangedEvent`: HUD equipment display updates
- [ ] On `item_tincture_small` used from inventory: calls `PlayerHealthManager.Heal(25f)`
- [ ] Player character sprite layer updates to show equipped weapon icon (overlay sprite)

**Acceptance Criteria:**

- Equipping a sword updates HUD weapon icon within 1 frame
- Equipping a shield in the Weapon slot fails (wrong slot)
- Using a small tincture heals exactly 25 HP (not above max)
- Dying triggers `PlayerDiedEvent`

**Tests Required:**

```csharp
// EquipmentManagerTest.cs
[Test] public void Equipment_EquipSword_ToWeaponSlot_Succeeds() {
    var mgr = new EquipmentManager(new MockItemDatabase(), new MockInventoryManager());
    bool result = mgr.Equip("item_sword_basic", EquipmentSlot.Weapon);
    Assert.IsTrue(result);
    Assert.AreEqual("item_sword_basic", mgr.GetEquipped(EquipmentSlot.Weapon).Definition.Id);
}
[Test] public void Equipment_EquipShield_ToWeaponSlot_Fails() {
    var mgr = new EquipmentManager(new MockItemDatabase(), new MockInventoryManager());
    bool result = mgr.Equip("item_shield_basic", EquipmentSlot.Weapon);
    Assert.IsFalse(result);
}
// PlayerHealthManagerTest.cs
[Test] public void Health_TakeDamage_ReducesHealth() {
    var mgr = new PlayerHealthManager(maxHealth: 100f, new EventBus());
    mgr.TakeDamage(30f);
    Assert.AreEqual(70f, mgr.CurrentHealth, 0.001f);
}
[Test] public void Health_Heal_DoesNotExceedMaxHealth() {
    var mgr = new PlayerHealthManager(maxHealth: 100f, new EventBus());
    mgr.TakeDamage(20f);
    mgr.Heal(50f);
    Assert.AreEqual(100f, mgr.CurrentHealth, 0.001f);
}
[Test] public void Health_AtZero_IsDead() {
    var bus = new EventBus();
    var mgr = new PlayerHealthManager(maxHealth: 100f, bus);
    mgr.TakeDamage(100f);
    Assert.IsTrue(mgr.IsDead);
}
[Test] public void Health_AtZero_PublishesDiedEvent() {
    var bus = new EventBus();
    bool died = false;
    bus.Subscribe<PlayerDiedEvent>(_ => died = true);
    var mgr = new PlayerHealthManager(maxHealth: 100f, bus);
    mgr.TakeDamage(100f);
    Assert.IsTrue(died);
}
```

---

**Sprint 11 Summary:**

| Story                    | Points | Owner     |
| ------------------------ | ------ | --------- |
| S11-01 Item Data System  | 5      | Developer |
| S11-02 Inventory Manager | 5      | Developer |
| S11-03 Equipment System  | 8      | Developer |
| **Total**                | **18** |           |

---

## Sprint 12 — Combat System (Melee & Ranged)

**Duration:** 2 weeks | **Dates:** Week 25–26  
**Goal:** Player can attack with sword (melee) and bow (ranged). Hit detection and
damage calculation work. Combat animations play correctly.

---

### S12-01 — Combat State & Attack Controller

**Type:** Gameplay  
**Assignee:** Lead Developer  
**Estimate:** 8 points

**Tasks:**

- [ ] Extend `PlayerState` enum: add `AttackMelee`, `AttackRanged`, `Blocking`
- [ ] Add valid transitions to `PlayerStateMachine`:
  - `Idle/Walking/Running → AttackMelee` (on `attack` input)
  - `Idle/Walking/Running → AttackRanged` (on `attack_alt` input, if bow equipped)
  - `AttackMelee → Idle` (on animation complete)
  - `Idle/Walking → Blocking` (on `block` input, if shield equipped)
  - `Blocking → Idle` (on `block` released)
- [ ] Add input actions: `attack` (left click / `Z`), `attack_alt` (right click / `X`), `block` (`Q`)
- [ ] Create `AttackController.cs` implementing `IAttackController`:
  - `PerformMeleeAttack(Direction facingDir)`
  - `PerformRangedAttack(Direction facingDir)`
  - `StartBlocking()` / `StopBlocking()`
  - Returns `AttackResult` (hit/miss, target, damage)
- [ ] Melee attack: instant hit detection in front-facing `Area2D` hitbox
- [ ] Ranged attack: spawn `ArrowProjectile.cs` node, travels in facing direction, max range 300px

**Acceptance Criteria:**

- Melee attack only hits targets directly in front of player (within hitbox)
- Ranged attack launches arrow that travels forward and despawns at max range or on hit
- Block input while blocking reduces incoming damage by 50%
- Player cannot perform two attacks simultaneously

**Tests Required:**

```csharp
// AttackControllerTest.cs
[Test] public void MeleeAttack_InFacingDirection_ReturnsHit_WhenTargetInRange() {
    var target = new MockAttackTarget { Position = new Vector2(50f, 0f) }; // right of player
    var ctrl = new AttackController(new MockHitboxDetector(target));
    var result = ctrl.PerformMeleeAttack(Direction.Right);
    Assert.IsTrue(result.Hit);
    Assert.AreEqual(target, result.Target);
}
[Test] public void MeleeAttack_AwayFromTarget_ReturnsMiss() {
    var target = new MockAttackTarget { Position = new Vector2(50f, 0f) }; // right
    var ctrl = new AttackController(new MockHitboxDetector(target));
    var result = ctrl.PerformMeleeAttack(Direction.Left); // attacking wrong direction
    Assert.IsFalse(result.Hit);
}
[Test] public void Block_Active_ReducesDamageByHalf() {
    var ctrl = new AttackController(new MockHitboxDetector());
    ctrl.StartBlocking();
    float damage = ctrl.ApplyIncomingDamage(40f);
    Assert.AreEqual(20f, damage, 0.001f);
}
```

---

### S12-02 — Damage & Health Integration

**Type:** Gameplay  
**Assignee:** Developer  
**Estimate:** 5 points

**Tasks:**

- [ ] Create `DamageCalculator.cs`:
  - `CalculateMeleeDamage(string weaponItemId) → float` — base weapon damage from item data
  - `CalculateRangedDamage(string weaponItemId) → float`
  - `ApplyDamageToTarget(IAttackTarget target, float damage)`
- [ ] Create `IAttackTarget` interface: `TakeDamage(float amount)`, `IsAlive → bool`
- [ ] `PlayerHealthManager` implements `IAttackTarget`
- [ ] Connect: on attack hit → `DamageCalculator.CalculateMeleeDamage` → `target.TakeDamage()`
- [ ] Player death: show "You Died" overlay, "Return to Last Save" button
- [ ] Damage numbers: spawn floating text node at hit position (`+25`, `-30`)

**Tests Required:**

```csharp
// DamageCalculatorTest.cs
[Test] public void DamageCalc_Sword_ReturnsBaseDamage() {
    var calc = new DamageCalculator(new MockItemDatabase());
    float dmg = calc.CalculateMeleeDamage("item_sword_basic");
    Assert.Greater(dmg, 0f);
}
[Test] public void DamageCalc_HitTarget_ReducesTargetHealth() {
    var target = new MockAttackTarget { Health = 100f };
    var calc = new DamageCalculator(new MockItemDatabase());
    calc.ApplyDamageToTarget(target, 30f);
    Assert.AreEqual(70f, target.Health, 0.001f);
}
```

---

### S12-03 — Combat Animation Pack

**Type:** Art  
**Assignee:** Artist  
**Estimate:** 8 points

**Description:** Create all combat animation frames for the player character sprite sheet.

**New Animation Sets (extending player spritesheet):**

- `attack_sword_down` (6 frames), `attack_sword_up` (6), `attack_sword_left` (6), `attack_sword_right` (6)
- `attack_bow_down` (8 frames), `attack_bow_up` (8), `attack_bow_left` (8), `attack_bow_right` (8)
- `block_down` (2 frames), `block_up` (2), `block_left` (2), `block_right` (2)
- `hit_damage` (3 frames — all directions share same hurt flash)
- `death` (6 frames)
- Arrow projectile sprite: `proj_arrow.png` — 4 directional variants (8×3 px each)

**Tasks:**

- [ ] Extend player spritesheet with combat frames
- [ ] Update `AnimatedSprite2D` AnimationLibrary with all new clips
- [ ] Ensure existing animation clips are not broken by spritesheet extension
- [ ] Create `proj_arrow.png` sprite (4 directional variants)
- [ ] PaletteValidator pass on all new frames

**Tests Required:**

```csharp
// SpriteSheetCombatTest.cs
[Test] public void PlayerAnimations_CombatClips_AllExist() {
    var expected = new[] {
        "attack_sword_down", "attack_sword_up", "attack_sword_left", "attack_sword_right",
        "attack_bow_down", "attack_bow_up", "attack_bow_left", "attack_bow_right",
        "block_down", "block_up", "block_left", "block_right",
        "hit_damage", "death"
    };
    var animLibrary = GD.Load<AnimationLibrary>("res://Assets/Animations/player_animations.tres");
    foreach (var clip in expected)
        Assert.IsTrue(animLibrary.HasAnimation(clip), $"Missing: {clip}");
}
```

---

**Sprint 12 Summary:**

| Story                     | Points | Owner     |
| ------------------------- | ------ | --------- |
| S12-01 Combat Controller  | 8      | Lead Dev  |
| S12-02 Damage Integration | 5      | Developer |
| S12-03 Combat Art Pack    | 8      | Artist    |
| **Total**                 | **21** |           |

---

## Sprint 13 — Enemy AI & Combat Encounters

**Duration:** 2 weeks | **Dates:** Week 27–28  
**Goal:** Enemy characters patrol, detect the player, and attack. Forest Path and Portal Chamber
have populated encounters. Boss enemy scripted for Portal Chamber.

---

### S13-01 — Enemy Controller & AI State Machine

**Type:** Gameplay / AI  
**Assignee:** Lead Developer  
**Estimate:** 8 points

**Tasks:**

- [ ] Create `EnemyState` enum: `Idle`, `Patrol`, `Alert`, `Chase`, `Attack`, `Dead`
- [ ] Create `EnemyStateMachine.cs` (reusing generic `StateMachine<TState>`)
- [ ] Create `EnemyController.cs` as `CharacterBody2D` wrapper:
  - `Patrol`: walks between 2 waypoints at `PatrolSpeed = 40f`
  - `Alert`: stops, plays alert animation (triggered by sight/proximity)
  - `Chase`: moves toward player at `ChaseSpeed = 70f`; loses player after 5 seconds out of range
  - `Attack`: stops within `AttackRange`, plays attack animation, deals damage
  - `Dead`: plays death animation, disables collision, despawns after 3 seconds
- [ ] Create `EnemySightDetector.cs` — `RayCast2D` from enemy toward player:
  - `SightRange = 200f`, `SightAngle = 90°` (forward cone)
  - Transitions `Idle/Patrol → Alert` when player enters sight
- [ ] Enemy implements `IAttackTarget` (can take damage)
- [ ] Enemy `AttackRange = 40f`; attacks once per 1.5 seconds

**Acceptance Criteria:**

- Enemy patrols between waypoints until player enters sight range
- Enemy chases player when spotted; loses player when out of range for 5 seconds
- Enemy attacks player when in range; player takes damage
- Enemy death plays animation and despawns cleanly

**Tests Required:**

```csharp
// EnemyControllerTest.cs
[Test] public void Enemy_WhenPlayerInSight_TransitionsToAlert() {
    var enemy = new EnemyController(new MockSightDetector(playerVisible: true), new EventBus());
    enemy.SimulatePhysicsFrame(0.016f);
    Assert.AreEqual(EnemyState.Alert, enemy.StateMachine.CurrentState);
}
[Test] public void Enemy_ChaseState_MovesTowardPlayer() {
    var enemy = new EnemyController(
        new MockSightDetector(playerVisible: true),
        new EventBus(),
        playerPosition: new Vector2(200f, 0f));
    enemy.StateMachine.TransitionTo(EnemyState.Chase);
    enemy.SimulatePhysicsFrame(0.016f);
    Assert.Greater(enemy.Position.X, 0f); // moved toward player
}
[Test] public void Enemy_TakeDamage_BelowZero_TransitionsToDead() {
    var enemy = new EnemyController(new MockSightDetector(), new EventBus());
    enemy.TakeDamage(1000f);
    Assert.AreEqual(EnemyState.Dead, enemy.StateMachine.CurrentState);
}
```

---

### S13-02 — Enemy Art Pack

**Type:** Art  
**Assignee:** Artist  
**Estimate:** 8 points

**Description:** Create sprites for two enemy types: Forest Bandit (melee) and Tower Sentinel (ranged). Boss enemy: The Dark Mage.

**Enemy Sprites Required:**

| Enemy            | File                        | Animations                                                  |
| ---------------- | --------------------------- | ----------------------------------------------------------- |
| Forest Bandit    | `enemy_bandit_sheet.png`    | idle (2f), walk (4f per dir), attack (4f), death (4f)       |
| Tower Sentinel   | `enemy_sentinel_sheet.png`  | idle (2f), walk (4f per dir), attack_shoot (6f), death (4f) |
| Dark Mage (Boss) | `enemy_boss_mage_sheet.png` | idle (4f), cast (8f), teleport (6f), death (8f)             |
| Enemy projectile | `proj_dark_bolt.png`        | 4-frame animated bolt (for boss + sentinel)                 |

**All sprites use approved palette. PaletteValidator must pass.**

---

### S13-03 — Populate Forest Path & Portal Chamber Encounters

**Type:** Level Design  
**Assignee:** Level Designer  
**Estimate:** 5 points

**Tasks:**

- [ ] Place 2× `EnemyController` (Forest Bandit type) in `Scene_ForestPath.tscn`:
  - Patrol path positions configured in editor
  - Positioned at mid-path and near north exit
- [ ] Place 4× `EnemyController` (Tower Sentinel) in `Scene_PortalChamber.tscn`:
  - Guard corners of boss arena
- [ ] Place `BossController.cs` (Dark Mage) at north of Portal Chamber:
  - Uses same `EnemyStateMachine` with additional `CastSpell` state
  - Boss has 3 attack phases keyed to health thresholds (100%, 60%, 30%)
  - On death: triggers child rescue cutscene via EventBus

**Tests Required:**

```csharp
// BossControllerTest.cs
[Test] public void Boss_At60PercentHealth_EntersPhase2() {
    var boss = new BossController(new MockEventBus());
    boss.TakeDamage(400f); // 400 of 1000 HP
    Assert.AreEqual(BossPhase.Phase2, boss.CurrentPhase);
}
[Test] public void Boss_OnDeath_PublishesChildRescuedEvent() {
    var bus = new EventBus();
    bool rescued = false;
    bus.Subscribe<ChildRescuedEvent>(_ => rescued = true);
    var boss = new BossController(bus);
    boss.TakeDamage(1000f);
    Assert.IsTrue(rescued);
}
```

---

**Sprint 13 Summary:**

| Story                      | Points | Owner          |
| -------------------------- | ------ | -------------- |
| S13-01 Enemy AI            | 8      | Lead Dev       |
| S13-02 Enemy Art Pack      | 8      | Artist         |
| S13-03 Populate Encounters | 5      | Level Designer |
| **Total**                  | **21** |                |

---

## Phase 5 — Narrative & Audio

---

## Sprint 14 — Story Content, NPC Dialogue & Cutscenes

**Duration:** 2 weeks | **Dates:** Week 29–30  
**Goal:** Complete narrative experience — all NPC dialogues written and wired, intro cutscene plays,
all 5 main quest triggers functional. Player can complete the full quest chain.

---

### S14-01 — Full NPC Dialogue Content

**Type:** Narrative / Content  
**Assignee:** Developer + (Narrative Designer / Lead Dev)  
**Estimate:** 8 points

**Tasks:**

- [ ] Write and implement complete dialogue trees in JSON for:
  - **Wife** (Act 1): kidnapping reaction, send player to city, reunion lines
  - **Local Mage** (Act 2–3): portal exposition, quest condition, portal activation
  - **Merchant** (City, optional): sells tinctures and basic equipment
  - **City Guard** (City gate): misdirection, hints about mage's tower location
  - **Townsperson × 2** (optional flavour): lore hints about portals and royal bloodline
- [ ] Wire dialogue triggers to NPC anchor points in all scenes
- [ ] Implement branching: Local Mage dialogue tree has 3 player response options
- [ ] Quest triggers on dialogue completion:
  - Wife Act 1 complete → start `q_seek_mage`
  - Mage dialogue complete → start `q_portal_prerequisite`
  - Mage activates portal → start `q_enter_portal`

**Acceptance Criteria:**

- All listed NPCs have at least one complete conversation
- Dialogue trees branch correctly based on player choices
- Quest progression advances correctly after each dialogue

**Tests Required:**

```csharp
// NpcDialogueIntegrationTest.cs
[Test] public void Wife_DialogueComplete_StartsSeekMageQuest() {
    var bus = new EventBus();
    string startedQuest = null;
    bus.Subscribe<QuestStartedEvent>(e => startedQuest = e.QuestId);
    var ctrl = new DialogueController(new MockDialogueService(), bus, new MockQuestService());
    ctrl.StartConversation("wife");
    ctrl.AdvanceToEnd();
    Assert.AreEqual("q_seek_mage", startedQuest);
}
[Test] public void Mage_PortalDialogue_UnlocksPortalTransition() {
    var transitionSvc = new MockAreaTransitionService();
    var ctrl = new DialogueController(new MockDialogueService(), transitionSvc: transitionSvc);
    ctrl.StartConversation("local_mage");
    ctrl.SelectOption("activate_portal");
    ctrl.AdvanceToEnd();
    Assert.IsTrue(transitionSvc.PortalTransitionUnlocked);
}
```

---

### S14-02 — Intro Cutscene (Mage Attack)

**Type:** Narrative / Content  
**Assignee:** Developer  
**Estimate:** 5 points

**Tasks:**

- [ ] Implement intro cutscene using `CutsceneSequencer` (framework from Sprint 7):
  1. `FadeStep(1.5s, black → clear)` — fade in from black
  2. `DialogueStep("wife_intro_01")` — wife greets player
  3. `CameraPanStep(portal_position, 2s)` — camera drifts to portal area
  4. `WaitStep(1s)`
  5. `PublishEventStep(MageAppearsEvent)` — dark mage NPC spawns
  6. `DialogueStep("mage_intro_01")` — mage one-liner
  7. `WaitStep(0.5s)`
  8. `PublishEventStep(PlayerKnockedOutEvent)` — screen flash
  9. `FadeStep(0.8s, white)` — hit flash
  10. `FadeStep(0.5s, black)` — fade out
  11. `WaitStep(2s)` — unconscious pause
  12. `FadeStep(1s, black → clear)` — fade in (morning after)
  13. `PublishEventStep(QuestStartedEvent("q_kidnapped"))` — first quest starts
- [ ] Cutscene plays automatically on new game start, never on continue/load
- [ ] Skip button: any key press skips to quest `q_kidnapped` start

**Acceptance Criteria:**

- Cutscene plays start-to-finish in correct order
- Skipping jumps directly to gameplay with `q_kidnapped` active
- Cutscene does not repeat on death/reload

**Tests Required:**

```csharp
// IntroCutsceneTest.cs
[Test] public void IntroCutscene_StepCount_IsThirteen() {
    var cutscene = CutsceneFactory.CreateIntro(new MockEventBus());
    Assert.AreEqual(13, cutscene.StepCount);
}
[Test] public void IntroCutscene_OnComplete_QuestKidnappedIsActive() {
    var questSvc = new MockQuestService();
    var cutscene = CutsceneFactory.CreateIntro(new MockEventBus(), questSvc);
    cutscene.PlayToEnd();
    Assert.AreEqual(QuestState.Active, questSvc.GetQuestState("q_kidnapped"));
}
[Test] public void IntroCutscene_OnSkip_ImmedatelyStartsQuest() {
    var questSvc = new MockQuestService();
    var cutscene = CutsceneFactory.CreateIntro(new MockEventBus(), questSvc);
    cutscene.Skip();
    Assert.AreEqual(QuestState.Active, questSvc.GetQuestState("q_kidnapped"));
}
```

---

### S14-03 — Ending Cutscene & Credits Flow

**Type:** Narrative / Content  
**Assignee:** Developer  
**Estimate:** 5 points

**Tasks:**

- [ ] Implement ending cutscene triggered by `ChildRescuedEvent` from boss death:
  1. `FadeStep(0.5s, black)`
  2. `FadeStep(1s, black → clear)` — Portal Chamber in ruin
  3. `DialogueStep("child_reunion_01")` — child's first words after rescue
  4. `DialogueStep("player_reunion_01")` — player response
  5. `CameraPanStep(escape_portal, 1.5s)`
  6. `FadeStep(1.5s, white)` — escape flash
  7. `FadeStep(2s, white → clear)` — cottage exterior, dawn
  8. `DialogueStep("wife_ending_01")` — family reunion
  9. `WaitStep(3s)`
  10. `FadeStep(2s, black)` — final fade
  11. Load `CreditsScreen.tscn`
- [ ] Credits screen displays correctly after ending (Sprint 5 reuse)
- [ ] "New Game" available from credits return → main menu

**Acceptance Criteria:**

- Ending plays automatically after boss death
- Credits load after final fade
- Cannot be skipped accidentally

---

**Sprint 14 Summary:**

| Story                       | Points | Owner                 |
| --------------------------- | ------ | --------------------- |
| S14-01 NPC Dialogue Content | 8      | Developer + Narrative |
| S14-02 Intro Cutscene       | 5      | Developer             |
| S14-03 Ending Cutscene      | 5      | Developer             |
| **Total**                   | **18** |                       |

---

## Sprint 15 — Audio System & Day/Night Cycle

**Duration:** 2 weeks | **Dates:** Week 31–32  
**Goal:** Every area has background music; key game events have sound effects;
day/night palette cycle shifts atmosphere. These are the last feature systems before Polish.

---

### S15-01 — Audio Manager

**Type:** System  
**Assignee:** Developer  
**Estimate:** 8 points

**Tasks:**

- [ ] Create `AudioManager.cs` implementing `IAudioManager`:
  - `PlayMusic(string trackId, bool loop = true, float fadeInDuration = 1f)`
  - `StopMusic(float fadeOutDuration = 1f)`
  - `PlaySfx(string sfxId, float volume = 1f)`
  - `SetMusicVolume(float volume)` / `SetSfxVolume(float volume)` (0.0–1.0)
  - Cross-fades between tracks (old track fades out while new fades in)
- [ ] Add audio volume settings to `UserConfig` and Settings Screen:
  - **Master Volume** slider (0–100%)
  - **Music Volume** slider (0–100%)
  - **SFX Volume** slider (0–100%)
- [ ] Audio bus structure in Godot: `Master → Music`, `Master → SFX`
- [ ] Subscribe to `AreaTransitionEvent` → switch background music for new area
- [ ] Audio placeholder files (silence `.ogg`): one per area + key SFX events

**Per-area Music Tracks (placeholder filenames):**

| Area           | Track File                   |
| -------------- | ---------------------------- |
| Main Menu      | `music_main_menu.ogg`        |
| Cottage        | `music_cottage_pastoral.ogg` |
| Farm           | `music_cottage_pastoral.ogg` |
| Forest Path    | `music_forest_tense.ogg`     |
| City           | `music_city_ambient.ogg`     |
| Mage's Tower   | `music_mage_tower.ogg`       |
| Portal Chamber | `music_boss_encounter.ogg`   |
| Boss Fight     | `music_boss_encounter.ogg`   |
| Ending         | `music_ending_theme.ogg`     |

**Key SFX (placeholder files):**

- `sfx_footstep_grass.ogg`, `sfx_footstep_stone.ogg`
- `sfx_sword_swing.ogg`, `sfx_sword_hit.ogg`
- `sfx_bow_draw.ogg`, `sfx_arrow_release.ogg`
- `sfx_arrow_hit.ogg`
- `sfx_item_pickup.ogg`
- `sfx_door_open.ogg`
- `sfx_ui_click.ogg`, `sfx_ui_hover.ogg`
- `sfx_portal_activate.ogg`
- `sfx_player_hurt.ogg`, `sfx_player_death.ogg`

**Acceptance Criteria:**

- Music cross-fades smoothly between areas (no pop/click)
- SFX plays at correct moment for each action
- Volume settings applied in real-time from Settings Screen
- Volume settings persist across sessions (saved to UserConfig)

**Tests Required:**

```csharp
// AudioManagerTest.cs
[Test] public void AudioManager_SetMusicVolume_ClampsTo0To1() {
    var mgr = new AudioManager(new MockAudioBus());
    mgr.SetMusicVolume(1.5f);
    Assert.AreEqual(1f, mgr.MusicVolume, 0.001f);
    mgr.SetMusicVolume(-0.5f);
    Assert.AreEqual(0f, mgr.MusicVolume, 0.001f);
}
[Test] public void AudioManager_OnAreaTransition_SwitchesMusicTrack() {
    var bus = new EventBus();
    var mgr = new AudioManager(new MockAudioBus(), bus);
    bus.Publish(new AreaTransitionEvent("cottage", "forest_path"));
    Assert.AreEqual("music_forest_tense", mgr.CurrentTrackId);
}
[Test] public void AudioManager_PlaySfx_WithUnknownId_DoesNotThrow() {
    var mgr = new AudioManager(new MockAudioBus());
    Assert.DoesNotThrow(() => mgr.PlaySfx("sfx_nonexistent"));
}
```

---

### S15-02 — Day/Night Cycle

**Type:** Gameplay / Visual  
**Assignee:** Developer  
**Estimate:** 5 points

**Tasks:**

- [ ] Create `DayNightController.cs`:
  - `TimeOfDay` float (0.0 = midnight, 0.5 = noon, 1.0 = midnight)
  - `CycleDurationSeconds = 600f` (10 real minutes per full day, configurable in Constants)
  - `IsNight → bool` (true when `TimeOfDay < 0.25` or `> 0.75`)
- [ ] Apply day/night via full-screen `ColorRect` with shader:
  - Day: `ColorRect` alpha = 0 (transparent — full bright palette)
  - Dusk/Dawn: `ColorRect` alpha = 0.3, color = `#ff6b00` (warm orange tint)
  - Night: `ColorRect` alpha = 0.55, color = `#1a1a4a` (deep purple-blue overlay)
- [ ] Smooth interpolation between states using Tween (no hard cut)
- [ ] Save `TimeOfDay` in `SaveData` (persists through sessions)
- [ ] City lamp posts emit point light when `IsNight = true` (Godot `PointLight2D`, radius 80px)

**Acceptance Criteria:**

- Daytime renders with no overlay (bright, normal palette)
- Nighttime clearly darker with blue-purple tint visible
- Transition between states is smooth (no flicker)
- Time is saved and resumes from correct point on load

**Tests Required:**

```csharp
// DayNightControllerTest.cs
[Test] public void DayNight_AtNoon_IsNotNight() {
    var ctrl = new DayNightController();
    ctrl.SetTimeOfDay(0.5f);
    Assert.IsFalse(ctrl.IsNight);
}
[Test] public void DayNight_AtMidnight_IsNight() {
    var ctrl = new DayNightController();
    ctrl.SetTimeOfDay(0.0f);
    Assert.IsTrue(ctrl.IsNight);
}
[Test] public void DayNight_Advances_WithDelta() {
    var ctrl = new DayNightController(cycleDuration: 100f);
    float before = ctrl.TimeOfDay;
    ctrl.SimulateTimePassed(50f); // half cycle
    Assert.AreEqual(0.5f, ctrl.TimeOfDay, 0.001f);
}
[Test] public void DayNight_OverFullCycle_WrapsAround() {
    var ctrl = new DayNightController(cycleDuration: 100f);
    ctrl.SetTimeOfDay(0.9f);
    ctrl.SimulateTimePassed(20f); // would go to 1.1 — should wrap to 0.1
    Assert.AreEqual(0.1f, ctrl.TimeOfDay, 0.001f);
}
```

---

**Sprint 15 Summary:**

| Story                  | Points | Owner     |
| ---------------------- | ------ | --------- |
| S15-01 Audio Manager   | 8      | Developer |
| S15-02 Day/Night Cycle | 5      | Developer |
| **Total**              | **13** |           |

---

## Phase 6 — Polish & Release

---

## Sprint 16 — Visual Polish, Performance & Release Build

**Duration:** 2 weeks | **Dates:** Week 33–34  
**Goal:** Game is stable, performant, visually polished, and ready for distribution.
Full end-to-end playthrough verified on all three platforms.

---

### S16-01 — NPC Character Art Pack

**Type:** Art  
**Assignee:** Artist  
**Estimate:** 8 points

**Description:** Create sprites for all story-critical NPC characters.

| NPC             | File                       | Animations                            |
| --------------- | -------------------------- | ------------------------------------- |
| Wife            | `npc_wife_sheet.png`       | idle (2f), talk (4f), distressed (4f) |
| Child           | `npc_child_sheet.png`      | idle (2f), scared (2f)                |
| Local Mage      | `npc_local_mage_sheet.png` | idle (4f), talk (4f), casting (6f)    |
| Merchant        | `npc_merchant_sheet.png`   | idle (2f), talk (2f)                  |
| City Guard      | `npc_guard_sheet.png`      | idle (2f), patrol (4f per dir)        |
| Dark Mage (NPC) | `npc_dark_mage_sheet.png`  | appear (4f) — intro cutscene only     |

**All sprites palette-compliant. PaletteValidator must pass.**

---

### S16-02 — Visual Effects Polish

**Type:** Visual  
**Assignee:** Developer  
**Estimate:** 5 points

**Tasks:**

- [ ] Screen-edge vignette shader (subtle `ColorRect` radial gradient, always on)
- [ ] Camera trauma/shake effect on player receiving damage: `CameraController.ApplyTrauma(float intensity)`
- [ ] Portal particle effect on `prop_portal_active`: `CPUParticles2D` with purple/gold particles
- [ ] Footstep dust particle: small `CPUParticles2D` at player feet on `Running` state
- [ ] Item pickup sparkle effect: 3-frame particle burst at pickup position
- [ ] Damage number text effect: float up and fade out over 1 second

**Tests Required:**

```csharp
// CameraTraumaTest.cs
[Test] public void Camera_Trauma_AddsOffset_ToPosition() {
    var cam = new CameraController();
    cam.ApplyTrauma(0.5f);
    cam.SimulatePhysicsFrame(0.016f);
    Assert.AreNotEqual(Vector2.Zero, cam.TraumaOffset);
}
[Test] public void Camera_Trauma_DecaysToZero_OverTime() {
    var cam = new CameraController();
    cam.ApplyTrauma(0.5f);
    for (int i = 0; i < 120; i++) cam.SimulatePhysicsFrame(0.016f); // 2 seconds
    Assert.AreEqual(0f, cam.TraumaIntensity, 0.001f);
}
```

---

### S16-03 — Full Game Coverage Audit

**Type:** QA / TDD  
**Assignee:** Lead Developer  
**Estimate:** 5 points

**Tasks:**

- [ ] Run full coverage report: `dotnet test --settings coverage.runsettings --collect:"XPlat Code Coverage"`
- [ ] All production C# classes ≥ 90% line coverage
- [ ] Write any missing tests to bring below-threshold classes up to gate
- [ ] Audit test quality: all assertions test behavior, not just execution
- [ ] Update `docs/test-coverage-report.md` with final numbers
- [ ] Final PR: all tests green in CI before merge to `main`

**Acceptance Criteria:**

- Overall coverage ≥ 90% (targeting > 95%)
- CI fully green
- All 5 main quests verified completable in manual walkthrough

---

### S16-04 — End-to-End Playthrough Validation

**Type:** QA / Integration  
**Assignee:** Lead Developer + QA  
**Estimate:** 5 points

**Tasks:**

- [ ] Automated integration test: simulate full quest chain via EventBus + QuestService
- [ ] Manual walkthrough checklist — verified on Windows, Linux, macOS:
  - [ ] Main Menu loads correctly
  - [ ] New Game starts intro cutscene
  - [ ] Player can reach City from Cottage without softlock
  - [ ] Talking to Local Mage progresses quest
  - [ ] All 5 quests completable in sequence
  - [ ] Defeating boss triggers ending cutscene
  - [ ] Credits play after ending
  - [ ] Save/load works at every area
  - [ ] Settings persist across restarts
  - [ ] Frame rate stable at 60 FPS in all areas
  - [ ] Audio plays in every area (music + key SFX)
  - [ ] Day/night cycle advances visibly
  - [ ] No unhandled exceptions in 2-hour play session

---

### S16-05 — Release Build & Distribution

**Type:** Build / DevOps  
**Assignee:** DevOps / Lead Developer  
**Estimate:** 3 points

**Tasks:**

- [ ] Increment project version to `1.0.0` in `project.godot`
- [ ] Update `export_presets.cfg` with release settings (strip debug, optimize binary)
- [ ] CI release pipeline: tag `v1.0.0` → triggers export + upload to GitHub Releases
- [ ] Verify all 3 platform builds run cleanly from a clean machine
- [ ] Update `README.md` with download links and system requirements

**Acceptance Criteria:**

- All 3 platform executables produced and attached to GitHub Release
- No editor dependency in any platform build
- System requirements documented in README

---

**Sprint 16 Summary:**

| Story                         | Points | Owner         |
| ----------------------------- | ------ | ------------- |
| S16-01 NPC Character Art      | 8      | Artist        |
| S16-02 Visual Effects Polish  | 5      | Developer     |
| S16-03 Coverage Audit         | 5      | Lead Dev      |
| S16-04 Playthrough Validation | 5      | Lead Dev + QA |
| S16-05 Release Build          | 3      | DevOps        |
| **Total**                     | **26** |               |

---

## Overall Sprint Summary

| Sprint    | Duration     | Phase                 | Focus                                    | Story Points   | Dependency            |
| --------- | ------------ | --------------------- | ---------------------------------------- | -------------- | --------------------- |
| Sprint 0  | Week 1–2     | Demo                  | Project foundation, CI, architecture     | 18             | ✅ Complete           |
| Sprint 1  | Week 3–4     | Demo                  | Isometric engine, camera, palette        | 24             | ✅ Complete           |
| Sprint 2  | Week 5–6     | Demo                  | Player controller & movement             | 19             | ✅ Complete           |
| Sprint 3  | Week 7–8     | Demo                  | Pixel art assets & cottage scene         | 27             | ✅ Complete           |
| Sprint 4  | Week 9–10    | Demo                  | HUD, integration, export                 | 18             | ✅ Complete           |
| Sprint 5  | Week 11–12   | Phase 1 — Persistence | Main menu, settings, save/load           | 26             | Sprint 4 complete     |
| Sprint 6  | Week 13–14   | Phase 1 — Foundation  | Full HUD, dialogue system, NPC framework | 27             | Sprint 5 complete     |
| Sprint 7  | Week 15–16   | Phase 2 — Quest/Story | Event bus, quest system, cutscene        | 23             | Sprint 6 complete     |
| Sprint 8  | Week 17–18   | Phase 3 — World       | Farm, Forest Path, area transitions      | 21             | Sprint 7 complete     |
| Sprint 9  | Week 19–20   | Phase 3 — World       | City area, Mage's Tower                  | 26             | Sprint 8 complete     |
| Sprint 10 | Week 21–22   | Phase 3 — World       | Portal Chamber, world nav, minimap       | 18             | Sprint 9 complete     |
| Sprint 11 | Week 23–24   | Phase 4 — Gameplay    | Inventory, equipment, health             | 18             | Sprint 7, 10 complete |
| Sprint 12 | Week 25–26   | Phase 4 — Gameplay    | Combat system (melee + ranged)           | 21             | Sprint 11 complete    |
| Sprint 13 | Week 27–28   | Phase 4 — Gameplay    | Enemy AI & combat encounters             | 21             | Sprint 12 complete    |
| Sprint 14 | Week 29–30   | Phase 5 — Narrative   | Story content, NPC dialogue, cutscenes   | 18             | Sprint 13, 7 complete |
| Sprint 15 | Week 31–32   | Phase 5 — Audio       | Audio system & day/night cycle           | 13             | Sprint 14 complete    |
| Sprint 16 | Week 33–34   | Phase 6 — Release     | Polish, coverage audit, release build    | 26             | Sprint 15 complete    |
| **Total** | **34 weeks** |                       |                                          | **364 points** |                       |

_(Demo: 106 points · Full game additional: 258 points · Cumulative: 364 points)_

---

## Parallel Tracks

| Track        | Sprint 5–7                        | Sprint 8–10                             | Sprint 11–13                     | Sprint 14–16                |
| ------------ | --------------------------------- | --------------------------------------- | -------------------------------- | --------------------------- |
| Developer    | Menu/Settings/Save, HUD, Dialogue | Area transitions, minimap               | Inventory, combat, health        | Cutscenes, audio, day/night |
| Artist       | NPC placeholder sprites           | Farm + forest + city tile/prop packs    | Combat animations, enemy sprites | NPC character art, polish   |
| Level Design | —                                 | Farm, Forest Path, City, Tower, Chamber | Populate enemy encounters        | —                           |
| QA           | Manual regression each sprint     | Navigation walkthrough                  | Combat encounter testing         | Full playthrough audit      |

---

## Risks & Mitigations

| Risk                                                  | Likelihood | Impact | Mitigation                                                      |
| ----------------------------------------------------- | ---------- | ------ | --------------------------------------------------------------- |
| Narrative content scope creep (too much dialogue)     | High       | Medium | Strict scope per Sprint 14: 5 NPCs × defined line count only    |
| Boss combat difficulty balancing takes excessive time | Medium     | Medium | Implement difficulty multiplier in Constants; tune in Sprint 16 |
| City area is too large to build in single sprint      | Medium     | High   | Prioritize navigable skeleton first; decorative details in S16  |
| Audio assets (music tracks) not ready by Sprint 15    | Medium     | Medium | Use placeholder `.ogg` files; system functional with silence    |
| Save/load data schema changes break existing saves    | Medium     | Low    | Version field in `SaveData`; migration handled in `SaveService` |
| Coverage gate fails due to GUT scene tests excluded   | Low        | Low    | `coverage.runsettings` already handles exclusions (Sprint 4)    |
| Cross-platform audio format incompatibility           | Low        | Medium | Use `.ogg` exclusively; validated in CI export step             |

---

## Definition of Ready (Per Story)

A story is ready to be picked up when:

- [ ] Acceptance criteria are written and understood by the developer
- [ ] All required asset paths/names agreed upon with Artist
- [ ] Dependencies on other stories are satisfied (prior sprint complete)
- [ ] Test stubs are listed in the document
- [ ] No blocker in CI (`main` branch is green)

---

## Team Agreements (Unchanged from Demo)

1. **No code merged without passing tests.** CI is mandatory gate.
2. **Tests written before implementation** (TDD cycle). PRs must include test + code together.
3. **Coverage never goes below 90%.** Tracked in every PR.
4. **Palette compliance is automatic.** `PaletteValidator` CI step rejects non-compliant assets.
5. **No magic numbers.** All constants live in `Constants.cs`.
6. **Feature branches** for all work. `main` always holds the latest passing build.
7. **All changes** must go through pull requests. No direct pushes to `main`.

---

**Document Status:** Ready for Sprint 5 kickoff
