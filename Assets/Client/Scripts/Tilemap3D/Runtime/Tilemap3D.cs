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

namespace MonsterWorld.Unity.Tilemap
{
    public class Tilemap3D : MonoBehaviour
    {
        [SerializeField] private Tileset3D _tileset;
        [SerializeField] private List<Tile3DInstanceData> _tileDataList = null;

        public Tileset3D Tileset => _tileset;
        public List<Tile3DInstanceData> TileDataList => _tileDataList;

        private List<Tilemap3DRenderer> _renderers = new List<Tilemap3DRenderer>(1);

        public bool SetTileset(Tileset3D tileset)
        {
            if (_tileset == null)
            {
                _tileset = tileset;
                return true;
            }
            return false;
        }

        public void OnValidate()
        {
            if (_tileset == null)
            {
                if (_tileDataList != null) _tileDataList.Clear();
                return;
            }

            if (_tileDataList == null || _tileDataList.Count != _tileset.Count)
            {
                if (_tileDataList == null)
                {
                    _tileDataList = new List<Tile3DInstanceData>(_tileset.Count);
                }
                else
                {
                    _tileDataList.Clear();
                }

                for (int i = 0; i < _tileset.Count; i++)
                {
                    _tileDataList.Add(new Tile3DInstanceData()
                    {
                        indexInTileset = i,
                        poses = new List<TilePose>()
                    });
                }
            }
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

        public DataBuilder Builder()
        {
            return new DataBuilder(this);
        }

        public class DataBuilder
        {
            private Tilemap3D _tilemap;
            private Dictionary<Vector3Int, int>[] _tileDictionaries;
            private List<Tile3DInstanceData> _tileDataList;

            public DataBuilder(Tilemap3D tilemap)
            {
                _tilemap = tilemap;
                _tileDataList = tilemap._tileDataList;
                _tileDictionaries = new Dictionary<Vector3Int, int>[Tile3D.LAYER_COUNT];
                ReadFromTilemap();
            }

            public bool HasTile(int layer, Vector3Int position)
            {
                if (_tilemap._tileset == null) return false;
                return _tileDictionaries[layer].ContainsKey(position);
            }

            public bool AddTile(int index, TilePose pose)
            {
                if (_tilemap._tileset == null) return false;

                if (index < 0 || index >= _tilemap._tileset.Count)
                {
                    return false;
                }

                var tile = _tilemap._tileset[index];

                if (HasTile(tile.Layer, pose.position))
                {
                    return false;
                }

                if (!tile.CanBeRotated && pose.rotation != 0)
                {
                    Debug.LogWarning("A tile was added with a rotation beside being marked as CanBeRotated = false.");
                }

                _tileDataList[index].poses.Add(pose);
                _tileDictionaries[tile.Layer].Add(pose.position, index);
                _tilemap.SetRenderersDirty();

                return true;
            }

            public bool RemoveTile(int layer, Vector3Int position)
            {
                if (_tilemap._tileset == null) return false;

                if (_tileDictionaries[layer].TryGetValue(position, out int index))
                {
                    _tileDataList[index].poses.RemoveAll((pose) => pose.position == position);
                    _tileDictionaries[layer].Remove(position);
                    _tilemap.SetRenderersDirty();
                    return true;
                }
                return false;
            }

            private void ReadFromTilemap()
            {
                for (int i = 0; i < _tileDictionaries.Length; i++)
                {
                    if (_tileDictionaries[i] != null)
                    {
                        _tileDictionaries[i].Clear();
                    }
                    else
                    {
                        _tileDictionaries[i] = new Dictionary<Vector3Int, int>();
                    }
                }

                if (_tileDataList == null) _tileDataList = new List<Tile3DInstanceData>();

                for (int i = 0; i < _tileDataList.Count; i++)
                {
                    var tileData = _tileDataList[i];
                    var tile = _tilemap._tileset[tileData.indexInTileset];
                    for (int j = 0; j < tileData.poses.Count; j++)
                    {
                        _tileDictionaries[tile.Layer].Add(tileData.poses[j].position, i);
                    }
                }
            }
        }
    }
}