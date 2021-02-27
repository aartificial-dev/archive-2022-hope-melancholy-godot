shader_type canvas_item;

uniform vec2 iResolution;
uniform vec4 blend_color: hint_color;
uniform sampler2D gradient;
uniform float offset = 0;


float sinc(float x) {
    return (x == 0.0) ? 1.0 : sin(x) / x;
}

float triIsolate(float x) {
    return abs(-1.0 + fract(clamp(x, -0.5, 0.5)) * 2.0);
}

// Probably not a healthy heart
float heartbeat(float x) {
    float prebeat = -sinc((x - 0.4) * 40.0) * 0.6 * triIsolate((x - 0.4) * 1.0);
    float mainbeat = (sinc((x - 0.5) * 60.0)) * 1.2 * triIsolate((x - 0.5) * 0.7);
    float postbeat = sinc((x - 0.85) * 15.0) * 0.5 * triIsolate((x - 0.85) * 0.6);
    return (prebeat + mainbeat + postbeat) * triIsolate((x - 0.625) * 0.8); // width 1.25
}

float distanceToLineSegment(vec2 p0, vec2 p1, vec2 p) {
    float distanceP0 = length(p0 - p);
    float distanceP1 = length(p1 - p);
    
    float l2 =pow(length(p0 - p1), 2.);
    float t = max(0., min(1., dot(p - p0, p1 - p0) / l2));
    vec2 projection = p0 + t * (p1 - p0); 
    float distanceToProjection = length(projection - p);
    
    return min(min(distanceP0, distanceP1), distanceToProjection);
}

float saw(float x, float period) {
	return x - ceil(x / period - 0.5) * period;
}

float function(float x) {
	x += 0.1;
	x *= 1.9;
	 if (x > 1.25) x -= 1.25;
	return 0.5 - heartbeat(x) / 2.;
}

float distanceToFunction(vec2 p, float xDelta) {
    float result = 100.;
    for (float i = -3.; i < 3.; i += 1.)
    {
        vec2 q = p;
        q.x += xDelta * i;
        
        vec2 p0 = vec2(q.x, function(q.x));
    	vec2 p1 = vec2(q.x + xDelta, function(q.x + xDelta));
        result = min(result, distanceToLineSegment(p0, p1, p));
    }

    return result;
}

float getFunctionLine(vec2 uv) {
	float distanceToPlot = distanceToFunction(uv, 1. / iResolution.x);
    float intensity = smoothstep(0., 1., 1. - distanceToPlot * 1. * iResolution.y);
    intensity = pow(intensity,1./2.2);
	return intensity;
}

float cot(float x) {
	return cos(x) / sin(x);
}

float sawtooth(float x, float amplitude, float period) {
	float a1 = (2. * amplitude) / 3.1416;
	float a2 = (x * 3.1416) / period;
	return -a1 * atan(cot(a2));
}

void fragment() {
    vec2 uv = UV;

    float intensity = getFunctionLine(uv);
    
    vec3 col = vec3(blend_color.rgb) * intensity;
	
	float grad = texture(gradient, vec2(UV.x - sawtooth(TIME + offset, 1., 2.), UV.y)).r;
	
    COLOR = vec4(col,1.0);
	COLOR.a = intensity * grad;
}