Shader "DanielIlett/Sketch"
{
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        #define E 2.71828f

#if UNITY_VERSION < 600000
        float4 _BlitTexture_TexelSize;
#endif

        uint _KernelSize;
        float _Spread;
        float _DepthSensitivity;
        uint _BlurStepSize;

        float gaussian(int x)
        {
            float sigmaSqu = _Spread * _Spread;
            return (1 / sqrt(TWO_PI * sigmaSqu))
                * pow(E, -(x * x) / (2 * sigmaSqu));
        }

        float sampleDepth(float2 uv)
        {
#if UNITY_REVERSED_Z
            return SampleSceneDepth(uv);
#else
            return lerp(
                UNITY_NEAR_CLIP_VALUE,
                1,
                SampleSceneDepth(uv)
            );
#endif
        }

        ENDHLSL


        // ============================================================
        // PASS 0 - SKETCH MAIN
        // ============================================================

        Pass
        {
            Name "Sketch Main"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment frag

            // Variantes necessárias para sombras da luz principal.
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"

            TEXTURE2D(_SketchTexture);

            float4 _SketchColor;
            float2 _SketchThresholds;
            float2 _SketchTiling;
            float _CrossHatching;


            // ========================================================
            // TRIPLANAR SAMPLE
            // ========================================================

            float4 triplanarSample(
                Texture2D tex,
                SamplerState texSampler,
                float2x2 rotation,
                float3 uv,
                float3 normals,
                float blend
            )
            {
                float2 uvX =
                    mul(rotation, uv.zy * _SketchTiling);

                float2 uvY =
                    mul(rotation, uv.xz * _SketchTiling);

                float2 uvZ =
                    mul(rotation, uv.xy * _SketchTiling);


                if (normals.x < 0)
                {
                    uvX.x = -uvX.x;
                }

                if (normals.y < 0)
                {
                    uvY.x = -uvY.x;
                }

                if (normals.z >= 0)
                {
                    uvZ.x = -uvZ.x;
                }


                float4 colX =
                    SAMPLE_TEXTURE2D(
                        tex,
                        texSampler,
                        uvX
                    );

                float4 colY =
                    SAMPLE_TEXTURE2D(
                        tex,
                        texSampler,
                        uvY
                    );

                float4 colZ =
                    SAMPLE_TEXTURE2D(
                        tex,
                        texSampler,
                        uvZ
                    );


                float3 blending =
                    pow(abs(normals), blend);

                blending /=
                    dot(blending, 1.0f);


                return
                    colX * blending.x +
                    colY * blending.y +
                    colZ * blending.z;
            }


            // ========================================================
            // MAIN FRAGMENT
            // ========================================================

            float4 frag(Varyings i) : SV_Target
            {
                // ----------------------------------------------------
                // COR ORIGINAL DA TELA
                // ----------------------------------------------------

                float4 col =
                    SAMPLE_TEXTURE2D(
                        _BlitTexture,
                        sampler_LinearClamp,
                        i.texcoord
                    );


                // ----------------------------------------------------
                // DEPTH
                // ----------------------------------------------------

                float depth =
                    sampleDepth(i.texcoord);


                // Não aplicar o efeito no Skybox.
                //
                // Em Reversed Z, pixels do céu normalmente possuem
                // profundidade próxima de 0.
                //
                // Isso evita tentar reconstruir sombras no infinito.
                if (depth <= 0.00001f)
                {
                    return col;
                }


                // ----------------------------------------------------
                // WORLD POSITION
                // ----------------------------------------------------

                float3 worldPos =
                    ComputeWorldSpacePosition(
                        i.texcoord,
                        depth,
                        UNITY_MATRIX_I_VP
                    );


                // ----------------------------------------------------
                // WORLD NORMAL
                // ----------------------------------------------------

                float3 worldNormal =
                    normalize(
                        SAMPLE_TEXTURE2D(
                            _CameraNormalsTexture,
                            sampler_LinearClamp,
                            i.texcoord
                        ).xyz
                    );


                // ----------------------------------------------------
                // SKETCH TEXTURE
                // ----------------------------------------------------

                float2x2 rotationMatrix =
                    float2x2(
                        1, 0,
                        0, 1
                    );


                float4 sketchTexture =
                    saturate(
                        triplanarSample(
                            _SketchTexture,
                            sampler_LinearRepeat,
                            rotationMatrix,
                            worldPos,
                            worldNormal,
                            10.0f
                        )
                    );


                // ----------------------------------------------------
                // CROSS HATCHING
                // ----------------------------------------------------

                if (_CrossHatching > 0.5f)
                {
                    // Rotação de aproximadamente 90 graus.
                    rotationMatrix =
                        float2x2(
                            0, -1,
                            1,  0
                        );


                    float4 sketchTexture2 =
                        saturate(
                            triplanarSample(
                                _SketchTexture,
                                sampler_LinearRepeat,
                                rotationMatrix,
                                worldPos,
                                worldNormal,
                                10.0f
                            )
                        );


                    sketchTexture.rgb =
                        saturate(
                            sketchTexture.rgb +
                            sketchTexture2.rgb
                        );

                    sketchTexture.a =
                        max(
                            sketchTexture.a,
                            sketchTexture2.a
                        );
                }


                sketchTexture *=
                    _SketchColor;


                // ====================================================
                // SHADOW CALCULATION
                // ====================================================
                //
                // Aqui está a mudança principal.
                //
                // Antes:
                //
                // _ShadowmapTexture
                //      ↓
                // Sample usando Screen UV
                //
                // Agora:
                //
                // Screen Depth
                //      ↓
                // World Position
                //      ↓
                // Shadow Coordinate
                //      ↓
                // GetMainLight()
                //      ↓
                // shadowAttenuation
                //


                float4 shadowCoord =
                    TransformWorldToShadowCoord(
                        worldPos
                    );


                Light mainLight =
                    GetMainLight(
                        shadowCoord
                    );


                // shadowAttenuation:
                //
                // 1 = iluminado
                // 0 = completamente na sombra

                float shadowAttenuation =
                    mainLight.shadowAttenuation;


                // Convertemos:
                //
                // 1 iluminado → 0
                // 0 sombra    → 1

                float shadows =
                    1.0f -
                    shadowAttenuation;


                // ----------------------------------------------------
                // THRESHOLD
                // ----------------------------------------------------

                shadows =
                    smoothstep(
                        _SketchThresholds.x,
                        _SketchThresholds.y,
                        shadows
                    );


                // ----------------------------------------------------
                // RESULTADO FINAL
                // ----------------------------------------------------

                return lerp(
                    col,
                    sketchTexture,
                    shadows *
                    sketchTexture.a
                );
            }

            ENDHLSL
        }


        // ============================================================
        // PASS 1 - HORIZONTAL BLUR
        // ============================================================

        Pass
        {
            Name "Horizontal Blur"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment frag_horizontal


            float4 frag_horizontal(
                Varyings i
            ) : SV_Target
            {
                float depth =
                    sampleDepth(
                        i.texcoord
                    );


                float3 col =
                    0.0f;

                float kernelSum =
                    0.001f;


                int upper =
                    ((_KernelSize - 1) / 2);

                int lower =
                    -upper;


                float2 uv;


                for (
                    int x = lower;
                    x <= upper;
                    x += _BlurStepSize
                )
                {
                    uv =
                        i.texcoord +
                        float2(
                            _BlitTexture_TexelSize.x * x,
                            0.0f
                        );


                    float newDepth =
                        sampleDepth(
                            uv
                        );


                    if (
                        newDepth > 0.001f &&
                        abs(depth - newDepth) <
                        _DepthSensitivity
                    )
                    {
                        float gauss =
                            gaussian(x);


                        kernelSum +=
                            gauss;


                        col +=
                            gauss *
                            SAMPLE_TEXTURE2D(
                                _BlitTexture,
                                sampler_LinearClamp,
                                uv
                            );
                    }
                }


                col /=
                    kernelSum;


                return
                    float4(
                        col,
                        1.0f
                    );
            }

            ENDHLSL
        }


        // ============================================================
        // PASS 2 - VERTICAL BLUR
        // ============================================================

        Pass
        {
            Name "Vertical Blur"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment frag_vertical


            float4 frag_vertical(
                Varyings i
            ) : SV_Target
            {
                float depth =
                    sampleDepth(
                        i.texcoord
                    );


                float3 col =
                    0.0f;

                float kernelSum =
                    0.001f;


                int upper =
                    ((_KernelSize - 1) / 2);

                int lower =
                    -upper;


                float2 uv;


                for (
                    int y = lower;
                    y <= upper;
                    y += _BlurStepSize
                )
                {
                    uv =
                        i.texcoord +
                        float2(
                            0.0f,
                            _BlitTexture_TexelSize.y * y
                        );


                    float newDepth =
                        sampleDepth(
                            uv
                        );


                    if (
                        newDepth > 0.001f &&
                        abs(depth - newDepth) <
                        _DepthSensitivity
                    )
                    {
                        float gauss =
                            gaussian(y);


                        kernelSum +=
                            gauss;


                        col +=
                            gauss *
                            SAMPLE_TEXTURE2D(
                                _BlitTexture,
                                sampler_LinearClamp,
                                uv
                            );
                    }
                }


                col /=
                    kernelSum;


                return
                    float4(
                        col,
                        1.0f
                    );
            }

            ENDHLSL
        }
    }
}