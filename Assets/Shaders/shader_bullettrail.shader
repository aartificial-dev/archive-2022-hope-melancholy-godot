shader_type canvas_item;
// render_mode light_only;

const int Quality = 16;
const int Directions = 32;
const float Pi = 6.28318530718;

uniform sampler2D noise;
uniform float alpha: hint_range(0.0, 1.0);
uniform vec4 color: hint_color = vec4(1.0);
uniform vec2 offset = vec2(0.);
uniform float contrast: hint_range(0.0, 10.0) = 1.0;

void fragment() {
    vec2 radius = vec2(0.3);
	vec4 oldCol = COLOR;
    vec4 Color = texture( TEXTURE, UV);
    for( float d=0.0;d<Pi;d+=Pi/float(Directions) )
    {
        for( float i=1.0/float(Quality);i<=1.0;i+=1.0/float(Quality) )
        {
                Color += texture( TEXTURE, UV+vec2(cos(d),sin(d))*radius*i);
        }
    }
    Color /= float(Quality)*float(Directions)+1.0;
	Color.rgb *= color.rgb;
	vec2 noise_uv = UV + offset + TIME / 10.;
    COLOR = Color * oldCol;
	vec4 noiseContrast = texture(noise, vec2(noise_uv.x, noise_uv.y / 2.));
	noiseContrast.rgb = ((noiseContrast.rgb - 0.5f) * max(contrast, 0)) + 0.5f;
	noiseContrast.r = clamp(noiseContrast.r, 0., 1.);
	noiseContrast.g = clamp(noiseContrast.g, 0., 1.);
	noiseContrast.b = clamp(noiseContrast.b, 0., 1.);
	COLOR.a *= alpha * ((noiseContrast.r + noiseContrast.g + noiseContrast.b) / 3.0); // 
	//COLOR = noiseContrast;
}