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
            for (int tileDataIndex = 0; tileDataIndex < tileDataList.Count; tileDataIndex++)
            {
                var tileData = tileDataList[tileDataIndex];
                var tile = tilemap.tileset[tileData.indexInTileset];
                if ((tile.Flags & 1) == 1)
                {
                    var offset = new Vector3(0.5f, 0.0f, 0.5f);
                    if ((tile.Flags & 2) == 2)
                    {
                        offset.y = 0.5f;
                    }

                    for (int poseIndex = 0; poseIndex < tileData.poses.Count; poseIndex++)
                    {
                        var nodeData = new ClientNavigationNodeData()
                        {
                            position = tilemap.transform.localToWorldMatrix.MultiplyPoint3x4(tileData.poses[poseIndex].position + offset)
                        };

                        graph._nodes.Add(new Node()
                        {
                            data = nodeData
                        });
                    }
                }
            }
            return graph;
        }
    }
}