sampler2D _DissolveMap;
float _DissolveTiling;
float _DissolveTriPlanarFalloff;
float _DissolveMin;
float _DissolveMax;

UNITY_INSTANCING_BUFFER_START(SphericalDissolve)
	UNITY_DEFINE_INSTANCED_PROP(float3, _dissolveCenter)
	UNITY_DEFINE_INSTANCED_PROP(float, _dissolveRadius)
	UNITY_DEFINE_INSTANCED_PROP(float, _dissolveLength)
UNITY_INSTANCING_BUFFER_END(SphericalDissolve)

inline float4 TriplanarSampling(sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index)
{
	float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
	projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
	float3 nsign = sign( worldNormal );
	half4 xNorm; half4 yNorm; half4 zNorm;
	xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
	yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
	zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
	return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
}