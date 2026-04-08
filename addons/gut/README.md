# GUT — Godot Unit Testing addon

This directory is reserved for the **GUT** (Godot Unit Test) addon.

## Installation

GUT must be installed manually — the addon files are not committed to this
repository (they are third-party code managed separately).

### Option A — Godot AssetLib (recommended)

1. Open the project in Godot 4.3
2. Go to **AssetLib** tab (top menu)
3. Search for **"GUT"**
4. Install version **≥ 9.3.0** (Godot 4 compatible)
5. Enable the plugin: **Project → Project Settings → Plugins → GUT → Enable**

### Option B — Manual install from GitHub

```bash
# From the repository root:
curl -L https://github.com/bitwes/Gut/releases/latest/download/gut_9.x.zip -o /tmp/gut.zip
unzip /tmp/gut.zip -d addons/
```

Then enable the plugin in Godot.

## GUT Test Location

GUT scene-based tests live in `src/Scripts/Tests/` alongside NUnit tests.  
GUT test scripts follow the naming convention: `test_*.gd`

## Running GUT Tests

- **In Editor:** Open `addons/gut/gut_cmdln.gd` panel or use the GUT panel
- **From CLI:** `godot --headless -s addons/gut/gut_cmdln.gd`

## Why Two Test Frameworks?

| Framework | Purpose                                                                       |
| --------- | ----------------------------------------------------------------------------- |
| **NUnit** | Pure C# logic: state machines, services, utilities — runs in CI without Godot |
| **GUT**   | Scene/node integration: tests that require the Godot tree to be running       |
