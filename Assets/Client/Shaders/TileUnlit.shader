Shader "MonsterWorld/URP Tile Unlit"
{
    Properties
    {
        _BaseMap("Texture", 2D) = "white" {}
        _BaseColor("Color", Color) = (1, 1, 1, 1)
        _Cutoff("AlphaCutout", Range(0.0, 1.0)) = 0.5

        // BlendMode
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("Src", Float) = 1.0
        [HideInInspector] _DstBlend("Dst", Float) = 0.0
        [HideInInspector] _ZWrite("ZWrite", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
    }

    SubShader
    {
        Pass // 0 - Unlit
        {
            Name "Unlit"
            Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM
            #pragma vertex TileVertex
            #pragma fragment Fragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON
            //--------------------------------------

            #pragma multi_compile_instancing
            #pragma multi_compile __ TILE_PREVIEW

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/Client/Shaders/Lib/TileUnlitInput.hlsl"
            #include "Assets/Client/Shaders/Lib/TileVertex.hlsl"

            half4 Fragment(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                AlphaDiscard(color.a, _Cutoff);

                color *= _BaseColor;
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
            #pragma vertex TileVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON
            //--------------------------------------

            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/Client/Shaders/Lib/TileUnlitInput.hlsl"
            #include "Assets/Client/Shaders/Lib/TileVertex.hlsl"

            half4 DepthOnlyFragment(Varyings IN) : SV_Target
            {
                #if defined(_ALPHATEST_ON)
                    half alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv).a;
                    AlphaDiscard(alpha, _Cutoff);
                #endif
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
            #pragma vertex TileVertex
            #pragma fragment PreviewFragment

            #pragma multi_compile_instancing
            #pragma multi_compile __ TILE_PREVIEW

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/Client/Shaders/Lib/TileUnlitInput.hlsl"
            #include "Assets/Client/Shaders/Lib/TileVertex.hlsl"

            half4 PreviewFragment(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                AlphaDiscard(color.a, _Cutoff);

                color *= _BaseColor;
                color.a *= 0.75;
                return color;
            }
            ENDHLSL
        }
    }
    CustomEditor "MonsterWorld.Unity.Tilemap3D.TileUnlitShaderGUI"
}