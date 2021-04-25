//-----------------------------------------------------------------
// File:         ClientNavigationGraphBuilder.cs
// Description:  Build a navigation graph
// Module:       Navigation
// Author:       Noé Masse
// Date:         24/04/2021
//-----------------------------------------------------------------
using System;
using UnityEngine;

using MonsterWorld.Unity.Tilemap;
using System.Collections.Generic;

namespace MonsterWorld.Unity.Navigation
{
    public class NavigationGraphBuilder
    {
        private ClientNavigationGraph clientGraph;
        private Dictionary<Vector3Int, int> mapFlags;
        private Dictionary<Vector3Int, int> nodeIndices;
        private List<ClientNavigationNodeData> nodeDatas;
        private ClientNavigationGraph.Node[] nodes;

        public NavigationGraphBuilder(ClientNavigationGraph graph)
        {
            clientGraph = graph;
        }

        private void ComputeMapFlags(Tilemap3D tilemap)
        {
            var tileDataList = tilemap.TileDataList;
            mapFlags = new Dictionary<Vector3Int, int>();

            for (int tileDataIndex = 0; tileDataIndex < tileDataList.Count; tileDataIndex++)
            {
                var tileData = tileDataList[tileDataIndex];
                var tile = tilemap.Tileset[tileData.indexInTileset];
                for (int poseIndex = 0; poseIndex < tileData.poses.Count; poseIndex++)
                {
                    var position = tileData.poses[poseIndex].position;
                    if (mapFlags.ContainsKey(position))
                    {
                        mapFlags[position] |= tile.Flags;
                    }
                    else
                    {
                        mapFlags.Add(position, tile.Flags);
                    }
                }
            }
        }

        private void AddNodes(Tilemap3D tilemap)
        {
            nodeIndices = new Dictionary<Vector3Int, int>(mapFlags.Count);
            nodeDatas = new List<ClientNavigationNodeData>();

            foreach (var entry in mapFlags)
            {
                var position = entry.Key;
                var flags = entry.Value;

                if ((flags & 1) == 0)
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

                    nodeIndices.Add(position, nodeDatas.Count);
                    nodeDatas.Add(nodeData);
                }
            }

            nodes = new ClientNavigationGraph.Node[nodeDatas.Count];
            for (int i = 0; i < nodeDatas.Count; i++)
            {
                nodes[i] = new ClientNavigationGraph.Node()
                {
                    northIndex = -1,
                    southIndex = -1,
                    eastIndex = -1,
                    westIndex = -1,
                    data = nodeDatas[i]
                };
            }
        }

        private void LinkNodes()
        {
            
            foreach (var entry in nodeIndices)
            {
                var position = entry.Key;
                var index = entry.Value;
                var westPosition = new Vector3Int(position.x - 1, position.y, position.z);
                var eastPosition = new Vector3Int(position.x + 1, position.y, position.z);
                var northPosition = new Vector3Int(position.x, position.y, position.z + 1);
                var southPosition = new Vector3Int(position.x, position.y, position.z - 1);

                LinkNode(index, northPosition, eastPosition, southPosition, westPosition);

                if ((mapFlags[position] & 2) == 2)
                {
                    westPosition.y += 1;
                    eastPosition.y += 1;
                    northPosition.y += 1;
                    southPosition.y += 1;
                    LinkNode(index, northPosition, eastPosition, southPosition, westPosition);
                }
            }

            clientGraph.Nodes.AddRange(nodes);
        }

        private void LinkNode(int index, Vector3Int northPosition, Vector3Int eastPosition, Vector3Int southPosition, Vector3Int westPosition)
        {
            ref var node = ref nodes[index];
            if (nodeIndices.TryGetValue(westPosition, out int westIndex))
            {
                node.westIndex = westIndex;
                nodes[westIndex].eastIndex = index;
            }

            if (nodeIndices.TryGetValue(eastPosition, out int eastIndex))
            {
                node.eastIndex = eastIndex;
                nodes[eastIndex].westIndex = index;
            }

            if (nodeIndices.TryGetValue(northPosition, out int northIndex))
            {
                node.northIndex = northIndex;
                nodes[northIndex].southIndex = index;
            }

            if (nodeIndices.TryGetValue(southPosition, out int southIndex))
            {
                node.southIndex = southIndex;
                nodes[southIndex].northIndex = index;
            }
        }

        public void BuildFromTilemap(Tilemap3D tilemap)
        {
            clientGraph.Clear();
            ComputeMapFlags(tilemap);
            AddNodes(tilemap);
            LinkNodes();
        }
    }
}