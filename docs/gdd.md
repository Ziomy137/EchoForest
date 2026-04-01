# Game Design Document

## "Echo Forest"

**Project Codename:** EchoForest  
**Document Version:** 1.0  
**Last Updated:** April 1, 2026

---

## 1. Executive Summary

"Echo Forest" is a 2D isometric adventure game set in medieval times, designed for short, engaging gaming sessions (2-3 days of gameplay). Players control a peasant whose child has been kidnapped through a magical portal by a mysterious mage, setting them on a quest to uncover the truth behind this mystical abduction and rescue their family.

---

## 2. Technical Specifications

### 2.1 Platform Support

- Windows
- Linux
- macOS

### 2.2 Technology Stack

- **Game Engine:** Godot
- **Scripting Language:** C#

### 2.3 Visual Style

- **Perspective:** 2D Isometric view
- **Art Style:** Pixel art / Retro art

### 2.4 Game Structure

- Clear beginning and end
- Single playthrough completion: 2-3 days of gameplay
- Designed for short, engaging gaming sessions

---

## 3. Core Concept

**Setting:** Medieval times  
**Tone:** Fantasy-adventure with dramatic narrative stakes

---

## 4. Narrative Design

### 4.1 Story Overview

The player begins as a peasant living in a cottage outside a city with their spouse and child. They work the land on their small farm, living a simple life.

_(Hidden lore: The peasant is the king's illegitimate child, a fact that sets the stage for the conflict to come.)_

### 4.2 Inciting Incident

A mysterious mage appears, arriving through a magical portal near the cottage. The mage attacks violently, striking the player character against a wall, and kidnaps the child before vanishing through the portal, which then becomes inactive.

### 4.3 Primary Quest

Upon awaking, the player realizes the portal is no longer active. The primary quest objective directs the player to seek out a local mage in the city who may possess knowledge about the portal's origins and the abduction.

### 4.4 Story Context

_(To be revealed during gameplay:)_

- The child is the target of kidnapping due to royal bloodline inheritance
- The magician's motivations and the nature of the portal will be uncovered through the quest

---

## 5. Gameplay Mechanics

### 5.1 Core Movement

- **Walking:** Movement in 4 cardinal directions
- **Running:** Accelerated movement for traversal
- (optional) **Sneaking/Crouching:** Stealth and concealment capability

### 5.2 Equipment System

- **Equipment Changes:** Dynamic changing of clothing (one costume) and gear
- **Weapon Combat:** Direct melee combat system and archery
- (optional) **Magic Combat:** Magic staff with range damage (fire, frost, lighting)

### 5.3 Inventory Elements

- **Clothing:** Full outfits
- **Weapons:** Swords, Bows, Shields and optional magic staffs
- **Consumables:** Tinctures (potions/healing items)
- **Artifacts:** quest items

---

## 6. Visual Design

### 6.1 Color Palette

A carefully curated, limited color palette creates the game's cohesive medieval aesthetic:

| Color Name   | Hex Code | Usage                        |
| ------------ | -------- | ---------------------------- |
| Deep Black   | #1a1a1a  | Background, shadows          |
| Dark Brown   | #2d2416  | Background, earth tones      |
| Dark Gray    | #3d3d3d  | Neutral tones                |
| Medium Gray  | #5a5a5a  | Stone, architecture          |
| Warm Brown   | #8b7355  | Stone, architecture, warmth  |
| Dark Leather | #5c3d2e  | Equipment, leather items     |
| Dark Red     | #8b0000  | Blood, danger indicators     |
| Deep Purple  | #2a1a4a  | Magic, mystical elements     |
| Dark Orange  | #ff6b00  | Light, fire                  |
| Gold         | #ffd700  | Highlights, treasure         |
| Dark Green   | #1a3a1a  | Nature, foliage              |
| Deep Water   | #1a3a5c  | Water elements               |
| Skin Tone    | #8b6f47  | Character skin (base)        |
| Light Skin   | #a88860  | Character skin (highlights)  |
| White        | #ffffff  | Light sources, highlights    |
| Light Gray   | #cccccc  | Light architectural elements |

### 6.2 Palette Application

- **🎨 Background:** #1a1a1a, #2d2416
- **🏰 Stone/Architecture:** #5a5a5a, #6b6b6b, #8b7355
- **⚔️ Metal/Armor:** #4a4a4a, #7a7a7a, #b8b8b8
- **🔥 Light/Fire:** #ff6b00, #ffaa00, #ffd700
- **💜 Magic:** #5c3a8c, #9d4edd
- **🩸 Blood/Danger:** #8b0000, #c41e3a
- **🌲 Nature:** #1a3a1a, #2d5a2d, #4a7a4a

### 6.3 Lighting Design

- **Day Cycle:** Full color palette as specified
- **Night Cycle:** All colors dimmed to create atmospheric nighttime experience

---

## 7. Content & Assets

### 7.1 Items & Equipment

- **Clothing:** Full outfits and individual pieces
- **Weapons:**
  - Swords (melee combat)
  - Bow (ranged combat)
  - Shield (defense)
  - (optional) Magic staffs (ranged combat)
- **Consumables:** Tinctures (healing and utility items)

### 7.2 World Elements

- **Map Layout:** Map divided into 7 areas. You can travel between them using roads or other special passages.
  Areas must be closed by forests, walls, mountains or water.
- **Environmental Objects:** Trees, bushes, rocks, river, lake, mountains, farm buildings, castle, towers, animals, caves, villages, villagers.

---

## 8. Localization

**Primary Language:** English

---

## 9. Game Scope & Duration

- **Estimated Playtime:** 20-30 hours of gameplay
- **Narrative Pacing:** Short, focused story arc with clear resolution
- **Player Progression:** Linear story progression with gameplay objectives

---

## 10. Design Pillars

1. **Immersive Medieval Setting:** Cohesive visual and narrative design rooted in medieval fantasy
2. **Engaging Story:** Clear narrative drive with emotional stakes (family rescue)
3. **Accessible Gameplay:** Simple, intuitive controls with short session design
4. **Visual Clarity:** Limited, purposeful color palette supporting readability and aesthetics
5. **Cross-Platform Accessibility:** Seamless experience across Windows, Linux, and macOS

---

## 11. Next Steps / To Be Determined

- Detailed level/map design and layout
- Enemy AI and combat balancing
- Dialogue and NPC interaction systems
- Quest progression details and branching
- Asset creation pipeline and timelines
- Audio design and soundtrack direction
- User interface and menu design
- Tutorial and onboarding sequences

---

**Document Status:** Draft - Ready for team review and expansion
