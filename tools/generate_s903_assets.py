#!/usr/bin/env python3
"""Generate palette-compliant pixel-art props for S9-03 Mage's Tower."""

from pathlib import Path

from PIL import Image

TRANSPARENT = (0, 0, 0, 0)
DEEP_BLACK = (0x1A, 0x1A, 0x1A, 255)
DARK_BROWN = (0x2D, 0x24, 0x16, 255)
DARK_GRAY = (0x3D, 0x3D, 0x3D, 255)
MEDIUM_GRAY = (0x5A, 0x5A, 0x5A, 255)
WARM_BROWN = (0x8B, 0x73, 0x55, 255)
DARK_LEATHER = (0x5C, 0x3D, 0x2E, 255)
DARK_RED = (0x8B, 0x00, 0x00, 255)
DEEP_PURPLE = (0x2A, 0x1A, 0x4A, 255)
DARK_ORANGE = (0xFF, 0x6B, 0x00, 255)
GOLD = (0xFF, 0xD7, 0x00, 255)
DEEP_WATER = (0x1A, 0x3A, 0x5C, 255)

PALETTE = {
    TRANSPARENT,
    DEEP_BLACK,
    DARK_BROWN,
    DARK_GRAY,
    MEDIUM_GRAY,
    WARM_BROWN,
    DARK_LEATHER,
    DARK_RED,
    DEEP_PURPLE,
    DARK_ORANGE,
    GOLD,
    DEEP_WATER,
}


def put_rect(image, x, y, width, height, color):
    for row in range(y, y + height):
        for column in range(x, x + width):
            if 0 <= column < image.width and 0 <= row < image.height:
                image.putpixel((column, row), color)


def generate_portal_inactive(path):
    image = Image.new("RGBA", (64, 64), TRANSPARENT)
    put_rect(image, 9, 13, 46, 38, DARK_GRAY)
    put_rect(image, 14, 9, 36, 46, MEDIUM_GRAY)
    put_rect(image, 19, 14, 26, 36, DARK_BROWN)
    put_rect(image, 23, 18, 18, 28, DEEP_PURPLE)
    put_rect(image, 28, 21, 3, 7, DEEP_PURPLE)
    put_rect(image, 34, 29, 3, 7, DEEP_PURPLE)
    put_rect(image, 27, 39, 9, 3, GOLD)
    put_rect(image, 5, 52, 54, 6, DARK_BROWN)
    put_rect(image, 12, 57, 40, 4, DEEP_BLACK)
    image.save(path)


def generate_bookshelf(path):
    image = Image.new("RGBA", (64, 96), TRANSPARENT)
    put_rect(image, 6, 3, 52, 90, DARK_BROWN)
    put_rect(image, 10, 7, 44, 82, DARK_LEATHER)
    for y in (27, 48, 69):
        put_rect(image, 9, y, 46, 4, WARM_BROWN)
    books = [DARK_RED, DEEP_PURPLE, GOLD, DARK_GRAY, WARM_BROWN, DEEP_WATER]
    for shelf_y in (10, 31, 52, 73):
        x = 12
        index = 0
        while x < 51:
            width = 4 + (index % 3)
            put_rect(image, x, shelf_y, width, 15, books[index % len(books)])
            put_rect(image, x + 1, shelf_y + 2, 1, 10, DARK_BROWN)
            x += width + 2
            index += 1
    put_rect(image, 4, 90, 56, 4, DEEP_BLACK)
    image.save(path)


def generate_alchemy_table(path):
    image = Image.new("RGBA", (64, 48), TRANSPARENT)
    put_rect(image, 7, 13, 50, 11, DARK_BROWN)
    put_rect(image, 10, 9, 44, 8, WARM_BROWN)
    put_rect(image, 13, 20, 6, 24, DARK_LEATHER)
    put_rect(image, 45, 20, 6, 24, DARK_LEATHER)
    put_rect(image, 11, 42, 42, 4, DEEP_BLACK)
    put_rect(image, 18, 5, 7, 5, DEEP_WATER)
    put_rect(image, 20, 2, 3, 4, MEDIUM_GRAY)
    put_rect(image, 34, 4, 10, 6, DEEP_PURPLE)
    put_rect(image, 37, 1, 4, 4, GOLD)
    image.save(path)


def generate_fireplace(path):
    image = Image.new("RGBA", (64, 64), TRANSPARENT)
    put_rect(image, 6, 9, 52, 48, MEDIUM_GRAY)
    put_rect(image, 11, 14, 42, 39, DARK_GRAY)
    put_rect(image, 16, 25, 32, 28, DEEP_BLACK)
    put_rect(image, 19, 43, 26, 8, DARK_BROWN)
    put_rect(image, 23, 34, 5, 12, DARK_ORANGE)
    put_rect(image, 29, 29, 7, 17, DARK_RED)
    put_rect(image, 37, 35, 5, 11, DARK_ORANGE)
    put_rect(image, 3, 55, 58, 6, DARK_BROWN)
    put_rect(image, 9, 61, 46, 3, DEEP_BLACK)
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
    props = root / "src" / "Assets" / "Sprites" / "Props"
    props.mkdir(parents=True, exist_ok=True)

    outputs = (
        (props / "prop_portal_inactive.png", (64, 64), generate_portal_inactive),
        (props / "prop_bookshelf.png", (64, 96), generate_bookshelf),
        (props / "prop_alchemy_table.png", (64, 48), generate_alchemy_table),
        (props / "prop_fireplace.png", (64, 64), generate_fireplace),
    )
    for path, expected_size, generator in outputs:
        generator(path)
        validate(path, expected_size)


if __name__ == "__main__":
    main()