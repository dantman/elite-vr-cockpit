using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using Utils = OverlayUtils;
    public class RadialLabelRenderer : MonoBehaviour
    {
        protected string id;
        public Color color = Color.white;
        public float width = .05f;
        public string label = "Test Label";
        public RenderTexture renderTexture = null;
        private ulong handle = OpenVR.k_ulOverlayHandleInvalid;

        void OnEnable()
        {
            OnValidate();
        }

        public void OnValidate()
        {
            createLabelTexture();
        }
        public string key
        {
            get
            {
                return Utils.GetKey("Label", GetInstanceID().ToString());
            }
        }

        public void createLabelTexture()
        {
            renderTexture = new RenderTexture(128, 64, 0, RenderTextureFormat.ARGB32);
            renderTexture.wrapMode = TextureWrapMode.Clamp;
            renderTexture.antiAliasing = 4;
            renderTexture.filterMode = FilterMode.Trilinear;
            renderTexture.name = key;

            renderTexture.Create();
            if (label != null && label != "")
            {
                RenderTextureTextCapture.RenderText(renderTexture, label, TMPro.TextAlignmentOptions.Top);

            }
            else
            {
                Debug.LogWarning($"Unable to render text into texture. Label is null: {this.label}");
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