shader_type canvas_item;
render_mode unshaded;

uniform sampler2D rainbow : source_color;

uniform vec4 color_1 : source_color;
uniform vec4 color_2 : source_color;

uniform vec4 pulse = vec4(4.0, 3, 1.0, 0.5);
uniform float glow = 1.5;
uniform float amplitude = 0.5;
 
vec3 pulsating(vec3 color)
{
	vec3 output;
	if (color.b <= 0.0)
	{
		output = glow >= 0.0 ?
			color_1.rgb * color.r * (cos(TIME*pulse.x + pulse.z) * amplitude + glow) + 
			color_2.rgb * color.g * (cos(TIME*pulse.y + pulse.w) * amplitude + glow)
			:
			(color.r * amplitude - glow) * texture(rainbow, vec2((cos(TIME*pulse.x + pulse.z)+1.0)*0.5)).rgb + 
			(color.g * amplitude - glow) * texture(rainbow, vec2((cos(TIME*pulse.y + pulse.w)+1.0)*0.5)).rgb;
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

