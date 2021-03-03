shader_type canvas_item;

uniform float warp = 0.75; // simulate curvature of CRT monitor
uniform float scan = 0.75; // simulate darkness between scanlines
uniform vec4 colBorder: hint_color;

void fragment()	{
	// squared distance from center
	vec2 uv = SCREEN_UV;
	vec2 dc = abs(0.5-uv);
	dc *= dc;

	// warp the fragment coordinates
	uv.x -= 0.5; uv.x *= 1.0+(dc.y*(0.3*warp)); uv.x += 0.5;
	uv.y -= 0.5; uv.y *= 1.0+(dc.x*(0.4*warp)); uv.y += 0.5;

	// determine if we are drawing in a scanline
	float apply = abs(sin(uv.y / SCREEN_PIXEL_SIZE.y)*0.5*scan);
	// sample inside boundaries, otherwise set to black
	if (uv.y > 1.0 || uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0) {
		// sample the texture
		COLOR = vec4(mix(colBorder.rgb,vec3(0.0),apply),1.0);
	} else {
		// sample the texture
		COLOR = vec4(mix(texture(SCREEN_TEXTURE,uv).rgb,vec3(0.0),apply),1.0);
	}
	
}