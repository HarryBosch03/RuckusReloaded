using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RuckusReloaded.Runtime.Rendering
{
    public class ViewportOverlayPass : ScriptableRenderPass
    {
        private Settings settings;
        private Material overlayMaterial;
        private Material clearMaterial;
        private Mesh mesh;
        private int rtHandle = Shader.PropertyToID("_ViewportOverlay");

        private List<ShaderTagId> shaderTagIdList;
        
        public ViewportOverlayPass(Settings settings)
        {
            this.settings = settings;
            this.settings.Validate();

            if (!overlayMaterial)
            {
                overlayMaterial = new Material(Shader.Find("Hidden/ViewportOverlay"));
                overlayMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            
            if (!clearMaterial)
            {
                clearMaterial = new Material(Shader.Find("Hidden/Clear"));
                clearMaterial.hideFlags = HideFlags.HideAndDontSave;
            }

            if (!mesh)
            {
                mesh = new Mesh();
                mesh.hideFlags = HideFlags.HideAndDontSave;

                mesh.vertices = new Vector3[]
                {
                    new(-1.0f, -1.0f, 0.0f),
                    new(3.0f, -1.0f, 0.0f),
                    new(-1.0f, 3.0f, 0.0f),
                };
                mesh.uv = new Vector2[]
                {
                    new(0.0f, 1.0f),
                    new(2.0f, 1.0f),
                    new(0.0f, -1.0f),
                };
                mesh.triangles = new int[]
                {
                    0, 1, 2,
                };
            }

            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

            shaderTagIdList = new List<ShaderTagId>();
            shaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
            shaderTagIdList.Add(new ShaderTagId("UniversalForward"));
            shaderTagIdList.Add(new ShaderTagId("UniversalForwardOnly"));
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.width >>= settings.downscale;
            descriptor.height >>= settings.downscale;
            descriptor.colorFormat = RenderTextureFormat.ARGB32;
            
            cmd.GetTemporaryRT(rtHandle, descriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("ViewportOverlay");
            cmd.Clear();
            
            cmd.SetRenderTarget(rtHandle);
            cmd.ClearRenderTarget(true, false, Color.clear);
            cmd.DrawMesh(mesh, Matrix4x4.identity, clearMaterial, 0, 0);
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque, 1 << Layers.Viewport);
            var drawingSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);

            cmd.SetRenderTarget("_CameraColorAttachmentA");
            cmd.SetGlobalTexture("_ViewportOverlay", rtHandle);
            cmd.DrawMesh(mesh, Matrix4x4.identity, overlayMaterial, 0, 0);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(rtHandle);
        }

        [System.Serializable]
        public struct Settings
        {
            public int downscale;

            public void Validate()
            {
                downscale = Mathf.Max(0, downscale);
            }
        }
    }
}