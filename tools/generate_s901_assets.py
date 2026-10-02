#!/usr/bin/env python3
"""Generate palette-compliant pixel-art tile and prop assets for S9-01."""

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


def generate_cobble(path):
    image = new_tile(MEDIUM_GRAY)
    for x, y, width in ((19, 10, 10), (32, 11, 13), (16, 16, 12), (30, 17, 14), (22, 22, 11), (38, 22, 10)):
        put_rect(image, x, y, width, 1, WARM_BROWN)
        put_rect(image, x, y + 1, 1, 4, DARK_GRAY)
    image.save(path)


def generate_cobble_variation(path):
    image = new_tile(MEDIUM_GRAY)
    for x, y, width in ((18, 11, 12), (33, 10, 12), (17, 19, 13), (32, 18, 13), (24, 24, 12)):
        put_rect(image, x, y, width, 1, WARM_BROWN)
        put_rect(image, x + width - 1, y + 1, 1, 3, DARK_GRAY)
    put_rect(image, 28, 14, 4, 1, DEEP_BLACK)
    put_rect(image, 31, 15, 1, 3, DEEP_BLACK)
    image.save(path)


def generate_city_stone(path):
    image = new_tile(MEDIUM_GRAY)
    for x, y in ((20, 12), (34, 15), (25, 20), (40, 19)):
        put_rect(image, x, y, 8, 1, DARK_GRAY)
        put_rect(image, x + 3, y + 1, 1, 3, DARK_GRAY)
    image.save(path)


def generate_city_wall(path):
    image = new_tile(MEDIUM_GRAY)
    put_rect(image, 18, 12, 29, 2, WARM_BROWN)
    put_rect(image, 22, 17, 23, 2, DARK_GRAY)
    put_rect(image, 26, 22, 15, 2, WARM_BROWN)
    for x, y in ((25, 14), (38, 19)):
        put_rect(image, x, y, 2, 3, DARK_GRAY)
    image.save(path)


def generate_city_roof(path):
    image = new_tile(DARK_GRAY)
    put_rect(image, 18, 11, 29, 2, DARK_LEATHER)
    put_rect(image, 21, 15, 23, 1, DARK_LEATHER)
    put_rect(image, 24, 19, 17, 1, DARK_LEATHER)
    put_rect(image, 27, 23, 11, 1, DARK_LEATHER)
    image.save(path)


def generate_stall_top(path):
    image = new_tile(WARM_BROWN)
    put_rect(image, 19, 12, 27, 3, GOLD)
    put_rect(image, 22, 17, 21, 2, DARK_RED)
    put_rect(image, 25, 21, 15, 2, GOLD)
    image.save(path)


def generate_lamppost(path):
    image = Image.new("RGBA", (24, 80), TRANSPARENT)
    put_rect(image, 9, 24, 6, 50, DARK_LEATHER)
    put_rect(image, 5, 72, 14, 5, DARK_BROWN)
    put_rect(image, 6, 17, 12, 5, DARK_LEATHER)
    put_rect(image, 8, 7, 8, 11, GOLD)
    put_rect(image, 6, 4, 12, 3, DARK_BROWN)
    put_rect(image, 7, 8, 2, 8, WARM_BROWN)
    put_rect(image, 3, 21, 18, 3, DARK_BROWN)
    image.save(path)


def generate_fountain(path):
    image = Image.new("RGBA", (96, 80), TRANSPARENT)
    put_rect(image, 9, 49, 78, 19, DARK_GRAY)
    put_rect(image, 4, 58, 88, 16, MEDIUM_GRAY)
    put_rect(image, 13, 53, 70, 4, WARM_BROWN)
    put_rect(image, 23, 32, 50, 22, MEDIUM_GRAY)
    put_rect(image, 28, 36, 40, 14, DARK_GRAY)
    put_rect(image, 32, 37, 32, 9, DEEP_WATER)
    put_rect(image, 43, 15, 10, 20, WARM_BROWN)
    put_rect(image, 39, 11, 18, 5, MEDIUM_GRAY)
    put_rect(image, 46, 5, 4, 7, DEEP_WATER)
    put_rect(image, 10, 72, 76, 4, DARK_BROWN)
    image.save(path)


def generate_market_stall(path):
    image = Image.new("RGBA", (96, 80), TRANSPARENT)
    put_rect(image, 11, 20, 74, 10, DARK_LEATHER)
    put_rect(image, 15, 29, 5, 43, DARK_BROWN)
    put_rect(image, 76, 29, 5, 43, DARK_BROWN)
    put_rect(image, 9, 18, 78, 6, GOLD)
    put_rect(image, 14, 24, 68, 4, WARM_BROWN)
    for x in range(14, 82, 16):
        put_rect(image, x, 28, 8, 6, DARK_RED if x % 2 else WARM_BROWN)
    put_rect(image, 18, 43, 60, 8, WARM_BROWN)
    put_rect(image, 20, 51, 56, 17, DARK_BROWN)
    put_rect(image, 23, 54, 50, 10, DARK_LEATHER)
    put_rect(image, 20, 71, 56, 4, DARK_BROWN)
    image.save(path)


