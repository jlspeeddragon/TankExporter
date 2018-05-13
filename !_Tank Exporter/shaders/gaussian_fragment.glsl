#version 330 core
// gausian_fragment
// blure shader

out vec4 FragColor;
  
in vec2 TexCoords;

uniform sampler2D image;
  
uniform int horizontal;
in float x1,x2,x3,x5,x6,x7;
in float y1,y2,y3,y5,y6,y7;
void main()
{
    vec4 color = vec4(0.0);

    color += texture2D( image, TexCoords.st + vec2( x1, y1 ) ) * 0.015625;
    color += texture2D( image, TexCoords.st + vec2( x2, y2 ) ) *0.09375;
    color += texture2D( image, TexCoords.st + vec2( x3, y3 ) ) *0.234375;
    color += texture2D( image, TexCoords.st + vec2( 0.0 , 0.0))*0.3125;
    color += texture2D( image, TexCoords.st + vec2( x5, y5 ) ) *0.234375;
    color += texture2D( image, TexCoords.st + vec2( x6, y6 ) ) *0.09375;
    color += texture2D( image, TexCoords.st + vec2( x7, y7 ) ) *0.015625;

    FragColor = color;
}