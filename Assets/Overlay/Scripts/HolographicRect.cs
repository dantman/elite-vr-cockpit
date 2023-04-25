using UnityEngine;
using Valve.VR;

namespace EVRC.Core.Overlay
{
    using Utils = OverlayUtils;

    public class HolographicRect : HolographicBase
    {
        public int pxWidth = 50;
        public int pxHeight = 50;
        private Texture2D texture2d;

        void OnEnable()
        {
            int w = pxWidth;
            int h = pxHeight;
            Color fillColor = color;
            texture2d = new Texture2D(w, h, TextureFormat.ARGB32, false);

            Color[] colors = new Color[w * h];
            for (int i = 0; i < w * h; ++i)
            {
                colors[i] = fillColor;
            }
            texture2d.SetPixels(0, 0, w, h, colors);
            texture2d.Apply();
        }

        // Override to use Texture2d instead of Texture
        protected override void Update()
        {
            CVROverlay overlay = OpenVR.Overlay;
            if (overlay == null) return;

            if (handle == OpenVR.k_ulOverlayHandleInvalid)
            {
                base.Init();
            }

            var o = new OverlayUtils.OverlayHelper(handle);
            // @todo figure out how to use the base Update method instead of duplicating the whole thing
            // This is the only line that changed from the original
            if (texture2d == null || !o.Valid) return; 
            o.Show();

            o.SetColorWithAlpha(color);
            o.SetWidthInMeters(size);

            o.SetInputMethod(VROverlayInputMethod.None);
            o.SetMouseScale(1, 1);

            var offset = new SteamVR_Utils.RigidTransform(transform);
            if (!OverlayUtils.IsFacingHmd(transform))
            {
                offset.rot *= Quaternion.AngleAxis(180, Vector3.up);
            }
            o.SetTransformAbsolute(ETrackingUniverseOrigin.TrackingUniverseStanding, offset);

            // This part would need to remain after base.Update()
            // Similar to the HolographicOverlay implementation
            o.SetFullTexture(texture2d);
        }

        public override void SetTexture(Texture newTexture)
        {
            texture = newTexture as Texture2D;

            // Make sure texture was converted to Texture2D
            if (texture != null) return;
            Debug.LogError("Invalid texture type");
        }

        protected virtual void OnDrawGizmos()
        {
            if (texture == null) return;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.yellow;

            var hw = size / 2f;
            var hh = (((float)texture.height / (float)texture.width) * size) / 2f;
            var ul = new Vector3(-hw, -hh);
            var ur = new Vector3(hw, -hh);
            var ll = new Vector3(-hw, hh);
            var lr = new Vector3(hw, hh);

            Gizmos.DrawLine(ul, ur);
            Gizmos.DrawLine(ur, lr);
            Gizmos.DrawLine(lr, ll);
            Gizmos.DrawLine(ll, ul);
        }
    }
}
