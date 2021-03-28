//-----------------------------------------------------------------
// File:         Tilemap3DRenderer.cs
// Description:  Tilemap Renderer
// Module:       Map Editor
// Author:       Noé Masse
// Date:         28/03/2021
//-----------------------------------------------------------------
using UnityEngine;
using UnityEngine.Rendering;


namespace MonsterWorld.Unity.Tilemap3D
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Tilemap3D))]
    public class Tilemap3DRenderer : MonoBehaviour
    {
        public static readonly int _TilemapMatrix = Shader.PropertyToID("_TilemapMatrix");
        private Tilemap3D _Tilemap3D;
        private CommandBuffer _commandBuffer;

        private void OnEnable()
        {
            _commandBuffer = new CommandBuffer()
            {
                name = "Tilemap 3D Renderer"
            };
            _Tilemap3D = GetComponent<Tilemap3D>();
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
        }

        private void OnDisable()
        {
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
            _commandBuffer.Release();
        }

        private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (camera.cameraType == CameraType.Preview) return;

            _commandBuffer.Clear();
            _commandBuffer.SetGlobalMatrix(_TilemapMatrix, transform.localToWorldMatrix);
            var renderList = _Tilemap3D.TileRenderDataList;
            foreach (var renderData in renderList)
            {
                _commandBuffer.DrawMeshInstanced(renderData.mesh, 0, renderData.material, 0, renderData.matrices.ToArray());
            }
            Graphics.ExecuteCommandBuffer(_commandBuffer);
        }

        private void Update()
        {
        }
    }
}