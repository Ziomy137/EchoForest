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
TRANSPARENT = (0, 0, 0, 0)

PALETTE = {BLACK, DARK_BROWN, DARK_GRAY, GRAY, STONE, LEATHER, RED, PURPLE, ORANGE, GOLD, GREEN, WATER, WHITE, LIGHT_GRAY, TRANSPARENT}

ISO_COLUMNS = 8
ISO_ROWS = 8
ISO_TILE_HALF_WIDTH = 4
ISO_TILE_HALF_HEIGHT = 2
ISO_ORIGIN_X = 32
ISO_ORIGIN_Y = 18


def base(color):
    image = Image.new("RGBA", (64, 64), TRANSPARENT)
    for row in range(ISO_ROWS):
        for column in range(ISO_COLUMNS):
            draw_iso_tile(image, column, row, color)
    return image


def iso_center(column, row):
    return (
        ISO_ORIGIN_X + (column - row) * ISO_TILE_HALF_WIDTH,
        ISO_ORIGIN_Y + (column + row) * ISO_TILE_HALF_HEIGHT,
    )


def draw_iso_tile(image, column, row, color, outline=BLACK):
    center_x, center_y = iso_center(column, row)
    points = [
        (center_x, center_y - ISO_TILE_HALF_HEIGHT),
        (center_x + ISO_TILE_HALF_WIDTH, center_y),
        (center_x, center_y + ISO_TILE_HALF_HEIGHT),
        (center_x - ISO_TILE_HALF_WIDTH, center_y),
    ]
    draw = ImageDraw.Draw(image)
    draw.polygon(points, fill=color)
    draw.line(points + [points[0]], fill=outline, width=1)


def paint_cells(image, cells, color):
    for column, row in cells:
        if 0 <= column < ISO_COLUMNS and 0 <= row < ISO_ROWS:
            draw_iso_tile(image, column, row, color)


def draw_path(image, cells, color=STONE, edges=LIGHT_GRAY):
    paint_cells(image, cells, color)
    for column, row in cells:
        center_x, center_y = iso_center(column, row)
        ImageDraw.Draw(image).line(
            (center_x - 2, center_y, center_x + 2, center_y), fill=edges, width=1)


def draw_building(image, column, row, roof, left_wall=LEATHER, right_wall=DARK_GRAY, height=7):
    center_x, center_y = iso_center(column, row)
    half_width = 6
    half_depth = 3
    ground = [
        (center_x, center_y - half_depth),
        (center_x + half_width, center_y),
        (center_x, center_y + half_depth),
        (center_x - half_width, center_y),
    ]
    top = [(x, y - height) for x, y in ground]
    draw = ImageDraw.Draw(image)
    draw.polygon((ground[3], ground[2], top[2], top[3]), fill=left_wall)
    draw.polygon((ground[1], ground[2], top[2], top[1]), fill=right_wall)
    draw.polygon(top, fill=roof)
    draw.line(top + [top[0]], fill=BLACK, width=1)
    draw.line((top[3], top[2]), fill=LIGHT_GRAY, width=1)


def draw_tree(image, column, row):
    center_x, center_y = iso_center(column, row)
    draw = ImageDraw.Draw(image)
    draw.line((center_x, center_y, center_x, center_y - 8), fill=DARK_BROWN, width=2)
    draw.polygon(
        ((center_x, center_y - 17), (center_x + 5, center_y - 10),
         (center_x, center_y - 5), (center_x - 5, center_y - 10)),
        fill=GREEN,
        outline=BLACK,
    )


def draw_furrows(image, cells, color=GOLD):
    draw = ImageDraw.Draw(image)
    for column, row in cells:
        center_x, center_y = iso_center(column, row)
        draw.line((center_x - 2, center_y + 1, center_x + 2, center_y - 1), fill=color, width=1)


def make_cottage():
    image = base(GREEN)
    draw_path(image, [(7, 7), (6, 6), (5, 5), (4, 4), (3, 4), (2, 5), (1, 5), (0, 6)])
    draw_building(image, 2, 3, RED, LEATHER, DARK_BROWN, height=10)
    paint_cells(image, [(6, 2), (7, 2)], WATER)
    center_x, center_y = iso_center(6, 2)
    ImageDraw.Draw(image).ellipse((center_x - 3, center_y - 4, center_x + 3, center_y + 1), fill=GRAY, outline=BLACK)
    ImageDraw.Draw(image).ellipse((center_x - 1, center_y - 3, center_x + 1, center_y - 1), fill=WATER)
    return image


