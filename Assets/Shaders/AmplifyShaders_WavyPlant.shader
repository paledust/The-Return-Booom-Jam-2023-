// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AmplifyShaders/WavyPlant"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_BumpTex("BumpTex", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Bumpness("Bumpness", Float) = 0
		_RoughnessTex("RoughnessTex", 2D) = "white" {}
		_OcclusionMap("OcclusionMap", 2D) = "white" {}
		_WindTexture("WindTexture", 2D) = "white" {}
		_WindSpeed("WindSpeed", Vector) = (0,0,0,0)
		_WindStrength("WindStrength", Float) = 0
		_WindTiling("WindTiling", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform float _WindStrength;
		uniform sampler2D _WindTexture;
		uniform float2 _WindSpeed;
		uniform float2 _WindTiling;
		uniform sampler2D _BumpTex;
		uniform float4 _BumpTex_ST;
		uniform float _Bumpness;
		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _RoughnessTex;
		uniform float4 _RoughnessTex_ST;
		uniform sampler2D _OcclusionMap;
		uniform float4 _OcclusionMap_ST;


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


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float temp_output_31_0 = ( ase_vertex3Pos.y * _WindStrength );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float2 appendResult10 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 panner12 = ( _Time.y * _WindSpeed + ( appendResult10 * _WindTiling ));
			float4 tex2DNode11 = tex2Dlod( _WindTexture, float4( panner12, 0, 0.0) );
			float3 rotatedValue16 = RotateAroundAxis( float3( 0,0,0 ), ase_vertex3Pos, float3( 1,0,0 ), ( temp_output_31_0 * tex2DNode11.r ) );
			float3 rotatedValue20 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue16, float3( 0,0,1 ), ( temp_output_31_0 * tex2DNode11.b ) );
			v.vertex.xyz = rotatedValue20;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BumpTex = i.uv_texcoord * _BumpTex_ST.xy + _BumpTex_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _BumpTex, uv_BumpTex ), _Bumpness );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			o.Albedo = ( _Color * tex2D( _MainTex, uv_MainTex ) ).rgb;
			float2 uv_RoughnessTex = i.uv_texcoord * _RoughnessTex_ST.xy + _RoughnessTex_ST.zw;
			o.Smoothness = ( 1.0 - tex2D( _RoughnessTex, uv_RoughnessTex ).r );
			float2 uv_OcclusionMap = i.uv_texcoord * _OcclusionMap_ST.xy + _OcclusionMap_ST.zw;
			o.Occlusion = tex2D( _OcclusionMap, uv_OcclusionMap ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.ColorNode;2;-834.6813,-228.8711;Inherit;False;Property;_Color;Color;2;0;Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-518.6204,-144.2427;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-931.1293,-47.65275;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-953.8526,158.0017;Inherit;True;Property;_BumpTex;BumpTex;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;5;-1125.716,205.1875;Inherit;False;Property;_Bumpness;Bumpness;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;7;-688.298,403.4181;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;6;-975.9649,373.8051;Inherit;True;Property;_RoughnessTex;RoughnessTex;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;8;-946.8351,576.429;Inherit;True;Property;_OcclusionMap;OcclusionMap;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-264.2478,50.08617;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;AmplifyShaders/WavyPlant;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Absolute;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.SamplerNode;11;-1845.583,1128.474;Inherit;True;Property;_WindTexture;WindTexture;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-1486.967,1039.467;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-1486.927,1219.103;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotateAboutAxisNode;16;-1031.455,979.7787;Inherit;False;False;4;0;FLOAT3;1,0,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RotateAboutAxisNode;20;-736.0146,984.6035;Inherit;False;False;4;0;FLOAT3;0,0,1;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PosVertexDataNode;15;-1227.465,1060.18;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;30;-1839.118,789.517;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-1575.118,872.517;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-1834.263,965.8647;Inherit;False;Property;_WindStrength;WindStrength;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;12;-2445.616,1159.038;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;13;-2621.616,1178.038;Inherit;False;Property;_WindSpeed;WindSpeed;7;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;14;-2630.616,1309.038;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;9;-2995,856.2197;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;10;-2820.445,905.3131;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-2640.423,967.5746;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;34;-2966.423,1013.575;Inherit;False;Property;_WindTiling;WindTiling;9;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
WireConnection;3;0;2;0
WireConnection;3;1;1;0
WireConnection;4;5;5;0
WireConnection;7;0;6;1
WireConnection;0;0;3;0
WireConnection;0;1;4;0
WireConnection;0;4;7;0
WireConnection;0;5;8;1
WireConnection;0;11;20;0
WireConnection;11;1;12;0
WireConnection;23;0;31;0
WireConnection;23;1;11;1
WireConnection;24;0;31;0
WireConnection;24;1;11;3
WireConnection;16;1;23;0
WireConnection;16;3;15;0
WireConnection;20;1;24;0
WireConnection;20;3;16;0
WireConnection;31;0;30;2
WireConnection;31;1;22;0
WireConnection;12;0;32;0
WireConnection;12;2;13;0
WireConnection;12;1;14;0
WireConnection;10;0;9;1
WireConnection;10;1;9;3
WireConnection;32;0;10;0
WireConnection;32;1;34;0
ASEEND*/
//CHKSM=2DF1A4DDC51B8533B8BF11867514AE14C6AEAC4E