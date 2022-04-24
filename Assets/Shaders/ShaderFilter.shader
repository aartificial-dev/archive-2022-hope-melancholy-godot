shader_type canvas_item;
render_mode unshaded;

uniform float mix_amount: hint_range(0, 1) = 0.5;
uniform bool flip = false;
uniform sampler2D gradient : hint_black; // It can be whatever palette you want


void fragment(){ 
	vec4 oldcol = texture(SCREEN_TEXTURE,SCREEN_UV);
	vec4 col = texture(SCREEN_TEXTURE,SCREEN_UV);
	
	float lum = dot(col.rgb,vec3(0.2126,0.7152,0.0722)); // luminance
	
	
	col = texture(gradient,vec2(abs(float(flip) - lum),0));
	
	
	COLOR = mix(oldcol, col, mix_amount);
}