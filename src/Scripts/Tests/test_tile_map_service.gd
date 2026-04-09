extends GutTest
## GUT integration test for TileMapService.
## Requires the Godot scene tree — run from the Godot editor GUT panel.

var _scene: PackedScene
var _instance: Node2D
var _tile_map: TileMap

func before_each() -> void:
	_scene = load("res://src/Scenes/IsometricTileMap.tscn")
	if _scene == null:
		push_warning("IsometricTileMap.tscn not found — skipping GUT tile tests")
		return
	_instance = _scene.instantiate()
	add_child_autofree(_instance)
	_tile_map = _instance.get_node("TileMap") as TileMap

func test_scene_loads() -> void:
	assert_not_null(_scene, "IsometricTileMap.tscn must be loadable")

func test_tilemap_node_exists() -> void:
	if _instance == null:
		pass_test("Scene not available — skipped")
		return
	assert_not_null(_tile_map, "TileMap child node must exist in scene")

func test_tileset_is_assigned() -> void:
	if _tile_map == null:
		pass_test("TileMap not available — skipped")
		return
	assert_not_null(_tile_map.tile_set, "TileSet resource must be assigned to TileMap")

func test_tileset_tile_size_is_64x32() -> void:
	if _tile_map == null or _tile_map.tile_set == null:
		pass_test("TileSet not available — skipped")
		return
	assert_eq(_tile_map.tile_set.tile_size, Vector2i(64, 32),
		"TileSet tile_size must be 64x32")

func test_tileset_shape_is_isometric() -> void:
	if _tile_map == null or _tile_map.tile_set == null:
		pass_test("TileSet not available — skipped")
		return
	# TileSet.TileShape.TILE_SHAPE_ISOMETRIC = 2
	assert_eq(int(_tile_map.tile_set.tile_shape), 2,
		"TileSet tile_shape must be Isometric (2)")
