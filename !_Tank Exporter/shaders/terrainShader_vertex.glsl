﻿#version 130
//terrainShader_vertex.glsl
//Used to light terrain

uniform sampler2D colorMap;
uniform sampler2D depthMap;
uniform sampler2D nomlMap;

uniform mat4 shadowProjection;

out vec4 ShadowCoord;
out vec3 v_Position;
out vec3 v_Normal;
out vec2 TC1;
out vec3 vertex;
void main(void){

    TC1 = gl_MultiTexCoord0.xy;
    ShadowCoord= shadowProjection * gl_Vertex;
    vertex = gl_Vertex.xyz;
    v_Normal = normalize(gl_NormalMatrix * gl_Normal);
    
    v_Position = vec3(gl_ModelViewMatrix * gl_Vertex);
    gl_Position    = ftransform();

}