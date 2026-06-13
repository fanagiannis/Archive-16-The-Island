shader_type canvas_item;

uniform vec4 line_color : source_color = vec4(0.0, 1.0, 0.2, 1.0);
uniform float speed : hint_range(0.1, 10.0) = 1.5;
uniform float pulse_frequency : hint_range(0.1, 5.0) = 1.5;
uniform float amplitude : hint_range(0.0, 1.0) = 0.6;
uniform float line_thickness : hint_range(0.001, 0.05) = 0.015;
uniform float glow_multiplier : hint_range(1.0, 5.0) = 2.0;

void fragment() {
    vec2 uv = UV;
    
    // Y goes down in Godot 2D, so we invert Y to make the spike go UP
    uv.y = 1.0 - uv.y; 
    
    float time = TIME * speed;
    
    // Clean horizontal scanline
    float scan_pos = fract(uv.x - time * 0.2);
    
    // Heartbeat timeline
    float t = fract(uv.x * 2.0 - time * pulse_frequency);
    
    float ekg = 0.0;
    
    // 1. The R-Spike (Main tall peak) - Using Gaussian curves for perfectly smooth slopes
    ekg += exp(-pow((t - 0.5) * 50.0, 2.0)) * amplitude;
    // 2. The S-Drop (Deep drop after spike)
    ekg -= exp(-pow((t - 0.53) * 60.0, 2.0)) * (amplitude * 0.4);
    // 3. The T-Wave (Small recovery bump)
    ekg += exp(-pow((t - 0.7) * 20.0, 2.0)) * (amplitude * 0.2);
    // 4. The P-Wave (Small prep bump)
    ekg += exp(-pow((t - 0.35) * 25.0, 2.0)) * (amplitude * 0.1);

    // Center the wave
    float target_y = 0.5 + ekg;
    
    // Calculate distance to our smooth math line
    float dist = abs(uv.y - target_y);
    
    // Core solid line
    float core = smoothstep(line_thickness, 0.0, dist);
    // Soft outer glow (4x thicker than the core)
    float glow = smoothstep(line_thickness * 4.0, 0.0, dist) * 0.5;
    
    float alpha = clamp(core + glow, 0.0, 1.0);
    
    // Fade the tail behind the moving scan line
    alpha *= smoothstep(0.0, 0.4, scan_pos);
    
    COLOR = vec4(line_color.rgb, alpha * line_color.a * glow_multiplier);
}