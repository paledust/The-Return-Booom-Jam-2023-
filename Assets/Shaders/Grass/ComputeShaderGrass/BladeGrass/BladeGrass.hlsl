#ifndef BLADEGRASS_INCLUDED
#define BLADEGRASS_INCLUDED

#include "NMGGrassBladeGraphicHelpers.hlsl"

//vertex on generated mesh
struct DrawVertex{
    float3 positionWS; //Position in world space
    float height;
};
//Draw mesh triangle
struct DrawTriangle{
    float3 normalWS; //Normal in world space
    DrawVertex vertices[3];
};


StructuredBuffer<DrawTriangle> _DrawTriangles;

float4 _Color;
float _TranslucentGain;

struct VertexOutput{
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 worldPos: TEXCOORD1;
    float3 worldNormal : TEXCOORD2;
    UNITY_SHADOW_COORDS(5)
};

VertexOutput vert(uint vertexID: SV_VertexID){
    VertexOutput o = (VertexOutput)0;

    DrawTriangle tri = _DrawTriangles[vertexID/3];
    DrawVertex i = tri.vertices[vertexID%3];
    o.worldPos = i.positionWS;
    o.worldNormal = tri.normalWS;
    o.uv = i.height;
    o.pos = UnityWorldToClipPos(i.positionWS);

    o._ShadowCoord = ComputeScreenPos(o.pos);
    UNITY_TRANSFER_SHADOW(o, o.uv);
    #if UNITY_PASS_SHADOWCASTER
        o.pos = UnityApplyLinearShadowBias(o.pos);
    #endif
    
    return o;
}

float4 frag(VertexOutput i, fixed facing : VFACE) : SV_Target{
    float3 normal = facing > 0 ? i.worldNormal : -i.worldNormal;

    UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos.xyz);
    float NdotL = saturate(saturate(dot(normal, _WorldSpaceLightPos0)) + _TranslucentGain) * attenuation;
    float3 ambient = ShadeSH9(float4(normal,1));
    float4 light = NdotL*_LightColor0 + float4(ambient, 1) + 0.01;

    float4 color = 1;
    color.rgb = _Color;
    color.rgb *= light;
    color.rgb *= saturate(i.uv.y*i.uv.y);

    return color;
}

float4 fragAdd(VertexOutput i, fixed facing : VFACE) : SV_Target{
    float3 normal = facing > 0 ? i.worldNormal : -i.worldNormal;
    
    UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos.xyz);
    float NdotL = saturate(saturate(dot(normal, _WorldSpaceLightPos0)) + _TranslucentGain) * attenuation;

    float3 diffuseReflection = NdotL * _LightColor0.rgb;
    diffuseReflection.xyz *= saturate(i.uv.y*i.uv.y);
    return float4(diffuseReflection*3, 1.0);
}

float4 fragShadow(VertexOutput i) : SV_Target{
    SHADOW_CASTER_FRAGMENT(i)
}

#endif