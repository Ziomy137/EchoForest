#!/usr/bin/env python3
"""Generate palette-compliant 32x32 inventory icons for S11-02."""

from pathlib import Path

from PIL import Image, ImageDraw

TRANSPARENT = (0, 0, 0, 0)
BLACK = (0x1A, 0x1A, 0x1A, 255)
DARK_BROWN = (0x2D, 0x24, 0x16, 255)
DARK_GRAY = (0x3D, 0x3D, 0x3D, 255)
GRAY = (0x5A, 0x5A, 0x5A, 255)
STONE = (0x8B, 0x73, 0x55, 255)
LEATHER = (0x5C, 0x3D, 0x2E, 255)
RED = (0x8B, 0x00, 0x00, 255)
PURPLE = (0x2A, 0x1A, 0x4A, 255)
ORANGE = (0xFF, 0x6B, 0x00, 255)
GOLD = (0xFF, 0xD7, 0x00, 255)
GREEN = (0x1A, 0x3A, 0x1A, 255)
WATER = (0x1A, 0x3A, 0x5C, 255)
WHITE = (0xFF, 0xFF, 0xFF, 255)
LIGHT_GRAY = (0xCC, 0xCC, 0xCC, 255)

PALETTE = {TRANSPARENT, BLACK, DARK_BROWN, DARK_GRAY, GRAY, STONE, LEATHER, RED, PURPLE, ORANGE, GOLD, GREEN, WATER, WHITE, LIGHT_GRAY}


def create_icon(draw_icon):
    image = Image.new("RGBA", (32, 32), TRANSPARENT)
    draw_icon(ImageDraw.Draw(image))
    return image


def draw_sword(draw):
    draw.line((8, 25, 24, 7), fill=LIGHT_GRAY, width=5)
    draw.line((10, 23, 23, 8), fill=WHITE, width=2)
    draw.line((7, 20, 15, 27), fill=GOLD, width=3)
    draw.line((5, 26, 10, 21), fill=LEATHER, width=4)
    draw.rectangle((22, 5, 26, 9), fill=GRAY)


def draw_bow(draw):
    draw.arc((5, 4, 26, 28), start=60, end=300, fill=LEATHER, width=4)
    draw.line((10, 7, 22, 25), fill=LIGHT_GRAY, width=1)
    draw.line((12, 14, 26, 14), fill=STONE, width=2)
    draw.polygon(((27, 14), (23, 11), (23, 17)), fill=GRAY)


def draw_shield(draw):
    draw.polygon(((6, 5), (26, 5), (24, 21), (16, 29), (8, 21)), fill=LEATHER, outline=GOLD)
    draw.polygon(((10, 9), (22, 9), (21, 19), (16, 24), (11, 19)), fill=GRAY, outline=BLACK)
    draw.line((16, 9, 16, 23), fill=GOLD, width=2)
    draw.line((11, 15, 21, 15), fill=GOLD, width=2)


def draw_tunic(draw, fabric):
    draw.polygon(((10, 5), (14, 3), (18, 3), (22, 5), (28, 10), (24, 16), (22, 14), (23, 28), (9, 28), (10, 14), (8, 16), (4, 10)), fill=fabric, outline=BLACK)
    draw.polygon(((14, 4), (18, 4), (20, 10), (16, 13), (12, 10)), fill=STONE, outline=BLACK)
    draw.line((16, 13, 16, 27), fill=GOLD, width=1)
    draw.rectangle((8, 18, 24, 20), fill=DARK_BROWN)


def draw_tincture(draw, liquid, height):
    draw.rectangle((12, 4, 20, 8), fill=LEATHER, outline=BLACK)
    draw.rectangle((10, 8, 22, 28), fill=GRAY, outline=BLACK)
    draw.rectangle((12, 14 + (10 - height), 20, 26), fill=liquid)
    draw.rectangle((13, 10, 15, 12), fill=WHITE)
    draw.rectangle((17, 2, 19, 4), fill=GOLD)


def draw_key(draw):
    draw.ellipse((4, 5, 22, 23), outline=GOLD, width=4)
    draw.ellipse((9, 10, 17, 18), fill=TRANSPARENT, outline=LIGHT_GRAY, width=2)
    draw.line((19, 20, 28, 29), fill=GOLD, width=4)
    draw.line((23, 24, 27, 20), fill=STONE, width=3)
    draw.line((25, 26, 29, 22), fill=STONE, width=3)


def validate(path, image):
    if image.size != (32, 32):
        raise ValueError(f"{path.name}: expected 32x32, got {image.size}")
    invalid_colors = {color for color in image.getdata() if color not in PALETTE}
    if invalid_colors:
        raise ValueError(f"{path.name}: non-palette colors {invalid_colors}")
    image.save(path)
    print(f"  palette OK  {path.name} 32x32")


def main():
    root = Path(__file__).resolve().parents[1]
    output = root / "src" / "Assets" / "Sprites" / "Items"
    output.mkdir(parents=True, exist_ok=True)
    icons = (
        ("item_sword_basic.png", draw_sword),
        ("item_bow_basic.png", draw_bow),
        ("item_shield_basic.png", draw_shield),
        ("item_tunic_peasant.png", lambda draw: draw_tunic(draw, STONE)),
        ("item_tunic_leather.png", lambda draw: draw_tunic(draw, LEATHER)),
        ("item_tincture_small.png", lambda draw: draw_tincture(draw, WATER, 5)),
        ("item_tincture_large.png", lambda draw: draw_tincture(draw, ORANGE, 9)),
        ("item_portal_key.png", draw_key),
    )
    for name, draw_icon in icons:
        validate(output / name, create_icon(draw_icon))


if __name__ == "__main__":
    main()