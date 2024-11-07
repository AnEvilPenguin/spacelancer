shader_type canvas_item;

uniform vec4 color1 : source_color;

// Called for every pixel the material is visible on.
void fragment() {
	vec4 input_color = texture(TEXTURE, UV);
	
	if (input_color.a == 0.0) {
		COLOR = input_color;
	} else {
		COLOR = color1;
	}
}
