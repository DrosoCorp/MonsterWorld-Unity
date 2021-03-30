//-----------------------------------------------------------------
// File:         Tilemap3D.cs
// Description:  Tilemap Data
// Module:       Map Editor
// Author:       Noé Masse
// Date:         28/03/2021
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterWorld.Unity.Tilemap3D
{
    public class Tilemap3D : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField] private List<GameObject> _prefabList = new List<GameObject>();
        [SerializeField] private List<Tile3DRenderData> _tileRenderDataList = new List<Tile3DRenderData>();

        public List<GameObject> PrefabList => _prefabList;
        public List<Tile3DRenderData> TileRenderDataList => _tileRenderDataList;

        private Dictionary<Vector3Int, int> _tiles;

        public bool HasTile(Vector3Int position)
        {
            return _tiles.ContainsKey(position);
        }

        public bool AddTile(int index, Vector3Int position, int rotation)
        {
            if (index < 0 || index >= _prefabList.Count)
            {
                return false;
            }

            if (_tiles.ContainsKey(position))
            {
                return false;
            }

            if (_tiles == null)
            {
                _tiles = new Dictionary<Vector3Int, int>();
            }

            _tileRenderDataList[index].positions.Add(position);

            var t = position + new Vector3(0.5f, 0.5f, 0.5f);
            var r = Quaternion.AngleAxis(90f * rotation, Vector3.up);

            _tileRenderDataList[index].matrices.Add(Matrix4x4.TRS(t, r, Vector3.one) * _prefabList[index].transform.localToWorldMatrix);
            _tiles.Add(position, index);

            return true;
        }

        public bool RemoveTile(Vector3Int position)
        {
            if (_tiles.TryGetValue(position, out int index))
            {
                int positionIndex = _tileRenderDataList[index].positions.IndexOf(position);
                _tileRenderDataList[index].positions.RemoveAt(positionIndex);
                _tileRenderDataList[index].matrices.RemoveAt(positionIndex);
                _tiles.Remove(position);
                return true;
            }
            return false;
        }

        public bool TryAddTilePrefab(GameObject prefab)
        {
            var meshFilter = prefab.GetComponent<MeshFilter>();
            var meshRenderer = prefab.GetComponent<MeshRenderer>();

            if (_prefabList.Contains(prefab))
            {
                Debug.LogWarning("[Tilemap] Prefab : " + prefab.name + " is already added.");
                return false;
            }

            if (meshFilter == null)
            {
                Debug.LogWarning("[Tilemap] Prefab : " + prefab.name + " does not have a MeshFilter component.");
                return false;
            }

            if (meshRenderer == null)
            {
                Debug.LogWarning("[Tilemap] Prefab : " + prefab.name + " does not have a MeshRenderer component.");
                return false;
            }

            var mesh = meshFilter.sharedMesh;
            var material = meshRenderer.sharedMaterial;

            if (material.enableInstancing == false)
            {
                Debug.LogWarning("[Tilemap] Prefab : " + prefab.name + " material's does not have GPU Instancing enabled.");
                return false;
            }

            _prefabList.Add(prefab);
            _tileRenderDataList.Add(new Tile3DRenderData()
            {
                mesh = mesh,
                material = material,
                positions = new List<Vector3Int>(),
                matrices = new List<Matrix4x4>()
            });
            return true;
        }

        public void OnBeforeSerialize() {}

        public void OnAfterDeserialize()
        {
            if (_tiles != null)
            {
                _tiles.Clear();
            }
            else
            {
                _tiles = new Dictionary<Vector3Int, int>();
            }

            for (int i = 0; i < _tileRenderDataList.Count; i++)
            {
                var renderData = _tileRenderDataList[i];
                for (int j = 0; j < renderData.positions.Count; j++)
                {
                    _tiles.Add(renderData.positions[j], i);
                }
            }
        }
    }
}