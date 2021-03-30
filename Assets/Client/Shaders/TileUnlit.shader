Shader "MonsterWorld/URP Tile Unlit"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white"
    }

    SubShader
    {
        Pass // 0 - Unlit
        {
            Name "Unlit"
            Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing
            #pragma multi_compile __ TILE_PREVIEW

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/Client/Shaders/Lib/TileUnlitInput.hlsl"

            Varyings vert(Attributes IN)
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                Varyings OUT;

                #if defined(TILE_PREVIEW)
                    OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                #else
                    // Get the TileMapSpace position
                    float4 positionTMS = mul(GetObjectToWorldMatrix(), float4(IN.positionOS.xyz, 1.0));
                    float4 positionWS = mul(_TilemapMatrix, positionTMS);
                    OUT.positionHCS = mul(GetWorldToHClipMatrix(), positionWS);
                #endif

                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                return color;
            }
            ENDHLSL
        }

        Pass // 1 - Depth Only
        {
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/Client/Shaders/Lib/TileUnlitInput.hlsl"

            Varyings vert(Attributes IN)
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                Varyings OUT;

                #if defined(TILE_PREVIEW)
                    OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                #else
                    // Get the TileMapSpace position
                    float4 positionTMS = mul(GetObjectToWorldMatrix(), float4(IN.positionOS.xyz, 1.0));
                    float4 positionWS = mul(_TilemapMatrix, positionTMS);
                    OUT.positionHCS = mul(GetWorldToHClipMatrix(), positionWS);
                #endif

                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }

        Pass // 2 - Preview
        {
            Name "Preview"
            Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalRenderPipeline" }
            
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing
            #pragma multi_compile __ TILE_PREVIEW

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/Client/Shaders/Lib/TileUnlitInput.hlsl"

            Varyings vert(Attributes IN)
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                Varyings OUT;

                #if defined(TILE_PREVIEW)
                    OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                #else
                // Get the TileMapSpace position
                float4 positionTMS = mul(GetObjectToWorldMatrix(), float4(IN.positionOS.xyz, 1.0));
                float4 positionWS = mul(_TilemapMatrix, positionTMS);
                OUT.positionHCS = mul(GetWorldToHClipMatrix(), positionWS);
                #endif

                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                color.a = 0.75;
                return color;
            }
            ENDHLSL
        }
    }

}