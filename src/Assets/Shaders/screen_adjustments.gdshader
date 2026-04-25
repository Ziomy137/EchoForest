// screen_adjustments.gdshader
// Full-screen post-process overlay for EchoForest.
//
// Applies brightness and gamma correction to the composited screen contents.
// Used by GodotDisplayServer to implement the Settings → Brightness/Gamma
// sliders without requiring a WorldEnvironment or 3D pipeline.
//
// Rendered in a CanvasLayer at layer 1000 (above all game and UI layers) so
// SCREEN_TEXTURE captures the fully composited frame before this draw call.

shader_type canvas_item;

// Hint tells Godot 4 to copy the screen framebuffer before rendering this node.
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

// Brightness multiplier. 1.0 = no change. Slider range 0–200 maps to 0.0–2.0.
uniform float brightness : hint_range(0.0, 2.0) = 1.0;

// Gamma exponent denominator. 1.0 = no change. Slider range 0–200 maps to 0.0–2.0.
// Applied as: out = pow(in, 1.0 / gamma). gamma > 1 → brighter; gamma < 1 → darker.
uniform float gamma : hint_range(0.001, 4.0) = 1.0;

void fragment() {
    vec4 col = texture(SCREEN_TEXTURE, SCREEN_UV);

    // Brightness: simple linear scale.
    col.rgb *= brightness;

    // Gamma: power-function correction. Clamp first to avoid NaN from negative values.
    col.rgb = pow(clamp(col.rgb, 0.0, 1.0), vec3(1.0 / max(gamma, 0.001)));

    COLOR = col;
}
