﻿//-----------------------------------------------------------------
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
    public class Tilemap3DDepthOnlyPass : ScriptableRenderPass
    {
        const string profilerTag = "Tilemap3D Depth Prepass";
        private static readonly ProfilingSampler profilingSampler = new ProfilingSampler(profilerTag);
        private RenderTargetHandle depthAttachmentHandle;

        public Tilemap3DDepthOnlyPass()
        {
            depthAttachmentHandle.Init("_CameraDepthTexture");
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
                    DrawTilemap3DRenderer(ref context, cmd, renderers[i]);
                }
            }
            CommandBufferPool.Release(cmd);
        }

        private void DrawTilemap3DRenderer(ref ScriptableRenderContext context, CommandBuffer cmd, Tilemap3DRenderer renderer)
        {
            var renderList = renderer.TileRenderList;
            if (renderList == null) return;

            cmd.BeginSample(renderer.name);
            cmd.SetGlobalMatrix(Tilemap3DRenderFeature._TilemapMatrix, renderer.transform.localToWorldMatrix);
            foreach (var renderData in renderList)
            {
                if (renderData.matrices.Count > 0)
                {
                    cmd.DrawMeshInstanced(renderData.mesh, 0, renderData.material, 1, renderData.matrices.ToArray());
                }
            }
            cmd.EndSample(renderer.name);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }

        public override void FrameCleanup(CommandBuffer cmd) {}
    }
}