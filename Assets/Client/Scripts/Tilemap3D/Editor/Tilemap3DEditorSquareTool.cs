//-----------------------------------------------------------------
// File:         Tilemap3DPaintTool.cs
// Description:  Paint Tiles
// Module:       Map Editor
// Author:       Noé Masse
// Date:         28/03/2021
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Rendering;

namespace MonsterWorld.Unity.Tilemap3D
{
    public class Tilemap3DEditorSquareTool : Tilemap3DEditorTool
    {
        private GUIContent m_IconContent;
        public override GUIContent toolbarIcon => m_IconContent;

        private Vector3Int _startPosition;
        private Vector3Int _endPosition;
        private bool _isExpanding = false;

        private Vector3Int _lastTilePosition;
        private List<Matrix4x4> _previewMatrices;

        public Tilemap3DEditorSquareTool(Tilemap3DEditor editor) : base(editor)
        {
            m_IconContent = EditorGUIUtility.IconContent("Grid.BoxTool");
            m_IconContent.tooltip = "Area Tool";
            _previewMatrices = new List<Matrix4x4>(64);
        }

        public override void DrawPreview(CommandBuffer cmd)
        {
            if (Editor.IsEraserEnabled) return;

            var selectedTileInfo = Editor.selectedTileInfo;
            if (_isExpanding)
            {
                cmd.DrawMeshInstanced(selectedTileInfo.tile.Mesh, 0, selectedTileInfo.tile.Material, 2, _previewMatrices.Take(1023).ToArray());
            }
            else
            {
                cmd.DrawMesh(selectedTileInfo.tile.Mesh, TilePose.Matrix * selectedTileInfo.tile.Prefab.transform.localToWorldMatrix, selectedTileInfo.tile.Material, 0, 2);
            }
        }

        public override void OnSceneGUI()
        {
            var controlID = GUIUtility.GetControlID(FocusType.Passive);
            switch (Event.current.type)
            {
                case EventType.Layout:
                    HandleUtility.AddDefaultControl(controlID);
                    break;
                case EventType.MouseDown:
                    if (Event.current.button == 0)
                    {
                        _isExpanding = true;
                        _startPosition = TilePosition;
                        _endPosition = TilePosition;
                        _lastTilePosition = TilePosition;
                        UpdatePreviewMatrices();
                    }
                    break;
                case EventType.MouseUp:
                    if (Event.current.button == 0)
                    {
                        if (_isExpanding)
                        {
                            _isExpanding = false;
                            _endPosition = TilePosition;
                            PutOrRemoveTiles();
                            _previewMatrices.Clear();
                        }
                    }
                    break;
                case EventType.MouseDrag:
                    if (Event.current.button == 0)
                    {
                        _endPosition = TilePosition;
                        if (TilePosition != _lastTilePosition)
                        {
                            _lastTilePosition = TilePosition;
                            UpdatePreviewMatrices();
                        }
                    }
                    break;
            }

            var handleMatrix = Handles.matrix;
            Handles.matrix = Editor.Tilemap.transform.localToWorldMatrix;
            DrawPreviewHandle();
            Handles.matrix = handleMatrix;
            SceneView.currentDrawingSceneView.Repaint();
        }

        public override void RefreshPreview()
        {
            if (_isExpanding) UpdatePreviewMatrices();
        }

        public override void CancelAction()
        {
            _isExpanding = false;
            _previewMatrices.Clear();
        }

        private void PutOrRemoveTiles()
        {
            var tilemap = Editor.Tilemap;
            int tileIndex = Editor.TileIndex;
            int rotation = Editor.selectedTileInfo.FinalRotation;

            Vector3Int min = new Vector3Int()
            {
                x = Mathf.Min(_startPosition.x, _endPosition.x),
                y = Mathf.Min(_startPosition.y, _endPosition.y),
                z = Mathf.Min(_startPosition.z, _endPosition.z)
            };

            Vector3Int max = new Vector3Int()
            {
                x = Mathf.Max(_startPosition.x, _endPosition.x),
                y = Mathf.Max(_startPosition.y, _endPosition.y),
                z = Mathf.Max(_startPosition.z, _endPosition.z)
            };

            for (int x = min.x; x <= max.x; x++)
            {
                for (int y = min.y; y <= max.y; y++)
                {
                    for (int z = min.z; z <= max.z; z++)
                    {
                        var pose = new TilePose() 
                        { 
                            position = new Vector3Int(x, y, z), 
                            rotation = rotation 
                        };
                        PutOrRemoveTile(tilemap, tileIndex, pose, Editor.IsEraserEnabled);
                    }
                }
            }

        }

        private void UpdatePreviewMatrices()
        {
            int rotation = Editor.selectedTileInfo.FinalRotation;
            if (Editor.selectedTileInfo.tile == null || !Editor.selectedTileInfo.tile.IsValid()) return;
            Matrix4x4 prefabMatrix = Editor.selectedTileInfo.tile.Prefab.transform.localToWorldMatrix;

            _previewMatrices.Clear();

            Vector3Int min = new Vector3Int()
            {
                x = Mathf.Min(_startPosition.x, _endPosition.x),
                y = Mathf.Min(_startPosition.y, _endPosition.y),
                z = Mathf.Min(_startPosition.z, _endPosition.z)
            };

            Vector3Int max = new Vector3Int()
            {
                x = Mathf.Max(_startPosition.x, _endPosition.x),
                y = Mathf.Max(_startPosition.y, _endPosition.y),
                z = Mathf.Max(_startPosition.z, _endPosition.z)
            };

            for (int x = min.x; x <= max.x; x++)
            {
                for (int y = min.y; y <= max.y; y++)
                {
                    for (int z = min.z; z <= max.z; z++)
                    {
                        var pose = new TilePose()
                        {
                            position = new Vector3Int(x, y, z),
                            rotation = rotation
                        };
                        _previewMatrices.Add(pose.Matrix * prefabMatrix);
                    }
                }
            }
        }

        private void DrawPreviewHandle()
        {
            Handles.color = Editor.IsEraserEnabled ? Color.red : Color.white;
            var offset = new Vector3(0.5f, 0.0f, 0.5f);

            if (_isExpanding)
            {
                var center = (Vector3)(_endPosition - _startPosition) / 2.0f + _startPosition + offset;

                var size = _endPosition - _startPosition;
                size.x = Mathf.Abs(size.x);
                size.y = Mathf.Abs(size.y);
                size.z = Mathf.Abs(size.z);
                size += new Vector3Int(1, 0, 1);

                Handles.DrawWireCube(center, size);
            }
            else
            {
                Handles.DrawWireCube(TilePosition + offset, new Vector3(1.0f, 0.0f, 1.0f));
            }
        }
    }
}