# Art Bible & Asset Style Guide

## "Echo Forest"

**Document Version:** 1.0  
**Last Updated:** April 1, 2026

---

## 1. Overview

This document defines the visual standards, asset specifications, and artistic direction for Echo Forest to ensure consistency across all in-game graphics.

---

## 2. Art Style Direction

### 2.1 Visual Identity

- **Style:** Pixel art / Retro art
- **Perspective:** 2D Isometric view
- **Color Depth:** Limited palette (16 primary colors + variations)
- **Artistic Tone:** Medieval fantasy with atmospheric, grounded aesthetic

### 2.2 Inspiration & Reference

- [ ] Reference games/art styles (specify examples)
- [ ] Mood boards and visual references
- [ ] Stylistic tone (gritty, whimsical, realistic, etc.)

---

## 3. Color Palette

### 3.1 Primary Palette

| Color Name   | Hex Code | Usage                 | Notes                 |
| ------------ | -------- | --------------------- | --------------------- |
| Deep Black   | #1a1a1a  | Backgrounds, shadows  | Primary dark tone     |
| Dark Brown   | #2d2416  | Backgrounds, earth    | Warm dark tone        |
| Dark Gray    | #3d3d3d  | Neutral tones         | Shadow mid-tone       |
| Medium Gray  | #5a5a5a  | Stone, architecture   | Mid-tone neutral      |
| Warm Brown   | #8b7355  | Stone, warmth         | Architectural accent  |
| Dark Leather | #5c3d2e  | Equipment, leather    | Equipment primary     |
| Dark Red     | #8b0000  | Blood, danger         | Warning indicator     |
| Deep Purple  | #2a1a4a  | Magic elements        | Mystical primary      |
| Dark Orange  | #ff6b00  | Light, fire           | Warm highlight        |
| Gold         | #ffd700  | Treasure, highlights  | Premium accent        |
| Dark Green   | #1a3a1a  | Nature, foliage       | Environmental primary |
| Deep Water   | #1a3a5c  | Water elements        | Water primary         |
| Skin Tone    | #8b6f47  | Character skin (base) | Mid-tone skin         |
| Light Skin   | #a88860  | Character highlights  | Highlight skin        |
| White        | #ffffff  | Highlights, light     | Brightest tone        |
| Light Gray   | #cccccc  | Light architecture    | Light neutral         |

### 3.2 Functional Color Groups

- **🎨 Background:** #1a1a1a, #2d2416
- **🏰 Stone/Architecture:** #5a5a5a, #6b6b6b, #8b7355
- **⚔️ Metal/Armor:** #4a4a4a, #7a7a7a, #b8b8b8
- **🔥 Light/Fire:** #ff6b00, #ffaa00, #ffd700
- **💜 Magic:** #5c3a8c, #9d4edd
- **🩸 Blood/Danger:** #8b0000, #c41e3a
- **🌲 Nature:** #1a3a1a, #2d5a2d, #4a7a4a

### 3.3 Day/Night Cycle

- **Day Palette:** Full colors as specified above
- **Night Palette Adjustment:** All colors reduced in brightness/saturation by [X%]
- **Transition Time:** [specify duration]
- **Implementation:** Shader overlay / Palette swap

---

## 4. Sprite Specifications

### 4.1 Base Tile Size

- **Grid Unit:** [specify: e.g., 32x32 pixels or 64x64]
- **Isometric Tile Base:** [specify: e.g., 32x16 for isometric diamond]
- **Sub-pixel Scaling:** [Yes/No]

### 4.2 Sprite Density

- **Resolution:** [specify pixel density, e.g., "1 game unit = 32 pixels"]
- **Scale Factor:** Consistent across all assets
- **Minimum Feature Size:** [specify minimum readable pixel size]

### 4.3 Sprite Layer Guidelines

**Layering Order (front to back):**

1. UI/Overlays
2. Flying particles/effects
3. Characters (actors)
4. Foreground objects (trees, structures)
5. Environmental details
6. Ground/floor
7. Background
8. Sky/distant elements

---

## 5. Character Design

### 5.1 Player Character

**Sprite Sheet Format:**

- [ ] Direction variations: 4-directional (facing left, right, up, down) / 8-directional
- [ ] Animation frames per action: [specify]

**Animation Sets:**

