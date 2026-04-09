extends GutTest
## GUT integration tests for IsometricCameraNode.
## Verifies that the Godot Camera2D wrapper correctly delegates follow behaviour
## to CameraController and keeps its position in sync.

var _camera: Camera2D
var _target: Node2D


func before_each() -> void:
	_camera = Camera2D.new()
	_camera.set_script(load("res://src/Scripts/Core/IsometricCameraNode.cs"))
	add_child_autofree(_camera)
	_target = Node2D.new()
	add_child_autofree(_target)


func test_camera_default_follow_speed_is_positive() -> void:
	assert_gt(_camera.FollowSpeed, 0.0, "Default FollowSpeed must be positive")


func test_camera_with_no_target_does_not_crash() -> void:
	# Should not throw — no follow target set
	_camera._process(0.016)
	assert_true(true, "Camera must not crash without a follow target")


func test_camera_with_target_moves_toward_it() -> void:
	_target.global_position = Vector2(500, 500)
	_camera.FollowTarget = _target
	var start_pos: Vector2 = _camera.global_position
	# Simulate several frames
	for i in range(10):
		_camera._process(0.016)
	var end_pos: Vector2 = _camera.global_position
	assert_gt(end_pos.length(), start_pos.length(),
		"Camera should move toward the non-zero target position")


func test_camera_snap_to_pixels_produces_integer_position() -> void:
	_target.global_position = Vector2(99.7, 150.3)
	_camera.FollowTarget = _target
	_camera.FollowSpeed = 100.0 # fast — nearly snaps immediately
	_camera.SnapToPixels = true
	_camera._ready() # re-apply exported properties
	_camera._process(1.0)
	var pos: Vector2 = _camera.global_position
	assert_eq(pos.x, round(pos.x), "X should be integer when SnapToPixels is on")
	assert_eq(pos.y, round(pos.y), "Y should be integer when SnapToPixels is on")


func test_camera_higher_follow_speed_converges_faster() -> void:
	var target_pos := Vector2(1000, 1000)
	_target.global_position = target_pos

	# Slow camera
	var slow_cam: Camera2D = Camera2D.new()
	slow_cam.set_script(load("res://src/Scripts/Core/IsometricCameraNode.cs"))
	add_child_autofree(slow_cam)
	slow_cam.FollowTarget = _target
	slow_cam.FollowSpeed = 1.0
	slow_cam._ready()
	for i in range(5):
		slow_cam._process(0.016)

	# Fast camera
	var fast_cam: Camera2D = Camera2D.new()
	fast_cam.set_script(load("res://src/Scripts/Core/IsometricCameraNode.cs"))
	add_child_autofree(fast_cam)
	fast_cam.FollowTarget = _target
	fast_cam.FollowSpeed = 20.0
	fast_cam._ready()
	for i in range(5):
		fast_cam._process(0.016)

	var slow_dist: float = slow_cam.global_position.distance_to(target_pos)
	var fast_dist: float = fast_cam.global_position.distance_to(target_pos)
	assert_lt(fast_dist, slow_dist, "Faster FollowSpeed should converge closer to target")
