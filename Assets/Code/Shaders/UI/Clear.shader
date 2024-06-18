Shader "Hidden/Clear"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

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
			};

			struct Varyings
			{
				float4 vertex : SV_POSITION;
			};

			Varyings vert (Attributes input)
			{
				Varyings output;
				output.vertex = input.vertex;
				return output;
			}

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);
			
			float4 frag () : SV_Target
			{
				return 0.0;
			}
			ENDHLSL
		}
	}
}