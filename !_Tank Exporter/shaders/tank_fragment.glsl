// tank_fragment.glsl
//Used to light all models
#version 330 compatibility

layout (location = 0) out vec4 gColor;
uniform sampler2D colorMap;
uniform sampler2D normalMap;
uniform sampler2D gmmMap;
uniform sampler2D aoMap;
uniform sampler2D detailMap;
uniform sampler2D camoMap;

uniform int is_GAmap;
uniform int alphaRef;
uniform vec2 detailTiling;
uniform vec4 tile_vec4;
uniform float detailPower;
uniform int use_camo; // if camo is active  1 = yes 0 = no
uniform int use_CM; // if using the CM mask 1 = yes 0 = no
uniform int exclude_camo; // if this item has camo  1 = yes 0 = no
uniform vec4 c0;
uniform vec4 c1;
uniform vec4 c2;
uniform vec4 c3;
uniform vec4 armorcolor;
uniform vec4 camo_tiling;
uniform float A_level;
uniform float S_level;
uniform float T_level;
in vec2 TC1;
in vec3 vVertex;
in mat3 TBN;
in vec3 t;
in vec3 b;
in vec3 n;



// ========================================================
// rextimmy gets full credit for figuring out how mixing works!
vec4 applyCamo(vec4 cc,vec4 camoTex){
    vec4 ac = armorcolor;
    cc   = ac ;
    cc   = mix(cc, c0 , camoTex.r * c0.a );
    cc   = mix(cc, c1 , camoTex.g * c1.a );
    cc   = mix(cc, c2 , camoTex.b * c2.a );
    cc   = mix(cc, c3 , camoTex.a * c3.a );
    return cc;
}
// ========================================================
vec3 correct(in vec3 hdrColor, in float exposure){  
    // Exposure tone mapping
    vec3 mapped = vec3(1.0) - exp(-hdrColor.rgb * exposure);
    // Gamma correction 
    return pow(mapped.rgb, vec3(1.0 / 0.75));    
}

const float PI = 3.14159265358;

float DistributionGGX(vec3 N, vec3 H, float a)
{
    float a2     = a*a;
    float NdotH  = max(dot(N, H), 0.0);
    float NdotH2 = NdotH*NdotH;
    
    float nom    = a2;
    float denom  = (NdotH2 * (a2 - 1.0) + 1.0);
    denom        = PI * denom * denom;
    
    return nom / denom;
}
float GeometrySchlickGGX(float NdotV, float k)
{
    float nom   = NdotV;
    float denom = NdotV * (1.0 - k) + k;
    
    return nom / denom;
}
  
float GeometrySmith(vec3 N, vec3 V, vec3 L, float k)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx1 = GeometrySchlickGGX(NdotV, k);
    float ggx2 = GeometrySchlickGGX(NdotL, k);
    
    return ggx1 * ggx2;
}
vec3 fresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
}

