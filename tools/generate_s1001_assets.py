#!/usr/bin/env python3
"""Generate palette-compliant pixel art for S10-01 Portal Chamber."""

from math import sqrt
from pathlib import Path

from PIL import Image

TRANSPARENT = (0, 0, 0, 0)
BLACK = (0x1A, 0x1A, 0x1A, 255)
DARK_GRAY = (0x3D, 0x3D, 0x3D, 255)
GRAY = (0x5A, 0x5A, 0x5A, 255)
BROWN = (0x5C, 0x3D, 0x2E, 255)
WARM_BROWN = (0x8B, 0x73, 0x55, 255)
PURPLE = (0x2A, 0x1A, 0x4A, 255)
GOLD = (0xFF, 0xD7, 0x00, 255)
WHITE = (0xCC, 0xCC, 0xCC, 255)
ORANGE = (0xFF, 0x6B, 0x00, 255)

PALETTE = {TRANSPARENT, BLACK, DARK_GRAY, GRAY, BROWN, WARM_BROWN, PURPLE, GOLD, WHITE, ORANGE}


def put_rect(image, x, y, width, height, color):
    for row in range(y, y + height):
        for column in range(x, x + width):
            if 0 <= column < image.width and 0 <= row < image.height:
                image.putpixel((column, row), color)


def generate_void_floor(path):
    image = Image.new("RGBA", (64, 32), TRANSPARENT)
    for row in range(32):
        half_width = row * 2 if row <= 16 else (31 - row) * 2
        for column in range(32 - half_width, 33 + half_width):
            if 0 <= column < 64:
                image.putpixel((column, row), PURPLE)
    for x, y in ((13, 8), (14, 8), (15, 8), (15, 9), (16, 10), (44, 20), (45, 20), (46, 20), (43, 21)):
        if 0 <= x < 64 and 0 <= y < 32 and image.getpixel((x, y))[3]:
            image.putpixel((x, y), DARK_GRAY)
    image.save(path)


def generate_rune_floor(path):
    image = Image.new("RGBA", (64, 32), TRANSPARENT)
    generate_void_floor(path)
    image = Image.open(path).convert("RGBA")
    for x, y in ((29, 8), (30, 8), (31, 8), (32, 8), (33, 8), (34, 8),
                 (28, 9), (35, 9), (27, 10), (36, 10), (27, 11), (36, 11),
                 (28, 12), (35, 12), (29, 13), (30, 13), (31, 13), (32, 13), (33, 13), (34, 13),
                 (31, 9), (32, 10), (33, 11), (30, 11), (31, 12)):
        image.putpixel((x, y), GOLD)
    for x, y in ((30, 10), (34, 10), (29, 11), (34, 12)):
        image.putpixel((x, y), WHITE)
    image.save(path)


def generate_magic_pillar(path):
    image = Image.new("RGBA", (64, 112), TRANSPARENT)
    put_rect(image, 10, 99, 44, 7, BLACK)
    put_rect(image, 13, 89, 38, 10, PURPLE)
    put_rect(image, 17, 22, 30, 69, DARK_GRAY)
    put_rect(image, 21, 19, 22, 73, GRAY)
    put_rect(image, 25, 27, 14, 58, PURPLE)
    put_rect(image, 29, 30, 6, 51, BLACK)
    put_rect(image, 16, 15, 32, 7, BROWN)
    put_rect(image, 20, 9, 24, 7, GRAY)
    put_rect(image, 25, 3, 14, 7, DARK_GRAY)
    put_rect(image, 30, 0, 4, 4, GOLD)
    put_rect(image, 13, 91, 38, 5, GOLD)
    image.save(path)


