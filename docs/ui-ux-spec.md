# UI/UX Specification Document

## "Echo Forest"

**Document Version:** 1.0  
**Last Updated:** April 1, 2026

---

## 1. Overview

This document defines all user interface (UI) and user experience (UX) elements for Echo Forest, including menus, HUD, dialogue systems, and player feedback.

---

## 2. Design Philosophy

### 2.1 UX Principles

- **Clarity:** Information displayed clearly without clutter
- **Consistency:** Uniform design language across all screens
- **Accessibility:** Readable text, colorblind-friendly elements
- **Immersion:** UI enhances medieval atmosphere without breaking it
- **Responsiveness:** Player actions get immediate feedback
- **Minimal Intrusion:** HUD doesn't block important gameplay area

### 2.2 Interaction Style

- **Medieval Aesthetic:** UI design reflects in-world craftsmanship
- **Practical Design:** Functional, not ornate (peasant perspective)
- **Color Palette Adherence:** All UI uses approved color palette
- **Intuitive Controls:** Minimal learning curve required

---

## 3. Main Menu

### 3.1 Main Menu Screens

**Menu Layout:**

- [ ] Logo/title placement: Top center or full background
- [ ] Button arrangement: Vertical stack, centered
- [ ] Background: [Specify image - cottage/peaceful scene suggested]
- [ ] Music: [List background music track name]

### 3.2 Main Menu Options

| Button        | Function                | Next Screen         |
| ------------- | ----------------------- | ------------------- |
| New Game      | Start fresh playthrough | Game Start Cutscene |
| Continue Game | Load last save          | Return to gameplay  |
| Load Game     | Load specific save file | Load Game Screen    |
| Settings      | Adjust options          | Settings Screen     |
| Credits       | View game credits       | Credits Screen      |
| Exit          | Quit game               | OS                  |

### 3.3 Menu Button Design

**Visual States:**

- [ ] Idle: Standard appearance
- [ ] Hover: Highlight/glow effect
- [ ] Pressed: Slight depression animation
- [ ] Disabled: Greyed out (if applicable)

**Button Styling:**

- **Color Scheme:** [Specify colors from palette]
  - Background: [Color hex]
  - Text: [Color hex]
  - Hover: [Color hex]

- **Font:** [From Art Bible]
  - Size: [Specify]
  - Weight: [Bold / Regular]
  - Style: [Serif / Sans-serif]

- **Dimensions:**
  - Width: [Specify in pixels or percentage]
  - Height: [Specify]
  - Padding: [Specify spacing]

**Sound Effects:**

- [ ] Hover sound: [Specify audio asset]
- [ ] Click sound: [Specify audio asset]
- [ ] Select sound: [Specify audio asset]

---

## 4. Game HUD (Heads-Up Display)

### 4.1 HUD Layout

**Screen Composition:**

```
┌─────────────────────────────────┐
│  [Quest Log]      [Health Bar]  │
│  [Objective]      [Equipment]   │
│         [GAMEPLAY AREA]          │
│         [PLAYER CHARACTER]       │
│                                  │
│  [Inventory] [Equipment] [Map]   │
└─────────────────────────────────┘
```

**HUD Placement:**

- [ ] Health bar: [Top-left / Top-right / Bottom-left]
- [ ] Quest objective: [Top-left / Top-center]
- [ ] Equipment display: [Top-right]
- [ ] Inventory access: [Bottom-left]
- [ ] Minimap: [Bottom-right / Corner]
- [ ] Interaction prompts: [Center-bottom / Dynamic]

### 4.2 Health Bar

**Visual Design:**

- **Position:** [Top-left corner, specify offset]
- **Size:** [Width x Height in pixels]
- **Style:**
  - Full health: [Color hex] (green suggested)
  - Damaged: [Color hex] (yellow suggested)
  - Critical: [Color hex] (red)
  - Background: [Dark container color]

**Components:**

