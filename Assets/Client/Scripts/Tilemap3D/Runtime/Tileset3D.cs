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
        [SerializeField] private List<Tile3D> _tiles = null;

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

        public IEnumerator<Tile3D> GetEnumerator()
        {
            return _tiles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _tiles.GetEnumerator();
        }
    }
}