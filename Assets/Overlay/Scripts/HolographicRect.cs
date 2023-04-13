using UnityEngine;
using Valve.VR;

namespace EVRC.Core.Overlay
{
    using Utils = OverlayUtils;

    public class HolographicRect : MonoBehaviour
    {
        public string id;
        public Color color = Color.white;
        public bool useHudColorMatrix = true;
        public float width = 0.5f;
        public int pxWidth = 50;
        public int pxHeight = 50;
        private Texture2D texture;
        private ulong handle = OpenVR.k_ulOverlayHandleInvalid;

        public string key
        {
            get
            {
                return Utils.GetKey("rect", id);
            }
        }

        void OnEnable()
        {
            int w = pxWidth;
            int h = pxHeight;
            Color fillColor = Color.white;
            texture = new Texture2D(w, h, TextureFormat.ARGB32, false);

            Color[] colors = new Color[w * h];
            for (int i = 0; i < w * h; ++i)
            {
                colors[i] = fillColor;
            }
            texture.SetPixels(0, 0, w, h, colors);
            texture.Apply();
        }

        void Update()
        {
            var overlay = OpenVR.Overlay;
            if (overlay == null) return;

            if (handle == OpenVR.k_ulOverlayHandleInvalid)
            {
                Utils.CreateOverlay(key, gameObject.name, ref handle);
            }

            var o = new Utils.OverlayHelper(handle);
            if (texture != null && o.Valid)
            {
                o.Show();

                o.SetColorWithAlpha(TransformColor(color));
                o.SetWidthInMeters(width);

                o.SetInputMethod(VROverlayInputMethod.None);
                o.SetMouseScale(1, 1);

                var offset = new SteamVR_Utils.RigidTransform(transform);
                if (!Utils.IsFacingHmd(transform))
                {
                    offset.rot = offset.rot * Quaternion.AngleAxis(180, Vector3.up);
                }
                o.SetFullTexture(texture);
                o.SetTransformAbsolute(ETrackingUniverseOrigin.TrackingUniverseStanding, offset);
            }
        }

        void OnDisable()
        {
            var o = new Utils.OverlayHelper(handle, false);
            if (o.Valid)
            {
                o.Destroy();
            }

            handle = OpenVR.k_ulOverlayHandleInvalid;
        }

        /**
         * Transforms colors with the HUD color matrix, if the option is set
         */
        protected Color TransformColor(Color color)
        {
            return EDStateManager.ConditionallyApplyHudColorMatrix(useHudColorMatrix, color);
        }
    }
}
