Shader "Water_FX/WaterSimulation"
{
	Properties
	{
    	_S2("PhaseVelocity^2", Range(0.0, .5)) = 0.2
    	[PowerSlider(0.01)]
    	_Atten("Attenuation", Range(0.0, 1.0)) = 0.999
    	_DeltaUV("Delta UV", Float) = 3
		_Force("Force", Float) = 0
		_FlowOpacity("Flow Opacity", Float) = 0.1
		_FlowNoise("Flow Noise", Range(0,1)) = 0
		_DynamicTex("DynamicTexture", 2D) = "Black"
	}
	//CGINCLUDE------------------------
		CGINCLUDE

		#include "UnityCustomRenderTexture.cginc"
		#pragma shader_feature USE_TEXTURE
		
		uniform half _S2;
		uniform float _Atten;
		uniform float _DeltaUV;
		uniform float _Force;
		uniform float _FlowOpacity;
		uniform float _FlowNoise;

		uniform sampler2D _DynamicTex;

		float random (float2 st) {
			return frac(sin(dot(st.xy,
								float2(12.9898,78.233)))*
				43758.5453123);
		}
		float randomTime(float2 st) {
			return frac(sin(dot(st.xy, float2(12.9898,78.233)) + random(float2(_Time.xx))*_FlowNoise)*43758.5453123);
		}
		
		fixed4 frag (v2f_customrendertexture i) : SV_Target
		{
			float2 uv = i.globalTexcoord;


    		float du = 1.0 / _CustomRenderTextureWidth;
    		float dv = 1.0 / _CustomRenderTextureHeight;
    		float3 duv = float3(du, dv, 0) * _DeltaUV;

    		float4 c = tex2D(_SelfTexture2D, uv);

			float height = (2 * c.r - c.g + _S2 * (
        					tex2D(_SelfTexture2D, uv - duv.zy).r +
        					tex2D(_SelfTexture2D, uv + duv.zy).r +
        					tex2D(_SelfTexture2D, uv - duv.xz).r +
        					tex2D(_SelfTexture2D, uv + duv.xz).r - 4 * c.r)) * _Atten;

			float _dh = tex2D(_DynamicTex, uv).r*_Force;
			height += _dh;

			c.a = saturate(c.a);

			return float4(height, c.r, height - c.r, c.a);
		}

		float4 frag_left_click(v2f_customrendertexture i) : SV_Target
		{
			return float4(-_Force, 0, 0, 0);
		}

		float4 frag_right_click(v2f_customrendertexture i) : SV_Target
		{
			return float4(_Force, 0, 0, 0);
		}

		ENDCG
		
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Update"
			CGPROGRAM
			#pragma vertex CustomRenderTextureVertexShader
			#pragma fragment frag

			ENDCG
		}

		Pass
		{
			Name "LeftClick"
			CGPROGRAM
			#pragma vertex CustomRenderTextureVertexShader
			#pragma fragment frag_left_click

			ENDCG
		}

		Pass
		{
			Name "RightClick"
			CGPROGRAM
			#pragma vertex CustomRenderTextureVertexShader
			#pragma fragment frag_right_click

			ENDCG
		}
	}
}
