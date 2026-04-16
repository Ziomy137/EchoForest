#!/usr/bin/env python3
"""
Generate S3-02 sprite assets for EchoForest.

Player spritesheet: 640×96 (10 cols × 4 rows, 64×24 per frame)
  Row 0 = Down, Row 1 = Left, Row 2 = Right, Row 3 = Up
  Cols 0-1 = Idle (2 frames), Cols 2-5 = Walk (4 frames), Cols 6-9 = Run (4 frames)

Props:
  prop_door.png       48×64
  prop_well.png       48×48
  prop_tree.png       48×64  — palette-approved colors only (no #2d5a2d)
  prop_haybale.png    48×32
  prop_fencepost.png  16×32
"""

from PIL import Image
import os

# ──────────────────────────────────────────
# Approved palette colors (RGBA)
# ──────────────────────────────────────────
TRANSPARENT  = (   0,    0,    0,   0)
DEEP_BLACK   = (0x1a, 0x1a, 0x1a, 255)
DARK_BROWN   = (0x2d, 0x24, 0x16, 255)
DARK_GRAY    = (0x3d, 0x3d, 0x3d, 255)
MEDIUM_GRAY  = (0x5a, 0x5a, 0x5a, 255)
WARM_BROWN   = (0x8b, 0x73, 0x55, 255)
DARK_LEATHER = (0x5c, 0x3d, 0x2e, 255)
DARK_RED     = (0x8b, 0x00, 0x00, 255)
DEEP_PURPLE  = (0x2a, 0x1a, 0x4a, 255)
DARK_ORANGE  = (0xff, 0x6b, 0x00, 255)
GOLD         = (0xff, 0xd7, 0x00, 255)
DARK_GREEN   = (0x1a, 0x3a, 0x1a, 255)
DEEP_WATER   = (0x1a, 0x3a, 0x5c, 255)
SKIN_TONE    = (0x8b, 0x6f, 0x47, 255)
LIGHT_SKIN   = (0xa8, 0x88, 0x60, 255)
WHITE        = (0xff, 0xff, 0xff, 255)
LIGHT_GRAY   = (0xcc, 0xcc, 0xcc, 255)


def rect(img, x, y, w, h, color):
    """Fill a rectangle of pixels with a specific color."""
    for row in range(y, y + h):
        for col in range(x, x + w):
            if 0 <= col < img.width and 0 <= row < img.height:
                img.putpixel((col, row), color)


# ──────────────────────────────────────────
# Character frame renderer
# ──────────────────────────────────────────

# Character is 16px wide × 22px tall, placed at (24, 1) inside a 64×24 frame.
# All sub-coordinates below are relative to the character's own top-left corner.

