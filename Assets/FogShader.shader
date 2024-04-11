Shader "Custom/FogShader" {
    Properties {
        _FogColor ("Fog Color", Color) = (0.5,0.5,0.5,1)
        _FogDensity ("Fog Density", Range(0,1)) = 0.01
        _FogStartDistance ("Fog Start Distance", Range(0,100)) = 0
        _FogEndDistance ("Fog End Distance", Range(0,1000)) = 10
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
            };

            float4 _FogColor;
            float _FogDensity;
            float _FogStartDistance;
            float _FogEndDistance;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                return o;
            }

            half4 frag (v2f i) : SV_Target {
                half fogFactor = saturate((i.pos.z - _FogStartDistance) / (_FogEndDistance - _FogStartDistance));
                half4 fogColor = _FogColor;
                half4 objectColor = half4(1, 1, 1, 1); // You can replace this with the object's color
                return lerp(objectColor, fogColor, fogFactor * _FogDensity);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
