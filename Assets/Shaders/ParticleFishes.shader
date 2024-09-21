// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "Custom/PFishes"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_AnimationSpeed("Animation Speed", Float) = 1
		_Yaw("Yaw", Float) = 0
		_YawAngle("YawAngle", Float) = 12
		_FadeSmooth("FadeSmooth", Float) = 0
		_PivotOffset("PivotOffset", Float) = 0
		_FadeScale("FadeScale", Float) = 0
		_FadeOffset("FadeOffset", Float) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry" }
		Cull Back
		CGPROGRAM

		#pragma target 4.5 //GPU Instancing Particle requires 4.5 or higher
		#pragma exclude_renderers gles //Exclude openGL ES

		#pragma surface surf Standard nolightmap nometa noforwardadd keepalpha fullforwardshadows addshadow vertex:vert 
		#pragma multi_compile_instancing
		#pragma instancing_options procedural:vertInstancingSetup

		struct vertStreamData{
			float3x4 transform;
			uint color;
			float rnd;
		};

		#define UNITY_PARTICLE_INSTANCE_DATA vertStreamData
		#define UNITY_PARTICLE_INSTANCE_DATA_NO_ANIM_FRAME

		#include "UnityCG.cginc"
		#include "UnityStandardParticleInstancing.cginc"

		struct Input
		{
			float2 uv_MainTex;
			float4 vertexColor;
		};

		uniform float _FadeScale;
		uniform float _AnimationSpeed;
		uniform float _FadeOffset;
		uniform float _FadeSmooth;
		uniform float _Yaw;
		uniform float _YawAngle;
		uniform float _PivotOffset;
		uniform sampler2D _MainTex;

		float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
		{
			original -= center;
			float C = cos( angle );
			float S = sin( angle );
			float t = 1 - C;
			float m00 = t * u.x * u.x + C;
			float m01 = t * u.x * u.y - S * u.z;
			float m02 = t * u.x * u.z + S * u.y;
			float m10 = t * u.x * u.y + S * u.z;
			float m11 = t * u.y * u.y + C;
			float m12 = t * u.y * u.z - S * u.x;
			float m20 = t * u.x * u.z - S * u.y;
			float m21 = t * u.y * u.z + S * u.x;
			float m22 = t * u.z * u.z + C;
			float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
			return mul( finalMatrix, original ) + center;
		}


		void vert(inout appdata_base v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o)
			vertInstancingColor(o.vertexColor);
			vertInstancingUVs(v.texcoord, o.uv_MainTex);

			float rnd = 0;
		#if defined(UNITY_PARTICLE_INSTANCING_ENABLED)
			UNITY_PARTICLE_INSTANCE_DATA data = unity_ParticleInstanceData[unity_InstanceID];
			rnd = data.rnd;
		#endif

			float3 pos = v.vertex.xyz;
			float fade = saturate(pow( (pos.z*_FadeScale+_FadeOffset), _FadeSmooth));
			float3 rotateOffset = float3(0.0 , 0.0 , _PivotOffset);
			float3 fishRotate = RotateAroundAxis(rotateOffset, pos, float3(0,1,0), (sin(((pos.z + _Time.y*_AnimationSpeed + rnd)*_Yaw)) * radians(_YawAngle)));
			float3 fishMovement = (fade * (fishRotate-pos));
			v.vertex.xyz += fishMovement;
			v.vertex.w = 1;


		}

		void surf(Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = i.vertexColor.rgb*tex2D(_MainTex, i.uv_MainTex);
			o.Metallic = 0.0;
			o.Smoothness = 1.0;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}