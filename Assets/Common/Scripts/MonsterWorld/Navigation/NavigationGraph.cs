//-----------------------------------------------------------------
// File:         NavigationGraph.cs
// Description:  Base class for the navigation graph
// Module:       Navigation
// Author:       Noé Masse
// Date:         28/03/2021
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterWorld.Unity.Navigation
{
    [Serializable]
    public class NavigationGraph<T> where T : struct
    {
        [Serializable]
        public struct Node
        {
            public int northIndex;
            public int eastIndex;
            public int southIndex;
            public int westIndex;
            public T data;
        }

        [SerializeField] protected List<Node> _nodes = new List<Node>();

        public NavigationGraph() {}

        public int Count => _nodes.Count;
        public Node this[int index] => _nodes[index];

        public static NavigationGraph<U> ConvertNavigationGraph<U>(NavigationGraph<T> graph, Func<T, U> conversion) where U : struct
        {
            NavigationGraph<U> destination = new NavigationGraph<U>();

            for (int i = 0; i < destination._nodes.Count; i++)
            {
                Node nodeToConvert = graph._nodes[i];
                destination._nodes.Add(new NavigationGraph<U>.Node()
                {
                    northIndex = nodeToConvert.northIndex,
                    eastIndex  = nodeToConvert.eastIndex,
                    southIndex = nodeToConvert.southIndex,
                    westIndex  = nodeToConvert.westIndex,
                    data = conversion(nodeToConvert.data)
                });
            }

            return destination;
        }
    }
}
