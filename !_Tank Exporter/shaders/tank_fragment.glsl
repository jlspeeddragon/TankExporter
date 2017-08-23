// bump_fragment.glsl
//Used to light all models
#version 130
uniform sampler2D colorMap;
uniform sampler2D normalMap;
uniform sampler2D gmmMap;
uniform sampler2D aoMap;
uniform int is_GAmap;
uniform int alphaTest;
uniform vec3 viewPos;

in vec2 TC1;
in vec3 vVertex;
in vec3 lightDirection;
in mat3 TBN;

in vec3 t;
in vec3 b;
in vec3 n;

out vec4 color_out;
//
void main(void) {
    vec3 bump;
    float alpha;
    float a;
    vec3 bumpMap;
    float spec_l = 1.0;
    float spec_power = 90.0;
    vec3 viewDir = normalize(viewPos - vVertex);
    vec2 GMM;
    float AO;
    // get normal map based on type
    vec4 base = texture2D(colorMap, TC1.st);
    vec4 color = base;
    a = base.a;
  if (is_GAmap == int(1)) {
        bumpMap.xy = (2.0 * texture2D(normalMap, TC1.st).ag - 1.0);
        ;
        bumpMap.z = sqrt(1.0 - dot(bumpMap.xy, bumpMap.xy));
        bumpMap   = normalize(bumpMap);
        a         = texture2D(normalMap, TC1.st).r;
        GMM       = texture2D(gmmMap, TC1.st).rg;// red color variance ; alpha = specular
        AO        = texture2D(aoMap, TC1.st).g;// red color variance ; alpha = specular
        spec_l    = GMM.r * 0.5;
        if (AO > 0.0)
        {
        color.rgb *= (GMM.r + AO) * 0.5;
        base.rgb = color.rgb;
        } else {
        color.rgb *= GMM.r;
        base.rgb = color.rgb;
        }
    } else {
        bumpMap = (2.0 * texture2D(normalMap, TC1.st).rgb) - 1.0;
        bumpMap = normalize(bumpMap);
        spec_l  = 0.2;
    }

    if (alphaTest == int(1)) {
        if (a < 0.5) {discard;}
        }
    
// Get the perturbed normal
    vec3 PN = normalize(TBN * bumpMap);
// light position
    vec3 L = normalize(lightDirection);
//caclulate lighting
    vec3 halfwayDir = normalize(L + viewDir);

    float NdotL = max(dot(PN, L), 0.0);
    color.rgb += (color.rgb * pow(NdotL, 1.0));
    color.rgb += pow(max(dot(PN, halfwayDir), 0.0), spec_power)  * spec_l;

    vec4 final_color = color;
    final_color.rgb += (base.rgb * 0.5);
    color_out = final_color;

}