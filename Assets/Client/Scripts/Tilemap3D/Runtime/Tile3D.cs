//-----------------------------------------------------------------
// File:         Tile3D.cs
// Description:  Tile asset
// Module:       Map Editor
// Author:       Noé Masse
// Date:         04/04/2021
//-----------------------------------------------------------------
using System;
using UnityEngine;

namespace MonsterWorld.Unity.Tilemap
{
    [CreateAssetMenu(fileName = "New Tile", menuName = "MonsterWorld/Tilemap/Tile", order = 10)]
    public class Tile3D : ScriptableObject
    {
        [SerializeField] private GameObject _prefab = null;
        [SerializeField] private bool _canBeRotated = true;
        [SerializeField] private int _flags = 0;

        [HideInInspector] [SerializeField] private Material _material;
        [HideInInspector] [SerializeField] private Mesh _mesh;

        public bool CanBeRotated => _canBeRotated;
        public GameObject Prefab => _prefab;
        public Mesh Mesh => _mesh;
        public Material Material => _material;
        public bool IsOpaque => _material.renderQueue < 2950;
        public int Flags => _flags;

        private void OnValidate()
        {
            if (!Validate()) _prefab = null;
        }

        public bool IsValid()
        {
            return _prefab != null && _mesh != null && _material != null;
        }

        public bool Validate()
        {
            var meshFilter = _prefab.GetComponent<MeshFilter>();
            var meshRenderer = _prefab.GetComponent<MeshRenderer>();

            if (meshFilter == null)
            {
                Debug.LogWarning("[Tilemap] Prefab : " + _prefab.name + " does not have a MeshFilter component.");
                return false;
            }

            if (meshRenderer == null)
            {
                Debug.LogWarning("[Tilemap] Prefab : " + _prefab.name + " does not have a MeshRenderer component.");
                return false;
            }

            var mesh = meshFilter.sharedMesh;
            var material = meshRenderer.sharedMaterial;

            if (material.enableInstancing == false)
            {
                Debug.LogWarning("[Tilemap] Prefab : " + _prefab.name + " material's does not have GPU Instancing enabled.");
                return false;
            }

            _mesh = mesh;
            _material = material;

            return true;
        }
    }
}