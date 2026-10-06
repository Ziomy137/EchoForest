#!/usr/bin/env python3
"""Generate static 64x64 area overview textures for S10-03."""

from pathlib import Path

from PIL import Image, ImageDraw

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

PALETTE = {BLACK, DARK_BROWN, DARK_GRAY, GRAY, STONE, LEATHER, RED, PURPLE, ORANGE, GOLD, GREEN, WATER, WHITE, LIGHT_GRAY}


def base(color):
    return Image.new("RGBA", (64, 64), color)


def make_cottage():
    image = base(GREEN)
    draw = ImageDraw.Draw(image)
    draw.line([(37, 63), (37, 47), (31, 39), (31, 27), (24, 19), (24, 0)], fill=STONE, width=7)
    draw.rectangle((12, 12, 29, 29), fill=LEATHER)
    draw.rectangle((10, 10, 31, 16), fill=RED)
    draw.rectangle((15, 18, 26, 26), fill=STONE)
    draw.rectangle((18, 23, 23, 29), fill=DARK_BROWN)
    draw.ellipse((3, 39, 14, 49), fill=WATER)
    draw.rectangle((3, 44, 14, 49), fill=WATER)
    draw.rectangle((34, 51, 43, 53), fill=GOLD)
    return image


def make_farm():
    image = base(GREEN)
    draw = ImageDraw.Draw(image)
    draw.rectangle((27, 0, 35, 63), fill=STONE)
    draw.rectangle((0, 46, 30, 54), fill=STONE)
    for rect in ((4, 10, 19, 23), (42, 10, 58, 23), (4, 27, 19, 40), (42, 27, 58, 40)):
        draw.rectangle(rect, fill=DARK_BROWN)
        for x in range(rect[0] + 2, rect[2], 4):
            draw.line((x, rect[1] + 2, x, rect[3] - 2), fill=GOLD if rect[1] < 25 else STONE, width=1)
    draw.rectangle((42, 3, 59, 8), fill=LEATHER)
    draw.rectangle((40, 1, 61, 4), fill=RED)
    draw.rectangle((25, 33, 37, 35), fill=WATER)
    return image


def make_forest_path():
    image = base(GREEN)
    draw = ImageDraw.Draw(image)
    draw.rectangle((0, 0, 11, 63), fill=DARK_BROWN)
    draw.rectangle((52, 0, 63, 63), fill=DARK_BROWN)
    for y in range(3, 62, 8):
        draw.ellipse((2, y, 14, y + 9), fill=BLACK)
        draw.ellipse((49, y + 3, 62, y + 12), fill=BLACK)
    draw.line([(30, 63), (35, 53), (29, 42), (30, 31), (26, 19), (33, 9), (31, 0)], fill=STONE, width=9)
    draw.rectangle((12, 30, 51, 33), fill=WATER)
    draw.line([(30, 63), (35, 53), (29, 42), (30, 34), (35, 31)], fill=STONE, width=8)
    draw.rectangle((19, 41, 24, 45), fill=GRAY)
    return image


def make_city():
    image = base(GRAY)
    draw = ImageDraw.Draw(image)
    draw.rectangle((27, 0, 36, 63), fill=STONE)
    draw.rectangle((0, 31, 63, 40), fill=STONE)
    draw.rectangle((8, 9, 20, 24), fill=DARK_GRAY)
    draw.rectangle((5, 6, 23, 11), fill=LEATHER)
    draw.rectangle((44, 7, 58, 24), fill=DARK_GRAY)
    draw.rectangle((41, 4, 61, 9), fill=RED)
    draw.rectangle((4, 46, 19, 59), fill=DARK_GRAY)
    draw.rectangle((44, 45, 60, 59), fill=DARK_GRAY)
    draw.rectangle((22, 27, 42, 44), fill=LEATHER)
    draw.ellipse((27, 32, 37, 40), fill=WATER)
    draw.rectangle((29, 53, 34, 60), fill=GOLD)
    return image


def make_mages_tower():
    image = base(STONE)
    draw = ImageDraw.Draw(image)
    draw.rectangle((0, 0, 29, 63), fill=GREEN)
    draw.line([(2, 62), (8, 51), (17, 43), (24, 34), (30, 28)], fill=STONE, width=6)
    draw.rectangle((5, 9, 23, 28), fill=LEATHER)
    draw.rectangle((3, 7, 25, 13), fill=GRAY)
    draw.rectangle((34, 0, 63, 63), fill=DARK_GRAY)
    draw.line((32, 0, 32, 63), fill=BLACK, width=3)
    for rect in ((37, 5, 44, 14), (51, 5, 59, 14), (37, 46, 44, 55)):
        draw.rectangle(rect, fill=LEATHER)
    draw.ellipse((46, 23, 57, 34), fill=PURPLE)
    draw.ellipse((49, 26, 54, 31), fill=GOLD)
    draw.rectangle((49, 41, 57, 44), fill=GRAY)
    return image


def make_portal_chamber():
    image = base(PURPLE)
    draw = ImageDraw.Draw(image)
    draw.rectangle((5, 4, 58, 22), fill=BLACK)
    draw.rectangle((9, 6, 54, 18), fill=DARK_GRAY)
    draw.rectangle((12, 9, 51, 14), fill=PURPLE)
    draw.rectangle((28, 18, 36, 56), fill=DARK_GRAY)
    draw.ellipse((23, 23, 41, 41), fill=GRAY)
    draw.ellipse((26, 25, 38, 39), fill=PURPLE)
    draw.ellipse((29, 28, 35, 36), fill=GOLD)
    for x, y in ((18, 22), (31, 19), (44, 22), (46, 31), (43, 42), (31, 45), (19, 42), (16, 31)):
        draw.rectangle((x - 2, y - 3, x + 2, y + 3), fill=STONE)
        draw.point((x, y - 1), fill=GOLD)
    draw.rectangle((27, 52, 37, 59), fill=GRAY)
    draw.ellipse((28, 52, 36, 57), fill=PURPLE)
    draw.point((32, 54), fill=ORANGE)
    return image


def validate(path, image):
    if image.size != (64, 64):
        raise ValueError(f"{path.name}: expected 64x64, got {image.size}")
    invalid_colors = {color for color in image.getdata() if color not in PALETTE}
    if invalid_colors:
        raise ValueError(f"{path.name}: non-palette colors {invalid_colors}")
    image.save(path)
    print(f"  palette OK  {path.name} 64x64")


def main():
    root = Path(__file__).resolve().parents[1]
    output = root / "src" / "Assets" / "Sprites" / "Minimaps"
    output.mkdir(parents=True, exist_ok=True)
    maps = (
        ("minimap_cottage.png", make_cottage),
        ("minimap_farm.png", make_farm),
        ("minimap_forest_path.png", make_forest_path),
        ("minimap_city.png", make_city),
        ("minimap_mages_tower.png", make_mages_tower),
        ("minimap_portal_chamber.png", make_portal_chamber),
    )
    for name, draw_map in maps:
        validate(output / name, draw_map())


if __name__ == "__main__":
    main()