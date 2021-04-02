//-----------------------------------------------------------------
// File:         TilePose.cs
// Description:  Describe the position and the rotation of a Tile
// Module:       Map Editor
// Author:       Noé Masse
// Date:         31/03/2021
//-----------------------------------------------------------------
using System;
using UnityEngine;

namespace MonsterWorld.Unity.Tilemap3D
{
    [Serializable]
    public struct TilePose
    {
        public Vector3Int position;
        public int rotation;

        public Matrix4x4 Matrix => Matrix4x4.TRS(position + new Vector3(0.5f, 0.5f, 0.5f), Quaternion.AngleAxis(90f * rotation, Vector3.up), Vector3.one);
    }
}