//-----------------------------------------------------------------
// File:         Tilemap3DRenderer.cs
// Description:  Tilemap Renderer
// Module:       Map Editor
// Author:       Noé Masse
// Date:         28/03/2021
//-----------------------------------------------------------------
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MonsterWorld.Unity.Tilemap
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Tilemap3D))]
    public class Tilemap3DRenderer : MonoBehaviour
    {
        private const int BATCH_SIZE = 1023;

        private Tilemap3D _Tilemap3D = null;
        private List<Tile3DRenderData> _opaqueRenderDataList;
        private List<Tile3DRenderData> _transparentRenderDataList;
        private bool _isDirty = true;

        private void OnEnable()
        {
            _Tilemap3D = GetComponent<Tilemap3D>();
            Tilemap3DRenderFeature.Tilemap3DRenderers.Add(this);
            _Tilemap3D.Subscribe(this);
            _isDirty = true;
        }

        private void OnDisable()
        {
            Tilemap3DRenderFeature.Tilemap3DRenderers.Remove(this);
            _Tilemap3D.Unsubscribe(this);
        }

        public void SetDirty()
        {
            _isDirty = true;
        }

        public void DrawTransparents(CommandBuffer cmd)
        {
            if (_transparentRenderDataList == null) return;

            cmd.BeginSample("Draw Tiles");
            cmd.SetGlobalMatrix(Tilemap3DRenderFeature._TilemapMatrix, transform.localToWorldMatrix);
            for (int i = 0; i < _transparentRenderDataList.Count; i++)
            {
                var renderData = _transparentRenderDataList[i];
                for (int j = 0; j < renderData.batches.Count; j++)
                {
                    if (renderData.batches[j].Length > 0)
                    {
                        cmd.DrawMeshInstanced(renderData.mesh, 0, renderData.material, 0, renderData.batches[j]);
                    }
                }
            }
            cmd.EndSample("Draw Tiles");
        }

        public void DrawOpaques(CommandBuffer cmd, int pass)
        {
            if (_isDirty) BuildTileRenderDataLists();
            if (_opaqueRenderDataList == null) return;

            cmd.BeginSample("Draw Tiles");
            cmd.SetGlobalMatrix(Tilemap3DRenderFeature._TilemapMatrix, transform.localToWorldMatrix);
            for (int i = 0; i <_opaqueRenderDataList.Count; i++)
            {
                var renderData = _opaqueRenderDataList[i];
                for (int j = 0; j < renderData.batches.Count; j++)
                {
                    if (renderData.batches[j].Length > 0)
                    {
                        cmd.DrawMeshInstanced(renderData.mesh, 0, renderData.material, pass, renderData.batches[j]);
                    }
                }
            }
            cmd.EndSample("Draw Tiles");
        }

        [Button("Refresh")]
        private void BuildTileRenderDataLists()
        {
            var tileDataList = _Tilemap3D.TileDataList;

            if (_opaqueRenderDataList == null)
            {
                _opaqueRenderDataList = new List<Tile3DRenderData>(tileDataList.Count);
                _transparentRenderDataList = new List<Tile3DRenderData>(tileDataList.Count);
            }
            else
            {
                _opaqueRenderDataList.Clear();
                _transparentRenderDataList.Clear();
            }

            for (int tileDataIndex = 0; tileDataIndex < tileDataList.Count; tileDataIndex++)
            {
                List<TilePose> poses = tileDataList[tileDataIndex].poses;
                if (poses.Count == 0) continue;

                int indexInTileset = tileDataList[tileDataIndex].indexInTileset;
                if (indexInTileset >= _Tilemap3D.tileset.Count)
                {
                    tileDataList.RemoveAt(tileDataIndex--);
                    continue;
                }
                Tile3D tile = _Tilemap3D.tileset[indexInTileset];
                Matrix4x4 prefabMatrix = tile.Prefab.transform.localToWorldMatrix;
                Tile3DRenderData renderData;

                renderData.material = tile.Material;
                renderData.mesh = tile.Mesh;

                // Create batches of 1023 instances
                int batchCount = 1 + (poses.Count - 1) / BATCH_SIZE;
                renderData.batches = new List<Matrix4x4[]>(batchCount);
                int remainingPosesCount = poses.Count;

                for (int batchIndex = 0; remainingPosesCount > 0; batchIndex++)
                {
                    int currentBatchSize = Mathf.Min(BATCH_SIZE, remainingPosesCount);
                    int currentBatchOffset = batchIndex * BATCH_SIZE;
                    renderData.batches.Add(new Matrix4x4[currentBatchSize]);
                    for (int instanceIndex = 0; instanceIndex < currentBatchSize; instanceIndex++)
                    {
                        renderData.batches[batchIndex][instanceIndex] = poses[currentBatchOffset + instanceIndex].Matrix * prefabMatrix;
                    }
                    remainingPosesCount -= currentBatchSize;
                }

                if (tile.IsOpaque)
                {
                    _opaqueRenderDataList.Add(renderData);
                }
                else
                {
                    _transparentRenderDataList.Add(renderData);
                }
            }

            _isDirty = false;
        }
    }
}