- [ ] Health bar (4-edge border)
- [ ] Heart icon or indicator
- [ ] Numerical health display: [X/Y format]
- [ ] Animation on damage: [Flash / Shake / Both]

**Mechanics:**

- [ ] Regenerates health: [Yes/No]
- [ ] Regeneration rate: [If yes, specify speed]
- [ ] Damage feedback: [Specify visual/audio feedback]

### 4.3 Equipment Display

**Visual Design:**

- **Position:** [Top-right corner]
- **Size:** [Grid of equipment slots]

**Equipment Slots Shown:**

- [ ] Current weapon (sword/bow/shield icon)
- [ ] Armor/clothing visual representation
- [ ] Quick-access item (if applicable)

**Interaction:**

- [ ] Click to access inventory: [Yes/No]
- [ ] Hover tooltip: [Shows equipment stats]
- [ ] Drag-and-drop support: [Yes/No]

**Styling:**

- [ ] Slot background: [Color]
- [ ] Border: [Color and thickness]
- [ ] Highlighting: [Color on selection/change]
- [ ] Animation: [Smooth fade or snap change]

### 4.4 Quest/Objective Display

**Objective Marker:**

**Position:** [Top-left, below health]

**Content Display:**

- [ ] Current quest name
- [ ] Current objective text
- [ ] Progress indicator (if multi-step): [1/3]
- [ ] Optional: Quest giver name

**Styling:**

- [ ] Font: [From Art Bible]
- [ ] Size: [Readable but not intrusive]
- [ ] Background: [Semi-transparent box]
- [ ] Text color: [From palette]

**Update Behavior:**

- [ ] Fades in when new objective
- [ ] Stays on screen for [X] seconds
- [ ] Clickable for details: [Yes/No]

### 4.5 Minimap

**Position:** [Bottom-right corner]

**Content:**

- [ ] Small world layout representation
- [ ] Player position marker
- [ ] NPCs: [If shown, specify icon]
- [ ] Enemies: [If shown, specify icon]
- [ ] Points of interest: [If shown, specify]
- [ ] Quest markers: [If shown, specify]

**Design:**

- [ ] Style: [Simplified, isometric-looking map]
- [ ] Size: [Specify dimensions - suggest 150x150px]
- [ ] Zoom level: [Fixed or adjustable]
- [ ] Color scheme: [Use palette, high contrast for clarity]
- [ ] Transparency: [Semi-transparent background]

**Functionality:**

- [ ] Click to expand full map: [Yes/No]
- [ ] Shows current location name: [Yes/No]
- [ ] Real-time update: [Yes]

### 4.6 Interaction Prompts

**Position:** [Center-bottom of screen, above HUD]

**Behavior:**

- [ ] Appears when near interactive object
- [ ] Disappears when distance exceeded
- [ ] Animation: [Fade in/out]

**Text Display:**

- [ ] Prompt text: "[SPACE] to interact" format
- [ ] Object name: Shows what player is about to interact with
- [ ] Font: [Readable size, maybe slightly larger than HUD text]

**Styling:**

- [ ] Background: [Light box behind text]
- [ ] Text color: [From palette]
- [ ] Keyboard button indicator: [Highlighted in different color]

---

## 5. Inventory System

### 5.1 Inventory Screen

**Access Method:**

- [ ] Hotkey: [Specify - suggest 'I' key]
- [ ] Menu navigation: [Alternative access]
- [ ] Pause level when opened: [Yes/No - recommended Yes]

**Layout:**

```
┌──────────────────────────────┐
│    INVENTORY               X │
├──────────────────────────────┤
│ Equipment Slots | Item Grid  │
│ [Head] [Body]   │ [Sword]    │
│ [Legs] [Feet]   │ [Bow]      │
│ [Hand] [Offhand]│ [Shield]   │
│                 │ [Potion]   │
│                 │ [Potion]   │
└──────────────────────────────┘
```

### 5.2 Equipment Slots Section

**Slots Display:**

