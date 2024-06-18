Shader "Hidden/ViewportOverlay"
{
    Properties { }
    SubShader
    {
        Pass
        {
            ZTest Always
            ZWrite Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"
            
            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.vertex = float4(input.vertex.xyz, 1.0);
                output.uv = input.uv;
                return output;
            }
            
            TEXTURE2D(_ViewportOverlay);
            SAMPLER(sampler_ViewportOverlay);
            float4 _ViewportOverlay_TexelSize;
            
            float dither(float4 ScreenPosition)
            {
                float2 uv = ScreenPosition.xy / _ViewportOverlay_TexelSize.xy;
                float DITHER_THRESHOLDS[16] =
                {
                    1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                    13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                    4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                    16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                };
                uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
                return DITHER_THRESHOLDS[index];
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_ViewportOverlay, sampler_ViewportOverlay, input.uv);

                float crunch = 32;
                col.rgb = pow(col.rgb, 1.0 / 2.2);
                col.rgb = floor(col.rgb * crunch - dither(float4(input.uv, 0.0, 0.0))) / crunch;
                col.rgb = pow(col.rgb, 2.2);

                col.rgb = saturate(col.rgb);
                
                clip(col.a * 255 - 1);
                return col;
            }
            ENDHLSL
        }
    }
}