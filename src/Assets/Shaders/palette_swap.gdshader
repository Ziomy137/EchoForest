// palette_swap.gdshader
// Runtime palette-swap shader for EchoForest.
//
// Replaces one of the 16 approved palette colors with a substitute color at
// draw time. Use this for day/night tinting, season changes, or character
// customisation without modifying the source textures.
//
// Usage (GDScript):
//   var mat := ShaderMaterial.new()
//   mat.shader = preload("res://src/Assets/Shaders/palette_swap.gdshader")
//   mat.set_shader_parameter("source_color", Palette.DarkGreen)
//   mat.set_shader_parameter("target_color", Color("#0a2a0a"))  # darker night green
//   mat.set_shader_parameter("tolerance", 0.01)
//   sprite.material = mat

shader_type canvas_item;

/// The palette color to replace (sample from Palette.All).
uniform vec4 source_color : source_color = vec4(0.1, 0.226, 0.1, 1.0); // DarkGreen default

/// The replacement color drawn instead.
uniform vec4 target_color : source_color = vec4(0.04, 0.1, 0.04, 1.0);

/// Per-channel RGB tolerance for the color match (0.0 = exact, 0.05 = loose).
/// Keep low (≤ 0.02) to avoid bleeding into adjacent palette colors.
uniform float tolerance : hint_range(0.0, 0.1) = 0.01;

void fragment() {
    vec4 current = texture(TEXTURE, UV);

    // Skip fully transparent pixels — they are not palette colors.
    if (current.a < 0.01) {
        COLOR = current;
        return;
    }

    vec3 diff = abs(current.rgb - source_color.rgb);
    bool matches = diff.r <= tolerance && diff.g <= tolerance && diff.b <= tolerance;

    COLOR = matches ? vec4(target_color.rgb, current.a) : current;
}
