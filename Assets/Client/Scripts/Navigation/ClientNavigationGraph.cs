//-----------------------------------------------------------------
// File:         ClientNavigationGraph.cs
// Description:  Client-side navigation graph
// Module:       Navigation
// Author:       Noé Masse
// Date:         28/03/2021
//-----------------------------------------------------------------
using System;
using UnityEngine;

using MonsterWorld.Unity.Tilemap;
using System.Collections.Generic;

namespace MonsterWorld.Unity.Navigation
{
    [Serializable]
    public struct ClientNavigationNodeData
    {
        public Vector3 position;
    }

    [Serializable]
    public class ClientNavigationGraph : NavigationGraph<ClientNavigationNodeData>
    {
        public static ClientNavigationGraph BuildFromTilemap(Tilemap3D tilemap)
        {
            ClientNavigationGraph graph = new ClientNavigationGraph();
            var tileDataList = tilemap.TileDataList;

            var mapFlags = new Dictionary<Vector3Int, int>();

            for (int tileDataIndex = 0; tileDataIndex < tileDataList.Count; tileDataIndex++)
            {
                var tileData = tileDataList[tileDataIndex];
                var tile = tilemap.Tileset[tileData.indexInTileset];
                for (int poseIndex = 0; poseIndex < tileData.poses.Count; poseIndex++)
                {
                    var position = tileData.poses[poseIndex].position;
                    if (mapFlags.ContainsKey(position))
                    {
                        mapFlags[position] &= tile.Flags;
                    }
                    else
                    {
                        mapFlags.Add(position, tile.Flags);
                    }
                }
            }

            foreach (var entry in mapFlags)
            {
                var position = entry.Key;
                var flags = entry.Value;

                if ((flags & 1) == 1)
                {
                    var offset = new Vector3(0.5f, 0.0f, 0.5f);
                    if ((flags & 2) == 2)
                    {
                        offset.y = 0.5f;
                    }

                    var nodeData = new ClientNavigationNodeData()
                    {
                        position = tilemap.transform.localToWorldMatrix.MultiplyPoint3x4(position + offset)
                    };

                    graph._nodes.Add(new Node()
                    {
                        data = nodeData
                    });
                }
            }

            return graph;
        }

        public void Clear()
        {
            Nodes.Clear();
        }
    }
}