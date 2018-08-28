using UnityEngine;
using Valve.VR;

namespace EVRC
{
    public class ControllerOverlay : MonoBehaviour
    {
        public Texture texture;
        public string overlayKey;
        private ulong handle = OpenVR.k_ulOverlayHandleInvalid;

        public string key
        {
            get
            {
                return "unity:" + Application.companyName + "." + Application.productName + "." + overlayKey;
            }
        }

        void Init()
        {
            var overlay = OpenVR.Overlay;
            if (overlay == null)
            {
                Debug.LogWarning("Overlay system not available");
                return;
            }

            var error = overlay.CreateOverlay(key, gameObject.name, ref handle);
            if (error != EVROverlayError.None)
            {
                Debug.Log(overlay.GetOverlayErrorNameFromEnum(error));
                return;
            }
        }

        // Update is called once per frame
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
                var error = overlay.ShowOverlay(handle);
                if (error == EVROverlayError.InvalidHandle || error == EVROverlayError.UnknownOverlay)
                {
                    Debug.Log(error.ToString());
                    overlay.FindOverlay(key, ref handle);
                    return;
                }

                var tex = new Texture_t();
                tex.handle = texture.GetNativeTexturePtr();
                tex.eType = OpenVR_Utils.textureType;
                tex.eColorSpace = EColorSpace.Auto;
                ReportError(overlay.SetOverlayTexture(handle, ref tex));
                
                var textureBounds = new VRTextureBounds_t();
                textureBounds.uMin = 0;
                textureBounds.vMin = 1;
                textureBounds.uMax = 1;
                textureBounds.vMax = 0;
                ReportError(overlay.SetOverlayTextureBounds(handle, ref textureBounds));

                ReportError(overlay.SetOverlayAlpha(handle, 1f));
                ReportError(overlay.SetOverlayWidthInMeters(handle, .05f));

                var vecMouseScale = new HmdVector2_t();
                vecMouseScale.v0 = 1;
                vecMouseScale.v1 = 1;
                ReportError(overlay.SetOverlayMouseScale(handle, ref vecMouseScale));

                var offset = new SteamVR_Utils.RigidTransform(transform);
                var t = offset.ToHmdMatrix34();
                ReportError(overlay.SetOverlayTransformAbsolute(handle, ETrackingUniverseOrigin.TrackingUniverseStanding, ref t));

                ReportError(overlay.SetOverlayInputMethod(handle, VROverlayInputMethod.None));
            }
        }

        void OnDisable()
        {
            if (handle != OpenVR.k_ulOverlayHandleInvalid)
            {
                var overlay = OpenVR.Overlay;
                if (overlay != null)
                {
                    overlay.DestroyOverlay(handle);
                }

                handle = OpenVR.k_ulOverlayHandleInvalid;
            }
        }

        void ReportError(EVROverlayError err)
        {
            if (err != EVROverlayError.None)
            {
                Debug.LogWarning(OpenVR.Overlay.GetOverlayErrorNameFromEnum(err));
            }
        }
    }
}
