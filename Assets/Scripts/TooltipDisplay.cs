using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using Utils = OverlayUtils;

    public class TooltipDisplay : MonoBehaviour
    {
        public string id;
        public string text = "Tooltip";
        public TMPro.TextAlignmentOptions textAlignment = TMPro.TextAlignmentOptions.Center;
        public Color color = Color.white;
        public float width = 1f;
        public RenderTexture renderTexture;
        private ulong handle = OpenVR.k_ulOverlayHandleInvalid;

        public string key
        {
            get
            {
                return Utils.GetKey("tooltip", id);
            }
        }

        void OnEnable()
        {
            Refresh();
        }

        private void OnValidate()
        {
            if (Application.isPlaying && enabled)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            TooltipTextCapture.RenderText(renderTexture, text, textAlignment);
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
            if (renderTexture && renderTexture.IsCreated() && o.Valid)
            {
                o.Show();

                o.SetColorWithAlpha(color);
                o.SetWidthInMeters(width);

                o.SetInputMethod(VROverlayInputMethod.None);
                o.SetMouseScale(1, 1);

                var offset = new SteamVR_Utils.RigidTransform(transform);
                if (!Utils.IsFacingHmd(transform))
                {
                    offset.rot = offset.rot * Quaternion.AngleAxis(180, Vector3.up);
                }
                o.SetFullTexture(renderTexture);
                o.SetTransformAbsolute(ETrackingUniverseOrigin.TrackingUniverseStanding, offset);
            }
        }

        void OnDisable()
        {
            renderTexture.DiscardContents();

            var o = new Utils.OverlayHelper(handle, false);
            if (o.Valid)
            {
                o.Destroy();
            }

            handle = OpenVR.k_ulOverlayHandleInvalid;
        }
    }
}
