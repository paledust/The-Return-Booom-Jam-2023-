Shader "Custom/BladeGrassCompute"
{
    Properties
    {
        _TranslucentGain("TranslucentGain",Range(0,1))=0.5
        _Color("Base Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Name "ForwardLit"
            Tags{"LightMode"="ForwardBase"}
            
            Cull Off
            
            CGPROGRAM
            #pragma multi_compile_fwdbase 

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 5.0

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityLightingCommon.cginc"
            #include "BladeGrass.hlsl"

            ENDCG
        }
        Pass
        {
            Name "ForwardLitAdd"
            Tags{"LightMode"="ForwardAdd"}

            Cull Off
            Blend One One
            
            CGPROGRAM
            #pragma multi_compile_fwdadd_fullshadows

            #pragma vertex vert
            #pragma fragment fragAdd
            #pragma target 5.0

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityLightingCommon.cginc"
            #include "BladeGrass.hlsl"

            ENDCG
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {"LightMode" = "ShadowCaster"}

            Cull Off
            CGPROGRAM
            #pragma multi_compile_shadowcaster
            #pragma vertex vert
            #pragma fragment fragShadow

            #pragma target 5.0

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityLightingCommon.cginc"
            #include "BladeGrass.hlsl"

            ENDCG
        }
    }
}
