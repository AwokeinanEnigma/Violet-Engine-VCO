			uniform vec2 tex_size;
			uniform vec2 tex_offset;

			uniform bool palette_enabled;
			uniform sampler2D palette_texture;
			uniform float palette_index;

			uniform vec2 axis_mode;
			uniform vec2 axis_frequency;
			uniform vec2 axis_amplitude;
			uniform vec2 axis_shift;
			
			uniform sampler2D tex;



			#define PI 3.1415926538

			//x vec2 is X, Y
			//y vec2 is Y, X
			float distort(int d_mode, vec2 f_position, int i_frequency, int i_amplitude, float i_shift)
			{
				float offset_pos = 0;
				if(d_mode == 0) { //oscillation
					offset_pos = i_amplitude * sin( ((1.0/i_frequency)*PI) * (f_position.y + i_shift) );
				}
				if(d_mode == 1) { //interlaced
					if(mod(f_position.y,2) == 0) {
						offset_pos = -(i_amplitude * sin( ((1.0/i_frequency)*PI) * (f_position.y + i_shift) ));
					} else {
						offset_pos = i_amplitude * sin( ((1.0/i_frequency)*PI) * (f_position.y + i_shift) );
					}
				}
				if(d_mode == 2) { //compression
					offset_pos = i_amplitude * sin( ((1.0/i_frequency)*PI) * (f_position.x + i_shift) );
				}
				if(d_mode == 3) { //linear scaling
					offset_pos =  (f_position.x+i_shift)*float(float(i_amplitude)/float(i_frequency)) ;
				}
				return offset_pos;
			}

			void main()
			{
				vec2 texture_coords = gl_TexCoord[0].xy;  
				vec2 distortion = vec2(
					distort(axis_mode.x, vec2(texture_coords.x,texture_coords.y), axis_frequency.x, axis_amplitude.x,axis_shift.x),
					distort(axis_mode.y, vec2(texture_coords.y,texture_coords.x), axis_frequency.y, axis_amplitude.y,axis_shift.y)
				);

				vec4 base_color = texture2D(tex, ((texture_coords+tex_offset+distortion)/tex_size));

				if (palette_enabled) {
					base_color = texture2D(palette_texture,vec2(base_color.r,palette_index));
				}

				gl_FragColor = base_color;
			}
			