def _draw_char(img, ox, oy, direction, anim, fidx):
    """
    Draw a single 16×22 character at absolute pixel offset (ox, oy).

    direction: 0=down, 1=left, 2=right, 3=up
    anim:      0=idle,  1=walk,  2=run
    fidx:      frame index within the animation
    """
    def px(x, y, color):
        img.putpixel((ox + x, oy + y), color)

    def pr(x, y, w, h, color):
        rect(img, ox + x, oy + y, w, h, color)

    # ── head (6×5 at char-x=5, char-y=0) ────────────────────────────
    pr(5, 0, 6, 5, LIGHT_SKIN)

    # direction eye hint
    if direction == 0:      # down – two front eyes
        px(6, 2, DARK_BROWN)
        px(9, 2, DARK_BROWN)
    elif direction == 1:    # left – left-side eye only
        px(5, 2, DARK_BROWN)
    elif direction == 2:    # right – right-side eye only
        px(10, 2, DARK_BROWN)
    # up: back of head, no eyes

    # hair (top of head, 1px row, different shade for up direction)
    if direction == 3:
        pr(5, 0, 6, 1, WARM_BROWN)

    # ── arms (2×6 strips each side, offset per frame) ─────────────────
    arm_offset = 0
    if anim == 1:  # walk – arms swing opposite to legs
        arm_swing = [1, 0, -1, 0]
        arm_offset = arm_swing[fidx % 4]
    elif anim == 2:  # run – larger swing
        arm_swing = [2, 0, -2, 0]
        arm_offset = arm_swing[fidx % 4]

    pr(3,  5 + arm_offset, 2, 6, SKIN_TONE)   # left arm
    pr(11, 5 - arm_offset, 2, 6, SKIN_TONE)   # right arm (opposite swing)

    # ── torso (10×8 at char-x=3, char-y=5) ───────────────────────────
    pr(3, 5, 10, 8, DARK_LEATHER)

    # ── belt (8×2 at char-x=4, char-y=13) ────────────────────────────
    pr(4, 13, 8, 2, DARK_BROWN)

    # ── legs (4×H each) ──────────────────────────────────────────────
    # leg positions: (l_y, l_h, r_y, r_h)
    if anim == 0:  # idle
        if fidx == 0:
            l_y, l_h, r_y, r_h = 15, 7, 15, 7
        else:
            l_y, l_h, r_y, r_h = 15, 7, 16, 6   # slight weight shift
    elif anim == 1:  # walk
        walk_legs = [
            (13, 9, 17, 5),   # frame 0: L fwd, R back
            (15, 7, 15, 7),   # frame 1: neutral
            (17, 5, 13, 9),   # frame 2: L back, R fwd
            (15, 7, 15, 7),   # frame 3: neutral
        ]
        l_y, l_h, r_y, r_h = walk_legs[fidx % 4]
    else:  # run
        run_legs = [
            (11,11, 17, 5),   # frame 0: L high fwd, R back
            (15, 7, 15, 7),   # frame 1: neutral
            (17, 5, 11,11),   # frame 2: L back, R high fwd
            (15, 7, 15, 7),   # frame 3: neutral
        ]
        l_y, l_h, r_y, r_h = run_legs[fidx % 4]

    pr(3, l_y, 4, l_h, WARM_BROWN)   # left leg
    pr(9, r_y, 4, r_h, WARM_BROWN)   # right leg

    # ── boots (slightly darker than legs) ───────────────────────────
    # bottom 2 rows of each leg
    boot_l_y = l_y + l_h - 2 if l_h >= 2 else l_y
    boot_r_y = r_y + r_h - 2 if r_h >= 2 else r_y
    pr(3, boot_l_y, 4, 2, DARK_LEATHER)
    pr(9, boot_r_y, 4, 2, DARK_LEATHER)


def generate_player_spritesheet(path):
    FRAME_W, FRAME_H = 64, 24
    img = Image.new('RGBA', (640, 96), TRANSPARENT)

    for row in range(4):         # 0=down, 1=left, 2=right, 3=up
        for col in range(10):    # 0-1=idle, 2-5=walk, 6-9=run
            ox = col * FRAME_W + 24   # character left edge within frame
            oy = row * FRAME_H + 1    # character top edge within frame

            if col < 2:
                anim, fidx = 0, col
            elif col < 6:
                anim, fidx = 1, col - 2
            else:
                anim, fidx = 2, col - 6

            _draw_char(img, ox, oy, row, anim, fidx)

    img.save(path)
    print(f"  player_spritesheet.png  {img.width}x{img.height}")


# ──────────────────────────────────────────
# Prop generators
# ──────────────────────────────────────────

def generate_prop_door(path):
    img = Image.new('RGBA', (48, 64), TRANSPARENT)
    # stone arch frame
    rect(img,  0,  0, 48, 64, DARK_LEATHER)
    # door panel
    rect(img,  4,  4, 40, 56, WARM_BROWN)
    # horizontal rail
    rect(img,  4, 32, 40,  4, DARK_LEATHER)
    # vertical rail
    rect(img, 22,  4,  4, 56, DARK_LEATHER)
    # knob
    rect(img, 37, 37,  4,  4, GOLD)
    img.save(path)
    print(f"  prop_door.png           {img.width}x{img.height}")


