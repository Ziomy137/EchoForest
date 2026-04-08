extends GutTest
## Sample GUT integration test — confirms GUT loads correctly in the Godot scene tree.
## This is the GUT equivalent of EngineBootTest.cs for NUnit.
##
## Run from GUT panel in editor, or via:
##   godot --headless -s addons/gut/gut_cmdln.gd

func test_gut_framework_loads_successfully() -> void:
	assert_true(true, "GUT framework is operational")

func test_sample_assertion() -> void:
	var value := 2 + 2
	assert_eq(value, 4, "Basic arithmetic works")
