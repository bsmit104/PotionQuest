#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

struct CustomLightingData{
    //position and orientation
    float3 positionWS;
    float3 normalWS;
    float3 viewDirectionWS;
    float4 shadowCoord;

    //surface attributes
    float3 albedo;
    float3 smoothness;
};

//translate a [0,1] smoothness value to an exponent
float GetSmoothnessPower(float rawSmoothness)
{
    return exp2(10 * rawSmoothness + 1);
}

#ifndef SHADERGRAPH_PREVIEW
float3 CustomLightHandling(CustomLightingData d, Light light)
{
    float3 radiance = light.color * (light.distanceAttenuation * light.shadowAttenuation);

    float diffuse = saturate(dot(d.normalWS, light.direction));

    float specularDot = saturate(dot(d.normalWS, normalize(light.direction + d.viewDirectionWS)));
    float specular = pow(specularDot, GetSmoothnessPower(d.smoothness)) * diffuse;

    float3 color = d.albedo * radiance * (diffuse + specular);

    return color;
}

//stole from RealtimeLights.hlsl
// Fills a light struct given a perObjectLightIndex
Light GetAdditionalPerObjectLight(int perObjectLightIndex, CustomLightingData d)
{
    // Abstraction over Light input constants
#if USE_STRUCTURED_BUFFER_FOR_LIGHT_DATA
    float4 lightPositionWS = _AdditionalLightsBuffer[perObjectLightIndex].position;
    half3 color = _AdditionalLightsBuffer[perObjectLightIndex].color.rgb;
    half4 distanceAndSpotAttenuation = _AdditionalLightsBuffer[perObjectLightIndex].attenuation;
    half4 spotDirection = _AdditionalLightsBuffer[perObjectLightIndex].spotDirection;
    uint lightLayerMask = _AdditionalLightsBuffer[perObjectLightIndex].layerMask;
#else
    float4 lightPositionWS = _AdditionalLightsPosition[perObjectLightIndex];
    half3 color = _AdditionalLightsColor[perObjectLightIndex].rgb;
    half4 distanceAndSpotAttenuation = _AdditionalLightsAttenuation[perObjectLightIndex];
    half4 spotDirection = _AdditionalLightsSpotDir[perObjectLightIndex];
    uint lightLayerMask = asuint(_AdditionalLightsLayerMasks[perObjectLightIndex]);
#endif

    // Directional lights store direction in lightPosition.xyz and have .w set to 0.0.
    // This way the following code will work for both directional and punctual lights.
    float3 lightVector = lightPositionWS.xyz - d.positionWS * lightPositionWS.w;
    float distanceSqr = max(dot(lightVector, lightVector), HALF_MIN);

    half3 lightDirection = half3(lightVector * rsqrt(distanceSqr));
    // full-float precision required on some platforms
    
    //replaced attenuation with my own here!
    //basically multiplied the 
    float2 distanceAttenuationFloat = float2(distanceAndSpotAttenuation.xy);
    half factor = half(distanceSqr * distanceAttenuationFloat.x);
    half smoothFactor = saturate(half(1.0) - factor * factor);
    smoothFactor = smoothFactor * smoothFactor;
    float attenuation = smoothFactor;
    //float attenuation = DistanceAttenuation(distanceSqr, distanceAndSpotAttenuation.xy) * AngleAttenuation(spotDirection.xyz, lightDirection, distanceAndSpotAttenuation.zw);

    Light light;
    light.direction = lightDirection;
    light.distanceAttenuation = attenuation;
    light.shadowAttenuation = 1.0; // This value can later be overridden in GetAdditionalLight(uint i, float3 positionWS, half4 shadowMask)
    light.color = color;
    light.layerMask = lightLayerMask;

    return light;
}

//stole from RealtimeLights.hlsl
Light GetAdditionalLight(uint i, CustomLightingData d)
{
#if USE_FORWARD_PLUS
    int lightIndex = i;
#else
    int lightIndex = GetPerObjectLightIndex(i);
#endif
    Light light = GetAdditionalPerObjectLight(lightIndex, d);

#if USE_STRUCTURED_BUFFER_FOR_LIGHT_DATA
    half4 occlusionProbeChannels = _AdditionalLightsBuffer[lightIndex].occlusionProbeChannels;
#else
    half4 occlusionProbeChannels = _AdditionalLightsOcclusionProbes[lightIndex];
#endif
    light.shadowAttenuation = AdditionalLightShadow(lightIndex, d.positionWS, light.direction, 1, occlusionProbeChannels); // 1 is shadow mask
#if defined(_LIGHT_COOKIES)
    real3 cookieColor = SampleAdditionalLightCookie(lightIndex, d.positionWS);
    light.color *= cookieColor;
#endif

    return light;
}
#endif



float3 CalculateCustomLighting(CustomLightingData d)
{
    #ifdef SHADERGRAPH_PREVIEW
        //estimate diffuse + specular in preview
        float3 lightDir = float3(0.5, 0.5, 0);
        float intensity = saturate(dot(d.normalWS, lightDir)) + 
            pow(saturate(dot(d.normalWS, normalize(d.viewDirectionWS + lightDir))), GetSmoothnessPower(d.smoothness));
        return d.albedo * intensity;

    #else // MAIN LIGHTING 
        Light mainLight = GetMainLight(d.shadowCoord, d.positionWS, 1) ;

        float3 color = 0;
        color += CustomLightHandling(d, mainLight);

        #ifdef _ADDITIONAL_LIGHTS
            //shade additional cone and point lights
            uint numAdditionalLights = GetAdditionalLightsCount();
            for (uint lightIndex = 0; lightIndex < numAdditionalLights; lightIndex++)
            {
                Light light = GetAdditionalLight(lightIndex, d);
                color += CustomLightHandling(d, light);
            }
        #endif
    
        return color;
    #endif
    
}

void CalculateCustomLighting_float(float3 Position, float3 Normal,
    float3 ViewDirection, float3 Albedo, float Smoothness,
    out float3 Color)
{
    CustomLightingData d;
    d.positionWS = Position;
    d.normalWS = Normal;
    d.viewDirectionWS = ViewDirection;
    d.albedo = Albedo;
    d.smoothness = Smoothness;
    
    //shadows
    #ifdef SHADERGRAPH_PREVIEW
        //in preview there are no shadows or bakedGI
        d.shadowCoord = 0;
        #else
        //calculate the main light shadow coord
        //there are two types depending on if cascades are enabled
        float4 positionCS = TransformWorldToHClip(Position);
        #if SHADOWS_SCREEN
            d.shadowCoord = ComputeScreenPos(positionCS);
        #else
            d.shadowCoord = TransformWorldToShadowCoord(Position);
        #endif

    #endif

    Color = CalculateCustomLighting(d);
}


#endif