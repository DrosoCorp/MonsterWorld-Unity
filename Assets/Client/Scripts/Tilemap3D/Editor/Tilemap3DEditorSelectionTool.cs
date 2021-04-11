//-----------------------------------------------------------------
// File:         Tilemap3DPaintTool.cs
// Description:  Paint Tiles
// Module:       Map Editor
// Author:       Noé Masse
// Date:         28/03/2021
//-----------------------------------------------------------------
using System;
using System.Linq;

using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Rendering;

namespace MonsterWorld.Unity.Tilemap
{
    public class Tilemap3DEditorSelectionTool : Tilemap3DEditorTool
    {
        private GUIContent m_IconContent;
        public override GUIContent toolbarIcon => m_IconContent;

        public Tilemap3DEditorSelectionTool(Tilemap3DEditor editor) : base(editor)
        {
            m_IconContent = EditorGUIUtility.IconContent("Grid.Default");
            m_IconContent.tooltip = "Default Tool";
        }

        public override void DrawPreview(CommandBuffer cmd)
        {
        }

        public override void OnSceneGUI()
        {
        }

        private void DrawGrid(Vector3Int position)
        {
            Handles.color = Color.white * 0.3f;
            for (int x = -1; x < 3; x++)
            {
                var p1 = position + new Vector3Int(x, 0, -1);
                var p2 = position + new Vector3Int(x, 0, 2);
                Handles.DrawLine(p1, p2);
            }

            for (int z = -1; z < 3; z++)
            {
                var p1 = position + new Vector3Int(-1, 0, z);
                var p2 = position + new Vector3Int(2, 0, z);
                Handles.DrawLine(p1, p2);
            }

            Handles.color = Color.white;
            Handles.DrawWireCube(position + new Vector3(0.5f, 0.0f, 0.5f), new Vector3(1.0f, 0.0f, 1.0f));
        }
    }
}