- [ ] **Idle:** [X] frames
- [ ] **Walk:** [X] frames (each direction)
- [ ] **Run:** [X] frames (each direction)
- [ ] **Jump:** [X] frames
- [ ] **Fall:** [X] frames
- [ ] **Land:** [X] frames
- [ ] **Sneak/Crouch:** [X] frames
- [ ] **Attack (sword):** [X] frames
- [ ] **Attack (bow draw):** [X] frames
- [ ] **Hit/Damage:** [X] frames
- [ ] **Death:** [X] frames

**Base Character Appearance:**

- [ ] Height in pixels: [specify]
- [ ] Base color scheme: [peasant medieval outfit]
- [ ] Customization points: [specify which parts change with equipment]

### 5.2 NPC Characters

**Local Mage:**

- [ ] Size/scale relative to player
- [ ] Idle animations
- [ ] Dialog gestures (if applicable)

**Wife/Family:**

- [ ] Appearance specifications
- [ ] Idle/ambient animations
- [ ] Expression variations

**Child:**

- [ ] Size (smaller than player)
- [ ] Appearance specifications
- [ ] Kidnapping animation state (if shown in intro)

**Enemies/NPCs:**

- [ ] [List other character types]
- [ ] Animation requirements for each

---

## 6. Equipment & Clothing System

### 6.1 Equipment Layers

**Clothing Composition:**

- [ ] Base body/torso
- [ ] Legs/lower body
- [ ] Footwear
- [ ] Head/hat (if applicable)
- [ ] Cloak/robe (if applicable)

**Equipment Appearance:**

- [ ] Each piece renders as a separate sprite layer
- [ ] Layering order for combinations
- [ ] Color variations [specify systems]

### 6.2 Weapon Sprites

**Sword:**

- [ ] Idle carry position
- [ ] Attack animation frames: [X]
- [ ] Size/scale specifications
- [ ] Directional variations (if different for each direction)

**Bow:**

- [ ] Idle carry position
- [ ] Draw animation frames: [X]
- [ ] Shot animation frames: [X]
- [ ] Arrow sprite specifications
- [ ] Directional variations

**Shield:**

- [ ] Passive positioning on back/arm
- [ ] Defense animation (if blocking shows movement)
- [ ] Damage/hit state (if visual change)

### 6.3 Equipment Sprite Sheet Organization

- [ ] One sheet per equipment piece (recommended)
- [ ] Or combined sheet with [X]x[Y] grid layout
- [ ] File naming convention: [specify pattern]

---

## 7. Environmental Assets

### 7.1 Tileset Specifications

**Tile Types:**

- [ ] Grass/ground
- [ ] Stone path/road
- [ ] Dirt/farmland
- [ ] Water
- [ ] Forest floor
- [ ] Architecture/walls
- [ ] Roofs
- [ ] Variations per tile (worn, damaged, etc.)

**Tile Size:** [Matches base tile size from 4.1]

**Tileset Sheet Organization:**

- [ ] Single atlas file or multiple files
- [ ] Grid layout: [X]x[Y] tiles
- [ ] File naming convention

### 7.2 Objects & Props

**Cottage/Building Elements:**

- [ ] Cottage walls (various states)
- [ ] Cottage roof
- [ ] Door/window variations
- [ ] Inside elements (furniture, beds, containers)

**Farm Elements:**

- [ ] Crops (growing, harvest-ready states)
- [ ] Fences
- [ ] Farm buildings
- [ ] Tools/implements

**Natural Elements:**

- [ ] Trees (various types)
- [ ] Bushes/shrubs
- [ ] Rocks/boulders
- [ ] Water features (streams, wells)

**Magical Elements:**

- [ ] Portal (idle animation)
- [ ] Portal (active/opening animation)
- [ ] Magical aura/effects

### 7.3 Interactive Objects

- [ ] Doors (idle, open, closed)
- [ ] Chests (closed, open)
- [ ] NPCs/characters interaction zones
- [ ] Quest items/pickup objects

---

## 8. Visual Effects & Particles

### 8.1 Effect Types

- [ ] **Dust clouds:** Movement, jumping
- [ ] **Blood:** Combat damage
- [ ] **Magic effects:** Spell casting, portal effects
- [ ] **Lighting/glow:** Fire, torches, magical aura
- [ ] **Transitions:** Fades, screen effects

### 8.2 Particle Specifications

