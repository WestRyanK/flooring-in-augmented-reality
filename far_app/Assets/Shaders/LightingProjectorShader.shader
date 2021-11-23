Shader "Projector/Lighting" {
    Properties {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "gray" {}
    }
    Subshader {
        Tags {"Queue"="Transparent"}
        Pass {
            ZWrite On
            Blend DstColor Zero

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4x4 unity_Projector;

            struct vertex2fragment {
                float4 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };


            vertex2fragment vert(float4 vertex : POSITION)
            {
                vertex2fragment output;
                output.pos = UnityObjectToClipPos(vertex);
                output.uv = mul(unity_Projector, vertex);
                return output;
            }

            fixed4 frag(vertex2fragment input) : SV_Target
            {
                fixed2 coords = (input.uv / input.uv.w).xy;
                fixed4 color = tex2D(_MainTex, coords);
                color.rgb *= 2;
                color.a = 1;
                return color;
            }
            ENDCG
        }
    }
}
// https://docs.unity3d.com/Manual/SL-ShaderSemantics.html