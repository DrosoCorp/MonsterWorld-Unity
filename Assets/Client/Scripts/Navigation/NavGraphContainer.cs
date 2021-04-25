using UnityEngine;
using System.Collections;
using MonsterWorld.Unity.Tilemap;
using MonsterWorld.Unity.Navigation;
using Sirenix.OdinInspector;

public class NavGraphContainer : MonoBehaviour
{
    [SerializeField] private ClientNavigationGraph _graph;

    public ClientNavigationGraph Graph => _graph;

    private void OnDrawGizmosSelected()
    {
        if (_graph == null) return;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.green;
        for (int i = 0; i < _graph.Count; i++)
        {
            var node = _graph[i];
            Gizmos.DrawSphere(node.data.position, 0.1f);
            
            if (node.southIndex >= 0)
            {
                var targetNode = _graph[node.southIndex];
                Gizmos.DrawLine(node.data.position, targetNode.data.position);
            }

            if (node.eastIndex >= 0)
            {
                var targetNode = _graph[node.eastIndex];
                Gizmos.DrawLine(node.data.position, targetNode.data.position);
            }

            if (node.northIndex >= 0)
            {
                var targetNode = _graph[node.northIndex];
                Gizmos.DrawLine(node.data.position, targetNode.data.position);
            }

            if (node.westIndex >= 0)
            {
                var targetNode = _graph[node.westIndex];
                Gizmos.DrawLine(node.data.position, targetNode.data.position);
            }
        }
    }

    [Button("Build Graph", ButtonSizes.Large, ButtonStyle.Box, Expanded = true)]
    public void BuildGraph(Tilemap3D tilemap)
    {
        var builder = new NavigationGraphBuilder(_graph);
        builder.BuildFromTilemap(tilemap);
    }
}