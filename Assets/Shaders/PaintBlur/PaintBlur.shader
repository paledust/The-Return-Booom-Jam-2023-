Shader "Hidden/Custom/PaintBlur"
{
    HLSLINCLUDE
// StdLib.hlsl holds pre-configured vertex shaders (VertDefault), varying structs (VaryingsDefault), and most of the data you need to write common effects.
        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
        #include "Assets/DevelopBasic/ShaderFunction/HelperFunc.cginc"

        sampler2D _MainTex;
        sampler2D _PaintBlurMaskTex;

        float4 _MainTex_ST;
        float4 _MainTex_TexelSize;
        
        int _Radius;
        int _Sample;

        half _MaskStrength;
        half _BlurSize;
        half _BrushSize;
        half _BrushAngle;
        half _Darken;

        inline float DecodeFloatRG( float2 enc ){
            float2 kDecodeDot = float2(1.0, 1/255.0);
            return dot( enc, kDecodeDot );
        }

        float4 Frag_Vertical(VaryingsDefault i) : SV_Target{
            float4 col = 0;

            float mask = tex2D(_PaintBlurMaskTex, i.texcoord).r;
            mask = lerp(1, mask, _MaskStrength);

            for(float index = 0; index < _Sample; index++){
                float2 uv = i.texcoord + float2(0, (index/(_Sample-1) - 0.5) * _BlurSize * mask);
                col += tex2D(_MainTex, uv) * lerp(1,1-_Darken,mask);
            }
            col /= _Sample;
            return col;
        }
        float4 Frag_Horizontal(VaryingsDefault i) : SV_Target{
            float invAspect = _ScreenParams.y/_ScreenParams.x;
            float4 col = 0;

            float mask = tex2D(_PaintBlurMaskTex, i.texcoord).r;
            mask = lerp(1, mask, _MaskStrength);

            for(float index = 0; index < _Sample; index++){
                float2 uv = i.texcoord + float2((index/(_Sample-1) - 0.5) * _BlurSize * invAspect * mask, 0);
                col += tex2D(_MainTex, uv) * lerp(1,1-_Darken,mask);
            }
            col = col/_Sample;
            return col;
        }
        float4 FragKuwaharaFilter(VaryingsDefault i) : SV_Target{
            float2 uv = i.texcoord;
            float mask = tex2D(_PaintBlurMaskTex, uv).r;
            mask = lerp(1, mask, _MaskStrength);

            float3 mean[4]  = {{0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}};
            float3 sigma[4] = {{0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}};
            float2 start[4] = {{-_Radius, -_Radius}, {-_Radius, 0}, {0, -_Radius}, {0,0}};

            float2 pos;
            float3 col;
            for(int k=0; k<4; k++){
                for(int i=0; i<=_Radius; i++){
                    for(int j=0; j<=_Radius; j++){
                        pos = float2(i, j) + start[k];
                        col = tex2Dlod(_MainTex, float4(uv + mask * _BrushSize * float2(pos.x * _MainTex_TexelSize.x, pos.y * _MainTex_TexelSize.y), 0., 0.)).rgb;
                        mean[k] += col;
                        sigma[k] += col*col;
                    }
                }
            }
            float sigma2;

            float n = (_Radius+1)*(_Radius+1);
            float4 color = tex2D(_MainTex, uv);
            float4 origColor = color;
            float min = 1;
            
            for(int i=0; i<4; i++){
                mean[i] /= n; //Calculate the average color of the sub square.
                sigma[i]=abs(sigma[i]/n - mean[i]*mean[i]); //Calculate the standard deviant
                sigma2  = sigma[i].r + sigma[i].g + sigma[i].b; //Because each color's deviant might be different, we sum it up to get the overall result

                if(sigma2<min){
                    min = sigma2;
                    color.rgb = mean[i].rgb; // Assign the average color to the most smooth result
                }
            }

            return lerp(origColor, color, mask);
        }
    ENDHLSL
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment FragKuwaharaFilter
            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment Frag_Vertical
            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment Frag_Horizontal
            ENDHLSL
        }
    }
}