void main(void) {

vec3 lightColors[3];
lightColors[0] = vec3(0.7);
lightColors[1] = vec3(0.7);
lightColors[2] = vec3(0.6);

//--------------------------------
vec4   cc = vec4(0.0);
vec3   lightDirection;
vec4   detailBump;
vec3   bump;
float  alpha;
vec4   color;
float  a;
vec3   sumC = vec3(0.0);
//--------------------------------
// setup tiling values
    vec2 ctc = TC1 * camo_tiling.xy; // from scripts/vehicle/nation/customiztion.xml file 
    ctc.xy *= tile_vec4.xy; // from scripts/vehicle/nation/tank.xml file
    ctc.xy += camo_tiling.zw; 
    ctc.xy += tile_vec4.zw;
//--------------------------------

// Load textures
    vec4 camoTexture = texture2D(camoMap,   ctc.st );
    vec4 detail      = texture2D(detailMap, TC1.st * detailTiling);
    vec4 AO          = texture2D(aoMap,     TC1.st);
    vec4 base        = texture2D(colorMap,  TC1.st);
    vec4 bumpMap     = texture2D(normalMap, TC1.st);
    vec3 GMM         = texture2D(gmmMap,    TC1.st).rgb;

//--------------------------------
    color.rgb  = correct(base.rgb * base.rgb *2.5,0.75);
    a = base.a;
    //convert to -1.0 to 1.0    
    detailBump.xyz = detail.xyz * 2.0 - 1.0;
//==================================

    if (is_GAmap == 1 && use_CM == 0)
    {
        
        bumpMap.ga = bumpMap.ag *2.0 - 1.0;
        bump.xy    = bumpMap.ag;
        bump.z     = sqrt(1.0 - dot(bumpMap.ga, bumpMap.ga));
        bump       = normalize(bump);
        bump.y *= - 1.0;
        a = texture2D(normalMap, TC1.st).r;
        
        //color.rgb = mix(color.rgb,1.0*(color.rgb * armorcolor.rgb),0.75);
       


      if (exclude_camo == 0)
      {
            // This is everyting but the chassis
            if (use_camo > 0 )
            {
                cc    = applyCamo(cc, camoTexture);
                //cc = mix(vec4(0.04),cc,AO.a/0.752)*0.5;
                color.rgb = mix(color.rgb, cc.rgb * cc.rgb,  AO.a);
                
            }
            color.rgb *= AO.g;
            // add detail noise to mix. Lots a tweaked values here.
            base.rgb  = color.rgb;
      } else {
            if (AO.g == 0.0 ){
            // If we are here, we are on the track treads
            AO = vec4(0.5,0.5,0.5,0.5);
          
           base = color;
          } else {
            // If we land here, we are on chassis
            base.rgb  = color.rgb;
           
            }// end AO.g
       }

    } else {
        // If we land here, a SD tank was loaded.
        // If there is no GMM map, there is no detail map
        // Some values have to be hard coded to handle them
        // being misssing.
        bump   = normalize(bumpMap.rgb*2.0 - 1.0);
        bump.y *= -1.0;
        if (use_camo > 0 && exclude_camo == 0)
        {
            cc    = applyCamo(cc, camoTexture);
            color = mix(color, cc, AO.b * cc.a);
        }            
        color.rgb *= 0.8;
        base.rgb  = color.rgb;
        GMM.g     = 0.4;
        GMM.r     = 0.5;
      
    } //end is_GAmap
    float aRef = float(alphaRef)/255.0;
    if (aRef > a) {discard;}
    
    float roughness = 1.0-GMM.r/0.8;
    float metallic = GMM.g/0.8;
    float scrach = detail.r-0.5;
    vec3 V = normalize(-vVertex);    // we are in Eye Coordinates, so EyePos is (0,0,0)  
    vec3 N = normalize(TBN * bump); // Get the perturbed normal

    color.rgb = mix(color.rgb,vec3(0.04),GMM.g);
    if (is_GAmap != 1) {GMM.xy = vec2(0.3,0.3); }


    vec3 F0 = vec3(0.04,0.04,0.06); 
    F0 = mix(F0, color.rgb, metallic);
               
 
   // loop thru lights and calc lighting.
    vec3 ambient = vec3(2.0) * color.rgb * A_level;
    for (int i = 0 ; i < 3 ; i++)
    {
       // calculate per-light radiance
        vec3 L = normalize(gl_LightSource[i].position.xyz - vVertex);
        vec3 H = normalize(V + L);
        vec3 R = normalize(reflect(-L,N));  
        float distance    = length(gl_LightSource[i].position.xyz - vVertex);
        float attenuation = 20.0 / (distance * distance);
        vec3 radiance     = lightColors[i] * attenuation * 3.5;        
        
        // cook-torrance brdf
        float NDF = DistributionGGX(N*1.0, H, roughness);        
        float G   = pow(GeometrySmith(N*3.0, V, L, roughness),3.0*(1.1-roughness));      
        vec3 F    = fresnelSchlick(max(dot(H, V), 0.0), F0);       
        
        vec3 kS = F;
        vec3 kD = vec3(1.0) - kS;
        kD *= 1.0 - metallic;     
        
        vec3 numerator    = NDF * G * F;
        float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0);
        vec3 specular     = numerator / max(denominator, 0.001) * S_level*0.8;
        float spec_scale = 3.0; // tweak value
        vec3 Metalspecular = vec3(0.25,0.25,0.3) * max(pow(max(dot(R,V),0.0),10.0) * (metallic -0.35) * S_level*spec_scale,0.0);
        vec3 MetalDetailspecular = vec3(0.5,0.5,0.7) * pow(max(dot(R,V),0.0),5.0) * (scrach ) * S_level*spec_scale*metallic*0.5;
        // add to outgoing radiance Lo
        float NdotL = max(dot(N, L), 0.0);  
              
        sumC += (kD * color.rgb /PI + specular + Metalspecular + MetalDetailspecular) * radiance * NdotL *6.0;
        //sumC += (color.rgb * vec3(0.001)) + kD*0.2;
    } //next light

gColor.rgb = correct((sumC.rgb + ambient) * T_level * 3.25,1.0);

gColor.a = 1.0;
}

