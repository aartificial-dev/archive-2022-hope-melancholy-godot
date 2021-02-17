shader_type canvas_item;

uniform float vignette_amount = 1.0;
uniform float chroma_amount = 1.0;
uniform float grain_amount = 16.0;

vec4 vignette(vec2 suv) {  
	float vig = 1.0 - length(suv - vec2(0.5));
	vig = pow(abs(vig), vignette_amount);
	return vec4(vig, vig, vig, 1.0);
}

vec4 chromatic(sampler2D tex, vec2 uv) {
	vec4 newCol = vec4(0., 0., 0., 1.);

	float dis = distance(uv , vec2(0.5)) * chroma_amount;

	newCol.r = texture(tex, uv + (dis*0.005)).r;
	newCol.g = texture(tex, uv).g;
	newCol.b = texture(tex, uv - (dis*0.005)).b;

	return newCol;
}

vec4 grain(vec2 suv, float time) {
	vec2 uv = suv;
	
	float x = (uv.x + 4.0 ) * (uv.y + 4.0 ) * (time * 10.0);
	vec4 grain = vec4(mod((mod(x, 13.0) + 1.0) * (mod(x, 123.0) + 1.0), 0.01)-0.005) * grain_amount;

	grain = 1.0 - grain;
	return grain;
}

void fragment() {  
	vec4 col = chromatic(SCREEN_TEXTURE, SCREEN_UV);
	col *= vignette(SCREEN_UV);
	col *= grain(SCREEN_UV, TIME);
	
	COLOR = col;
}
