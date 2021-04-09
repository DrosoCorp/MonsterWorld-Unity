//-----------------------------------------------------------------
// File:         Tile3D.cs
// Description:  Tile asset
// Module:       Map Editor
// Author:       Noé Masse
// Date:         04/04/2021
//-----------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterWorld.Unity.Tilemap3D
{
    [CreateAssetMenu(fileName = "New Tileset", menuName = "MonsterWorld/Tilemap/Tileset", order = 11)]
    public class Tileset3D : ScriptableObject, IEnumerable<Tile3D>
    {
        public struct TileReference
        {
            public Tile3D tile;
            public int indexInTileset;
        }

        [SerializeField] private List<Tile3D> _tiles = new List<Tile3D>();

        public List<Tile3D> Tiles => _tiles;

        public Tile3D this[int index]
        {
            get
            {
                return _tiles[index];
            }
            set
            {
                _tiles[index] = value;
            }
        }

        public int Count => _tiles.Count;

        private void OnValidate()
        {
        }

        public bool TryAddTile(Tile3D tile)
        {
            if (tile == null || !tile.IsValid() || _tiles.Contains(tile)) return false;
            _tiles.Add(tile);
            return true;
        }

        public IEnumerator<Tile3D> GetEnumerator()
        {
            return _tiles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _tiles.GetEnumerator();
        }

        public List<TileReference> Filter(Func<Tile3D, bool> predicate)
        {
            var result = new List<TileReference>(_tiles.Count / 4);
            for (int i = 0; i < _tiles.Count; i++)
            {
                if (predicate(_tiles[i]))
                {
                    result.Add(new TileReference()
                    {
                        tile = _tiles[i],
                        indexInTileset = i
                    });
                }
            }
            return result;
        }
    }
}