- [ ] Character preview: [Shows what player looks like with current equipment]
- [ ] Equipment slots arranged by body part
- [ ] Drag-and-drop to change: [Yes/No]
- [ ] Double-click to equip: [Yes/No]

**Slot Details:**

- Equipment slot for each:
  - Head
  - Body/Chest
  - Legs
  - Feet
  - Hand (primary weapon)
  - Off-hand (shield/secondary)

**Character Preview:**

- [ ] Shows isometric character model
- [ ] Updates as equipment changes
- [ ] Rotatable: [Yes/No]

### 5.3 Item Grid Section

**Grid Layout:**

- [ ] Grid size: [Specify - e.g., 4x5 items]
- [ ] Item size in grid: [Specify dimensions]
- [ ] Scrollable: [Yes if inventory exceeds grid]
- [ ] Categorized tabs: [Weapons / Armor / Consumables / Quest Items]

**Item Display:**

- [ ] Item icon
- [ ] Item name on hover
- [ ] Quantity (if stacking): [Number displayed]
- [ ] Rarity indication: [Color coding or star system]

**Item Actions:**

- [ ] Drag-and-drop to equip
- [ ] Right-click context menu: [Options like Use/Equip/Drop/Details]
- [ ] Double-click to use (consumables)
- [ ] Hotkey access: [Yes/No for consumables]

### 5.4 Item Details Panel

**Visibility:**

- [ ] Shows when item selected/hovered
- [ ] Displays to the right of grid
- [ ] Updates in real-time

**Information Shown:**

- [ ] Item name
- [ ] Item description/flavor text
- [ ] Item stats (if applicable):
  - Damage (weapons)
  - Defense (armor)
  - Effect (consumables)
- [ ] Weight/stackable indicator
- [ ] Sell/use value

---

## 6. Equipment Management

### 6.1 Equipment Categories

**Clothing/Armor:**

- [ ] Head gear (optional)
- [ ] Body/torso
- [ ] Leg wear
- [ ] Footwear

**Weapons:**

- [ ] Sword (primary melee)
- [ ] Bow (ranged)
- [ ] Shield (off-hand defense)

**Consumables:**

- [ ] Tinctures/Potions
- [ ] Quantity tracking
- [ ] Quick-use option

### 6.2 Stat Display

**Equipment Stats:**

- [ ] Name
- [ ] Damage value (weapons)
- [ ] Defense value (armor)
- [ ] Special effects (if any)
- [ ] Requirements (if player level matters)

**Comparison Display:**

- [ ] When selecting new equipment
- [ ] Shows current vs. proposed stats
- [ ] Indicates improvement/downgrade with +/- indicators

---

## 7. Dialogue System

### 7.1 Dialogue Box Design

**Position:** [Bottom of screen, occupy lower third suggests]

**Layout:**

```
┌─────────────────────────────────┐
│ NPC Name                    [X] │
├─────────────────────────────────┤
│ "Here is example dialogue text  │
│  that spans multiple lines      │
│  showing how it appears on      │
│  screen during conversation."   │
│                                 │
│ [Choice 1] [Choice 2] [Choice 3]│
└─────────────────────────────────┘
```

**Components:**

- [ ] NPC name header (left side)
- [ ] Portrait/character indicator: [If yes, specify position]
- [ ] Close button (X) top-right
- [ ] Dialogue text area (scrollable if long)
- [ ] Dialogue choices below text

### 7.2 Dialogue Text Formatting

**Typography:**

- [ ] Font: [From Art Bible]
- [ ] Size: [Readable, spec px]
- [ ] Color: [Text color from palette]
- [ ] Line height: [Spacing between lines]
- [ ] Text speed: [Instant / Typewriter effect - specify]

**Special Text Formatting:**

- [ ] Emphasized text: [Bold / Color change / Both]
- [ ] Character names in dialogue: [Special formatting]
- [ ] Sound effects/actions: [Italics / Color / Spec]

### 7.3 Dialogue Choice Presentation

**Choice Button Design:**

