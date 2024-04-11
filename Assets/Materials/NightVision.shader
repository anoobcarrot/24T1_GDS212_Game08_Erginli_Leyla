Shader "Custom/NightVision"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Brightness ("Brightness", Range(0, 50)) = 1
        _Tint ("Tint", Color) = (0, 1, 0, 1) // Green tint for night vision
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.5
    }
 
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
 
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Fog { Mode off }
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Brightness;
            fixed4 _Tint;
            sampler2D _NoiseTex;
            float _NoiseStrength;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb *= _Brightness;
                col.rgb *= _Tint.rgb;
                fixed4 noiseColor = tex2D(_NoiseTex, i.uv);
                col.rgb += noiseColor.rgb * _NoiseStrength;
                return col;
            }
            ENDCG
        }
    }
 
    Fallback "Diffuse"
}