def generate_notice_board(path):
    image = Image.new("RGBA", (48, 64), TRANSPARENT)
    put_rect(image, 21, 35, 6, 29, DARK_BROWN)
    put_rect(image, 8, 7, 32, 32, DARK_LEATHER)
    put_rect(image, 12, 11, 24, 23, WARM_BROWN)
    put_rect(image, 16, 15, 16, 2, DARK_BROWN)
    put_rect(image, 16, 20, 12, 2, DARK_BROWN)
    put_rect(image, 16, 25, 15, 2, GOLD)
    put_rect(image, 4, 37, 40, 4, DARK_BROWN)
    image.save(path)


def generate_city_gate(path):
    image = Image.new("RGBA", (160, 128), TRANSPARENT)
    put_rect(image, 8, 25, 34, 96, MEDIUM_GRAY)
    put_rect(image, 118, 25, 34, 96, MEDIUM_GRAY)
    put_rect(image, 2, 18, 46, 12, WARM_BROWN)
    put_rect(image, 112, 18, 46, 12, WARM_BROWN)
    put_rect(image, 32, 9, 96, 15, DARK_GRAY)
    put_rect(image, 38, 24, 84, 7, WARM_BROWN)
    put_rect(image, 52, 38, 56, 83, DARK_BROWN)
    put_rect(image, 59, 42, 42, 79, DARK_LEATHER)
    for y in range(48, 112, 12):
        put_rect(image, 59, y, 42, 2, DARK_BROWN)
    put_rect(image, 77, 73, 5, 5, GOLD)
    for x in (14, 29, 130, 145):
        put_rect(image, x, 35, 5, 77, DARK_GRAY)
    put_rect(image, 3, 116, 154, 8, DARK_BROWN)
    image.save(path)


def generate_tower_wall(path):
    image = Image.new("RGBA", (192, 224), TRANSPARENT)
    put_rect(image, 27, 37, 138, 174, MEDIUM_GRAY)
    put_rect(image, 18, 29, 156, 15, DARK_GRAY)
    put_rect(image, 34, 15, 124, 18, WARM_BROWN)
    put_rect(image, 44, 3, 104, 16, DARK_GRAY)
    for x in (38, 151):
        put_rect(image, x, 58, 11, 24, DARK_BROWN)
        put_rect(image, x + 3, 61, 5, 18, DARK_LEATHER)
        put_rect(image, x, 105, 11, 24, DARK_BROWN)
        put_rect(image, x + 3, 108, 5, 18, DARK_LEATHER)
    put_rect(image, 70, 122, 52, 89, DARK_BROWN)
    put_rect(image, 77, 129, 38, 82, DARK_LEATHER)
    put_rect(image, 94, 163, 5, 5, GOLD)
    for x in range(34, 158, 16):
        put_rect(image, x, 211, 9, 8, DARK_GRAY)
    image.save(path)


def generate_tower_door(path):
    image = Image.new("RGBA", (64, 96), TRANSPARENT)
    put_rect(image, 7, 3, 50, 93, DARK_GRAY)
    put_rect(image, 13, 9, 38, 87, DARK_BROWN)
    put_rect(image, 18, 14, 28, 82, DARK_LEATHER)
    put_rect(image, 18, 50, 28, 4, DARK_BROWN)
    put_rect(image, 39, 54, 4, 4, GOLD)
    for y in range(18, 47, 12):
        put_rect(image, 21, y, 22, 2, WARM_BROWN)
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
    tiles.mkdir(parents=True, exist_ok=True)
    props.mkdir(parents=True, exist_ok=True)

    outputs = (
        (tiles / "tile_cobble.png", (64, 32), generate_cobble),
        (tiles / "tile_cobble_var.png", (64, 32), generate_cobble_variation),
        (tiles / "tile_city_stone.png", (64, 32), generate_city_stone),
        (tiles / "tile_city_wall.png", (64, 32), generate_city_wall),
        (tiles / "tile_city_roof.png", (64, 32), generate_city_roof),
        (tiles / "tile_stall_top.png", (64, 32), generate_stall_top),
        (props / "prop_lamppost.png", (24, 80), generate_lamppost),
        (props / "prop_fountain.png", (96, 80), generate_fountain),
        (props / "prop_market_stall.png", (96, 80), generate_market_stall),
        (props / "prop_notice_board.png", (48, 64), generate_notice_board),
        (props / "prop_city_gate.png", (160, 128), generate_city_gate),
        (props / "prop_tower_wall.png", (192, 224), generate_tower_wall),
        (props / "prop_tower_door.png", (64, 96), generate_tower_door),
    )
    for path, expected_size, generator in outputs:
        generator(path)
        validate(path, expected_size)


if __name__ == "__main__":
    main()