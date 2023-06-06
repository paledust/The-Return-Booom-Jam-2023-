#ifndef PYRAMIDFACES_INCLUDED
#define PYRAMIDFACES_INCLUDED

// #include "NMGPyramidGraphicHelpers.hlsl"

//vertex on generated mesh
struct DrawVertex{
    float3 positionWS; //Position in world space
    float2 uv;
};
//Draw mesh triangle
struct DrawTriangle{
    float3 normalWS; //Normal in world space
    DrawVertex vertices[3];
};
StructuredBuffer<DrawTriangle> _DrawTriangles;

struct VertexOutput{
    float4 positionCS : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 positionWS: TEXCOORD1;
    float3 normalWS : TEXCOORD2;
};

sampler2D _MainTex; float4 _MainTex_ST;

VertexOutput vert(uint vertexID: SV_VertexID){
    VertexOutput o = (VertexOutput)0;

    DrawTriangle tri = _DrawTriangles[vertexID/3];
    DrawVertex i = tri.vertices[vertexID%3];

    o.positionWS = i.positionWS;
    o.normalWS = tri.normalWS;
    o.uv = TRANSFORM_TEX(i.uv, _MainTex);
    o.positionCS = mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_V,float4(i.positionWS,1)));

    return o;
}

float4 frag(VertexOutput i) : SV_Target{
#ifdef SHADOW_CASTER_PASS
    return 0;
#else
    float4 color = tex2D(_MainTex, i.uv);
    return color;
#endif
}

#endif