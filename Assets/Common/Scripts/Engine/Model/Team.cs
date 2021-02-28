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
    public class Team : ScriptableObject
    {
        [SerializeField] private Monster[] _monsters;
    }
}
