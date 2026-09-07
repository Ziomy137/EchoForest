#!/usr/bin/env python3
"""Generate palette-compliant pixel-art tile and prop assets for S8-02."""

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
GOLD = (0xFF, 0xD7, 0x00, 255)
DARK_GREEN = (0x1A, 0x3A, 0x1A, 255)
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
    GOLD,
    DARK_GREEN,
    DEEP_WATER,
}


def put_rect(image, x, y, width, height, color):
    for row in range(y, y + height):
        for column in range(x, x + width):
            if 0 <= column < image.width and 0 <= row < image.height:
                image.putpixel((column, row), color)


def draw_diamond(image, fill, outline=DARK_BROWN):
    center_x = image.width // 2
    half_width = image.width // 2
    half_height = image.height // 2
    for y in range(image.height):
        horizontal_radius = int(half_width * (1 - abs(y - half_height) / half_height))
        start_x = center_x - horizontal_radius
        end_x = min(image.width - 1, center_x + horizontal_radius)
        for x in range(start_x, end_x + 1):
            is_edge = x in (start_x, end_x) or y in (0, image.height - 1)
            image.putpixel((x, y), outline if is_edge else fill)


def new_tile(fill):
    image = Image.new("RGBA", (64, 32), TRANSPARENT)
    draw_diamond(image, fill)
    return image


def generate_crop_young(path):
    image = new_tile(WARM_BROWN)
    for x in range(18, 47, 7):
        put_rect(image, x, 13, 2, 7, DARK_GREEN)
        put_rect(image, x - 2, 15, 6, 2, DARK_GREEN)
    image.save(path)


def generate_crop_mature(path):
    image = new_tile(WARM_BROWN)
    for x in range(17, 48, 7):
        put_rect(image, x, 11, 2, 10, DARK_GREEN)
        put_rect(image, x - 1, 10, 4, 4, GOLD)
    image.save(path)


def generate_soil_dry(path):
    image = new_tile(WARM_BROWN)
    for x, y in ((21, 12), (30, 17), (39, 13), (27, 21), (44, 18)):
        put_rect(image, x, y, 5, 1, DARK_GRAY)
        put_rect(image, x + 2, y - 1, 1, 3, DARK_GRAY)
    image.save(path)


def generate_irrigation(path):
    image = new_tile(MEDIUM_GRAY)
    put_rect(image, 17, 13, 30, 7, DEEP_WATER)
    put_rect(image, 17, 12, 30, 1, DARK_BROWN)
    put_rect(image, 17, 20, 30, 1, DARK_BROWN)
    image.save(path)


def generate_hay(path):
    image = new_tile(WARM_BROWN)
    for y in range(11, 23, 3):
        put_rect(image, 18, y, 28, 1, GOLD)
    image.save(path)


def generate_barn(path):
    image = Image.new("RGBA", (192, 160), TRANSPARENT)
    put_rect(image, 16, 64, 160, 80, WARM_BROWN)
    put_rect(image, 8, 56, 176, 12, DARK_LEATHER)
    for step in range(8):
        put_rect(image, 24 + step * 8, 48 - step * 6, 144 - step * 16, 6, DARK_RED)
    put_rect(image, 76, 96, 40, 48, DARK_BROWN)
    put_rect(image, 84, 100, 24, 44, DARK_LEATHER)
    put_rect(image, 20, 76, 36, 28, DARK_GRAY)
    put_rect(image, 136, 76, 36, 28, DARK_GRAY)
    put_rect(image, 91, 116, 4, 4, GOLD)
    image.save(path)


def generate_scarecrow(path):
    image = Image.new("RGBA", (32, 64), TRANSPARENT)
    put_rect(image, 14, 22, 4, 42, DARK_BROWN)
    put_rect(image, 4, 30, 24, 4, DARK_BROWN)
    put_rect(image, 8, 32, 16, 12, WARM_BROWN)
    put_rect(image, 10, 10, 12, 12, WARM_BROWN)
    put_rect(image, 8, 7, 16, 4, GOLD)
    put_rect(image, 12, 14, 2, 2, DARK_BROWN)
    put_rect(image, 18, 14, 2, 2, DARK_BROWN)
    image.save(path)


def generate_plow(path):
    image = Image.new("RGBA", (48, 32), TRANSPARENT)
    put_rect(image, 6, 24, 36, 4, DARK_LEATHER)
    put_rect(image, 30, 5, 4, 21, DARK_BROWN)
    put_rect(image, 22, 4, 16, 4, WARM_BROWN)
    put_rect(image, 4, 28, 40, 4, MEDIUM_GRAY)
    image.save(path)


def validate(path, expected_size):
    image = Image.open(path).convert("RGBA")
    if image.size != expected_size:
        raise ValueError(f"{path.name}: expected {expected_size}, got {image.size}")
    invalid_colors = {color for color in image.get_flattened_data() if color not in PALETTE}
    if invalid_colors:
        raise ValueError(f"{path.name}: non-palette colors {invalid_colors}")
    print(f"  palette OK  {path.name} {image.width}x{image.height}")


def main():
    root = Path(__file__).resolve().parents[1]
    tiles = root / "src" / "Assets" / "Sprites" / "Tiles"
    props = root / "src" / "Assets" / "Sprites" / "Props"
    tiles.mkdir(parents=True, exist_ok=True)
    props.mkdir(parents=True, exist_ok=True)

    outputs = (
        (tiles / "tile_crop_young.png", (64, 32), generate_crop_young),
        (tiles / "tile_crop_mature.png", (64, 32), generate_crop_mature),
        (tiles / "tile_soil_dry.png", (64, 32), generate_soil_dry),
        (tiles / "tile_irrigation.png", (64, 32), generate_irrigation),
        (tiles / "tile_hay.png", (64, 32), generate_hay),
        (props / "prop_barn.png", (192, 160), generate_barn),
        (props / "prop_scarecrow.png", (32, 64), generate_scarecrow),
        (props / "prop_plow.png", (48, 32), generate_plow),
    )
    for path, expected_size, generator in outputs:
        generator(path)
        validate(path, expected_size)


if __name__ == "__main__":
    main()