//-----------------------------------------------------------------
// File:         Monster.cs
// Description:  Describe a monster
// Module:       Main
// Author:       Noé Masse
// Date:         27/02/2021
//-----------------------------------------------------------------
using System;

using UnityEngine;

namespace MonsterWorld.Unity
{
    [Serializable]
    public class Monster : ScriptableObject
    {

        [SerializeField] private Species _species = null;

        [SerializeField] private int level = 1;
        [SerializeField] private int totalExp = 0;
        [SerializeField] private int currentExp = 0;

        [SerializeField] private int currentHealth = 0;


        public Species Species => _species;
    }
}
