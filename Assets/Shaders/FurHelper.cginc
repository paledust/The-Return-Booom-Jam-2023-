#pragma target 3.0

#include "Lighting.cginc"
#include "UnityCG.cginc"
#include "Autolight.cginc"

struct v2f
{
    float4 pos: SV_POSITION;
    half4 uv: TEXCOORD0;
    unityShadowCoord4 _ShadowCoord : TEXCOORD1;
    float3 worldNormal: TEXCOORD2;
    float3 worldPos: TEXCOORD3;
    half3 tspace0 : TEXCOORD4;
    half3 tspace1 : TEXCOORD5;
    half3 tspace2 : TEXCOORD6;
};

fixed4 _Color;
// fixed4 _Specular;
// half _Shininess;
half _Transclucency;

sampler2D _MainTex;
sampler2D _FurTex;
sampler2D _BumpMap;
half4 _MainTex_ST;
half4 _FurTex_ST;
half4 _BumpMap_ST;

half _FurNormalScale;

fixed _FurLength;
fixed _FurDensity;
fixed _FurThinness;
fixed _FurShading;

float4 _ForceGlobal;
float4 _ForceLocal;

v2f vert_base(appdata_full v)
{
    v2f o;
    float3 P = v.vertex.xyz + v.normal * _FurLength * FURSTEP;
    P += clamp(mul(unity_WorldToObject, _ForceGlobal).xyz + _ForceLocal.xyz, -1, 1) * pow(FURSTEP, 3) * _FurLength;
    o.pos = UnityObjectToClipPos(float4(P, 1.0));
    o._ShadowCoord = ComputeScreenPos(o.pos);
    o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
    o.uv.zw = TRANSFORM_TEX(v.texcoord, _FurTex);
    o.worldNormal = UnityObjectToWorldNormal(v.normal);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

    half3 wNormal = UnityObjectToWorldNormal(v.normal);
    half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
    half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
    half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
    o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
    o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
    o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);

    return o;
}

fixed4 frag_base(v2f i): SV_Target
{
    half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.uv * _BumpMap_ST.xy + _BumpMap_ST.zw));
    tnormal.xy *= _FurNormalScale;
    tnormal = normalize(tnormal);
    half3 worldNormal;
    worldNormal.x = dot(i.tspace0, tnormal);
    worldNormal.y = dot(i.tspace1, tnormal);
    worldNormal.z = dot(i.tspace2, tnormal);

    // fixed3 worldNormal = normalize(i.worldNormal);
    fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
    fixed3 worldView = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
    fixed3 worldHalf = normalize(worldView + worldLight);
    float shadow = SHADOW_ATTENUATION(i);

    fixed4 albedo = tex2D(_MainTex, i.uv.xy)*_Color;
    albedo.rgb -= (pow(1 - FURSTEP, 3)) * _FurShading;
    albedo.rgb = saturate(albedo.rgb);

    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
    
    fixed3 diffuse = saturate(dot(worldNormal, worldLight));

    // fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(worldNormal, worldHalf)), _Shininess);

    fixed3 color = (ambient + _LightColor0.rgb * (diffuse) * shadow) * albedo.rgb;
    fixed3 noise = tex2D(_FurTex, i.uv.zw * _FurThinness).rgb;
    fixed alpha = clamp(noise - (FURSTEP * FURSTEP) * _FurDensity, 0, 1);
    alpha *= albedo.a;
    
    return fixed4(color, alpha);
}