def make_farm():
    image = base(GREEN)
    north_path = [(3, row) for row in range(8)]
    cross_path = [(column, 5) for column in range(8)]
    field_cells = [(column, row) for column in (0, 1, 6, 7) for row in (1, 2, 3, 4)]
    paint_cells(image, field_cells, DARK_BROWN)
    draw_furrows(image, field_cells)
    draw_path(image, north_path + cross_path)
    paint_cells(image, [(column, 0) for column in range(1, 7)], WATER)
    draw_building(image, 6, 1, RED, LEATHER, DARK_BROWN, height=9)
    return image


def make_forest_path():
    image = base(GREEN)
    stream = [(column, 3) for column in range(8)]
    paint_cells(image, stream, WATER)
    path = [(3, 7), (3, 6), (4, 5), (4, 4), (3, 3), (2, 2), (3, 1), (3, 0)]
    draw_path(image, path)
    for column in (0, 1, 6, 7):
        for row in (0, 2, 4, 6):
            draw_tree(image, column, row)
    draw_path(image, [(3, 3), (4, 3)], color=STONE, edges=LIGHT_GRAY)
    return image


def make_city():
    image = base(GRAY)
    draw_path(image, [(3, row) for row in range(8)] + [(column, 4) for column in range(8)])
    draw_building(image, 1, 1, RED, LEATHER, DARK_GRAY)
    draw_building(image, 6, 1, LEATHER, GRAY, DARK_GRAY, height=8)
    draw_building(image, 1, 6, DARK_GRAY, LEATHER, GRAY, height=8)
    draw_building(image, 6, 6, RED, LEATHER, DARK_GRAY, height=9)
    center_x, center_y = iso_center(4, 4)
    ImageDraw.Draw(image).ellipse((center_x - 5, center_y - 3, center_x + 5, center_y + 3), fill=WATER, outline=LIGHT_GRAY)
    ImageDraw.Draw(image).ellipse((center_x - 2, center_y - 1, center_x + 2, center_y + 1), fill=GRAY)
    return image


def make_mages_tower():
    image = base(STONE)
    paint_cells(image, [(column, row) for column in range(5) for row in range(8)], GREEN)
    draw_path(image, [(0, 7), (1, 6), (1, 5), (2, 4), (2, 3), (3, 2), (4, 1)])
    draw_building(image, 6, 3, DARK_GRAY, GRAY, BLACK, height=15)
    center_x, center_y = iso_center(5, 5)
    ImageDraw.Draw(image).ellipse((center_x - 5, center_y - 3, center_x + 5, center_y + 3), fill=PURPLE, outline=GOLD)
    ImageDraw.Draw(image).ellipse((center_x - 2, center_y - 1, center_x + 2, center_y + 1), fill=GOLD)
    return image


def make_portal_chamber():
    image = base(PURPLE)
    platform = [(column, row) for column in range(2, 6) for row in range(2, 6)]
    paint_cells(image, platform, DARK_GRAY)
    draw_path(image, [(3, 7), (4, 6), (4, 5), (4, 4)])
    for column, row in ((2, 4), (3, 2), (5, 2), (6, 4), (5, 6), (3, 6)):
        draw_building(image, column, row, GRAY, STONE, DARK_GRAY, height=7)
    center_x, center_y = iso_center(4, 4)
    draw = ImageDraw.Draw(image)
    draw.ellipse((center_x - 11, center_y - 5, center_x + 11, center_y + 5), fill=PURPLE, outline=GOLD, width=2)
    draw.ellipse((center_x - 6, center_y - 3, center_x + 6, center_y + 3), fill=BLACK, outline=LIGHT_GRAY)
    draw.ellipse((center_x - 2, center_y - 2, center_x + 2, center_y + 2), fill=ORANGE)
    draw_building(image, 4, 1, DARK_GRAY, LEATHER, GRAY, height=6)
    return image


def validate(path, image):
    if image.size != (64, 64):
        raise ValueError(f"{path.name}: expected 64x64, got {image.size}")
    ground_corners = ((32, 16), (63, 32), (32, 47), (0, 32))
    if any(image.getpixel(point)[3] == 0 for point in ground_corners):
        raise ValueError(f"{path.name}: isometric ground must span the 64x32 center band")
    texture_corners = ((0, 0), (63, 0), (0, 63), (63, 63))
    if any(image.getpixel(point)[3] != 0 for point in texture_corners):
        raise ValueError(f"{path.name}: square texture corners must remain transparent around the isometric ground")
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