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
        [SerializeField] private int _level = 1;
        [SerializeField] private int _totalExp = 0;
        [SerializeField] private int _currentExp = 0;
        [SerializeField] private int _currentHealth = 0;

        public Species Species => _species;
        public int Level => _level;
        public int TotalExp => _totalExp;
        public int CurrentExp => _currentExp;
        public int CurrentHealth => _currentHealth;
    }
}
