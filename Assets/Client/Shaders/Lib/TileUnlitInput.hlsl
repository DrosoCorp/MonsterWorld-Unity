#ifndef MW_TILE_UNLIT_INPUT_INCLUDED
#define MW_TILE_UNLIT_INPUT_INCLUDED

struct Attributes
{
    float4 positionOS   : POSITION;
    float2 uv           : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionHCS  : SV_POSITION;
    float2 uv           : TEXCOORD0;
};

TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);

float4x4 _TilemapMatrix;

CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
half4 _BaseColor;
half _Cutoff;
CBUFFER_END

#endif