Shader "Unlit/ToonShaderBW"
{
    Properties
    {
        _MainTex ("Texture (unused)", 2D) = "white" {}
        _Threshold ("Toon Threshold", Range(0,1)) = 0.5
        _Feather ("Edge Softness", Range(0,0.5)) = 0.08
        _Brightness("Brightness", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
  
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                half3 worldNormal: NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Threshold;
            float _Feather;
            float _Brightness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 n = normalize(i.worldNormal);
                float3 l = normalize(_WorldSpaceLightPos0.xyz);
                float ndotl = saturate(dot(n, l));
                float val = smoothstep(_Threshold - _Feather, _Threshold + _Feather, ndotl);
                val = saturate(val + _Brightness);
                return float4(val, val, val, 1.0);
            }
            ENDCG
        }

        Pass 
        {
            Name "CastShadow"
            Tags { "LightMode" = "ShadowCaster" }
    
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"
    
            struct v2f 
            { 
                V2F_SHADOW_CASTER;
            };
    
            v2f vert( appdata_base v )
            {
                v2f o;
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
    
            float4 frag( v2f i ) : COLOR
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
}

