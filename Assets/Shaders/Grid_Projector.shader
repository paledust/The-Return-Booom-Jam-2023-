// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AmplifyShaders/Grid_Projector"
{
	Properties
	{
		[HDR]_Color("Color", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
		_glitchSpeed("glitchSpeed", Float) = 0
		_GlitchPixel("GlitchPixel", Int) = 16
		_glitchWidth("glitchWidth", Float) = 0
		_glitchFreq("glitchFreq", Float) = 0
		_GltichRange("GltichRange", Float) = 0

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha One
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset -1 , -1
		
		
		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_FRAG_POSITION


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform float4 _Color;
			uniform sampler2D _MainTex;
			float4x4 unity_Projector;
			uniform float _GltichRange;
			uniform int _GlitchPixel;
			uniform float _glitchFreq;
			uniform float _glitchSpeed;
			uniform float _glitchWidth;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord1 = v.vertex;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float4 temp_output_4_0 = mul( unity_Projector, i.ase_texcoord1 );
				float2 temp_output_10_0 = ( (temp_output_4_0).xy / (temp_output_4_0).w );
				float2 break67 = temp_output_10_0;
				float2 appendResult88 = (float2(_Time.y , ( floor( ( (temp_output_10_0).y * _GlitchPixel ) ) / _GlitchPixel )));
				float dotResult4_g1 = dot( appendResult88 , float2( 12.9898,78.233 ) );
				float lerpResult10_g1 = lerp( -_GltichRange , _GltichRange , frac( ( sin( dotResult4_g1 ) * 43758.55 ) ));
				float glitch94 = lerpResult10_g1;
				float2 appendResult96 = (float2(( break67.x + glitch94 ) , break67.y));
				float mulTime70 = _Time.y * _glitchSpeed;
				float2 lerpResult80 = lerp( temp_output_10_0 , appendResult96 , step( (sin( ( ( break67.y * _glitchFreq ) + mulTime70 ) )*0.5 + 0.5) , _glitchWidth ));
				float2 projectorUV11 = lerpResult80;
				
				
				finalColor = ( _Color * tex2D( _MainTex, projectorUV11 ) );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.CommentaryNode;23;-3442.629,-1883.397;Inherit;False;2500.65;1405.518;Comment;32;67;74;76;78;77;75;66;71;70;69;65;92;89;91;88;85;84;82;11;80;10;9;8;1;3;4;93;94;95;96;97;98;Projector UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;2;-1113.23,114.632;Inherit;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;0;False;0;False;-1;None;e057151dad8022c41adc44de8b274b72;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-773.6891,94.82556;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;13;-1041.445,-71.55429;Inherit;False;Property;_Color;Color;0;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;0,6.462745,11.98431,0.9490196;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;12;-581.5867,97.9173;Float;False;True;-1;2;ASEMaterialInspector;100;5;AmplifyShaders/Grid_Projector;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;8;5;False;;1;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;1;False;;True;3;False;;True;True;-1;False;;-1;False;;True;1;RenderType=Transparent=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-3119.523,-1277.764;Inherit;False;2;2;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PosVertexDataNode;3;-3300.175,-1241.765;Inherit;False;1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.UnityProjectorMatrixNode;1;-3267.238,-1315.638;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.ComponentMaskNode;8;-2956.385,-1330.771;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;9;-2948.185,-1198.47;Inherit;False;False;False;False;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;10;-2762.184,-1268.27;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;80;-1473.792,-1255.428;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;11;-1317.346,-1260.359;Inherit;False;projectorUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;15;-1353.473,253.357;Inherit;False;11;projectorUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;87;-1009.209,361.3337;Inherit;False;FLOAT4;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;-2400.723,-1505.497;Inherit;False;2;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;84;-2257.723,-1505.497;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;85;-2122.723,-1433.497;Inherit;False;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;88;-1989.171,-1528.407;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;91;-2196.953,-1601.133;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;92;-2591.098,-1517.081;Inherit;False;False;True;True;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;65;-2564.915,-1415.009;Inherit;False;Property;_GlitchPixel;GlitchPixel;3;0;Create;True;0;0;0;False;0;False;16;64;False;0;1;INT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;69;-2314.373,-824.6963;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;70;-2493.373,-801.6963;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;71;-2650.373,-808.6963;Inherit;False;Property;_glitchSpeed;glitchSpeed;2;0;Create;True;0;0;0;False;0;False;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;66;-2199.548,-825.7899;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;75;-2200.929,-752.2167;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-2440.965,-975.6464;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-2601.965,-910.6476;Inherit;False;Property;_glitchFreq;glitchFreq;5;0;Create;True;0;0;0;False;0;False;0;0.56;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-2165.866,-621.6492;Inherit;False;Property;_glitchWidth;glitchWidth;4;0;Create;True;0;0;0;False;0;False;0;0.004;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;67;-2566.015,-1047.578;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RegisterLocalVarNode;94;-1626.071,-1527.053;Inherit;False;glitch;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;93;-2146.955,-1189.214;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;96;-1983.118,-1190.344;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;95;-2323.738,-1169.265;Inherit;False;94;glitch;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;89;-1852.455,-1527.133;Inherit;False;Random Range;-1;;1;7b754edb8aebbfb4a9ace907af661cfc;0;3;1;FLOAT2;0,0;False;2;FLOAT;-0.05;False;3;FLOAT;0.05;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;74;-1993.129,-710.3177;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.06;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;98;-2019.062,-1702.225;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;97;-2194.062,-1708.225;Inherit;False;Property;_GltichRange;GltichRange;6;0;Create;True;0;0;0;False;0;False;0;0.015;0;0;0;1;FLOAT;0
WireConnection;2;1;15;0
WireConnection;14;0;13;0
WireConnection;14;1;2;0
WireConnection;12;0;14;0
WireConnection;4;0;1;0
WireConnection;4;1;3;0
WireConnection;8;0;4;0
WireConnection;9;0;4;0
WireConnection;10;0;8;0
WireConnection;10;1;9;0
WireConnection;80;0;10;0
WireConnection;80;1;96;0
WireConnection;80;2;74;0
WireConnection;11;0;80;0
WireConnection;87;0;15;0
WireConnection;82;0;92;0
WireConnection;82;1;65;0
WireConnection;84;0;82;0
WireConnection;85;0;84;0
WireConnection;85;1;65;0
WireConnection;88;0;91;0
WireConnection;88;1;85;0
WireConnection;92;0;10;0
WireConnection;69;0;77;0
WireConnection;69;1;70;0
WireConnection;70;0;71;0
WireConnection;66;0;69;0
WireConnection;75;0;66;0
WireConnection;77;0;67;1
WireConnection;77;1;78;0
WireConnection;67;0;10;0
WireConnection;94;0;89;0
WireConnection;93;0;67;0
WireConnection;93;1;95;0
WireConnection;96;0;93;0
WireConnection;96;1;67;1
WireConnection;89;1;88;0
WireConnection;89;2;98;0
WireConnection;89;3;97;0
WireConnection;74;0;75;0
WireConnection;74;1;76;0
WireConnection;98;0;97;0
ASEEND*/
//CHKSM=8A0D714BCBE1E4D9822F391E0EC69EF30C8FCEC6