- [ ] Sprite size and frame count
- [ ] Animation speed
- [ ] Spawn behavior (one-shot vs. looping)
- [ ] Color variations
- [ ] Layer positioning

---

## 9. UI & Icon Design

### 9.1 UI Style

- [ ] UI font type and sizes
- [ ] Button styles and states (idle, hover, pressed)
- [ ] Panel/window designs
- [ ] Consistent spacing and alignment

### 9.2 Icons

**Equipment Icons:**

- [ ] Size: [X]x[X] pixels
- [ ] Background style
- [ ] Clothing/weapon variations

**Inventory Icons:**

- [ ] Tinctures/potions
- [ ] Quest items
- [ ] Consumables

**UI Icons:**

- [ ] Health indicator
- [ ] Status effects
- [ ] Menu navigation

---

## 10. Animation Standards

### 10.1 Frame Rate & Timing

- [ ] Animation playback frame rate: [X] fps (typical: 12-24 for pixel art)
- [ ] Frame duration: [specify timing per frame or groups]
- [ ] Transition timing between states

### 10.2 Animation Categories

**Locomotion:**

- [ ] Idle stance
- [ ] Walking cycle
- [ ] Running cycle
- [ ] Sneaking/crouching

**Actions:**

- [ ] Jumping (anticipation, airtime, landing)
- [ ] Attacking (wind-up, strike, recovery)
- [ ] Getting hit/knocked back
- [ ] Interacting with objects

**Emotion/Expression:**

- [ ] Idle variations (breathing, weight shift)
- [ ] Reaction animations

---

## 11. Isometric Perspective Guidelines

### 11.1 Perspective Rules

- [ ] Camera angle: Typically 45 degrees rotated, but [specify exact angle]
- [ ] Vertical offset for depth: [specify vertical offset per grid unit]
- [ ] Sprite positioning relative to isometric grid
- [ ] Sorting order based on Y position (depth sorting)

### 11.2 Sprite Positioning

- [ ] Anchor point for character sprites: [e.g., "bottom center"]
- [ ] Anchor point for object sprites: [e.g., "base of object"]
- [ ] Z-ordering calculation: [describe formula or system]

---

## 12. Asset Organization

### 12.1 File Structure

```
Assets/
├── Sprites/
│   ├── Characters/
│   │   ├── Player/
│   │   ├── NPCs/
│   │   └── Enemies/
│   ├── Equipment/
│   │   ├── Clothing/
│   │   └── Weapons/
│   ├── Environment/
│   │   ├── Tilesets/
│   │   ├── Buildings/
│   │   └── Nature/
│   ├── Effects/
│   │   ├── Particles/
│   │   └── UI/
│   └── UI/
│       ├── Icons/
│       └── Panels/
└── Textures/
```

### 12.2 Naming Conventions

- [ ] Filename pattern: [specify, e.g., "character_player_walk_right_0001.png"]
- [ ] Spritesheet naming: [specify]
- [ ] Version control in filenames: [Yes/No - if yes, how]

---

## 13. Optimization & Technical Requirements

### 13.1 Compression & Format

- [ ] Image format: PNG (with transparency)
- [ ] Color depth: 8-bit / 32-bit
- [ ] Compression settings
- [ ] Maximum texture atlas size: [specify, e.g., "2048x2048"]

### 13.2 Performance Targets

- [ ] Asset memory footprint: [specify]
- [ ] Texture streaming approach (if large maps)
- [ ] LOD (Level of Detail) strategy: [if applicable]

---

## 14. Quality Assurance Checklist

- [ ] All sprites use approved color palette only
- [ ] Consistent pixel density across assets
- [ ] Proper animation frame timings
- [ ] Equipment/layer combinations tested visually
- [ ] Day/night palette adjustments verified
- [ ] Isometric perspective consistency
- [ ] No aliasing or anti-aliasing (pure pixel art)
- [ ] Proper transparency/alpha handling

---

## 15. Asset Delivery Timeline

- [ ] **Phase 1 (Week X):** Player character, base environment
- [ ] **Phase 2 (Week X):** NPCs, basic equipment
- [ ] **Phase 3 (Week X):** Weapons, effects, polish
- [ ] **Phase 4 (Week X):** Final optimization and integration

---

**Document Status:** Template - Ready for artist input and asset specifications
