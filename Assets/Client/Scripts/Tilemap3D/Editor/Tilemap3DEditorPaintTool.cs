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

namespace MonsterWorld.Unity.Tilemap3D
{
    public class Tilemap3DEditorPaintTool : Tilemap3DEditorTool
    {
        private GUIContent m_IconContent;
        public override GUIContent toolbarIcon => m_IconContent;

        public Tilemap3DEditorPaintTool(Tilemap3DEditor editor) : base(editor)
        {
            m_IconContent = EditorGUIUtility.IconContent("Grid.PaintTool");
            m_IconContent.tooltip = "Paint Tool";
        }

        public override void DrawPreview(CommandBuffer cmd)
        {
            if (Editor.IsEraserEnabled) return;

            var selectedTileInfo = Editor.selectedTileInfo;
            cmd.DrawMesh(selectedTileInfo.mesh, GetTilePreviewMatrix(), selectedTileInfo.material, 0, 2);
        }

        public override void OnSceneGUI()
        {
            var tilemap = Editor.Tilemap;
            int tileIndex = Editor.TileIndex;

            if (tilemap == null || tileIndex < 0) return;

            var controlID = GUIUtility.GetControlID(FocusType.Passive);
            switch (Event.current.type)
            {
                case EventType.Layout:
                    HandleUtility.AddDefaultControl(controlID);
                    break;
                case EventType.MouseDrag:
                    if (Event.current.button == 0)
                    {
                        PutOrRemoveTile(tilemap, tileIndex, TilePose, Editor.IsEraserEnabled);
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseDown:
                    if (Event.current.button == 0)
                    {
                        PutOrRemoveTile(tilemap, tileIndex, TilePose, Editor.IsEraserEnabled);
                        Event.current.Use();
                    }
                    break;
            }

            var handleMatrix = Handles.matrix;
            Handles.matrix = tilemap.transform.localToWorldMatrix;
            DrawGrid(TilePosition);
            Handles.matrix = handleMatrix;
            SceneView.currentDrawingSceneView.Repaint();
        }

        private Matrix4x4 GetTilePreviewMatrix()
        {           
            return TilePose.Matrix * Editor.selectedTileInfo.prefab.transform.localToWorldMatrix;
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

            Handles.color = Editor.IsEraserEnabled ? Color.red : Color.white;
            Handles.DrawWireCube(position + new Vector3(0.5f, 0.0f, 0.5f), new Vector3(1.0f, 0.0f, 1.0f));
        }
    }
}