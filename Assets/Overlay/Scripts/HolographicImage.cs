using UnityEngine;
using Valve.VR;

namespace EVRC.Core.Overlay
{
    using Utils = OverlayUtils;

    /**
     * Simple overlay like the holographic buttons but just outputs an image
     */
    public class HolographicImage : HolographicBase, IRenderable
    {
        public enum RenderMode
        {
            Update,
            OnDemand,
        }

        public Texture backface;
        public RenderMode renderMode = RenderMode.Update;
        public Camera renderCamera;
        private Texture lastTexture;
        private bool isFacingHmd = true;

        protected override void Update()
        {
            base.Update();
            
            bool wasFlipped = isFacingHmd != Utils.IsFacingHmd(transform);
            isFacingHmd = Utils.IsFacingHmd(transform);
            
            if (renderMode == RenderMode.Update || wasFlipped)
            {
                DoRenderTexture();
            }
        }

        public void Render()
        {
            if (renderMode != RenderMode.OnDemand)
            {
                Debug.LogErrorFormat("IRenderable interface can only be used with OnDemand render mode, not {0}", renderMode.ToString());
                return;
            }

            DoRenderTexture();
        }

        private void DoRenderTexture()
        {
            var o = new Utils.OverlayHelper(handle, false);
            if (texture == null || !o.Valid) return;

            if (renderCamera)
            {
                renderCamera.Render();
            }

            if (isFacingHmd)
            {
                if (ShouldRenderTexture(texture))
                    o.SetTexture(texture);
                o.FillTextureBounds();
            }
            else
            {
                if (backface == null)
                {
                    if (ShouldRenderTexture(texture))
                        o.SetTexture(texture);
                    o.SetTextureBounds(1, 0, 0, 1);
                }
                else
                {
                    if (ShouldRenderTexture(backface))
                        o.SetTexture(backface);
                    o.FillTextureBounds();
                }
            }
        }

        private bool ShouldRenderTexture(Texture texture)
        {
            var textureChanged = lastTexture != texture;
            lastTexture = texture;

            // Should render if the texture has changed, or is a dynamic render texture
            // @todo Support a "static"/"ondemand" fps flag for rarely changing render textures
            return textureChanged || texture is RenderTexture;
        }

        private void OnEnable()
        {
            var renderTexture = texture as RenderTexture;
            if (renderTexture != null && !renderTexture.IsCreated())
            {
                renderTexture.Create();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            lastTexture = null;
            isFacingHmd = true;
        }

    }
}
