shader_type canvas_item;
render_mode unshaded;

uniform float line_width;
uniform vec4 color : hint_color;
uniform vec4 border_color : hint_color;

void fragment() {
	float variance = UV.y;
	if(variance > 0.5) {
		variance = 1.0 - variance;
	}
	variance = variance * 3.0;
	float weight = texture(TEXTURE, UV + TIME).r;
	float xpos = UV.x + ((((weight / 2.0) - 0.25)) * variance);
	if(xpos > 0.5) {
		xpos = 1.0 - xpos;
	}
	if(xpos > (0.5 - line_width)) {
		COLOR = color;
	}
	else {
		if(xpos > (0.5 - (line_width * 2.0))) {
			// convert the distance
			float alpha = (xpos - (0.5 - (line_width * 2.0))) / line_width;
			alpha = 1.0 - alpha;
			if(alpha < 0.5) {
				alpha = 0.0;
			}
			COLOR = border_color;
			COLOR.a = border_color.a;
		}
		else {
			COLOR = vec4(0, 0, 0, 0);	
		}
	}
}
