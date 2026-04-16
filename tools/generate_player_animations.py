#!/usr/bin/env python3
"""
Generate src/Assets/Animations/player_animations.tres — a Godot 4 SpriteFrames resource
containing all 12 animation clips for the player character.

Spritesheet layout  (640×96, 10 cols × 4 rows, 64×24 per frame):
  Row 0 = Down  │  cols 0-1 idle  │  cols 2-5 walk  │  cols 6-9 run
  Row 1 = Left  │  cols 0-1 idle  │  cols 2-5 walk  │  cols 6-9 run
  Row 2 = Right │  cols 0-1 idle  │  cols 2-5 walk  │  cols 6-9 run
  Row 3 = Up    │  cols 0-1 idle  │  cols 2-5 walk  │  cols 6-9 run
"""

import os

FRAME_W = 64
FRAME_H = 24

# (anim_name, row, start_col, frame_count, fps)
CLIPS = [
    ("idle_down",  0, 0, 2,  5.0),
    ("walk_down",  0, 2, 4,  8.0),
    ("run_down",   0, 6, 4, 10.0),
    ("idle_left",  1, 0, 2,  5.0),
    ("walk_left",  1, 2, 4,  8.0),
    ("run_left",   1, 6, 4, 10.0),
    ("idle_right", 2, 0, 2,  5.0),
    ("walk_right", 2, 2, 4,  8.0),
    ("run_right",  2, 6, 4, 10.0),
    ("idle_up",    3, 0, 2,  5.0),
    ("walk_up",    3, 2, 4,  8.0),
    ("run_up",     3, 6, 4, 10.0),
]


def generate(output_path: str) -> None:
    lines = []

    # ── header ──────────────────────────────────────────────────────────────
    # Count total AtlasTexture sub-resources = sum of frame counts = 40
    total_frames = sum(c[3] for c in CLIPS)  # 40
    load_steps   = 1 + total_frames + 1       # 1 ext_resource + 40 sub_resources + 1 [resource]

    lines.append(f"[gd_resource type=\"SpriteFrames\" load_steps={load_steps} format=3]")
    lines.append("")
    lines.append("[ext_resource type=\"Texture2D\" path=\"res://src/Assets/Sprites/Characters/player_spritesheet.png\" id=\"1_player\"]")
    lines.append("")

    # ── AtlasTexture sub-resources ──────────────────────────────────────────
    sub_id  = 1        # incrementing id for each sub-resource
    frame_ref_map: list[list[int]] = []   # frame_ref_map[clip_idx] = [sub_id, ...]

    for clip_name, row, start_col, frame_count, fps in CLIPS:
        refs = []
        for f in range(frame_count):
            col = start_col + f
            x   = col * FRAME_W
            y   = row * FRAME_H
            lines.append(f"[sub_resource type=\"AtlasTexture\" id=\"AtlasTexture_{sub_id}\"]")
            lines.append(f"atlas = ExtResource(\"1_player\")")
            lines.append(f"region = Rect2({x}, {y}, {FRAME_W}, {FRAME_H})")
            lines.append("")
            refs.append(sub_id)
            sub_id += 1
        frame_ref_map.append(refs)

    # ── [resource] ──────────────────────────────────────────────────────────
    lines.append("[resource]")
    lines.append("animations = [")

    for idx, (clip_name, row, start_col, frame_count, fps) in enumerate(CLIPS):
        refs = frame_ref_map[idx]
        frames_str = ", ".join(
            '{"duration": 1.0, "texture": SubResource("AtlasTexture_' + str(r) + '")}'
            for r in refs
        )
        comma = "" if idx == len(CLIPS) - 1 else ","
        lines.append(f'{{"frames": [{frames_str}], "loop": true, "name": &"{clip_name}", "speed": {fps}}}{comma}')

    lines.append("]")
    lines.append("")

    os.makedirs(os.path.dirname(output_path), exist_ok=True)
    with open(output_path, "w", encoding="utf-8") as f:
        f.write("\n".join(lines))
    print(f"  player_animations.tres  ({total_frames} frames, {len(CLIPS)} clips)")


if __name__ == "__main__":
    base  = os.path.join(os.path.dirname(__file__), "..", "src", "Assets", "Animations")
    out   = os.path.join(base, "player_animations.tres")
    print("Generating player_animations.tres …")
    generate(out)
    print("Done.")
