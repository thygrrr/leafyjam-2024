shader_type canvas_item;
render_mode unshaded;

uniform vec4 color_1 : source_color;
uniform vec4 color_2 : source_color;

uniform vec4 pulse = vec4(4.0, 3, 1.0, 0.5);
uniform float glow = 1.5;
uniform float amplitude = 0.5;

void fragment() 
{
	if (COLOR.b <= 0.0)
	{
		float alpha = COLOR.a;
		COLOR = 
			color_1 * COLOR.r * (sin(TIME*pulse.x + pulse.z) * amplitude + glow) + 
			color_2 * COLOR.g * (sin(TIME*pulse.y + pulse.w) * amplitude + glow);
		COLOR.a = alpha;			
	}
}