- **Appearance:** [Specify visual style]
- **Numbering:** [1/2/3 or A/B/C or labeled]
- **Interactivity:**
  - [ ] Keyboard input to select (1, 2, 3 keys)
  - [ ] Mouse click on choice
  - [ ] Arrow keys + Enter

**Styling:**

- Idle state: [Standard appearance]
- Hover state: [Highlight color]
- Selected state: [Different highlight]
- Unavailable: [Greyed out, if applicable]

**Behavior:**

- [ ] Multiple choices visible at once
- [ ] One choice per line
- [ ] Scrollable if many options
- [ ] Selection confirmation sound: [Yes, specify audio]

### 7.4 Character Portraits

**If Included:**

- [ ] Position: [Left side of dialogue box typical]
- [ ] Size: [Specify dimensions]
- [ ] Style: [Isometric portrait or close-up face]
- [ ] Expressions: [Idle, happy, sad, angry variations]

**Animation:**

- [ ] Fade in/out with dialogue
- [ ] Expression changes with dialogue tone

---

## 8. Quest Log / Journal

### 8.1 Quest Log Access

**Hotkey:** [Specify - suggest 'J' or 'Q']

**Screen:**

```
┌──────────────────────────────┐
│  QUEST LOG                   │
├──────────────────────────────┤
│ □ Active Quests              │
│ □ Completed Quests           │
│ □ Failed/Abandoned Quests    │
│                              │
│ [Quest Name 1]               │
│ [Quest Name 2]  ← Selected   │
│ [Quest Name 3]               │
│                              │
│ Current Objective:           │
│ Find the local mage          │
│                              │
│ Quest Details:               │
│ [Full text description...]   │
└──────────────────────────────┘
```

### 8.2 Quest List Organization

**Categories:**

- [ ] Active quests (in progress)
- [ ] Completed quests (for reference)
- [ ] Failed quests (if applicable)

**Quest Entry:**

- [ ] Quest name
- [ ] Status indicator (active/complete/failed)
- [ ] Progress bar (optional)
- [ ] Quest giver name

### 8.3 Quest Details Panel

**Information Displayed:**

- [ ] Full quest title
- [ ] Full quest description
- [ ] Current objective(s)
- [ ] Past objectives (already completed)
- [ ] Quest giver location
- [ ] Map marker: [If quest is trackable]

**UI Features:**

- [ ] Abandon quest button (if allowed)
- [ ] Mark as tracked: [Yes/No]
- [ ] Expand/collapse: [Sections]

---

## 9. Pause Menu

### 9.1 Pause Screen

**Trigger:** [ESC key or Pause button on controller]

**Appearance:**

- [ ] Pauses game world
- [ ] Darkens/blurs background
- [ ] Modal pause menu overlays

**Menu Options:**

| Button    | Function           | Action          |
| --------- | ------------------ | --------------- |
| Resume    | Return to game     | Unpause         |
| Save Game | Save progress      | Save Screen     |
| Load Game | Load previous save | Load Screen     |
| Settings  | Adjust options     | Settings Screen |
| Main Menu | Return to title    | Confirm exit    |
| Quit Game | Close application  | Confirm exit    |

### 9.2 Pause Menu Layout

- [ ] Centered on screen
- [ ] Buttons arranged vertically
- [ ] Semi-transparent background
- [ ] Darkened world visible behind menu

### 9.3 Save/Load Screens

**Save Game Screen:**

- [ ] Shows current save slots (suggest 5-10 slots)
- [ ] Each slot shows:
  - Save name (editable)
  - Character level (if applicable)
  - Location
  - Play time
  - Save date/time

**Load Game Screen:**

- [ ] Same slot information
- [ ] Delete option per save
- [ ] Load selected save confirmation

---

## 10. Settings Menu

### 10.1 Settings Categories

**Display Settings:**

- [ ] Resolution: [Dropdown menu]
- [ ] Fullscreen: [Toggle]
- [ ] V-Sync: [Toggle]
- [ ] Brightness: [Slider]
- [ ] Contrast: [Slider]

