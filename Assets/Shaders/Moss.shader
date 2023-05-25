Shader "Fur/Moss_Interaction"
{
    Properties
    {
        _Cutoff( "Mask Clip Value", Float ) = 0.5

        _Color ("Color", Color) = (1, 1, 1, 1)
        [HDR]_MudColor ("Mud Color", Color) = (1,1,1,1)
        
        _MainTex ("Texture", 2D) = "white" {}
        _FurTex ("Fur Pattern", 2D) = "white" {}
        _BumpMap ("Bump Texture", 2D) = "Bump" {}

        _MaskTex ("Mask Texture", 2D) = "black" {}

        _CutOff("Cut Off Value", Range(0,1)) = 0
        _MaxValue("Lerp Max Value", Range(0,1)) = 0
        _MinValue("Lerp Min Value", Range(0,1)) = 0

        _FurLength ("Fur Length", Range(0.0, 1)) = 0.5
        _FurDensity ("Fur Density", Range(0, 2)) = 0.11
        _FurThinness ("Fur Thinness", Range(0.01, 10)) = 1
        _FurShading ("Fur Shading", Range(0.0, 1)) = 0.25
        _FurNormalScale("Fur Normal", Float) = 1

        _ForceGlobal ("Force Global", Vector) = (0, 0, 0, 0)
        _ForceLocal ("Force Local", Vector) = (0, 0, 0, 0)

		_DissolveMap("DissolveMap", 2D) = "white" {}
		_DissolveTiling("DissolveTiling", Float) = 1
		_DissolveTriPlanarFalloff("DissolveTriPlanarFalloff", Float) = 1
		_DissolveMin("DissolveMin", Float) = 0
		_DissolveMax("DissolveMax", Float) = 0
		[Toggle]_InvertDissolveMap("InvertDissolveMap", Float) = 0
		[Toggle]_InvertDissolveDirection("InvertDissolveDirection", Float) = 1
		_dissolveLength("dissolveLength", Float) = 1
		_dissolveRadius("dissolveRadius", Float) = 2
		_dissolveCenter("dissolveCenter", Vector) = (0,0,0,0)
        _AnimeSpeed("Animation speed", Float) = 0.05
    }
    
    Category
    {

        Tags { "RenderType" = "Opaque" "IgnoreProjector" = "True" "Queue" = "AlphaTest" "LightMode" = "ForwardBase"}
        Blend SrcAlpha OneMinusSrcAlpha
        
        SubShader
        {
            CGINCLUDE
                #define DISSOLVE
                sampler2D _MaskTex;

                float _AnimeSpeed;
                float _InvertDissolveDirection;
                float _InvertDissolveMap;
                float4 _MaskTex_ST;
                float4 _MudColor;
                float _CutOff;
                float _MaxValue;
                float _MinValue;
            ENDCG

            Pass
            {
                CGPROGRAM

                #define FURSTEP 0.05
                #include "FurHelper.cginc"
                #pragma vertex vert_base
                #pragma fragment frag_Dissolve
                
                ENDCG
                
            }
            
            Pass
            {
                CGPROGRAM
                
                #define FURSTEP 0.10
                #include "FurHelper.cginc"
                #pragma vertex vert_base
                #pragma fragment frag_Dissolve
                
                ENDCG
                
            }
            
            Pass
            {
                CGPROGRAM
                
                #define FURSTEP 0.15
                #include "FurHelper.cginc"
                #pragma vertex vert_base
                #pragma fragment frag_Dissolve
                
                ENDCG
                
            }
            
            Pass
            {
                CGPROGRAM
                
                #define FURSTEP 0.20
                #include "FurHelper.cginc"
                #pragma vertex vert_base
                #pragma fragment frag_Dissolve
                
                ENDCG
                
            }
            
            Pass
            {
                CGPROGRAM
                
                #define FURSTEP 0.25
                #include "FurHelper.cginc"
                #pragma vertex vert_base
                #pragma fragment frag_Dissolve
                
                ENDCG
                
            }
            
            Pass
            {
                CGPROGRAM
                
                #define FURSTEP 0.30
                #include "FurHelper.cginc"
                #pragma vertex vert_base
                #pragma fragment frag_Dissolve
                
                ENDCG
                
            }
            
            Pass
            {
                CGPROGRAM
                
                #define FURSTEP 0.35
                #include "FurHelper.cginc"
                #pragma vertex vert_base
                #pragma fragment frag_Dissolve
                
                ENDCG
                
            }
            
            Pass
            {
                CGPROGRAM
                
                #define FURSTEP 0.40
                #include "FurHelper.cginc"
                #pragma vertex vert_base
                #pragma fragment frag_Dissolve
                
                ENDCG
                
            }
            
            Pass
            {
                CGPROGRAM
                
                #define FURSTEP 0.45
                #include "FurHelper.cginc"
                #pragma vertex vert_base
                #pragma fragment frag_Dissolve
                
                ENDCG
                
            }
            
            Pass
            {
                CGPROGRAM
                
                #define FURSTEP 0.50
                #include "FurHelper.cginc"
                #pragma vertex vert_base
                #pragma fragment frag_Dissolve
                
                ENDCG
                
            }
            
            Pass
            {
                CGPROGRAM
                
                #define FURSTEP 0.55
                #include "FurHelper.cginc"
                #pragma vertex vert_base
                #pragma fragment frag_Dissolve
                
                ENDCG
                
            }
            
            Pass
            {
                CGPROGRAM
                
                #define FURSTEP 0.60
                #include "FurHelper.cginc"
                #pragma vertex vert_base
                #pragma fragment frag_Dissolve
                
                ENDCG
                
            }
            
            Pass
            {
                CGPROGRAM
                
                #define FURSTEP 0.65
                #include "FurHelper.cginc"
                #pragma vertex vert_base
                #pragma fragment frag_Dissolve
                
                ENDCG
                
            }
            
            Pass
            {
                CGPROGRAM
                
                #define FURSTEP 0.70
                #include "FurHelper.cginc"
                #pragma vertex vert_base
                #pragma fragment frag_Dissolve
                
                ENDCG
                
            }
            
            Pass
            {
                CGPROGRAM
                
                #define FURSTEP 0.75
                #include "FurHelper.cginc"
                #pragma vertex vert_base
                #pragma fragment frag_Dissolve
                
                ENDCG
                
            }
            
            Pass
            {
                CGPROGRAM
                
                #define FURSTEP 0.80
                #include "FurHelper.cginc"
                #pragma vertex vert_base
                #pragma fragment frag_Dissolve
                
                ENDCG
                
            }
            
            Pass
            {
                CGPROGRAM
                
                #define FURSTEP 0.85
                #include "FurHelper.cginc"
                #pragma vertex vert_base
                #pragma fragment frag_Dissolve
                
                ENDCG
                
            }
            
            Pass
            {
                CGPROGRAM
                
                #define FURSTEP 0.90
                #include "FurHelper.cginc"
                #pragma vertex vert_base
                #pragma fragment frag_Dissolve
                
                ENDCG
                
            }
            
            Pass
            {
                CGPROGRAM
                
                #define FURSTEP 0.95
                #include "FurHelper.cginc"
                #pragma vertex vert_base
                #pragma fragment frag_Dissolve
                
                ENDCG
                
            }

            Pass
            {
                CGPROGRAM
                
                #define FURSTEP 1.00
                #include "FurHelper.cginc"
                #pragma vertex vert_base
                #pragma fragment frag_Dissolve
                
                ENDCG
                
            }
        }
    }
}