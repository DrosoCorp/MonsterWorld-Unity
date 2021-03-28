//-----------------------------------------------------------------
// File:         Tile3D.cs
// Description:  Tile Asset
// Module:       Map Editor
// Author:       Noé Masse
// Date:         28/03/2021
//-----------------------------------------------------------------
using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "MonsterWorld/Tilemap/Tile", order = 1)]
public class Tile3D : ScriptableObject
{
    public GameObject prefab;
}
