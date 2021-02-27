shader_type canvas_item;

uniform float warp = 0.75; // simulate curvature of CRT monitor
uniform float scan = 0.75; // simulate darkness between scanlines
uniform float noiseBrightness = 0.;
uniform float noiseContrast = 1.;
uniform sampler2D S_Noise;

void fragment()	{
	// squared distance from center
	vec2 uv = SCREEN_UV;
	vec2 dc = abs(0.5-uv);
	dc *= dc;

	// warp the fragment coordinates
	uv.x -= 0.5; uv.x *= 1.0+(dc.y*(0.3*warp)); uv.x += 0.5;
	uv.y -= 0.5; uv.y *= 1.0+(dc.x*(0.4*warp)); uv.y += 0.5;

	// sample inside boundaries, otherwise set to black
	if (uv.y > 1.0 || uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0) {
		COLOR = vec4(0.0,0.0,0.0,1.0);
	} else {
		// determine if we are drawing in a scanline
		float apply = abs(sin(uv.y / SCREEN_PIXEL_SIZE.y)*0.5*scan);
		// sample the texture
		COLOR = vec4(mix(texture(SCREEN_TEXTURE,uv).rgb,vec3(0.0),apply),1.0);
	}
	
	vec2 screenSize = vec2(1.) / SCREEN_PIXEL_SIZE;
	float screenAspect = screenSize.y / screenSize.x;
	float noise = texture(S_Noise, vec2(SCREEN_UV.x, SCREEN_UV.y * screenAspect)).r;
	// Apply contrast.
	noise = ((noise - 0.5f) * max(noiseContrast, 0)) + 0.5f;
	// Apply brightness.
	noise += noiseBrightness;
	COLOR.rgb *= clamp(noise, 0, 1);
	
}