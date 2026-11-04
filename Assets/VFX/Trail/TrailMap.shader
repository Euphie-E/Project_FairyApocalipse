Shader "Custom/TrailMap"
{
	Properties
	{
		_MainTex("Main Texture (RGB)", 2D) = "white" {}
		_MainTexVFade("MainTex V Fade", Range(0, 1)) = 1
		_MainTexVFadePow("MainTex V Fade Pow", Float) = 1
		_MainTexPow("Main Texture Gamma", Float) = 1
		_MainTexMultiplier("Main Texture Multiplier", Float) = 1
		_TintTex("Tint Texture (RGB)", 2D) = "white" {}
		_Multiplier("Multiplier", Float) = 1
		_MainScrollSpeedU("Main Scroll U Speed", Float) = 3
		_MainScrollSpeedV("Main Scroll V Speed", Float) = 0
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent"}
			Blend One One
			ZWrite Off

			Pass
			{
				HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

				struct Attributes
				{
					float4 positionOS : POSITION;
					float2 uv : TEXCOORD0;
					half4 color : COLOR;
				};

				struct Varyings
				{
					float2 uv : TEXCOORD0;
					float2 uvOrigin : TEXCOORD1;
					float4 positionHCS : SV_POSITION;
					half4 color : COLOR;
				};

				sampler2D _MainTex;
				sampler2D _TintTex;

				CBUFFER_START(UnityPerMaterial)
					half4 _MainTex_ST;
					half _MainTexVFade;
					half _MainTexVFadePow;
					half _MainTexPow;
					half _MainTexMultiplier;
					half _Multiplier;
					half _MainScrollSpeedU;
					half _MainScrollSpeedV;
					
					half _MoveToMaterialUV;
				CBUFFER_END

				Varyings vert(Attributes IN)
				{
					Varyings o;
					o.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
					o.uv = TRANSFORM_TEX(IN.uv, _MainTex);
					o.uv.x -= frac(_Time.x * _MainScrollSpeedU) + _MoveToMaterialUV;
					o.uv.y -= frac(_Time.x * _MainScrollSpeedV);
					o.uvOrigin = IN.uv;
					o.color = IN.color;
					return o;
				}

				
                half4 frag(Varyings IN) : SV_Target
                {
                    half4 mainTex = tex2D(_MainTex, IN.uv);

                    // Fade nas extremidades da largura do Trail
                    half vFade = 1 - abs(IN.uvOrigin.y - 0.5) * 2;
                    vFade = pow(saturate(vFade), _MainTexVFadePow);
                    vFade = lerp(1, vFade, _MainTexVFade);

                    // Ajuste da textura principal
                    mainTex.rgb *= vFade;
                    mainTex.rgb = pow(abs(mainTex.rgb), _MainTexPow)
                                * _MainTexMultiplier;

                    // Intensidade
                    half intensity = _Multiplier * IN.color.a;

                    // Luminosidade da textura
                    half avr = dot(mainTex.rgb, half3(0.3333, 0.3334, 0.3333));
                    avr = saturate(avr * intensity);

                    // Busca a cor na paleta
                    half4 col = tex2D(_TintTex, half2(avr, 0.5));

                    // Garante que regiões escuras e bordas
                    // transparentes não produzam brilho
                    half mask = saturate(
                        dot(mainTex.rgb, half3(0.3333, 0.3334, 0.3333))
                    );

                    col.rgb *= mask;

                    half intensityHigh = max(1, intensity);
                    col.rgb *= intensityHigh * IN.color.rgb;

                    return col;
                }
				ENDHLSL
			}
		}
}
