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
    [ExecuteInEditMode]
    public class Tilemap3D : MonoBehaviour, ISerializationCallbackReceiver
    {
        public Tileset3D tileset;
        [SerializeField] private List<Tile3DInstanceData> _tileDataList = null;
        private Dictionary<Vector3Int, int> _tiles;
        private List<Tilemap3DRenderer> _renderers = new List<Tilemap3DRenderer>(1);

        public List<Tile3DInstanceData> TileDataList => _tileDataList;

        public void OnValidate()
        {
            if (tileset == null)
            {
                if (_tileDataList != null) _tileDataList.Clear();
                return;
            }

            if (_tileDataList == null || _tileDataList.Count != tileset.Count)
            {
                if (_tileDataList == null)
                {
                    _tileDataList = new List<Tile3DInstanceData>(tileset.Count);
                }
                else
                {
                    _tileDataList.Clear();
                }

                for (int i = 0; i < tileset.Count; i++)
                {
                    _tileDataList.Add(new Tile3DInstanceData()
                    {
                        indexInTileset = i,
                        poses = new List<TilePose>()
                    });
                }
            }
        }

        public bool HasTile(Vector3Int position)
        {
            if (tileset == null) return false;
            if (_tiles == null) _tiles = new Dictionary<Vector3Int, int>();
            return _tiles.ContainsKey(position);
        }

        public bool AddTile(int index, TilePose pose)
        {
            if (tileset == null) return false;
            if (_tiles == null) _tiles = new Dictionary<Vector3Int, int>();

            if (index < 0 || index >= tileset.Count)
            {
                return false;
            }

            if (_tiles.ContainsKey(pose.position))
            {
                return false;
            }

            if (!tileset[_tileDataList[index].indexInTileset].CanBeRotated && pose.rotation != 0)
            {
                Debug.LogWarning("A tile was added with a rotation besire being marked as CanBeRotated = false.");
            }

            _tileDataList[index].poses.Add(pose);
            _tiles.Add(pose.position, index);
            SetRenderersDirty();

            return true;
        }

        public bool RemoveTile(Vector3Int position)
        {
            if (tileset == null) return false;
            if (_tiles == null) _tiles = new Dictionary<Vector3Int, int>();

            if (_tiles.TryGetValue(position, out int index))
            {
                _tileDataList[index].poses.RemoveAll((pose) => pose.position == position);
                _tiles.Remove(position);
                SetRenderersDirty();
                return true;
            }
            return false;
        }

        public void Subscribe(Tilemap3DRenderer renderer)
        {
            if (!_renderers.Contains(renderer)) _renderers.Add(renderer);
        }

        public void Unsubscribe(Tilemap3DRenderer renderer)
        {
            _renderers.Remove(renderer);
        }

        private void SetRenderersDirty()
        {
            foreach (var renderer in _renderers) renderer.SetDirty();
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

            for (int i = 0; i < _tileDataList.Count; i++)
            {
                var tileData = _tileDataList[i];
                for (int j = 0; j < tileData.poses.Count; j++)
                {
                    _tiles.Add(tileData.poses[j].position, i);
                }
            }
        }
    }
}