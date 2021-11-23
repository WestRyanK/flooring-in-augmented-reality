Shader "Projector/Segmentation" {
	Properties {
		[NoScaleOffset] _MainTex ("Texture", 2D) = "gray" {}
		_ClipCutoff("Clip Cutoff", Range(0, 1)) = 0.5
	}
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			// ZWrite Off
			// ColorMask RGB
            ColorMask 0
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct vertex2fragment {
				float4 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};
			
			float4x4 unity_Projector;
			
			vertex2fragment vert(float4 vertex : POSITION)
			{
				vertex2fragment output;
				output.pos = UnityObjectToClipPos(vertex);
				output.uv = mul(unity_Projector, vertex);
				return output;
			}
			
			sampler2D _MainTex;
			
			fixed4 frag(vertex2fragment input) : SV_Target
			{
				fixed2 coords = (input.uv / input.uv.w).xy;
				if (any(coords.xy > 1) || any(coords.xy < 0))
					return fixed(1);

                fixed color = tex2D(_MainTex, coords).r - 0.5;
                clip(color);

				return fixed(1);
			}
			ENDCG
		}
	}
}
