using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using Utils = OverlayUtils;

    public class ControllerOverlay : MonoBehaviour
    {
        public Texture texture;
        public string overlayKey;
        private ulong handle = OpenVR.k_ulOverlayHandleInvalid;

        public string key
        {
            get
            {
                return Utils.GetKey(overlayKey);
            }
        }

        void Init()
        {
            Utils.CreateOverlay(key, gameObject.name, ref handle);
        }

        void Update()
        {
            var overlay = OpenVR.Overlay;
            if (overlay == null) return;

            if (handle == OpenVR.k_ulOverlayHandleInvalid)
            {
                Init();
            }

            if (texture != null && handle != OpenVR.k_ulOverlayHandleInvalid)
            {
                var o = new Utils.OverlayHelper(handle);
                o.Show();

                o.SetFullTexture(texture);
                o.SetColorWithAlpha(Color.black);
                o.SetWidthInMeters(.05f);

                o.SetInputMethod(VROverlayInputMethod.None);
                o.SetMouseScale(1, 1);

                var offset = new SteamVR_Utils.RigidTransform(transform);
                if (!Utils.IsFacingHmd(transform))
                {
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