#ifdef INTERACTION
fixed4 frag_interactable(v2f i): SV_Target
{
    half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.uv * _BumpMap_ST.xy + _BumpMap_ST.zw));
    tnormal.xy *= _FurNormalScale;
    tnormal = normalize(tnormal);
    half3 worldNormal;
    worldNormal.x = dot(i.tspace0, tnormal);
    worldNormal.y = dot(i.tspace1, tnormal);
    worldNormal.z = dot(i.tspace2, tnormal);

    // fixed3 worldNormal = normalize(i.worldNormal);
    fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
    fixed3 worldView = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
    fixed3 worldHalf = normalize(worldView + worldLight);
    float shadow = SHADOW_ATTENUATION(i);

    fixed4 albedo = tex2D(_MainTex, i.uv.xy)*_Color;
    albedo.rgb -= (pow(1 - FURSTEP, 3)) * _FurShading;
    albedo.rgb = saturate(albedo.rgb);

    fixed noise = tex2D(_FurTex, i.uv.zw * _FurThinness).r;
    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
    
    fixed3 diffuse = saturate(dot(worldNormal, worldLight));

    // fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(worldNormal, worldHalf)), _Shininess);

    fixed3 color = (ambient + _LightColor0.rgb *  (diffuse) * shadow) * albedo.rgb;
    fixed mask = saturate(1-tex2D(_MaskTex, i.uv.xy*_MainTex_ST.xy*_MaskTex_ST.xy + _MaskTex_ST.zw).r);
    mask = smoothstep(_MinValue, _MaxValue, mask);
    mask = (mask>_CutOff)?mask:0;
    fixed alpha = clamp(noise - (FURSTEP * FURSTEP) * _FurDensity, 0, 1);
    alpha *= albedo.a;
    
    color = lerp(color, _MudColor.rgb, saturate(1-mask));
    
    return fixed4(color, alpha*mask);
}
#endif

#ifdef DISSOLVE
#include "Assets/Shaders/Dissolve/DissolveHelper.cginc"

fixed4 frag_Dissolve(v2f i): SV_Target
{
    half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.uv * _BumpMap_ST.xy + _BumpMap_ST.zw));
    tnormal.xy *= _FurNormalScale;
    tnormal = normalize(tnormal);
    half3 worldNormal;
    worldNormal.x = dot(i.tspace0, tnormal);
    worldNormal.y = dot(i.tspace1, tnormal);
    worldNormal.z = dot(i.tspace2, tnormal);

    // fixed3 worldNormal = normalize(i.worldNormal);
    fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
    fixed3 worldView = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
    fixed3 worldHalf = normalize(worldView + worldLight);
    float shadow = SHADOW_ATTENUATION(i);

    fixed4 albedo = tex2D(_MainTex, i.uv.xy)*_Color;
    albedo.rgb -= (pow(1 - FURSTEP, 3)) * _FurShading;
    albedo.rgb = saturate(albedo.rgb);

    fixed noise = tex2D(_FurTex, i.uv.zw * _FurThinness).r;
    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
    
    fixed3 diffuse = saturate(dot(worldNormal, worldLight));

    //Dissolve
    float3 _dissolveCenter_Instance = UNITY_ACCESS_INSTANCED_PROP(SphericalDissolve, _dissolveCenter);
    float  _dissolveRadius_Instance = UNITY_ACCESS_INSTANCED_PROP(SphericalDissolve, _dissolveRadius);
    float  _dissolveLength_Instance = UNITY_ACCESS_INSTANCED_PROP(SphericalDissolve, _dissolveLength);
    
    float3 worldPos  = i.worldPos;
    float3 worldUV   = worldPos +  _Time.y*_AnimeSpeed;
    float dist = distance(worldPos, _dissolveCenter_Instance);
    float sphereValue = 1.0-(dist-(_dissolveRadius_Instance-_dissolveLength_Instance))/_dissolveLength_Instance;
    float dissolveValue = TriplanarSampling( _DissolveMap, worldUV, worldNormal, _DissolveTriPlanarFalloff, _DissolveTiling, 1.0, 0).r;

    sphereValue   = _InvertDissolveDirection?sphereValue:1.0-sphereValue;
    dissolveValue = _InvertDissolveMap?dissolveValue:1.0-dissolveValue; 
    float clipValue = smoothstep(_DissolveMin, (_DissolveMin + _DissolveMax), sphereValue + dissolveValue + dissolveValue);

    // fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(worldNormal, worldHalf)), _Shininess);

    fixed3 color = (ambient + _LightColor0.rgb *  (diffuse) * shadow) * albedo.rgb;
    fixed mask = saturate(1-tex2D(_MaskTex, i.uv.xy*_MainTex_ST.xy*_MaskTex_ST.xy + _MaskTex_ST.zw).r);
    mask = smoothstep(_MinValue, _MaxValue, mask);
    mask = (mask>_CutOff)?mask:0;
    fixed alpha = clamp(noise - (FURSTEP * FURSTEP) * _FurDensity, 0, 1);
    alpha *= albedo.a;
    
    color = lerp(color, _MudColor.rgb, saturate(1-mask));
    clip(clipValue-0.5);
    
    return fixed4(color, alpha*mask);
}
#endif