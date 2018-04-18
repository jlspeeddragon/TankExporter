#version 130
//fbx_vertex.glsl
//Used to light all FBX imports
out vec3 vVertex;
out vec2 TC1;
out mat3 TBN;

out vec3 t;
out vec3 b;
out vec3 n;
void main(void) {

    TC1 = gl_MultiTexCoord0.xy;

    vec3 n = normalize(gl_NormalMatrix * gl_Normal);
    vec3 t = normalize(gl_NormalMatrix * gl_MultiTexCoord1.xyz);
    vec3 b = normalize(gl_NormalMatrix * gl_MultiTexCoord2.xyz);

    float invmax = inversesqrt(max(dot(t, t), dot(b, b)));
    TBN = mat3(t * invmax, b * invmax, n * invmax);

    vVertex = vec3(gl_ModelViewMatrix * gl_Vertex);

    gl_Position    = ftransform();

}