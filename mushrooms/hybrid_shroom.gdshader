shader_type canvas_item;
render_mode unshaded;

uniform vec4 color_1 : source_color;
uniform vec4 color_2 : source_color;

uniform vec4 pulse = vec4(4.0, 3, 1.0, 0.5);
uniform float glow = 1.5;
uniform float amplitude = 0.5;


float when_lt(float x, float y) {
    return max(sign(y - x), 0.0);
}
 
vec3 rainbow2(float hue_in) {
	float hue = fract(hue_in);	
    float x = 1.0 - abs(mod(hue / (1.0 / 6.0), 2.0) - 1.0);
    vec3 color = vec3(0.0);
 
    color += vec3(1.0, x, 0.0) * when_lt(hue, 1.0 / 6.0);
    color += vec3(x, 1.0, 0.0) * when_lt(hue, 1.0 / 3.0) * when_lt(1.0 / 6.0, hue);
    color += vec3(0.0, 1.0, x) * when_lt(hue, 1.0 / 2.0) * when_lt(1.0 / 3.0, hue);
    color += vec3(0.0, x, 1.0) * when_lt(hue, 2.0 / 3.0) * when_lt(1.0 / 2.0, hue);
    color += vec3(x, 0.0, 1.0) * when_lt(hue, 5.0 / 6.0) * when_lt(2.0 / 3.0, hue);
    color += vec3(1.0, 0.0, x) * when_lt(hue, 1.1) * when_lt(5.0 / 6.0, hue);
 
    return color;
}
 
vec3 pulsating(vec3 color)
{
	vec3 output;
	if (color.b <= 0.0)
	{
		output = glow > 0.0 ?
			color_1.rgb * color.r * (cos(TIME*pulse.x + pulse.z) * amplitude + glow) + 
			color_2.rgb * color.g * (cos(TIME*pulse.y + pulse.w) * amplitude + glow)
			:
			color_1.rgb * color.r * (rainbow2(TIME*pulse.x + pulse.z) * amplitude + glow) + 
			color_2.rgb * color.g * (rainbow2(TIME*pulse.y + pulse.w) * amplitude + glow)
			;
	}
	else
	{
		output = color.rgb;		
	}
		 		
	return output;
}

void fragment() 
{
	vec3 color = pulsating(COLOR.rgb);
	COLOR = vec4(color, COLOR.a);
}

