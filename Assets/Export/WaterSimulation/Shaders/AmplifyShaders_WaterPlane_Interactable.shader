// Made with Amplify Shader Editor v1.9.6.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AmplifyShaders/WaterPlane_Interactable"
{
	Properties
	{
		_OverAllOpacity("OverAllOpacity", Range( 0 , 1)) = 0
		_WaterNormal("Water Normal", 2D) = "white" {}
		_NormalScale("Normal Scale", Float) = 0
		_Distortion("Distortion", Float) = 0.5
		_RippleNormalTex("RippleNormalTex", 2D) = "black" {}
		_RippleScale("RippleScale", Float) = 0
		_ShalowColor("Shalow Color", Color) = (0,0,0,0)
		_DeepColor("Deep Color", Color) = (1,1,1,0)
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_WaterDepth("Water Depth", Float) = 0
		_WaterFallOff("WaterFallOff", Float) = 0
		_WaterFalloffPower("WaterFalloffPower", Float) = 0
		_ReflectionTex("ReflectionTex", 2D) = "white" {}
		_ReflectionIntensity("ReflectionIntensity", Range( 0 , 1)) = 0
		_ReflectionDistortion("ReflectionDistortion", Range( 0 , 1)) = 0
		_DarkControl("DarkControl", Range( 0 , 1)) = 1
		_NormalPanVelocity("NormalPanVelocity", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.5
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float4 screenPos;
			float4 screenPosition443;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float _OverAllOpacity;
		uniform sampler2D _ReflectionTex;
		uniform sampler2D _WaterNormal;
		uniform float4 _NormalPanVelocity;
		uniform float4 _WaterNormal_ST;
		uniform float _NormalScale;
		uniform sampler2D _RippleNormalTex;
		uniform float4 _RippleNormalTex_ST;
		uniform float _RippleScale;
		uniform float _ReflectionDistortion;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _Distortion;
		uniform float4 _ShalowColor;
		uniform float4 _DeepColor;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _WaterDepth;
		uniform float _WaterFallOff;
		uniform float _WaterFalloffPower;
		uniform float _ReflectionIntensity;
		uniform float _Smoothness;
		uniform float _DarkControl;


inline float4 ASE_ComputeGrabScreenPos( float4 pos )
{
	#if UNITY_UV_STARTS_AT_TOP
	float scale = -1.0;
	#else
	float scale = 1.0;
	#endif
	float4 o = pos;
	o.y = pos.w * 0.5f;
	o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
	return o;
}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 vertexPos443 = ase_vertex3Pos;
			float4 ase_screenPos443 = ComputeScreenPos( UnityObjectToClipPos( vertexPos443 ) );
			o.screenPosition443 = ase_screenPos443;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 unityObjectToClipPos342 = UnityObjectToClipPos( ase_vertex3Pos );
			float4 computeScreenPos340 = ComputeScreenPos( unityObjectToClipPos342 );
			computeScreenPos340 = computeScreenPos340 / computeScreenPos340.w;
			computeScreenPos340.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? computeScreenPos340.z : computeScreenPos340.z* 0.5 + 0.5;
			float2 appendResult469 = (float2(_NormalPanVelocity.x , _NormalPanVelocity.y));
			float2 uv_WaterNormal = i.uv_texcoord * _WaterNormal_ST.xy + _WaterNormal_ST.zw;
			float2 panner381 = ( 0.0 * _Time.y * float2( 0.1,0.1 ) + ( uv_WaterNormal * float2( 0.5,0.5 ) ));
			float2 temp_output_367_0 = ( uv_WaterNormal + ( (UnpackNormal( tex2D( _WaterNormal, panner381 ) )).xy * 0.01 ) );
			float2 panner160 = ( _Time.y * appendResult469 + temp_output_367_0);
			float2 appendResult470 = (float2(_NormalPanVelocity.z , _NormalPanVelocity.w));
			float2 panner157 = ( _Time.y * appendResult470 + temp_output_367_0);
			float2 uv_RippleNormalTex = i.uv_texcoord * _RippleNormalTex_ST.xy + _RippleNormalTex_ST.zw;
			float4 tex2DNode405 = tex2D( _RippleNormalTex, uv_RippleNormalTex );
			float3 appendResult414 = (float3(( (tex2DNode405).rg * _RippleScale ) , 1.0));
			float3 normalizeResult416 = normalize( appendResult414 );
			float3 RippleNormal406 = normalizeResult416;
			float3 normal235 = BlendNormals( BlendNormals( UnpackScaleNormal( tex2D( _WaterNormal, panner160 ), _NormalScale ) , UnpackScaleNormal( tex2D( _WaterNormal, panner157 ), _NormalScale ) ) , RippleNormal406 );
			float4 refl308 = tex2D( _ReflectionTex, ( ( (computeScreenPos340).xy + ( (normal235).xy * _ReflectionDistortion ) ) / (computeScreenPos340).w ) );
			SurfaceOutputStandard s464 = (SurfaceOutputStandard ) 0;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 screenColor196 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( float3( (ase_grabScreenPosNorm).xy ,  0.0 ) + ( normal235 * _Distortion ) ).xy);
			float4 ase_screenPos443 = i.screenPosition443;
			float4 ase_screenPosNorm443 = ase_screenPos443 / ase_screenPos443.w;
			ase_screenPosNorm443.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm443.z : ase_screenPosNorm443.z * 0.5 + 0.5;
			float screenDepth443 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm443.xy ));
			float distanceDepth443 = abs( ( screenDepth443 - LinearEyeDepth( ase_screenPosNorm443.z ) ) / ( 1.0 ) );
			float waterDepth245 = saturate( pow( ( ( abs( distanceDepth443 ) + _WaterDepth ) / _WaterFallOff ) , _WaterFalloffPower ) );
			float4 lerpResult185 = lerp( _ShalowColor , _DeepColor , waterDepth245);
			float4 lerpResult202 = lerp( screenColor196 , lerpResult185 , waterDepth245);
			float4 lerpResult345 = lerp( lerpResult202 , refl308 , _ReflectionIntensity);
			s464.Albedo = lerpResult345.rgb;
			s464.Normal = normalize( WorldNormalVector( i , normal235 ) );
			s464.Emission = float3( 0,0,0 );
			s464.Metallic = 0.0;
			s464.Smoothness = _Smoothness;
			s464.Occlusion = 1.0;

			data.light = gi.light;

			UnityGI gi464 = gi;
			#ifdef UNITY_PASS_FORWARDBASE
			Unity_GlossyEnvironmentData g464 = UnityGlossyEnvironmentSetup( s464.Smoothness, data.worldViewDir, s464.Normal, float3(0,0,0));
			gi464 = UnityGlobalIllumination( data, s464.Occlusion, s464.Normal, g464 );
			#endif

			float3 surfResult464 = LightingStandard ( s464, viewDir, gi464 ).rgb;
			surfResult464 += s464.Emission;

			#ifdef UNITY_PASS_FORWARDADD//464
			surfResult464 -= s464.Emission;
			#endif//464
			float smoothstepResult457 = smoothstep( 0.0 , 1.0 , tex2DNode405.a);
			float darkMask447 = saturate( ( smoothstepResult457 + _DarkControl ) );
			float4 lerpResult465 = lerp( refl308 , float4( surfResult464 , 0.0 ) , darkMask447);
			c.rgb = lerpResult465.rgb;
			c.a = _OverAllOpacity;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19603