**Audio Settings:**

- [ ] Master Volume: [Slider 0-100]
- [ ] Music Volume: [Slider]
- [ ] SFX Volume: [Slider]
- [ ] Dialogue Volume: [Slider]
- [ ] Mute All: [Toggle]

**Gameplay Settings:**

- [ ] Difficulty: [Radio buttons - Easy/Normal/Hard]
- [ ] Quest Markers: [Toggle/Hidden/Always on]
- [ ] HUD Scale: [Slider - 75%-150%]
- [ ] Colorblind Mode: [Toggle with type options]
- [ ] Auto-save: [Toggle]

**Controls Settings:**

- [ ] Input Method: [Keyboard / Controller / Both]
- [ ] Key Remapping: [View/Edit controls]
- [ ] Sensitivity: [Slider if applicable]

**Accessibility:**

- [ ] Text Size: [Slider]
- [ ] Subtitle Size: [Slider]
- [ ] Subtitle Background: [Toggle]
- [ ] High Contrast: [Toggle]
- [ ] Text-to-Speech: [Toggle if applicable]

### 10.2 Settings Implementation

**Save Behavior:**

- [ ] Auto-save changes
- [ ] Confirm before major changes
- [ ] Default settings button

---

## 11. Map Screen

### 11.1 Full Map View

**Access Method:**

- [ ] From HUD minimap (click to expand)
- [ ] Pause menu option
- [ ] Hotkey: [Specify - suggest 'M']

**Map Display:**

- [ ] Full world layout
- [ ] Current player position indicator
- [ ] Explored vs. unexplored areas: [Different color / fog of war]
- [ ] Points of interest marked: [Cities, dungeons, NPC homes]
- [ ] Quest locations highlighted: [Yes/No]

**Interactivity:**

- [ ] Pan/scroll map
- [ ] Zoom in/out: [If applicable]
- [ ] Click location for details: [Optional]
- [ ] Travel marker placement: [If fast travel available]

---

## 12. Feedback Systems

### 12.1 Visual Feedback

**Enemy Hit Feedback:**

- [ ] Flash/shake screen on player hit
- [ ] Enemy knockback animation
- [ ] Damage number popup: [Shows damage value]
- [ ] Color indicator (red for damage)

**Item Pickup Feedback:**

- [ ] Item popup notification:
  - "You obtained [Item Name]"
  - Icon display
  - Fades after [X] seconds

**Ability/Action Feedback:**

- [ ] Ability activation sound
- [ ] Animation playing
- [ ] Effect visual (magic sparkles, sword arc, etc.)

### 12.2 Audio Feedback

**Feedback Sounds:**

- [ ] Button click: [Specify audio asset]
- [ ] Item pickup: [Specify audio asset]
- [ ] Quest complete: [Specify audio asset]
- [ ] Dialogue open/close: [Specify audio asset]
- [ ] Menu navigation: [Specify audio asset]
- [ ] Damage taken: [Character sound]
- [ ] Attack hit: [Satisfying impact sound]

**Volume Levels:**

- [ ] UI sounds: [Loud enough to hear, -6dB below effects]
- [ ] Dialogue: [Clear, louder than ambient]
- [ ] Effects: [Prominent, full volume]

### 12.3 Haptic Feedback (if controller support)

- [ ] Controller vibration on hit taken: [Yes/No]
- [ ] Vibration intensity: [Specify if yes]
- [ ] Vibration on special actions: [Specify]

---

## 13. Error & Status Messages

### 13.1 Message Presentation

**Format:**

- [ ] Position: [Center-top or center-middle of screen]
- [ ] Duration: [X seconds auto-dismiss]
- [ ] Dismissible: [Yes - click or ESC]

**Message Types:**

- **Info Messages:** "Saved game"
- **Warning Messages:** "Low health!"
- **Error Messages:** "Cannot save while in combat"
- **Success Messages:** "Quest completed!"

