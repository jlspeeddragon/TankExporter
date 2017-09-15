// tank_fragment.glsl
//Used to light all models
#version 130
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
uniform int use_camo; // if camo is active
uniform int exclude_camo; // if this item has camo
uniform vec4 c0;
uniform vec4 c1;
uniform vec4 c2;
uniform vec4 c3;
uniform vec4 armorcolor;
uniform vec4 camo_tiliing;
in vec2 TC1;
in vec3 vVertex;
//in vec3 lightDirection;
in mat3 TBN;
in vec3 t;
in vec3 b;
in vec3 n;

out vec4 color_out;
//
void main(void) {

//==================================

    vec2 ctc = TC1 * camo_tiliing.xy; // from scripts/vehicle/nation/customiztion.xml file 
    ctc.xy *= tile_vec4.xy;
    ctc.xy += camo_tiliing.zw; // from scripts/vehicle/nation/tank.xml file
    ctc.xy += tile_vec4.zw;

    //ctc.y*= -1.0;
    vec4 camoTexture = texture2D(camoMap, ctc.st );
    vec4 cc = vec4(0.0);
     


    //==================================
    vec3 lightDirection;
    vec3 detailBump;
    vec3 bump;
    float alpha;
    vec4 sum = vec4(0.0);
    float a;
    vec3 bumpMap;
    vec3 detBump;
    float spec_l = 1.0;
    float specPower = 60.0;
    vec2 GMM;
    //==================================
    // get data
    vec4 AO = texture2D(aoMap, TC1.st);
    vec4 detail = texture2D(detailMap, TC1.st * detailTiling);
    vec3 dr = vec3(detail.r) ;
    vec3 db = vec3(detail.b) ;
    vec4 base = texture2D(colorMap, TC1.st);
    vec4 color = base;
    float factor = 0.8;
    a = base.a;

    if (is_GAmap == int(1))
    {
 
        detBump.xy = (2.0 * texture2D(detailMap, TC1.st* detailTiling).gb - 1.0);
        detBump.z  = sqrt(1.0 - dot(detBump.xy, detBump.xy));
        detBump    = normalize(detBump);
        detBump.y *= - 1.0;


        bumpMap.xy = (2.0 * texture2D(normalMap, TC1.st).ga - 1.0);
        bumpMap.z  = sqrt(1.0 - dot(bumpMap.xy, bumpMap.xy));
        bumpMap    = normalize(bumpMap);
        bumpMap.y *= - 1.0;
        a          = texture2D(normalMap, TC1.st).r;
        // red color variance ; alpha = specular
        GMM       = texture2D(gmmMap, TC1.st).rg;

        spec_l    = GMM.g;
        color += (color * GMM.r)*0.5;
        color *= 0.6;
        color = mix(color,armorcolor,0.2);
        if (AO.g > 0.0)
        {
            if (use_camo > 0 && exclude_camo == 0)
            {
                // This is based on RexTimmy's orginal work. Thanks!
                cc       = armorcolor ;
                cc       = mix(cc, c0 , camoTexture.r * c0.a );
                cc       = mix(cc, c1 , camoTexture.g * c1.a );
                cc       = mix(cc, c2 , camoTexture.b * c2.a );
                cc       = mix(cc, c3 , camoTexture.a * c3.a );
                color    = mix(color, cc, AO.a * AO.g)*.75;

                base.rgb = color.rgb;
            }            
            color.rgb = mix(color.rgb,color.rgb*detail.rgb*AO.r,detail.a*.13)*.6;

            base.rgb  = color.rgb;

        } else {
            color.rgb +=  color.rgb * color.rgb  * detail.r / (GMM.g * 0.02) * detail.r;
            color.rgb = pow(color.rgb * GMM.g , vec3(.5-GMM.g)*1.5);
            color.rgb *=color.rgb *.2 ;
            base.rgb = color.rgb;
            detBump  *= vec3(0.1);
            specPower = 1.0;
            AO.r      = GMM.g * 4.0;
            factor    = 0.1;
        }// end AO.g

    } else {
        color.rgb *= 2.0;
        bumpMap = (2.0 * texture2D(normalMap, TC1.st).rgb) - 1.0;
        bumpMap = normalize(bumpMap);
        spec_l  = 0.2;
    } //end is_GAmap

    if (alphaTest == int(1)) { if (a < 0.5) {discard;} }



    vec3 E = normalize(-vVertex); // we are in Eye Coordinates, so EyePos is (0,0,0)  
    vec3 PN = (TBN * bumpMap);  // Get the perturbed normal
    vec3 PN_D = normalize(TBN * detBump);  // Get the perturbed normal
    if (is_GAmap == int(0)) PN_D = vec3(0.0);
    vec4 Ispec1 = vec4(0.0);
    vec4 Ispec2 = vec4(0.0);
    vec4 Ispec3 = vec4(0.0);
    vec4 IspecSum = vec4(0.0);
    vec4 Iamb     = color*0.5;           //calculate Ambient Term:  
    vec4 Idiff2 = vec4(0.0);
    vec4 Idiff1 = vec4(0.0);
    for (int i = 0 ; i < 3 ; i++)
    {
    // GMM.g is metel spec level
    // GMM.r is surface roughness
        vec3 L = normalize(gl_LightSource[i].position.xyz - vVertex);   
        vec3 R = normalize(reflect(-L,PN));  
        vec3 R_D = normalize(reflect(-L,PN_D)); 
        if (is_GAmap != 1) {GMM.xy = vec2(0.3,0.3); AO.a = 0.0;}
        //calculate Diffuse Term:  
        Idiff1 = color * max(dot(PN,L*GMM.g), 0.0)* 5.0 * AO.g;//color light level
        Idiff2 = color * max(dot(PN_D,L), 0.0)* 0.5 * AO.a * detail.a; // detail color light level
        
        Idiff1 = clamp(Idiff1, 0.0, 1.0);     
        Idiff2 = clamp(Idiff2, 0.0, 1.0);     

        // calculate Specular Term:
        Ispec1 = vec4(0.3) * pow(max(dot(R,E*GMM.g),0.0),specPower * (1.0-GMM.g) ) * GMM.r * AO.r * 3.0 * factor; //metel spec

        Ispec2 = vec4(0.3) * pow(max(dot(R,E),0.0),specPower * GMM.r ) * GMM.r * AO.r * 3.0 * factor; // bump map spec
        
        Ispec3 = vec4(0.5) * pow(max(dot(R_D,E),0.0), detailPower) * GMM.g * 3.0 * factor * AO.a; //detail spec
        
        Ispec1 = clamp(Ispec1, 0.0, 1.0); 
        Ispec2 = clamp(Ispec2, 0.0, 1.0); 
        Ispec3 = clamp(Ispec3, 0.0, 1.0);
        vec4 Ispec1 = clamp(mix(Ispec1 + Ispec2, Ispec3,0.33),0.0,1.0);
        sum += Idiff1 + Idiff2 + Ispec1;
        IspecSum += Ispec1;

    } //next light

gl_FragColor     = Iamb + sum;   // write mixed Color:  
}

