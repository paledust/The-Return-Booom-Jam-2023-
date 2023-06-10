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
float4 _TipColor;

struct VertexOutput{
    float4 positionCS : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 positionWS: TEXCOORD1;
    float3 normalWS : TEXCOORD2;
};

VertexOutput vert(uint vertexID: SV_VertexID){
    VertexOutput o = (VertexOutput)0;

    DrawTriangle tri = _DrawTriangles[vertexID/3];
    DrawVertex i = tri.vertices[vertexID%3];

    o.positionWS = i.positionWS;
    o.normalWS = tri.normalWS;
    o.uv = i.height;
    o.positionCS = UnityWorldToClipPos(i.positionWS);

    return o;
}

float4 frag(VertexOutput i) : SV_Target{
#ifdef SHADOW_CASTER_PASS
    return 0;
#else
    float4 color = 1;
    color.rgb = lerp(_TipColor, _Color, i.uv.y);
    return color;
#endif
}

#endif