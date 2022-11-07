        uniform vec2 tex_size;
        uniform int planar_mode;
        uniform float planar_amplitude;
        uniform sampler2D image;


        #ifdef VERTEX
        vec4 position(mat4 transform_projection, vec4 vertex_position)
        {
            vec4 transformed = transform_projection * vertex_position;
            return transformed;
        }
        #endif

        #define PI 3.1415926538

        vec2 distort(int d_mode, vec2 f_position, float i_amplitude)
        {
            vec2 offset_pos = f_position;

            if (d_mode == 0) { // fishbowl
                vec2 p = f_position;

                float prop = 1.0; //screen proroption
            
                vec2 m = vec2(0.5, 0.5);//center coords
            
                vec2 d = p - m;//vector from center to current fragment
            
                float r = sqrt(dot(d, d)); // distance of pixel from center
        
                float power = ( 2.0 * PI / (2.0 * sqrt(dot(m, m))) ) * (i_amplitude);//amount of effect
        
                float bind;//radius of 1:1 effect
            
                if (power > 0.0) bind = sqrt(dot(m, m));//stick to corners
                else {if (prop < 1.0) bind = m.x; else bind = m.y;}//stick to borders
        
                //Weird formulas
                if (power > 0.0)//fisheye
                    offset_pos = m + normalize(d) * tan(r * power) * bind / tan( bind * power);
                else if (power < 0.0)//antifisheye
                    offset_pos = m + normalize(d) * atan(r * -power * 10.0) * bind / atan(-power * bind * 10.0);
                else offset_pos = p;//no effect for power = 1.0
            }
            if (d_mode == 1) { // fishbowl
                offset_pos = tex_size;
            }
            return offset_pos;
        }


        void main()
        {
            vec2 coord = gl_TexCoord[0].xy;   
            vec2 distortion = distort(planar_mode, coord, planar_amplitude);

            vec4 base_color = texture2D(image, distortion);        
            gl_FragColor = base_color;
        }
