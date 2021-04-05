//-----------------------------------------------------------------
// File:         Tilemap3DEditor.cs
// Description:  Edit tilemaps
// Module:       Map Editor
// Author:       Noé Masse
// Date:         28/03/2021
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;
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

        [MenuItem("GameObject/MonsterWorld/Tilemap 3D", false, 10)]
        private static void CreateTilemap(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Tilemap 3D");
            go.AddComponent<Tilemap3D>();
            go.AddComponent<Tilemap3DRenderer>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        public struct TileInfo
        {
            public Tile3D tile;
            public int rotation;

            public int FinalRotation => tile.CanBeRotated ? rotation : 0;
        }

        private Tilemap3D _Tilemap3D;
        private int _tileIndex = 0;
        private int _height = 0;
        private bool _isEraserEnabled = false;

        public Tilemap3D Tilemap => _Tilemap3D;
        public int TileIndex => _tileIndex;
        public int Height => _height;
        public bool IsEraserEnabled => _isEraserEnabled;

        public TileInfo selectedTileInfo;

        private List<Tilemap3DEditorTool> _tools;
        private int _currentToolIndex = 0;

        private Tilemap3DEditorTool CurrentTool => _tools[_currentToolIndex];

        private GUIContent _increaseHeightIcon;
        private GUIContent _decreaseHeightIcon;
        private GUIContent _eraserIcon;
        private GUIContent _rotateIcon;
        //private GUIContent _addIcon;

        private Vector2 _scrollPosition;
        private string _searchText;
        //private int _prefabPickerControlId = -1;
        private CommandBuffer _commandBuffer;

        private void OnEnable()
        {
            _increaseHeightIcon = EditorGUIUtility.IconContent("TerrainInspector.TerrainToolRaise");
            _decreaseHeightIcon = EditorGUIUtility.IconContent("TerrainInspector.TerrainToolLowerAlt");
            _eraserIcon = EditorGUIUtility.IconContent("Grid.EraserTool");
            _rotateIcon = EditorGUIUtility.IconContent("RotateTool");

            _increaseHeightIcon.tooltip = "Increase Height (Numpad +)";
            _decreaseHeightIcon.tooltip = "Decrease Height (Numpad -)";
            _eraserIcon.tooltip = "Toggle Eraser (E)";
            _rotateIcon.tooltip = "Rotate Tile (R)";

            //_addIcon = EditorGUIUtility.IconContent("CreateAddNew");

            _tools = new List<Tilemap3DEditorTool>();
            _tools.Add(new Tilemap3DEditorSelectionTool(this));
            _tools.Add(new Tilemap3DEditorPaintTool(this));
            _tools.Add(new Tilemap3DEditorSquareTool(this));

            _searchText = "";

            _Tilemap3D = target as Tilemap3D;
            _commandBuffer = new CommandBuffer()
            {
                name = "Tilemap 3D Editor Preview"
            };
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;

            selectedTileInfo.tile = _Tilemap3D.tileset[0];
        }

        private void OnDisable()
        {
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
            _commandBuffer.Release();
            Tools.hidden = false;
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public void RotateTile()
        {
            selectedTileInfo.rotation = (selectedTileInfo.rotation + 1) % 4;
        }

        public void IncreaseHeight()
        {
            _height++;
        }

        public void DecreaseHeight()
        {
            _height--;
        }

        public void ToggleEraser()
        {
            _isEraserEnabled = !_isEraserEnabled;
            Repaint();
        }

        private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (camera.cameraType == CameraType.SceneView)
            {
                if (selectedTileInfo.tile != null && selectedTileInfo.tile.IsValid())
                {
                    // Draw Preview
                    _commandBuffer.Clear();
                    _commandBuffer.SetGlobalMatrix(Tilemap3DRenderFeature._TilemapMatrix, _Tilemap3D.transform.localToWorldMatrix);
                    CurrentTool.DrawPreview(_commandBuffer);
                    context.ExecuteCommandBuffer(_commandBuffer);
                    context.Submit();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            HandleInputs();
            EditorGUI.BeginChangeCheck();
            _Tilemap3D.tileset = (Tileset3D) EditorGUILayout.ObjectField("Tileset", _Tilemap3D.tileset, typeof(Tileset3D), false);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_Tilemap3D);
            }
            if (_Tilemap3D.tileset != null)
            {
                DrawToolbar();
                DrawTilePalette();
            }
            else
            {

            }
        }

        private void HandleInputs()
        {
            var controlID = GUIUtility.GetControlID(FocusType.Passive);
            switch (Event.current.type)
            {
                case EventType.Layout:
                    HandleUtility.AddDefaultControl(controlID);
                    break;
                case EventType.KeyDown:
                    if (ProcessKeyInput(Event.current.keyCode))
                    {
                        Event.current.Use();
                    }
                    break;
            }
        }

        private void DrawToolbar()
        {
            GUILayout.BeginHorizontal(GUILayout.Height(24f));
            EditorGUILayout.Space();
            _currentToolIndex = GUILayout.Toolbar(_currentToolIndex, _tools.Select((tool) => tool.toolbarIcon).ToArray(), GUILayout.Height(24f));
            Tools.hidden = _currentToolIndex > 0;

            EditorGUILayout.Space();
            _isEraserEnabled = GUILayout.Toggle(_isEraserEnabled, _eraserIcon, "Button", GUILayout.Height(24f));

            EditorGUILayout.Space();
            if (GUILayout.Button(_rotateIcon, GUILayout.Height(24f))) RotateTile();
            if (GUILayout.Button(_increaseHeightIcon, GUILayout.Height(24f))) IncreaseHeight();
            if (GUILayout.Button(_decreaseHeightIcon, GUILayout.Height(24f))) DecreaseHeight();
            EditorGUILayout.Space();

            GUILayout.EndHorizontal();
        }

        private void DrawTilePalette()
        {
            GUILayout.Space(10f);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Tile Palette", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            _searchText = GUILayout.TextField(_searchText, EditorStyles.toolbarSearchField, GUILayout.Width(80f));
            //DrawAddPrefabButton();
            EditorGUILayout.EndHorizontal();

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true, GUILayout.Height(300f));
            EditorGUI.BeginChangeCheck();
            var gridElements = _Tilemap3D.tileset.Select((p) => AssetPreview.GetAssetPreview(p.Prefab)).ToArray();
            _tileIndex = GUILayout.SelectionGrid(_tileIndex, gridElements, 4);
            if (EditorGUI.EndChangeCheck())
            {
                selectedTileInfo.tile = Tilemap.tileset[_tileIndex];
            }
            GUILayout.EndScrollView();
        }

        //private void DrawAddPrefabButton()
        //{
        //    if (GUILayout.Button(_addIcon, EditorStyles.toolbarButton, GUILayout.Width(40f)) && _prefabPickerControlId == -1)
        //    {
        //        _prefabPickerControlId = GUIUtility.GetControlID(FocusType.Passive);
        //        EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "pfb_tile", _prefabPickerControlId);
        //    }

        //    string commandName = Event.current.commandName;
        //    if (commandName == "ObjectSelectorClosed" && EditorGUIUtility.GetObjectPickerControlID() == _prefabPickerControlId)
        //    {
        //        GameObject prefab = EditorGUIUtility.GetObjectPickerObject() as GameObject;
        //        if (prefab != null)
        //        {
        //            if (_Tilemap3D.TryAddTilePrefab(prefab))
        //            {
        //                EditorUtility.SetDirty(_Tilemap3D);
        //            }
        //        }
        //        _prefabPickerControlId = -1;
        //    }
        //}

        private void OnSceneGUI()
        {
            HandleInputs();
            CurrentTool.PrepareSceneGUI();
            CurrentTool.OnSceneGUI();
        }

        private bool ProcessKeyInput(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.Escape:
                    CurrentTool.CancelAction();
                    return true;
                case KeyCode.Tab:
                    _currentToolIndex = (_currentToolIndex + 1) % _tools.Count;
                    return true;
                case KeyCode.KeypadPlus:
                    IncreaseHeight();
                    CurrentTool.RefreshPreview();
                    return true;
                case KeyCode.KeypadMinus:
                    DecreaseHeight();
                    CurrentTool.RefreshPreview();
                    return true;
                case KeyCode.R:
                    RotateTile();
                    CurrentTool.RefreshPreview();
                    return true;
                case KeyCode.E:
                    ToggleEraser();
                    return true;
                default:
                    return false;
            }
        }
    }
}