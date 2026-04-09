extends GutTest
## GUT integration tests for IsometricYSorterNode.
## Verifies that the Godot component correctly updates ZIndex based on global position.

var _sorter_node: Node2D


func before_each() -> void:
	_sorter_node = Node2D.new()
	_sorter_node.set_script(load("res://src/Scripts/Core/IsometricYSorterNode.cs"))
	add_child_autofree(_sorter_node)
func test_auto_sort_enabled_by_default() -> void:
	assert_true(_sorter_node.AutoSort, "AutoSort should be true by default")


func test_z_index_updates_with_position() -> void:
	_sorter_node.global_position = Vector2(0, 100)
	_sorter_node._process(0.016)
	assert_eq(_sorter_node.z_index, 100, "ZIndex should equal Y=100")


func test_higher_y_yields_higher_z_index() -> void:
	_sorter_node.global_position = Vector2(0, 50)
	_sorter_node._process(0.016)
	var z_low: int = _sorter_node.z_index

	_sorter_node.global_position = Vector2(0, 150)
	_sorter_node._process(0.016)
	var z_high: int = _sorter_node.z_index

	assert_gt(z_high, z_low, "Higher Y position must yield higher ZIndex")


func test_auto_sort_disabled_freezes_z_index() -> void:
	_sorter_node.global_position = Vector2(0, 80)
	_sorter_node._process(0.016)
	var frozen_z: int = _sorter_node.z_index

	_sorter_node.AutoSort = false
	_sorter_node.global_position = Vector2(0, 999)
	_sorter_node._process(0.016)

	assert_eq(_sorter_node.z_index, frozen_z, "ZIndex must not change when AutoSort is false")


func test_z_index_at_origin_is_zero() -> void:
	_sorter_node.global_position = Vector2.ZERO
	_sorter_node._process(0.016)
	assert_eq(_sorter_node.z_index, 0, "ZIndex at origin must be 0")
