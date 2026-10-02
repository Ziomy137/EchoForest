#!/usr/bin/env python3
"""Generate palette-compliant pixel-art tile and prop assets for S8-03."""

from pathlib import Path

from PIL import Image

TRANSPARENT = (0, 0, 0, 0)
DEEP_BLACK = (0x1A, 0x1A, 0x1A, 255)
DARK_BROWN = (0x2D, 0x24, 0x16, 255)
DARK_GRAY = (0x3D, 0x3D, 0x3D, 255)
MEDIUM_GRAY = (0x5A, 0x5A, 0x5A, 255)
WARM_BROWN = (0x8B, 0x73, 0x55, 255)
DARK_LEATHER = (0x5C, 0x3D, 0x2E, 255)
DARK_GREEN = (0x1A, 0x3A, 0x1A, 255)

PALETTE = {
    TRANSPARENT,
    DEEP_BLACK,
    DARK_BROWN,
    DARK_GRAY,
    MEDIUM_GRAY,
    WARM_BROWN,
    DARK_LEATHER,
    DARK_GREEN,
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


def generate_forest_floor(path):
    image = new_tile(DARK_GREEN)
    for x, y in ((19, 13), (26, 19), (36, 12), (43, 18), (31, 23)):
        put_rect(image, x, y, 3, 1, DARK_BROWN)
    image.save(path)


def generate_forest_shadow(path):
    image = new_tile(DARK_GREEN)
    for x, y, width in ((18, 12, 12), (27, 16, 21), (21, 21, 16)):
        put_rect(image, x, y, width, 3, DEEP_BLACK)
    image.save(path)


def generate_mud(path):
    image = new_tile(WARM_BROWN)
    for x, y, width in ((18, 13, 10), (31, 18, 17), (24, 23, 14)):
        put_rect(image, x, y, width, 2, DARK_BROWN)
        put_rect(image, x + 3, y - 1, 3, 1, DARK_GRAY)
    image.save(path)


def generate_rock_small(path):
    image = new_tile(DARK_BROWN)
    for x, y, width, height in ((20, 16, 7, 4), (34, 12, 8, 5), (41, 20, 5, 3)):
        put_rect(image, x, y, width, height, MEDIUM_GRAY)
        put_rect(image, x, y + height - 1, width, 1, DARK_GRAY)
    image.save(path)


def generate_dense_tree(path):
    image = Image.new("RGBA", (64, 96), TRANSPARENT)
    put_rect(image, 27, 62, 10, 34, DARK_BROWN)
    put_rect(image, 23, 63, 18, 8, DARK_LEATHER)
    put_rect(image, 16, 40, 32, 28, DARK_GREEN)
    put_rect(image, 8, 26, 48, 26, DARK_GREEN)
    put_rect(image, 16, 10, 32, 22, DARK_GREEN)
    put_rect(image, 11, 39, 8, 4, DARK_LEATHER)
    put_rect(image, 45, 39, 8, 4, DARK_LEATHER)
    put_rect(image, 22, 20, 20, 4, DEEP_BLACK)
    image.save(path)


def generate_fallen_log(path):
    image = Image.new("RGBA", (96, 48), TRANSPARENT)
    put_rect(image, 8, 24, 80, 16, DARK_BROWN)
    put_rect(image, 12, 20, 72, 6, WARM_BROWN)
    put_rect(image, 8, 28, 80, 4, DARK_LEATHER)
    put_rect(image, 4, 24, 8, 16, DEEP_BLACK)
    put_rect(image, 84, 24, 8, 16, DEEP_BLACK)
    put_rect(image, 30, 21, 4, 18, DARK_LEATHER)
    put_rect(image, 58, 21, 4, 18, DARK_LEATHER)
    image.save(path)


def generate_mushrooms(path):
    image = Image.new("RGBA", (32, 24), TRANSPARENT)
    put_rect(image, 7, 13, 5, 8, WARM_BROWN)
    put_rect(image, 4, 9, 11, 5, MEDIUM_GRAY)
    put_rect(image, 18, 15, 4, 6, WARM_BROWN)
    put_rect(image, 15, 12, 10, 4, MEDIUM_GRAY)
    put_rect(image, 1, 20, 29, 3, DARK_GREEN)
    image.save(path)


def generate_boulder(path):
    image = Image.new("RGBA", (64, 64), TRANSPARENT)
    put_rect(image, 12, 32, 40, 24, DARK_GRAY)
    put_rect(image, 18, 20, 28, 16, MEDIUM_GRAY)
    put_rect(image, 24, 14, 16, 8, MEDIUM_GRAY)
    put_rect(image, 12, 52, 40, 4, DEEP_BLACK)
    put_rect(image, 18, 36, 10, 4, DEEP_BLACK)
    put_rect(image, 38, 28, 6, 5, DEEP_BLACK)
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
        (tiles / "tile_forest_floor.png", (64, 32), generate_forest_floor),
        (tiles / "tile_forest_shadow.png", (64, 32), generate_forest_shadow),
        (tiles / "tile_mud.png", (64, 32), generate_mud),
        (tiles / "tile_rock_small.png", (64, 32), generate_rock_small),
        (props / "prop_dense_tree.png", (64, 96), generate_dense_tree),
        (props / "prop_fallen_log.png", (96, 48), generate_fallen_log),
        (props / "prop_mushrooms.png", (32, 24), generate_mushrooms),
        (props / "prop_boulder.png", (64, 64), generate_boulder),
    )
    for path, expected_size, generator in outputs:
        generator(path)
        validate(path, expected_size)


if __name__ == "__main__":
    main()