**Styling:**

- [ ] Background color indicates type
- [ ] Icon or indicator symbol
- [ ] Font size large enough to read quickly

---

## 14. Accessibility Features

### 14.1 Text Accessibility

- [ ] Adjustable text size: [Range in settings]
- [ ] Colorblind modes: [Specify types - Deuteranopia, Protanopia, Tritanopia]
- [ ] High contrast mode: [Yes/No]
- [ ] Font: [Serif/Sans-serif - Sans-serif generally more readable]

### 14.2 Gameplay Accessibility

- [ ] Difficulty scaling: [Easy/Normal/Hard affects combat]
- [ ] Quest marker toggle: [On/Off or always visible]
- [ ] Subtitle options: [On/Off, size, background]
- [ ] Remappable controls: [Essential actions]
- [ ] Controller support: [Yes/No]

### 14.3 Audio Accessibility

- [ ] Subtitles for all dialogue: [Required]
- [ ] Sound effect captions: [Optional but recommended]
- [ ] Speaker identification: [Shows who's speaking]
- [ ] Ambient sound descriptions: [When important]

---

## 15. Mobile/Console Adaptation (if applicable)

### 15.1 Controller Support

**Button Mapping:**

- [ ] A: Interact / Confirm
- [ ] B: Cancel / Back
- [ ] X: Inventory
- [ ] Y: Quest Log
- [ ] LB/RB: Weapon swap
- [ ] RT/LT: Attack / Aim
- [ ] Start: Pause menu
- [ ] Back: Map

**Stick Functions:**

- [ ] Left stick: Movement
- [ ] Right stick: [Camera / Menus navigation]

### 15.2 Touch Input (if mobile)

- [ ] Virtual joystick: [Left side of screen]
- [ ] Action buttons: [Right side of screen]
- [ ] Tap to interact: [Proximity detection]
- [ ] Drag for inventory: [Drag-and-drop support]

---

## 16. Tutorial & Onboarding

### 16.1 Tutorial Screens

**Controlled Tutorial:**

- [ ] Movement tutorial (first moments)
- [ ] Jumping tutorial
- [ ] Combat tutorial (first enemy)
- [ ] Inventory/equipment tutorial
- [ ] Dialogue tutorial (first NPC)

**Tooltip System:**

- [ ] First time seeing UI element
- [ ] Appears once, dismissed by clicking
- [ ] Can be reviewed in help menu

### 16.2 Tutorial Content

- Overlay highlights on-screen elements
- Text explanation of controls
- Optional: Guided hand pointer
- Player must demonstrate understanding (e.g., try action)

---

## 17. Performance Considerations

### 17.1 UI Optimization

- [ ] Canvas rendering: [Batched for performance]
- [ ] Font atlasing: [Texture pre-rendering]
- [ ] Image compression: [Optimized for load times]
- [ ] Update frequency: [Only update when needed]

### 17.2 Memory Management

- [ ] UI pooling: [Reuse UI objects where possible]
- [ ] Asset unloading: [Clear unused text/images]
- [ ] Memory targets: [Specify - e.g., UI < 50MB]

---

## 18. Testing Checklist

- [ ] All buttons are clickable and responsive
- [ ] Text is readable on all screen resolutions
- [ ] Colors meet accessibility standards
- [ ] Colorblind mode is tested
- [ ] Dialogues fit in boxes without overflow
- [ ] Tooltip information is helpful and accurate
- [ ] Sound effects match UI feedback
- [ ] Controller support tested on actual controller
- [ ] Save/load functionality works correctly
- [ ] Settings persist after reload
- [ ] Performance meets frame rate targets
- [ ] Mobile/console controls feel responsive

---

## 19. Revision History

| Version | Date          | Changes          |
| ------- | ------------- | ---------------- |
| 1.0     | April 1, 2026 | Initial template |
| —       | —             | —                |

---

**Document Status:** Template - Ready for UI/UX designer input and implementation planning
