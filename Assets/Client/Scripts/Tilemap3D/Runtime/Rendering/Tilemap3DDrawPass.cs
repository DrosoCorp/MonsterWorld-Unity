//-----------------------------------------------------------------
// File:         Tilemap3DRenderer.cs
// Description:  Tilemap Renderer
// Module:       Map Editor
// Author:       Noé Masse
// Date:         28/03/2021
//-----------------------------------------------------------------
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MonsterWorld.Unity.Tilemap3D
{
    public class Tilemap3DDrawPass : ScriptableRenderPass
    {
        const string profilerTagOpaque = "Tilemap3D Pass (Opaque)";
        const string profilerTagTransparent = "Tilemap3D Pass (Transparent)";
        private static readonly ProfilingSampler profilingSamplerOpaque = new ProfilingSampler(profilerTagOpaque);
        private static readonly ProfilingSampler profilingSamplerTransparent = new ProfilingSampler(profilerTagTransparent);

        private bool _isOpaquePass;

        public Tilemap3DDrawPass(bool opaque)
        {
            _isOpaquePass = opaque;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {}

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get(_isOpaquePass ? profilerTagOpaque : profilerTagTransparent);

            using (new ProfilingScope(cmd, _isOpaquePass ? profilingSamplerOpaque : profilingSamplerTransparent))
            {
                cmd.Clear();

                var renderers = Tilemap3DRenderFeature.Tilemap3DRenderers;
                for (int i = 0; i < renderers.Count; i++)
                {
                    if (_isOpaquePass)
                    {
                        renderers[i].DrawOpaques(cmd, 0);
                    }
                    else
                    {
                        renderers[i].DrawTransparents(cmd);
                    }
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();
                }
            }

            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd) { }
    }
}