def generate_chains(path):
    image = Image.new("RGBA", (64, 96), TRANSPARENT)
    put_rect(image, 8, 3, 48, 6, DARK_GRAY)
    put_rect(image, 13, 9, 4, 8, GRAY)
    put_rect(image, 47, 9, 4, 8, GRAY)
    for x in (15, 43):
        for y in range(16, 78, 9):
            put_rect(image, x, y, 6, 4, GRAY if (y // 9) % 2 else DARK_GRAY)
            put_rect(image, x + 1, y + 4, 4, 5, BLACK)
    put_rect(image, 12, 77, 10, 5, GRAY)
    put_rect(image, 42, 77, 10, 5, GRAY)
    put_rect(image, 9, 83, 14, 4, GOLD)
    put_rect(image, 41, 83, 14, 4, GOLD)
    image.save(path)


def generate_child_cage(path):
    image = Image.new("RGBA", (96, 96), TRANSPARENT)
    put_rect(image, 8, 83, 80, 7, BLACK)
    put_rect(image, 13, 78, 70, 7, BROWN)
    put_rect(image, 17, 18, 6, 62, DARK_GRAY)
    put_rect(image, 73, 18, 6, 62, DARK_GRAY)
    put_rect(image, 23, 20, 50, 5, GRAY)
    put_rect(image, 23, 70, 50, 5, GRAY)
    for x in (29, 39, 49, 59, 69):
        put_rect(image, x, 25, 3, 45, GRAY if x % 2 else DARK_GRAY)
    put_rect(image, 13, 13, 70, 8, WARM_BROWN)
    put_rect(image, 18, 8, 60, 6, DARK_GRAY)
    put_rect(image, 43, 46, 9, 14, PURPLE)
    put_rect(image, 46, 41, 4, 6, GOLD)
    image.save(path)


def generate_active_portal(path):
    image = Image.new("RGBA", (256, 96), TRANSPARENT)
    for frame in range(4):
        offset = frame * 64
        put_rect(image, offset + 8, 82, 48, 5, BLACK)
        put_rect(image, offset + 13, 77, 38, 5, PURPLE)
        for y in range(16, 80):
            normalized = (y - 48) / 32
            if abs(normalized) > 1:
                continue
            outer = round(23 * sqrt(1 - normalized * normalized))
            inner = round(17 * sqrt(1 - normalized * normalized))
            for side in (-1, 1):
                for thickness in range(4):
                    x = offset + 32 + side * (outer - thickness)
                    image.putpixel((x, y), GOLD if thickness == 0 and y % 3 == frame % 3 else GRAY)
            for x in range(offset + 32 - inner, offset + 33 + inner):
                image.putpixel((x, y), PURPLE if (x + y + frame * 3) % 7 else DARK_GRAY)
        for step in range(12):
            x = 24 + ((step * 7 + frame * 5) % 17)
            y = 25 + ((step * 11 + frame * 3) % 43)
            color = (GOLD, WHITE, ORANGE, PURPLE)[(step + frame) % 4]
            image.putpixel((offset + x, y), color)
            if y + 1 < 78:
                image.putpixel((offset + x, y + 1), color)
        put_rect(image, offset + 12, 78, 40, 4, GRAY)
    image.save(path)


def validate(path, expected_size):
    image = Image.open(path).convert("RGBA")
    if image.size != expected_size:
        raise ValueError(f"{path.name}: expected {expected_size}, got {image.size}")
    invalid_colors = {color for color in image.getdata() if color not in PALETTE}
    if invalid_colors:
        raise ValueError(f"{path.name}: non-palette colors {invalid_colors}")
    print(f"  palette OK  {path.name} {image.width}x{image.height}")


def main():
    root = Path(__file__).resolve().parents[1]
    tiles = root / "src" / "Assets" / "Sprites" / "Tiles"
    props = root / "src" / "Assets" / "Sprites" / "Props"
    outputs = (
        (tiles / "tile_void_floor.png", (64, 32), generate_void_floor),
        (tiles / "tile_rune_floor.png", (64, 32), generate_rune_floor),
        (props / "prop_portal_active.png", (256, 96), generate_active_portal),
        (props / "prop_magic_pillar.png", (64, 112), generate_magic_pillar),
        (props / "prop_chains.png", (64, 96), generate_chains),
        (props / "prop_child_cage.png", (96, 96), generate_child_cage),
    )
    for path, expected_size, generator in outputs:
        generator(path)
        validate(path, expected_size)


if __name__ == "__main__":
    main()