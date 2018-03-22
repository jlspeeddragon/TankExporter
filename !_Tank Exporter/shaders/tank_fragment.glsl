// tank_fragment.glsl
//Used to light all models
#version 120
uniform sampler2D colorMap;
uniform sampler2D normalMap;
uniform sampler2D gmmMap;
uniform sampler2D aoMap;
uniform sampler2D detailMap;
uniform sampler2D camoMap;
uniform int is_GAmap;
uniform int alphaTest;
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
    ac.a = 0.50;
    cc   = ac ;
    cc   = mix(cc, c0 , camoTex.r * c0.a );
    cc   = mix(cc, c1 , camoTex.g * c1.a );
    cc   = mix(cc, c2 , camoTex.b * c2.a );
    cc   = mix(cc, c3 , camoTex.a * c3.a );
    return cc;
}
// ========================================================

void main(void) {

//--------------------------------
vec4   cc = vec4(0.0);
vec3   lightDirection;
vec4   detailBump;
vec3   bump;
vec3   bumpD;
vec3   PN_D;
float  alpha;
vec4   color;
float  a;
float  specPower = 60.0;
float  MetalSpec = 5.0;
vec4   Ispec1 = vec4(0.0);
vec4   Ispec2 = vec4(0.0);
vec4   Ispec3 = vec4(0.0);
vec4   Idiff1 = vec4(0.0);
vec4   Idiff2 = vec4(0.0);
vec4   sum = vec4(0.0);
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
    //AO     = vec4(1.0);
    //GMM = vec2 (1.0);
//--------------------------------
    color = base;
    float factor = 1.0;
    a = base.a;
    //convert to -1.0 to 1.0    
    detailBump.xyz = detail.xyz * 2.0 - 1.0;
//==================================

    if (is_GAmap == 1 && use_CM == 0)
    {
        detailBump.yz = detailBump.yz *2.0 - 1.0;
        bumpD.xy - detailBump.yz;
        bumpD.z = sqrt(1.0 - dot(detailBump.yz, detailBump.yz)); //g,b
        bumpD   = normalize(bumpD);
        bumpD.y *= - 1.0;
        
        bumpMap.ga = bumpMap.ag *2.0 - 1.0;
        bump.xy    = bumpMap.ag;
        bump.z     = sqrt(1.0 - dot(bumpMap.ga, bumpMap.ga));
        bump       = normalize(bump);
        bump.y *= - 1.0;
        a = texture2D(normalMap, TC1.st).r;
        
        color.rgb = mix(color.rgb,1.5*(color.rgb * armorcolor.rgb),0.5);

      if (exclude_camo == 0)
      {
            // This is everyting but the chassis
            if (use_camo > 0 )
            {
                cc    = applyCamo(cc, camoTexture);
                color = mix(color, cc*1.5,  AO.a*cc.a);
            }
            color.rgb *= AO.g;
            // add detail noise to mix. Lots a tweaked values here.
            color.rgb = mix(color.rgb,color.rgb*detail.rgb*AO.g,detail.r*.9)*1.5;
            base.rgb  = color.rgb;
            specPower = 10.0;
            factor    = 0.9;
             MetalSpec = 5.0;
      } else {
            if (AO.g == 0.0 ){
            // If we are here, we are on the track treads
            AO = vec4(0.5,0.5,0.5,0.5);
             color.rgb = mix(color.rgb,detail.rgb*color.rgb,GMM.r)*1.0;
            AO.g      = 0.8;
            AO.r      = 2.5;
            specPower = 30.0;
            MetalSpec = 1.0;
            factor    = 1.0;
           base = color;
          } else {
            // If we land here, we are on chassis
            color.rgb = mix(color.rgb,color.rgb*detail.rgb*AO.a,detail.r)*1.0;
            base.rgb  = color.rgb;
            specPower = 2.0;//AO.r*5.0;
            factor    = clamp((0.6-AO.r)*2.5, 0.0 ,1.0)*3.0;
            MetalSpec = 30.0;
            }
       }// end AO.g

    } else {
        // If we land here, a SD tank was loaded.
        // If there is no GMM map, there is no detail map
        // Some values have to be hard coded to handle them
        // being misssing.
        PN_D   = vec3(0.0);
        detail = vec4(0.0);
        bump   = normalize(bumpMap.rgb*2.0 - 1.0);
        bump.y *= -1.0;
        if (use_camo > 0 && exclude_camo == 0)
        {
            cc    = applyCamo(cc, camoTexture);
            color = mix(color, cc, AO.b * cc.a);
        }            
        AO.r      = 0.5;
        AO.g      = 0.5;
        color.rgb *= 0.8;
        base.rgb  = color.rgb;
        GMM.g     = 1.0;
        GMM.r     = 1.0;
        factor    = 0.5;
    } //end is_GAmap

    if (alphaTest == int(1)) { if (a < 0.5) {discard;} }
    
    vec3 E = normalize(-vVertex);    // we are in Eye Coordinates, so EyePos is (0,0,0)  
    vec3 PN = normalize(TBN * bump); // Get the perturbed normal
    PN_D = normalize(TBN * bumpD);   // Get the perturbed normal

    vec4 Iamb     = color * 0.5 * 2.0 * A_level ; //calculate Ambient Term:  
    
    // loop thru lights and calc lighting.
    for (int i = 0 ; i < 3 ; i++)
    {
        // GMM.g is metel spec level
        // GMM.r is surface/paint roughness

        vec3 L = normalize(gl_LightSource[i].position.xyz - vVertex);   
        vec3 R = normalize(reflect(-L,PN));  
        vec3 R_D = normalize(reflect(-L,PN_D)); 
        if (is_GAmap != 1) {GMM.xy = vec2(0.3,0.3); AO.a = 0.0;}
        //calculate Diffuse Terms:  
        Idiff1 = color * max(dot(PN,L*GMM.g), 0.0) * AO.g * 3.0;//color light level
        Idiff2 = color *  max(dot(PN_D,L), 0.0)* 0.5 * AO.g * detail.a; // detail color light level
        
        Idiff1 = clamp(Idiff1, 0.0, 1.0);     
        Idiff2 = clamp(Idiff2, 0.0, 1.0);     

        // calculate Specular Terms:
        Ispec1 = vec4(0.1,0.1,0.15,1.0) * pow(max(dot(R,E*GMM.g),0.0), MetalSpec * max(GMM.g-.5,0.0))
                * (max(GMM.g-.5,0.0)) * AO.r * 5.0 * factor*0.8; //metel spec

        Ispec2 = vec4(0.3) * pow(max(dot(R,E),0.0),specPower)
                * (GMM.r-.5) * AO.r * 16.0 * factor; // bump map spec
        
        Ispec3 = vec4(0.9) * pow(max(dot(R_D,E),0.0), detailPower) * GMM.g * 1.0 * factor * AO.a; //detail spec
        
        Ispec1 = clamp(Ispec1, 0.0, 1.0); 
        Ispec2 = clamp(Ispec2, 0.0, 1.0); 
        Ispec3 = clamp(Ispec3, 0.0, 1.0);

        vec4 IspecMix = clamp(mix(Ispec1 + Ispec2, Ispec3,0.33),0.0,1.0) * 2.0 * S_level;
        sum += clamp(Idiff1 + Idiff2 + IspecMix, 0.0, 1.0);

    } //next light

gl_FragColor = (Iamb + sum) * T_level * 2.0;   // write mixed Color:  
//gl_FragColor = base;   // write mixed Color:  
}

