//-----------------------------------------------------------------
// File:         Tilemap3DRenderer.cs
// Description:  Tilemap Renderer
// Module:       Map Editor
// Author:       Noé Masse
// Date:         28/03/2021
//-----------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MonsterWorld.Unity.Tilemap3D
{
    public class Tilemap3DRenderFeature : ScriptableRendererFeature
    {
        public static readonly int _TilemapMatrix = Shader.PropertyToID("_TilemapMatrix");
        public static readonly List<Tilemap3DRenderer> Tilemap3DRenderers = new List<Tilemap3DRenderer>();

        private Tilemap3DDepthOnlyPass _depthPrepass;
        private Tilemap3DDrawPass _drawPass;

        public override void Create()
        {
            _depthPrepass = new Tilemap3DDepthOnlyPass();
            _depthPrepass.renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
            _depthPrepass.Setup();

            _drawPass = new Tilemap3DDrawPass();
            _drawPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var cameraType = renderingData.cameraData.camera.cameraType;
            if (cameraType == CameraType.Game || cameraType == CameraType.SceneView)
            {
                renderer.EnqueuePass(_depthPrepass);
                renderer.EnqueuePass(_drawPass);
            }

        }
    }
}


