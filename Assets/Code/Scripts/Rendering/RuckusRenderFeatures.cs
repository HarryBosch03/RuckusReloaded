using UnityEngine.Rendering.Universal;

namespace RuckusReloaded.Runtime.Rendering
{
    public class RuckusRenderFeatures : ScriptableRendererFeature
    {
        public ViewportOverlayPass.Settings viewportOverlaySettings;

        private ViewportOverlayPass viewportOverlayPass;
        
        public override void Create()
        {
            viewportOverlayPass = new ViewportOverlayPass(viewportOverlaySettings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(viewportOverlayPass);
        }
    }
}