//-----------------------------------------------------------------
// File:         Tilemap3DEditor.cs
// Description:  Edit tilemaps
// Module:       Map Editor
// Author:       Noé Masse
// Date:         28/03/2021
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace MonsterWorld.Unity.Tilemap3D
{
    [CustomEditor(typeof(Tile3D))]
    public class Tile3DEditor : Editor
    {
        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
        {
            Tile3D tile = target as Tile3D;

            if (tile == null || tile.Prefab == null)
                return null;

            Texture2D cache = new Texture2D(width, height);
            EditorUtility.CopySerialized(AssetPreview.GetAssetPreview(tile.Prefab), cache);
            return cache;
        }
    }
}