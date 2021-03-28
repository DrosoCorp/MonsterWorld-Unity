//-----------------------------------------------------------------
// File:         Tilemap3DEditor.cs
// Description:  Edit tilemaps
// Module:       Map Editor
// Author:       Noé Masse
// Date:         28/03/2021
//-----------------------------------------------------------------
using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace MonsterWorld.Unity.Tilemap3D
{
    [CustomEditor(typeof(Tilemap3D))]
    public class Tilemap3DEditor : Editor
    {
        private static readonly string[] _toolbarContent = new string[] { "Tiles", "Paint" };

        private struct TilePreviewInfo
        {
            public Mesh mesh;
            public Material material;
            public Vector3Int localPosition;
        }

        private Tilemap3D _Tilemap3D;
        private int _tabIndex;

        private int _tileIndex = 0;
        private TilePreviewInfo _tilePreview;

        private int _prefabPickerControlId = -1;

        private CommandBuffer _commandBuffer;

        private void OnEnable()
        {
            _Tilemap3D = target as Tilemap3D;
            _commandBuffer = new CommandBuffer()
            {
                name = "Tilemap 3D Editor Preview"
            };
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
        }

        private void OnDisable()
        {
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
            _commandBuffer.Release();
            Tools.hidden = false;
        }

        private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (camera.cameraType == CameraType.SceneView)
            {
                if (_tilePreview.mesh != null && _tilePreview.material != null)
                {
                    // Draw Preview
                    _commandBuffer.Clear();
                    _commandBuffer.SetGlobalMatrix(Tilemap3DRenderer._TilemapMatrix, _Tilemap3D.transform.localToWorldMatrix);
                    _commandBuffer.DrawMesh(_tilePreview.mesh, GetTilePreviewMatrix(), _tilePreview.material, 0, 0);
                    context.ExecuteCommandBuffer(_commandBuffer);
                    context.Submit();
                }

            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            _tabIndex = GUILayout.Toolbar(_tabIndex, _toolbarContent);
            if (EditorGUI.EndChangeCheck())
            {
                Tools.hidden = _tabIndex == 1;
                SceneView.RepaintAll();
            }

            if (_tabIndex == 0)
            {
                DrawAddPrefabButton();
            }

            EditorGUI.BeginChangeCheck();
            _tileIndex = GUILayout.SelectionGrid(_tileIndex, _Tilemap3D.PrefabList.Select((p) => AssetPreview.GetAssetPreview(p)).ToArray(), 4);
            if (EditorGUI.EndChangeCheck())
            {
                var data = _Tilemap3D.TileRenderDataList[_tileIndex];
                _tilePreview.mesh = data.mesh;
                _tilePreview.material = data.material;
            }
        }

        private void DrawAddPrefabButton()
        {
            if (GUILayout.Button("Add Prefab") && _prefabPickerControlId == -1)
            {
                _prefabPickerControlId = GUIUtility.GetControlID(FocusType.Passive);
                EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "pfb_tile", _prefabPickerControlId);
            }

            string commandName = Event.current.commandName;
            if (commandName == "ObjectSelectorClosed" && EditorGUIUtility.GetObjectPickerControlID() == _prefabPickerControlId)
            {
                GameObject prefab = EditorGUIUtility.GetObjectPickerObject() as GameObject;
                if (prefab != null)
                {
                    if (_Tilemap3D.TryAddTilePrefab(prefab))
                    {
                        EditorUtility.SetDirty(_Tilemap3D);
                    }
                }
                _prefabPickerControlId = -1;
            }
        }

        private void OnSceneGUI()
        {
            if (_tabIndex == 1)
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
                            if (_Tilemap3D.AddTile(_tileIndex, _tilePreview.localPosition) == false)
                            {
                                if (_Tilemap3D.RemoveTile(_tilePreview.localPosition))
                                {
                                    EditorUtility.SetDirty(_Tilemap3D);
                                }
                            }
                            else
                            {
                                EditorUtility.SetDirty(_Tilemap3D);
                            }
                            Event.current.Use();
                        }
                        break;
                }

                // Raycast
                var handleMatrix = Handles.matrix;
                Handles.matrix = _Tilemap3D.transform.localToWorldMatrix;
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

                Plane plane = new Plane(_Tilemap3D.transform.up, _Tilemap3D.transform.position + new Vector3(0.0f, 0.1f, 0.0f));
                if (plane.Raycast(ray, out float distance))
                {
                    var position = ray.origin + ray.direction * distance;
                    var localPosition = _Tilemap3D.transform.worldToLocalMatrix.MultiplyPoint3x4(position);
                    Vector3Int localPositionInt = new Vector3Int();

                    localPositionInt.x = Mathf.FloorToInt(localPosition.x);
                    localPositionInt.y = Mathf.FloorToInt(localPosition.y);
                    localPositionInt.z = Mathf.FloorToInt(localPosition.z);

                    Handles.DrawWireCube(localPositionInt + new Vector3(0.5f, 0.0f, 0.5f), new Vector3(1.0f, 0.0f, 1.0f));
                    _tilePreview.localPosition = localPositionInt;
                    SceneView.currentDrawingSceneView.Repaint();

                }
                Handles.matrix = handleMatrix;
            }
        }

        private Matrix4x4 GetTilePreviewMatrix()
        {
            return Matrix4x4.Translate(_tilePreview.localPosition) * _Tilemap3D.PrefabList[_tileIndex].transform.localToWorldMatrix;
        }
    }
}