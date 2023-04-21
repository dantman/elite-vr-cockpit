using UnityEngine;
using Valve.VR;

namespace EVRC.Core.Overlay
{
    public class HolographicOverlay : HolographicBase
    {
        public enum RenderMode
        {
            Update,
            OnDemand
        }
        public RenderMode renderMode = RenderMode.Update;
        private Texture lastTexture;

        protected override void Update()
        {
            base.Update();

            if (texture == lastTexture) return;
            var o = new OverlayUtils.OverlayHelper(handle);
            o.SetFullTexture(texture);
            lastTexture = texture;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            lastTexture = null; // required to force a re-render when the object is re-enabled
        }
    }
}