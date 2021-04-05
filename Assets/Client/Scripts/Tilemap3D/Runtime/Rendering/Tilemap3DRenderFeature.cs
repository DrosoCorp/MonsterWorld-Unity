//-----------------------------------------------------------------
// File:         Tilemap3DRenderer.cs
// Description:  Tilemap Renderer
// Module:       Map Editor
// Author:       Noé Masse
// Date:         28/03/2021
//-----------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MonsterWorld.Unity.Tilemap3D
{
    public class Tilemap3DRenderFeature : ScriptableRendererFeature
    {
        public static readonly int _TilemapMatrix = Shader.PropertyToID("_TilemapMatrix");
        public static readonly List<Tilemap3DRenderer> Tilemap3DRenderers = new List<Tilemap3DRenderer>();

        private Tilemap3DDepthOnlyPass _depthPrepass;
        private Tilemap3DDrawPass _drawOpaquePass;
        private Tilemap3DDrawPass _drawTransparentPass;

        public override void Create()
        {
            _depthPrepass = new Tilemap3DDepthOnlyPass();
            _depthPrepass.renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;

            _drawOpaquePass = new Tilemap3DDrawPass(true);
            _drawOpaquePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

            _drawTransparentPass = new Tilemap3DDrawPass(false);
            _drawTransparentPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            ref CameraData cameraData = ref renderingData.cameraData;
            var cameraType = cameraData.camera.cameraType;

            if (cameraType == CameraType.Game || cameraType == CameraType.SceneView)
            {
                if (RequiresDepthPrepass(ref cameraData))
                {
                    renderer.EnqueuePass(_depthPrepass);
                }
                renderer.EnqueuePass(_drawOpaquePass);
                renderer.EnqueuePass(_drawTransparentPass);
            }
        }

        private bool RequiresDepthPrepass(ref CameraData cameraData)
        {
            // From ForwardRenderer.cs
            bool isSceneViewCamera = cameraData.isSceneViewCamera;
            bool requiresDepthTexture = cameraData.requiresDepthTexture;
            bool isStereoEnabled = cameraData.isStereoEnabled;

            bool requiresDepthPrepass = isSceneViewCamera;
            requiresDepthPrepass |= (requiresDepthTexture && !CanCopyDepth(ref cameraData));
            requiresDepthPrepass |= (isStereoEnabled && requiresDepthTexture);

            return requiresDepthPrepass;
        }

        private bool CanCopyDepth(ref CameraData cameraData)
        {
            // From ForwardRenderer.cs
            bool msaaEnabledForCamera = cameraData.cameraTargetDescriptor.msaaSamples > 1;
            bool supportsTextureCopy = SystemInfo.copyTextureSupport != CopyTextureSupport.None;
            bool supportsDepthTarget = RenderingUtils.SupportsRenderTextureFormat(RenderTextureFormat.Depth);
            bool supportsDepthCopy = !msaaEnabledForCamera && (supportsDepthTarget || supportsTextureCopy);

            // TODO:  We don't have support to highp Texture2DMS currently and this breaks depth precision.
            // currently disabling it until shader changes kick in.
            //bool msaaDepthResolve = msaaEnabledForCamera && SystemInfo.supportsMultisampledTextures != 0;
            bool msaaDepthResolve = false;
            return supportsDepthCopy || msaaDepthResolve;
        }
    }
}


