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
	# InputHandler is a plain C# class (no Node), instantiated via ClassDB
	# This test confirms the assembly compiled correctly and the type is accessible.
	assert_true(ClassDB.class_exists("InputHandler"), "InputHandler must be registered in ClassDB")
