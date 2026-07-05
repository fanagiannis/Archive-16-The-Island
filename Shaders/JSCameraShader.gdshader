shader_type canvas_item;

uniform float opacity : hint_range(0.0, 1.0) = 0.5;

// Higher numbers = tinier, finer grain. Lower numbers = chunky retro pixels.
uniform float static_density : hint_range(100.0, 3000.0) = 1500.0; 

// Adds horizontal camcorder lines to break up the harsh digital look
uniform float scanlines : hint_range(0.0, 0.5) = 0.15;

float random(vec2 uv) {
    return fract(sin(dot(uv.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

void fragment() {
    // 1. Snap UVs to a custom grid so you control the exact size of the static
    vec2 grid_uv = floor(UV * static_density) / static_density;
    
    // 2. Scramble the noise in-place every frame like real TV snow
    vec2 time_shift = vec2(fract(TIME * 54.321), fract(TIME * 12.345));
    float noise = random(grid_uv + time_shift);
    
    // 3. Add subtle horizontal lines (scanlines)
    float line_effect = sin(UV.y * 1000.0) * scanlines;
    noise -= line_effect;
    
    // 4. Output the final look
    COLOR = vec4(noise, noise, noise, opacity);
}