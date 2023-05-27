// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AmplifyShaders/GroundWithDecal"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_WetColor("WetColor", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
		_MetallicGlossMap("MetallicGlossMap", 2D) = "white" {}
		_BumpMap("BumpMap", 2D) = "bump" {}
		_BumpScale("BumpScale", Float) = 0
		_OcclusionMap("OcclusionMap", 2D) = "white" {}
		_WetTex("WetTex", 2D) = "black" {}
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_WetSmoothness("WetSmoothness", Range( 0 , 1)) = 0
		_WetMetallic("WetMetallic", Range( 0 , 1)) = 0.5
		_OverAllWetness("OverAllWetness", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _BumpMap;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _BumpScale;
		uniform float4 _Color;
		uniform float4 _WetColor;
		uniform sampler2D _WetTex;
		uniform float4x4 RainDropMatrix;
		uniform float _OverAllWetness;
		uniform float _Metallic;
		uniform sampler2D _MetallicGlossMap;
		uniform float _WetMetallic;
		uniform float _Smoothness;
		uniform float _WetSmoothness;
		uniform sampler2D _OcclusionMap;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _BumpMap, uv_MainTex ), _BumpScale );
			float3 ase_worldPos = i.worldPos;
			float4 appendResult13 = (float4(ase_worldPos.x , ase_worldPos.y , ase_worldPos.z , 1.0));
			float4 break15 = mul( RainDropMatrix, appendResult13 );
			float2 appendResult16 = (float2(break15.x , break15.y));
			float rainMask21 = saturate( ( tex2D( _WetTex, ( ( ( appendResult16 / break15.w ) * 0.5 ) + float2( 0.5,0.5 ) ) ).r + _OverAllWetness ) );
			float4 lerpResult24 = lerp( _Color , _WetColor , rainMask21);
			o.Albedo = ( lerpResult24 * tex2D( _MainTex, uv_MainTex ) ).rgb;
			float4 tex2DNode3 = tex2D( _MetallicGlossMap, uv_MainTex );
			float lerpResult37 = lerp( ( _Metallic * tex2DNode3.r ) , _WetMetallic , rainMask21);
			o.Metallic = lerpResult37;
			float lerpResult31 = lerp( ( tex2DNode3.a * _Smoothness ) , _WetSmoothness , rainMask21);
			o.Smoothness = lerpResult31;
			o.Occlusion = tex2D( _OcclusionMap, uv_MainTex ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.SamplerNode;4;-711.5046,-374.0984;Inherit;True;Property;_MainTex;MainTex;2;0;Create;True;0;0;0;False;0;False;-1;None;992f8426259e8f64985d1e26628e4998;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-713.5046,-174.0984;Inherit;True;Property;_BumpMap;BumpMap;4;0;Create;True;0;0;0;False;0;False;-1;None;3b463d7df9e48e24a94cf0033cb321d3;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;6;-883.4796,-75.41449;Inherit;False;Property;_BumpScale;BumpScale;5;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-395.2637,-505.5819;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-2664.668,-1055.91;Inherit;False;2;2;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Matrix4X4Node;10;-2964.668,-1118.91;Inherit;False;Global;RainDropMatrix;RainDropMatrix;6;0;Create;True;0;0;0;False;0;False;1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;15;-2475.028,-1095.378;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;16;-2326.028,-1093.378;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldPosInputsNode;11;-3002.569,-985.5098;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;13;-2837.669,-988.9099;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;14;-2177.028,-1046.378;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScaleNode;20;-2063.758,-1044.449;Inherit;False;0.5;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-1898.134,-1045.816;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;7;-821.2637,-893.5819;Inherit;False;Property;_Color;Color;0;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;23;-786.6885,-716.0569;Inherit;False;Property;_WetColor;WetColor;1;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;24;-530.6885,-734.0569;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;22;-745.5807,-541.6187;Inherit;False;21;rainMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;15.34736,-195.9187;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;AmplifyShaders/GroundWithDecal;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.SamplerNode;1;-566.8352,847.9759;Inherit;True;Property;_OcclusionMap;OcclusionMap;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;25;-760.3068,127.9922;Inherit;False;Property;_Metallic;Metallic;8;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-780.9348,236.2759;Inherit;True;Property;_MetallicGlossMap;MetallicGlossMap;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-468.8912,174.0381;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-462.9692,402.303;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-750.4151,458.2605;Inherit;False;Property;_Smoothness;Smoothness;9;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;31;-309.5475,501.9903;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;32;-530.2736,674.6297;Inherit;False;21;rainMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;5;-1337.249,-151.4536;Inherit;False;0;4;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;33;-1720.64,-850.3553;Inherit;False;Property;_OverAllWetness;OverAllWetness;12;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;34;-1418.909,-943.8936;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;35;-1300.909,-939.8936;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;21;-1168.687,-946.8979;Inherit;False;rainMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;17;-1732.659,-1076.634;Inherit;True;Property;_WetTex;WetTex;7;0;Create;True;0;0;0;False;0;False;-1;None;2e3b57e8add22294ba16d883757d6ae4;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;30;-646.2473,572.1901;Inherit;False;Property;_WetSmoothness;WetSmoothness;10;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;37;-298.8887,223.9419;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-89.77264,494.2331;Inherit;False;Property;_WetMetallic;WetMetallic;11;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
WireConnection;4;1;5;0
WireConnection;2;1;5;0
WireConnection;2;5;6;0
WireConnection;8;0;24;0
WireConnection;8;1;4;0
WireConnection;12;0;10;0
WireConnection;12;1;13;0
WireConnection;15;0;12;0
WireConnection;16;0;15;0
WireConnection;16;1;15;1
WireConnection;13;0;11;1
WireConnection;13;1;11;2
WireConnection;13;2;11;3
WireConnection;14;0;16;0
WireConnection;14;1;15;3
WireConnection;20;0;14;0
WireConnection;18;0;20;0
WireConnection;24;0;7;0
WireConnection;24;1;23;0
WireConnection;24;2;22;0
WireConnection;0;0;8;0
WireConnection;0;1;2;0
WireConnection;0;3;37;0
WireConnection;0;4;31;0
WireConnection;0;5;1;1
WireConnection;1;1;5;0
WireConnection;3;1;5;0
WireConnection;27;0;25;0
WireConnection;27;1;3;1
WireConnection;28;0;3;4
WireConnection;28;1;26;0
WireConnection;31;0;28;0
WireConnection;31;1;30;0
WireConnection;31;2;32;0
WireConnection;34;0;17;1
WireConnection;34;1;33;0
WireConnection;35;0;34;0
WireConnection;21;0;35;0
WireConnection;17;1;18;0
WireConnection;37;0;27;0
WireConnection;37;1;36;0
WireConnection;37;2;32;0
ASEEND*/
//CHKSM=81C959090F49A65D78068B98758F2BF8EF58AA36