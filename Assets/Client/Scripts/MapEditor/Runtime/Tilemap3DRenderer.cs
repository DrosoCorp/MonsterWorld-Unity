//-----------------------------------------------------------------
// File:         Tilemap3DRenderer.cs
// Description:  Tilemap Renderer
// Module:       Map Editor
// Author:       Noé Masse
// Date:         28/03/2021
//-----------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MonsterWorld.Unity.Tilemap3D
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Tilemap3D))]
    public class Tilemap3DRenderer : MonoBehaviour
    {
        private Tilemap3D _Tilemap3D = null;

        public List<Tile3DRenderData> TileRenderList => _Tilemap3D == null ? null : _Tilemap3D.TileRenderDataList;

        private void Start()
        {
            _Tilemap3D = GetComponent<Tilemap3D>();
        }

        private void OnEnable()
        {
            Tilemap3DRenderFeature.Tilemap3DRenderers.Add(this);
        }

        private void OnDisable()
        {
            Tilemap3DRenderFeature.Tilemap3DRenderers.Remove(this);
        }
    }
}