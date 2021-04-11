//-----------------------------------------------------------------
// File:         Tile3DRenderData.cs
// Description:  Contains data needed for tile rendering
// Module:       Map Editor
// Author:       Noé Masse
// Date:         28/03/2021
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterWorld.Unity.Tilemap
{
    [Serializable]
    public struct Tile3DInstanceData
    {
        public int indexInTileset;
        public List<TilePose> poses;
    }
}