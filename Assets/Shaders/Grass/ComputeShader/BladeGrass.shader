Shader "Custom/BladeGrassCompute"
{
    Properties
    {
        _Color("Base Color", Color) = (1,1,1,1)
        _TipColor("Tip Color", Color) = (1,1,1,1)
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
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma target 5.0

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityLightingCommon.cginc"
            #include "BladeGrass.hlsl"

            ENDCG
        }
    }
}
