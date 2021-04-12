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

namespace MonsterWorld.Unity.Tilemap
{
    public class Tilemap3DDepthOnlyPass : ScriptableRenderPass
    {
        const string profilerTag = "Tilemap3D Depth Prepass";
        private RenderTargetHandle depthAttachmentHandle;

        public Tilemap3DDepthOnlyPass()
        {
            depthAttachmentHandle.Init("_CameraDepthTexture");
            profilingSampler = new ProfilingSampler(profilerTag);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) 
        {
            ConfigureTarget(depthAttachmentHandle.Identifier());
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get(profilerTag);

            using (new ProfilingScope(cmd, profilingSampler))
            {
                cmd.Clear();

                var renderers = Tilemap3DRenderFeature.Tilemap3DRenderers;
                for (int i = 0; i < renderers.Count; i++)
                {
                    renderers[i].DrawOpaques(cmd, 1);
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();
                }
            }
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd) {}
    }
}