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

        private Vector3Int _tilePosition;

        public Tilemap3DEditorPaintTool(Tilemap3DEditor editor) : base(editor)
        {
            m_IconContent = EditorGUIUtility.IconContent("Grid.PaintTool");
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
            int height = Editor.Height;

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
                        PutOrRemoveTile(tilemap, tileIndex, _tilePosition, Editor.selectedTileInfo.rotation);
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseDown:
                    if (Event.current.button == 0)
                    {
                        PutOrRemoveTile(tilemap, tileIndex, _tilePosition, Editor.selectedTileInfo.rotation);
                        Event.current.Use();
                    }
                    break;
            }

            // Raycast
            var handleMatrix = Handles.matrix;
            Handles.matrix = tilemap.transform.localToWorldMatrix;
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            Plane plane = new Plane(tilemap.transform.up, tilemap.transform.position + tilemap.transform.up * (0.1f + height));
            if (plane.Raycast(ray, out float distance))
            {
                var position = ray.origin + ray.direction * distance;
                var localPosition = tilemap.transform.worldToLocalMatrix.MultiplyPoint3x4(position);
                Vector3Int localPositionInt = new Vector3Int();

                localPositionInt.x = Mathf.FloorToInt(localPosition.x);
                localPositionInt.y = Mathf.FloorToInt(localPosition.y);
                localPositionInt.z = Mathf.FloorToInt(localPosition.z);

                DrawGrid(localPositionInt);

                _tilePosition = localPositionInt;
                SceneView.currentDrawingSceneView.Repaint();

            }
            Handles.matrix = handleMatrix;
        }

        private void PutOrRemoveTile(Tilemap3D tilemap, int tileIndex, Vector3Int tilePosition, int rotation)
        {
            if (tilemap.HasTile(tilePosition))
            {
                tilemap.RemoveTile(tilePosition);
                EditorUtility.SetDirty(tilemap);
            }

            if (!Editor.IsEraserEnabled)
            {
                tilemap.AddTile(tileIndex, _tilePosition, Editor.selectedTileInfo.rotation);
                EditorUtility.SetDirty(tilemap);
            }
        }

        private Matrix4x4 GetTilePreviewMatrix()
        {
            var translation = _tilePosition + new Vector3(0.5f, 0.5f, 0.5f);
            var rotation = Quaternion.AngleAxis(90f * Editor.selectedTileInfo.rotation, Vector3.up);
            
            return Matrix4x4.TRS(translation, rotation, Vector3.one) * Editor.selectedTileInfo.prefab.transform.localToWorldMatrix;
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