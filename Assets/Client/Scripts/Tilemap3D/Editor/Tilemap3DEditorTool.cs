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

namespace MonsterWorld.Unity.Tilemap
{
    public abstract class Tilemap3DEditorTool
    {
        private Tilemap3DEditor _editor;
        private Vector3Int _tilePosition;

        protected Tilemap3DEditor Editor => _editor;
        protected Vector3Int TilePosition => _tilePosition;
        protected TilePose TilePose => new TilePose() { position = _tilePosition, rotation = Editor.selectedTileInfo.FinalRotation };

        public Tilemap3DEditorTool(Tilemap3DEditor editor)
        {
            _editor = editor;
        }

        public abstract GUIContent toolbarIcon { get; }
        public abstract void DrawPreview(CommandBuffer cmd);
        public abstract void OnSceneGUI();

        public virtual void RefreshPreview() { }
        public virtual void CancelAction() { }

        public void PrepareSceneGUI()
        {
            if (RaycastTilePosition(out Vector3Int position))
            {
                _tilePosition = position;
            }
        }

        private bool RaycastTilePosition(out Vector3Int position)
        {
            var tilemap = _editor.Tilemap;
            var height = _editor.Height;

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Plane plane = new Plane(tilemap.transform.up, tilemap.transform.position + tilemap.transform.up * (0.1f + height));

            position = new Vector3Int();

            if (plane.Raycast(ray, out float distance))
            {
                var worldSpacePosition = ray.origin + ray.direction * distance;
                var localPosition = tilemap.transform.worldToLocalMatrix.MultiplyPoint3x4(worldSpacePosition);

                position.x = Mathf.FloorToInt(localPosition.x);
                position.y = Mathf.FloorToInt(localPosition.y);
                position.z = Mathf.FloorToInt(localPosition.z);
                return true;
            }
            return false;
        }

        protected void PutOrRemoveTile(int tileIndex, TilePose tilePose, bool erase)
        {
            var tilemapBuilder = _editor.TilemapDataBuilder;
            var tilemap = _editor.Tilemap;
            var tile = tilemap.Tileset[tileIndex];

            if (tilemapBuilder.HasTile(tile.Layer, tilePose.position))
            {
                tilemapBuilder.RemoveTile(tile.Layer, tilePose.position);
                EditorUtility.SetDirty(tilemap);
            }

            if (!erase)
            {
                tilemapBuilder.AddTile(tileIndex, tilePose);
                EditorUtility.SetDirty(tilemap);
            }
        }
    }
}