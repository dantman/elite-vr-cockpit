using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using Utils = OverlayUtils;

    public class RadialMenuOverlay : MonoBehaviour
    {
        public Texture texture;
        private string overlayKey;
        private ulong handle = OpenVR.k_ulOverlayHandleInvalid;
        private Texture lastTexture;
        public float width = .1f;
        public Color color = Color.gray;

        void Awake()
        {
            overlayKey = $"RadialAction{UnityEngine.Random.Range(-10.0f, 10.0f).ToString()}";
        }

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

                if (texture != lastTexture)
                {
                    o.SetFullTexture(texture);
                    lastTexture = texture;
                }
                o.SetColorWithAlpha(color);
                o.SetWidthInMeters(width);

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
            var o = new Utils.OverlayHelper(handle, false);
            if (o.Valid)
            {
                o.Destroy();
            }

            handle = OpenVR.k_ulOverlayHandleInvalid;
            lastTexture = null; // required to force a re-render when the object is re-enabled
        }
    }
}
