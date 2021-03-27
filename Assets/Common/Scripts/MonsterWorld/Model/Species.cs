//-----------------------------------------------------------------
// File:         Species.cs
// Description:  Describe a monster species
// Module:       Main
// Author:       Noé Masse
// Date:         27/02/2021
//-----------------------------------------------------------------
using System;

using UnityEngine;

namespace MonsterWorld.Unity
{
    [Serializable]
    public class Species : ScriptableObject
    {

        [SerializeField] private int _id = 0;
        [SerializeField] private float _height = 1.0f;
        [SerializeField] private float _weight = 1.0f;

        public int Id => _id;
        public float Height => _height;
        public float Weight => _weight;
    }
}
