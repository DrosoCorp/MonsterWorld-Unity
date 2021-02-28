//-----------------------------------------------------------------
// File:         Team.cs
// Description:  Monster list of someone
// Module:       Main
// Author:       Noé Masse
// Date:         27/02/2021
//-----------------------------------------------------------------
using System;

using UnityEngine;

namespace MonsterWorld.Unity
{
    [Serializable]
    public struct Statistics
    {
        public int health;
        public int energy;
        public int attack;
        public int power;
        public int defense;
        public int resistance;
    }
}
