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
    [CustomEditor(typeof(Tileset3D))]
    public class Tileset3DEditor : Editor
    {
        private static readonly string[] _categories = new string[] { "Ground", "Test" };

        private Tileset3D _Tileset3D;
        private int _tileIndex = -1;
        private int _paletteIndex = -1;
        
        public Tileset3D Tileset => _Tileset3D;
        public int TileIndex => _tileIndex;

        private Vector2 _scrollPosition;
        private string _searchFilter;
        private int _tilePickerControlId = -1;

        private GUIContent _addIcon;
        private GUIContent _removeIcon;

        private void OnEnable()
        {
            _addIcon = EditorGUIUtility.IconContent("Toolbar Plus");
            _removeIcon = EditorGUIUtility.IconContent("Toolbar Minus");
            _searchFilter = "";
            _tileIndex = -1;

            _Tileset3D = target as Tileset3D;
        }

        private void OnDisable()
        {
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(10f);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Tileset", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            _searchFilter = DrawSearchField(_searchFilter, ref _paletteIndex);
            DrawAddTileButton();
            DrawRemoveTileButton();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            bool changed = false;
            DrawTilesetPalette(_Tileset3D, _searchFilter, ref _tileIndex, ref _paletteIndex, ref _scrollPosition, ref changed);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawRemoveTileButton()
        {
            EditorGUI.BeginDisabledGroup(_tileIndex < 0 || _tileIndex >= _Tileset3D.Count);
            if (GUILayout.Button(_removeIcon, EditorStyles.toolbarButton, GUILayout.Width(40f)))
            {
                _Tileset3D.Tiles.RemoveAt(_tileIndex);
                EditorUtility.SetDirty(_Tileset3D);
            }
            EditorGUI.EndDisabledGroup();
        }

        private void DrawAddTileButton()
        {
            if (GUILayout.Button(_addIcon, EditorStyles.toolbarButton, GUILayout.Width(40f)) && _tilePickerControlId == -1)
            {
                _tilePickerControlId = GUIUtility.GetControlID(FocusType.Passive);
                EditorGUIUtility.ShowObjectPicker<Tile3D>(null, false, "", _tilePickerControlId);
            }

            string commandName = Event.current.commandName;
            if (commandName == "ObjectSelectorClosed" && EditorGUIUtility.GetObjectPickerControlID() == _tilePickerControlId)
            {
                Tile3D tile = EditorGUIUtility.GetObjectPickerObject() as Tile3D;
                if (tile != null)
                {
                    if (_Tileset3D.TryAddTile(tile))
                    {
                        EditorUtility.SetDirty(_Tileset3D);
                    }
                }
                _tilePickerControlId = -1;
            }
        }

        public static void DrawTilesetPalette(Tileset3D tileset, string searchFilter, ref int tileIndex, ref int paletteIndex, ref Vector2 scrollPosition, ref bool changed, params GUILayoutOption[] options)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
            EditorGUI.BeginChangeCheck();

            var tileReferences = tileset.Filter((tile) => tile.name.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0);
            var gridElements = tileReferences.Select((tileReference) =>
            {
                var content = new GUIContent(AssetPreview.GetAssetPreview(tileReference.tile));
                content.tooltip = tileReference.tile.name;
                return content;
            }).ToArray();
            paletteIndex = GUILayout.SelectionGrid(paletteIndex, gridElements, 4, options);

            if (EditorGUI.EndChangeCheck())
            {
                changed = true;
                if (paletteIndex < 0 || paletteIndex >= tileReferences.Count)
                {
                    tileIndex = -1;
                }
                else
                {
                    tileIndex = tileReferences[paletteIndex].indexInTileset;
                }
            }
            GUILayout.EndScrollView();
        }

        public static string DrawSearchField(string searchText, ref int paletteIndex)
        {
            EditorGUI.BeginChangeCheck();
            searchText = GUILayout.TextField(searchText, EditorStyles.toolbarSearchField, GUILayout.Width(80f));
            if (EditorGUI.EndChangeCheck())
            {
                paletteIndex = -1;
            }
            return searchText;
        }
    }
}