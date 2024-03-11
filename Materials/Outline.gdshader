shader_type spatial;

render_mode unshaded, cull_front;

uniform bool enable = false;
uniform float border_width : hint_range(0,1,0.001);
uniform vec4 color : source_color = vec4(1.0);

void vertex() {
	// Called for every vertex the material is visible on.
	if(enable == true)
	{
		VERTEX += VERTEX * border_width;
	}
}

void fragment() {
	// Called for every pixel the material is visible on.
	if(enable == true)
	{
		ALBEDO = color.xyz;
		EMISSION = color.xyz;
	}
}

void light() {
	// Called for every pixel for every light affecting the material.
}
