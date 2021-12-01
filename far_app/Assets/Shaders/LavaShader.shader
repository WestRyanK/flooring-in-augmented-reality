// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge

Shader "Lava" {
    Properties {
        _color ("color", Color) = (1,0.475862,0,1)
        _FlowMap ("FlowMap", 2D) = "bump" {}
        _U_Speed ("U_Speed", Float ) = 0
        _V_Speed ("V_Speed", Float ) = 0
        _Strench ("Strench", Float ) = 0.2
        _MainTexture ("MainTexture", 2D) = "black" {}
        _TextureDistance ("Texture Distance", Range(0, 1)) = 1
        _FresnelStrench ("Fresnel Strench", Float ) = 1
        _TextureStrench ("Texture Strench", Range(-1, 1)) = 1
        _UndoEffectTexture ("Undo Effect Texture", 2D) = "black" {}
        [MaterialToggle] _Fresnel ("Fresnel", Float ) = 0
        _EmmisionStrench ("Emmision Strench", Range(0, 8)) = 1.8
        _Relief ("Relief", Range(0.1, 2)) = 0.5
        _TextureChangeSpeed ("Texture Change Speed", Range(0, 20)) = 6.917714
        _OutlineFresnel ("Outline Fresnel", Float ) = 2
    }
    SubShader {
        Tags {
            "RenderType"="Transparent"
        }
        Pass {
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            uniform sampler2D _FlowMap; uniform float4 _FlowMap_ST;
            uniform float _U_Speed;
            uniform float _V_Speed;
            uniform float _Strench;
            uniform sampler2D _MainTexture; uniform float4 _MainTexture_ST;
            uniform float4 _color;
            uniform float _TextureDistance;
            uniform float _FresnelStrench;
            uniform float _TextureStrench;
            uniform sampler2D _UndoEffectTexture; uniform float4 _UndoEffectTexture_ST;
            uniform fixed _Fresnel;
            uniform float _EmmisionStrench;
            uniform float _Relief;
            uniform float _TextureChangeSpeed;
            uniform float _OutlineFresnel;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_4620 = _Time;
                float2 node_1175 = (o.uv0+(node_4620.g*_TextureChangeSpeed)*float2(0.1,0.1));
                float4 node_7690 = tex2Dlod(_MainTexture,float4(TRANSFORM_TEX(node_1175, _MainTexture),0.0,0));
                // v.vertex.xyz += (saturate(node_7690.rgb)*v.normal*_Relief);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            fixed4 frag(VertexOutput i) : SV_Target {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 node_3379 = _Time;
                float2 node_388 = (i.uv0+(node_3379.g*_U_Speed)*float2(1,0));
                float3 node_1054 = UnpackNormal(tex2D(_FlowMap,TRANSFORM_TEX(node_388, _FlowMap)));
                float2 node_2673 = (i.uv0+(node_3379.g*_V_Speed)*float2(0,1));
                float3 node_7769 = UnpackNormal(tex2D(_FlowMap,TRANSFORM_TEX(node_2673, _FlowMap)));
                float2 node_7014 = ((i.uv0+(float2(node_1054.r,node_7769.g)*_Strench))+_TextureDistance*float2(1,0));
                float4 node_9700 = tex2D(_MainTexture,TRANSFORM_TEX(node_7014, _MainTexture));
                float4 node_4620 = _Time;
                float2 node_1175 = (i.uv0+(node_4620.g*_TextureChangeSpeed)*float2(0.1,0.1));
                float4 node_7690 = tex2D(_MainTexture,TRANSFORM_TEX(node_1175, _MainTexture));
                float3 node_4951 = (_color.rgb*(node_9700.r*node_7690.g)*_EmmisionStrench);
                float2 UV = i.uv0;
                float2 node_4937 = UV;
                float4 _UndoEffectTexture_var = tex2D(_UndoEffectTexture,TRANSFORM_TEX(node_4937, _UndoEffectTexture));
                float3 node_6801 = saturate((_UndoEffectTexture_var.rgb+_TextureStrench));
                float node_3101 = (1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelStrench));
                float3 emissive = lerp( (node_4951*node_6801), (node_4951*saturate((node_3101*node_3101*_OutlineFresnel))*node_6801), _Fresnel );
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                return finalRGBA;
                    // return fixed4(1,0,0,1);
            }
            ENDCG
        }
    }
}
