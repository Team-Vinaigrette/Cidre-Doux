shader_type canvas_item;

// Position of the vertex, relative to the center.
varying vec2 to_center;

void vertex() {
	// Get the distance to the center.
	to_center = normalize(VERTEX - vec2(0, 0));
}

void fragment() {
	// Get the length of the distance to the center.
	float len = length(to_center);
	
	// Apply some cutoff to the length.
	len = (len - 0.75) * 3.;
	
	// Simply render the distance as a shade of red.
	COLOR = vec4(1, 1, 1, len) * COLOR;
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
