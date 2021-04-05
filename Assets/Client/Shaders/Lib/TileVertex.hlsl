#ifndef MW_TILE_VERTEX_INCLUDED
#define MW_TILE_VERTEX_INCLUDED

Varyings TileVertex(Attributes IN)
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
#endif