def generate_prop_well(path):
    img = Image.new('RGBA', (48, 48), TRANSPARENT)
    # stone base
    rect(img,  4, 30, 40, 18, MEDIUM_GRAY)
    rect(img,  0, 34, 48, 14, MEDIUM_GRAY)
    # shaft walls
    rect(img, 10, 16, 28, 18, MEDIUM_GRAY)
    # water inside
    rect(img, 14, 20, 20, 10, DEEP_WATER)
    # wooden posts
    rect(img,  6,  4,  4, 28, WARM_BROWN)
    rect(img, 38,  4,  4, 28, WARM_BROWN)
    # cross beam
    rect(img,  4,  2, 40,  4, WARM_BROWN)
    # rope
    rect(img, 22,  6,  4,  8, DARK_BROWN)
    # crank handle
    rect(img, 40,  6,  4,  4, DARK_BROWN)
    img.save(path)
    print(f"  prop_well.png           {img.width}x{img.height}")


def generate_prop_tree(path):
    # Using ONLY palette-approved colors:
    #   DARK_GREEN  = #1a3a1a
    #   DARK_BROWN  = #2d2416
    # The spec mentions #2d5a2d which is NOT in the approved palette — omitted.
    img = Image.new('RGBA', (48, 64), TRANSPARENT)
    # trunk
    rect(img, 20, 44,  8, 20, DARK_BROWN)
    # foliage layers (pyramid shape)
    rect(img, 16, 30, 16, 18, DARK_GREEN)
    rect(img, 10, 18, 28, 16, DARK_GREEN)
    rect(img, 14,  4, 20, 18, DARK_GREEN)
    # branch hints
    rect(img,  8, 26,  4,  3, DARK_BROWN)
    rect(img, 36, 26,  4,  3, DARK_BROWN)
    img.save(path)
    print(f"  prop_tree.png           {img.width}x{img.height}")


def generate_prop_haybale(path):
    img = Image.new('RGBA', (48, 32), TRANSPARENT)
    # main body
    rect(img,  2,  2, 44, 28, WARM_BROWN)
    # hay strands
    for y in range(4, 30, 3):
        rect(img, 2, y, 44, 1, GOLD)
    # binding rope: horizontal
    rect(img,  0, 14, 48,  4, DARK_BROWN)
    # binding rope: vertical
    rect(img, 20,  0,  8, 32, DARK_BROWN)
    img.save(path)
    print(f"  prop_haybale.png        {img.width}x{img.height}")


def generate_prop_fencepost(path):
    img = Image.new('RGBA', (16, 32), TRANSPARENT)
    # post body
    rect(img,  4,  4,  8, 28, DARK_LEATHER)
    # pointed top (3 steps → approximate wedge)
    rect(img,  5,  2,  6,  2, DARK_LEATHER)
    rect(img,  6,  0,  4,  2, DARK_LEATHER)
    # grain lines
    for y in range(8, 30, 6):
        rect(img, 5, y, 6, 1, WARM_BROWN)
    img.save(path)
    print(f"  prop_fencepost.png      {img.width}x{img.height}")


# ──────────────────────────────────────────
# Entry point
# ──────────────────────────────────────────

if __name__ == '__main__':
    base = os.path.join(os.path.dirname(__file__), '..', 'src', 'Assets', 'Sprites')
    chars_dir = os.path.join(base, 'Characters')
    props_dir  = os.path.join(base, 'Props')
    os.makedirs(chars_dir, exist_ok=True)
    os.makedirs(props_dir,  exist_ok=True)

    print("Generating S3-02 sprite assets …")
    generate_player_spritesheet(os.path.join(chars_dir, 'player_spritesheet.png'))
    generate_prop_door        (os.path.join(props_dir,  'prop_door.png'))
    generate_prop_well        (os.path.join(props_dir,  'prop_well.png'))
    generate_prop_tree        (os.path.join(props_dir,  'prop_tree.png'))
    generate_prop_haybale     (os.path.join(props_dir,  'prop_haybale.png'))
    generate_prop_fencepost   (os.path.join(props_dir,  'prop_fencepost.png'))
    print("Done.")
