Shader "Custom/StarSkybox"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (0.25,0.1,0.5,1)
        _BottomColor ("Bottom Color", Color) = (0.02,0.02,0.08,1)

        _StarDensity ("Star Density", Range(10,1000)) = 120
        _StarThreshold ("Star Threshold", Range(0.9,0.9999)) = 0.985
        _StarSize ("Star Size", Range(1,100)) = 40
    }

    SubShader
    {
        Tags
        {
            "Queue"="Background"
            "RenderType"="Background"
            "PreviewType"="Skybox"
        }

        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 dir : TEXCOORD0;
            };

            float4 _TopColor;
            float4 _BottomColor;

            float _StarDensity;
            float _StarThreshold;
            float _StarSize;

            float hash(float2 p)
            {
                p = frac(p * float2(234.34, 435.345));
                p += dot(p, p + 34.23);

                return frac(p.x * p.y);
            }

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);

                o.dir = normalize(v.vertex.xyz);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 dir = normalize(i.dir);

                float gradient =
                    saturate(dir.y * 0.5 + 0.5);

                float3 sky =
                    lerp(
                        _BottomColor.rgb,
                        _TopColor.rgb,
                        gradient
                    );

                float2 uv = dir.xz;

                uv *= _StarDensity;

                float2 cell = floor(uv);

                float2 local = frac(uv) - 0.5;

                float rnd = hash(cell);

                float star =
                    step(_StarThreshold, rnd);

                float dist =
                    length(local);

                float size =
                    1.0 / _StarSize;

                star *=
                    (1.0 - smoothstep(
                        0.0,
                        size,
                        dist
                    ));

                float3 finalColor =
                    sky + (star * 2);

                return float4(finalColor, 1);
            }

            ENDHLSL
        }
    }

    FallBack Off
}