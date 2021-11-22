Shader "Projector/Segmentation" {
	Properties {
		_MainTex ("Texture", 2D) = "gray" {}
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
			
			struct v2f {
				float4 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};
			
			float4x4 unity_Projector;
			
			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(vertex);
				o.uv = mul (unity_Projector, vertex);
				return o;
			}
			
			sampler2D _MainTex;
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed2 coords = (i.uv / i.uv.w).xy;
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
