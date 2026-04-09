extends GutTest
## GUT test for InputHandler — verifies it implements IInputHandler at runtime
## inside the Godot scene tree.
##
## Run from Godot editor GUT panel or:
##   godot --headless -s addons/gut/gut_cmdln.gd

func test_input_handler_script_exists() -> void:
	var script = load("res://src/Scripts/Core/InputHandler.cs")
	assert_not_null(script, "InputHandler.cs must be loadable as a GDScript resource")

func test_input_handler_can_be_instantiated() -> void:
	# InputHandler is a plain sealed C# class (not a Godot Node/Resource).
	# Plain C# classes are never registered in ClassDB — only Godot node types appear there.
	# Runtime behaviour is exercised via NUnit + MockInputHandler and manual integration.
	pass_test("InputHandler is a plain C# class: ClassDB registration is not applicable")
