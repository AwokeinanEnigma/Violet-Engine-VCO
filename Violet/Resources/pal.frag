uniform sampler2D image, palette;
uniform float palIndex;
uniform vec4 blend;
uniform float blendMode;
uniform float palSize;
// uniform float delta;
 uniform float time;
 
void main() {
	vec4 index = texture2D(image, gl_TexCoord[0].xy);
	vec4 baseColor = texture2D(palette, vec2(((index.r * 255.0) + 0.5) / palSize, palIndex));
	vec3 baseNoAlpha = baseColor.rgb;
	vec3 finalColor = vec3(0, 0, 0);
					//vec3(
					//	((-cos(delta * baseColor.r) / 2.0) + 0.5),
					//	((-cos(delta * baseColor.r) / 2.0) + 0.5),
					//	((-cos(delta * baseColor.r) / 2.0) + 0.5)
					//	);

	
	if (blendMode < 0.1) {
		finalColor = blend.rgb;
	} else if (blendMode < 1.1) {
		finalColor = baseNoAlpha * blend.rgb;
	} else if (blendMode < 2.1) {
		finalColor = 1.0 - (1.0 - blend.rgb) * (1.0 - baseNoAlpha);
	}
	

	//these make the sprites fade in and out	
	// vec2 coord = gl_TexCoord[0].xy;   
	// vec4 pixel_color = texture2D(image, coord);
    // float alpha=0.5+0.5*cos(2*time);

    // gl_FragColor = vec4 (pixel_color.xyz,min(alpha,pixel_color.w));
	// gl_FragColor = vec4(finalColor, min(baseColor.a,alpha));

	
	gl_FragColor = vec4(finalColor, baseColor.a);

}