Node;AmplifyShaderEditor.CommentaryNode;150;-7048.619,-1354.824;Inherit;False;3578.382;829.257;Blend panning normals to fake noving ripples;22;235;408;407;430;176;165;163;157;162;160;376;367;371;391;374;392;381;389;155;468;469;470;WaterNormal;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;155;-6852.03,-1146.178;Inherit;False;0;163;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;389;-6827.645,-1005.276;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;381;-6681.637,-1001.732;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0.1;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;392;-6415.968,-1013.25;Inherit;True;Property;_TextureSample4;Texture Sample 4;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Instance;163;Auto;Texture2D;8;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.CommentaryNode;415;-1824.662,735.1385;Inherit;False;1567.006;794.4202;Comment;12;447;406;416;414;413;412;411;405;457;459;460;461;RippleNormal;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;374;-5733.375,-882.8186;Float;False;Constant;_Float1;Float 1;29;0;Create;True;0;0;0;False;0;False;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;391;-6065.577,-1012.943;Inherit;True;True;True;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;405;-1807.068,784.1385;Inherit;True;Property;_RippleNormalTex;RippleNormalTex;7;0;Create;True;0;0;0;False;0;False;-1;None;ff3906d860051c742871eeaac54ffccd;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;412;-1102.159,900.4294;Float;False;Property;_RippleScale;RippleScale;8;0;Create;True;0;0;0;False;0;False;0;-0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;371;-5721.473,-1002.116;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;411;-1288.135,787.1836;Inherit;False;True;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;468;-5959.054,-1217.698;Inherit;False;Property;_NormalPanVelocity;NormalPanVelocity;19;0;Create;True;0;0;0;False;0;False;0,0,0,0;-0.1,0.02,-0.05,-0.05;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;413;-1064.546,793.4606;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;367;-5444.448,-1135.061;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;376;-5494.061,-1217.895;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;470;-5683.721,-1093.565;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;469;-5702.954,-1270.998;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;414;-885.6529,823.9456;Inherit;False;FLOAT3;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-5075.272,-1146.875;Float;False;Property;_NormalScale;Normal Scale;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;160;-5231.895,-1264.426;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.03,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;157;-5207.024,-1044.981;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.04,0.04;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;165;-4875.675,-1295.865;Inherit;True;Property;_Normal2;Normal2;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Instance;163;Auto;Texture2D;8;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.NormalizeNode;416;-746.6177,824.5326;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;163;-4867.604,-1073.542;Inherit;True;Property;_WaterNormal;Water Normal;1;0;Create;True;0;0;0;False;0;False;-1;None;c288925dd7aaca24d87b165291e64070;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.CommentaryNode;152;-5263.332,-315.9633;Inherit;False;2305.525;645.4458;Depths controls and colors;18;185;181;179;192;245;171;175;177;174;164;417;166;418;159;161;151;443;444;;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;444;-5149.393,-143.2073;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendNormalsNode;176;-4549.5,-1163.314;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;406;-579.3577,818.4346;Float;False;RippleNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DepthFade;443;-4913.547,-146.9097;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;407;-4297.296,-915.6259;Inherit;False;406;RippleNormal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;430;-4200.038,-1015.885;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.AbsOpNode;151;-4623.013,-145.6869;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;312;-5084.724,2127.621;Inherit;False;1852.295;908.25;Comment;14;351;350;348;307;346;306;304;347;340;342;341;308;287;266;Reflection;1,1,1,1;0;0
Node;AmplifyShaderEditor.BlendNormalsNode;408;-4064.574,-1007.769;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;161;-4282.917,-19.61691;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;159;-4057.336,101.112;Float;False;Property;_WaterDepth;Water Depth;12;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;341;-5069.385,2568.938;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;235;-3739.084,-1013.95;Float;False;normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;418;-3866.537,103.5171;Float;False;Property;_WaterFallOff;WaterFallOff;13;0;Create;True;0;0;0;False;0;False;0;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;166;-4042.728,-21.75044;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.UnityObjToClipPosHlpNode;342;-4882.004,2569.923;Inherit;False;1;0;FLOAT3;0,0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;347;-4492.767,2589.029;Inherit;False;235;normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;417;-3813.289,1.726189;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;164;-3676.083,119.2795;Float;False;Property;_WaterFalloffPower;WaterFalloffPower;14;0;Create;True;0;0;0;False;0;False;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComputeScreenPosHlpNode;340;-4710.429,2570.234;Inherit;False;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;351;-4356.251,2675.126;Float;False;Property;_ReflectionDistortion;ReflectionDistortion;17;0;Create;True;0;0;0;False;0;False;0;0.391;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;348;-4326.767,2590.029;Inherit;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PowerNode;174;-3674.068,27.09521;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;350;-4132.251,2619.126;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;304;-4480.193,2490.718;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;170;-2746.112,-1358.316;Inherit;False;991.6011;476.6005;Get screen color for refraction and disturbe it with normals;8;182;196;191;188;187;184;252;247;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;177;-3506.813,34.55342;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;184;-2696.005,-1298.958;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;182;-2671.804,-1043.008;Float;False;Property;_Distortion;Distortion;3;0;Create;True;0;0;0;False;0;False;0.5;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;252;-2691.199,-1125.203;Inherit;False;235;normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;306;-4491.36,2766.736;Inherit;False;False;False;False;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;346;-4085.248,2496.134;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;175;-3901.601,-266.6419;Float;False;Property;_ShalowColor;Shalow Color;9;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.2701562,0.3933854,0.4549999,1;False;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;171;-3606.264,-165.206;Float;False;Property;_DeepColor;Deep Color;10;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.02896937,0.05731613,0.06603767,1;False;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;187;-2319.111,-1136.316;Inherit;False;2;2;0;FLOAT3;1,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;188;-2392.658,-1241.824;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;307;-3960.786,2750.361;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;245;-3363.535,38.07006;Float;False;waterDepth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;266;-4180.154,2222.348;Float;True;Property;_ReflectionTex;ReflectionTex;15;0;Create;True;0;0;0;False;0;False;None;;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleAddOpNode;191;-2166.012,-1203.216;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;287;-3825.82,2365.74;Inherit;True;Property;_TextureSample1;Texture Sample 1;23;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.WireNode;192;-3094.766,72.56043;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;179;-3358.315,-203.0461;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;181;-3290.702,-78.39047;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;459;-1648.396,1235.123;Inherit;False;Property;_DarkControl;DarkControl;18;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;457;-1673.666,1099.863;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;185;-3122.796,-115.2352;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScreenColorNode;196;-1974.511,-1207.016;Float;False;Global;_WaterGrab;WaterGrab;-1;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;194;-1774.004,12.04325;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;460;-1443.781,1098.471;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;308;-3505.244,2368.886;Float;False;refl;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;202;-1616.621,-658.7708;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;337;-1360.993,-479.4946;Float;False;Property;_ReflectionIntensity;ReflectionIntensity;16;0;Create;True;0;0;0;False;0;False;0;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;461;-1286.781,1097.471;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;310;-1275.081,-571.3168;Inherit;False;308;refl;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;345;-1042.452,-649.1666;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;236;-902.0123,-184.8335;Inherit;False;235;normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;447;-1118.423,1090.313;Inherit;False;darkMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;364;-914.9753,-67.35305;Float;False;Property;_Smoothness;Smoothness;11;0;Create;True;0;0;0;False;0;False;0;0.818;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;431;-7187.116,-276.3848;Inherit;False;1526.251;872.6939;Comment;9;435;439;440;434;433;438;432;441;442;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CustomStandardSurface;464;-564.3663,-199.614;Inherit;False;Metallic;Tangent;6;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,1;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;467;-502.8535,-367.7737;Inherit;False;308;refl;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;448;-498.55,13.08918;Inherit;False;447;darkMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;432;-6928.152,-74.97021;Inherit;False;0;433;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;441;-7159.149,238.4529;Inherit;False;Property;_wobbleSpeed;wobbleSpeed;6;0;Create;True;0;0;0;False;0;False;0;0.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;440;-6233.243,25.94038;Inherit;False;Property;_wobbleIntensity;wobbleIntensity;5;0;Create;True;0;0;0;False;0;False;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;442;-6972.816,239.218;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;434;-6242.659,-75.00486;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;433;-6536.741,-101.1798;Inherit;True;Property;_WobbleTexture;WobbleTexture;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.PannerNode;438;-6706.047,60.81942;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.01,0.02;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;435;-5958.208,-80.08509;Inherit;False;wobble;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;439;-6088.636,-78.17548;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;454;-297.7907,353.4488;Inherit;False;447;darkMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;465;-198.2867,-216.2967;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;247;-2656,-960;Inherit;False;245;waterDepth;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;204;-208,-400;Float;False;Property;_OverAllOpacity;OverAllOpacity;0;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;224.6793,-442.7685;Float;False;True;-1;3;ASEMaterialInspector;0;0;CustomLighting;AmplifyShaders/WaterPlane_Interactable;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;2;10;25;False;0.5;False;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;389;0;155;0
WireConnection;381;0;389;0
WireConnection;392;1;381;0
WireConnection;391;0;392;0
WireConnection;371;0;391;0
WireConnection;371;1;374;0
WireConnection;411;0;405;0
WireConnection;413;0;411;0
WireConnection;413;1;412;0
WireConnection;367;0;155;0
WireConnection;367;1;371;0
WireConnection;470;0;468;3
WireConnection;470;1;468;4
WireConnection;469;0;468;1
WireConnection;469;1;468;2
WireConnection;414;0;413;0
WireConnection;160;0;367;0
WireConnection;160;2;469;0
WireConnection;160;1;376;0
WireConnection;157;0;367;0
WireConnection;157;2;470;0
WireConnection;157;1;376;0
WireConnection;165;1;160;0
WireConnection;165;5;162;0
WireConnection;416;0;414;0
WireConnection;163;1;157;0
WireConnection;163;5;162;0
WireConnection;176;0;165;0
WireConnection;176;1;163;0
WireConnection;406;0;416;0
WireConnection;443;1;444;0
WireConnection;430;0;176;0
WireConnection;151;0;443;0
WireConnection;408;0;430;0
WireConnection;408;1;407;0
WireConnection;161;0;151;0
WireConnection;235;0;408;0
WireConnection;166;0;161;0
WireConnection;166;1;159;0
WireConnection;342;0;341;0
WireConnection;417;0;166;0
WireConnection;417;1;418;0
WireConnection;340;0;342;0
WireConnection;348;0;347;0
WireConnection;174;0;417;0
WireConnection;174;1;164;0
WireConnection;350;0;348;0
WireConnection;350;1;351;0
WireConnection;304;0;340;0
WireConnection;177;0;174;0
WireConnection;306;0;340;0
WireConnection;346;0;304;0
WireConnection;346;1;350;0
WireConnection;187;0;252;0
WireConnection;187;1;182;0
WireConnection;188;0;184;0
WireConnection;307;0;346;0
WireConnection;307;1;306;0
WireConnection;245;0;177;0
WireConnection;191;0;188;0
WireConnection;191;1;187;0
WireConnection;287;0;266;0
WireConnection;287;1;307;0
WireConnection;192;0;245;0
WireConnection;179;0;175;0
WireConnection;181;0;171;0
WireConnection;457;0;405;4
WireConnection;185;0;179;0
WireConnection;185;1;181;0
WireConnection;185;2;245;0
WireConnection;196;0;191;0
WireConnection;194;0;192;0
WireConnection;460;0;457;0
WireConnection;460;1;459;0
WireConnection;308;0;287;0
WireConnection;202;0;196;0
WireConnection;202;1;185;0
WireConnection;202;2;194;0
WireConnection;461;0;460;0
WireConnection;345;0;202;0
WireConnection;345;1;310;0
WireConnection;345;2;337;0
WireConnection;447;0;461;0
WireConnection;464;0;345;0
WireConnection;464;1;236;0
WireConnection;464;4;364;0
WireConnection;442;0;441;0
WireConnection;434;0;433;1
WireConnection;434;1;433;2
WireConnection;433;1;438;0
WireConnection;438;0;432;0
WireConnection;438;1;442;0
WireConnection;435;0;439;0
WireConnection;439;0;434;0
WireConnection;439;1;440;0
WireConnection;465;0;467;0
WireConnection;465;1;464;0
WireConnection;465;2;448;0
WireConnection;0;9;204;0
WireConnection;0;13;465;0
ASEEND*/
//CHKSM=4821C30143F488E0D2227A69C1BD2D6A43DB6D4F