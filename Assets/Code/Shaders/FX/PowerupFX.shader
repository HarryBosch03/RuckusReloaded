Shader "Unlit/PowerupFX"
{
	Properties
	{
		_Brightness("Brightness", float) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Cull Back
			
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			struct Attributes
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
			};

			struct Varyings
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float4 depth : VAR_DEPTH;
			};

			struct Pixel
			{
				half4 color : SV_Target;
				float depth : SV_Depth;
			};

			Varyings vert (Attributes input)
			{
				Varyings output;
				output.vertex = TransformObjectToHClip(input.vertex.xyz);
				output.color = input.color;

				float3 positionWS = TransformObjectToWorld(input.vertex.xyz);
				float3 dir = normalize(positionWS - _WorldSpaceCameraPos);
				output.depth = TransformWorldToHClip(positionWS + dir);
				return output;
			}

			float _Brightness;
			
			Pixel frag (Varyings input)
			{
				Pixel pixel;
				pixel.color = float4(input.color.rgb * _Brightness, input.color.a);
				pixel.depth = input.depth.z / input.depth.w;
				
				return pixel;
			}
			ENDHLSL
		}
	}
}