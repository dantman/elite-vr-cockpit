using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using Utils = OverlayUtils;

    public class HolographicButton : MonoBehaviour
    {
        public string buttonId;
        public Texture texture;
        public Texture backface;
        public Color color = Color.white;
        public float size = .05f;
        private ulong handle = OpenVR.k_ulOverlayHandleInvalid;

        public string key
        {
            get
            {
                return Utils.GetKey("button", buttonId);
            }
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

                o.SetColorWithAlpha(color);
                o.SetWidthInMeters(size);

                o.SetInputMethod(VROverlayInputMethod.None);
                o.SetMouseScale(1, 1);

                var offset = new SteamVR_Utils.RigidTransform(transform);
                if (Utils.IsFacingHmd(transform))
                {
                    o.SetFullTexture(texture);
                }
                else
                {
                    o.SetFullTexture(backface == null ? texture : backface);
                    offset.rot = offset.rot * Quaternion.AngleAxis(180, Vector3.up);
                }
                o.SetTransformAbsolute(ETrackingUniverseOrigin.TrackingUniverseStanding, offset);
            }
        }

        void OnDisable()
        {
            var o = new Utils.OverlayHelper(handle);
            if (o.Valid)
            {
                o.Destroy();
            }

            handle = OpenVR.k_ulOverlayHandleInvalid;
        }
    }
}
