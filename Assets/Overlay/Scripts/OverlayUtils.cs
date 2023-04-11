using System;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    public class OverlayUtils
    {
        /**
         * Not overlay specfic, but simplify the boilerplate for making singleton controller objects
         */
        public static T Singleton<T>(ref T _instance, string name, bool create = true) where T : MonoBehaviour
        {

            if (_instance == null)
            {
                _instance = UnityEngine.Object.FindObjectOfType<T>();

                if (_instance == null)
                {
                    if (create)
                    {
                        var hmd = new GameObject(name);
                        _instance = hmd.AddComponent<T>();
                    }
                    else
                    {
                        Debug.LogErrorFormat("Instance for {0} ({1}) does not exist in the scene", name, typeof(T).Name);
                        return null;
                    }
                }
            }

            return _instance;
        }

        /**
         * Make a key for the OpenVR overlay, given a simple application level key
         */
        public static string GetKey(params string[] keys)
        {
            return "unity:" + Application.companyName + "." + Application.productName + "." + string.Join(".", keys);
        }

        /**
         * Safely create an OpenVR overlay
         */
        public static bool CreateOverlay(string key, string name, ref ulong handle)
        {
            var overlay = OpenVR.Overlay;
            if (overlay == null)
            {
                Debug.LogWarning("Overlay system not available");
                return false;
            }

            return ReportError(overlay.CreateOverlay(key, name, ref handle));
        }

        /**
         * If an overlay error is not None, log it to the console as a warning
         */
        public static bool ReportError(EVROverlayError err)
        {
            if (err != EVROverlayError.None)
            {
                Debug.LogWarning(OpenVR.Overlay.GetOverlayErrorNameFromEnum(err));
                return false;
            }

            return true;
        }

        /**
         * Checks to see if an overlay is facing the Hmd
         * If not, the overlay should be flipped or a backface overlay should be rendered instead
         */
        public static bool IsFacingHmd(Transform overlayTransform)
        {
            var hmd = TrackedHMD.Transform;

            // @note Overlays face -z, so .forward is actually the back of the overlay
            var dot = Vector3.Dot(overlayTransform.forward, (hmd.position - overlayTransform.position).normalized);
            // @note Since we have a dot product based on the back of the overlay, we consider positive numbers to not be facing the HMD
            return dot <= 0f;
        }

        /**
         * A helper for interacting with OpenVR Overlay methods.
         * - Reduces the necessary boilerplate:
         *   - "Overlay" in every function name
         *   - Specifying the "handle" in every call
         *   - Getting an OpenVR.Overlay reference before making calls
         * - Makes sure to report all errors with OverlayUtils.ReportError
         * - Simplifies verbose APIs, e.g. provides methods accepting Unity types in place of OpenVR's verbose structs
         */
        public class OverlayHelper
        {
            private ulong handle = OpenVR.k_ulOverlayHandleInvalid;
            private CVROverlay overlay;

            public bool Valid
            {
                get
                {
                    if (overlay == null) return false;
                    if (handle == OpenVR.k_ulOverlayHandleInvalid) return false;
                    return true;
                }
            }

            public OverlayHelper(bool warn = true)
            {
                overlay = OpenVR.Overlay;
                if (overlay == null && warn)
                {
                    Debug.LogError("Overlay system not available");
                }
            }

            public OverlayHelper(ulong handle, bool warn = true) : this(warn)
            {
                this.handle = handle;
            }

            /**
             * Destroy the overlay
             */
            public void Destroy()
            {
                ReportError(overlay.DestroyOverlay(handle));
            }

            /**
             * Show the overlay
             */
            public void Show()
            {
                ReportError(overlay.ShowOverlay(handle));
            }

            /**
             * Hide the overlay
             */
            public void Hide()
            {
                ReportError(overlay.HideOverlay(handle));
            }

            /**
             * Set an overlay's color
             */
            public void SetColor(float red, float green, float blue)
            {
                ReportError(overlay.SetOverlayColor(handle, red, green, blue));
            }

            /**
             * Set an overlay's color
             */
            public void SetColorWithoutAlpha(Color color)
            {
                SetColor(color.r, color.g, color.b);
            }

            /**
             * Set an overlay's alpha value
             */
            public void SetAlpha(float alpha)
            {
                ReportError(overlay.SetOverlayAlpha(handle, alpha));
            }

            /**
             * Set and overlay's color and alpha
             */
            public void SetColorWithAlpha(Color color)
            {
                SetColorWithoutAlpha(color);
                SetAlpha(color.a);
            }

            /**
             * Set an overlay's width, in meters
             */
            public void SetWidthInMeters(float width)
            {
                ReportError(overlay.SetOverlayWidthInMeters(handle, width));
            }

            /**
             * Set an overlay's texture to a unity Texture
             */
            public void SetTexture(Texture texture)
            {
                var tex = new Texture_t();
                tex.handle = texture.GetNativeTexturePtr();
                tex.eType = OpenVR_Utils.textureType;
                tex.eColorSpace = EColorSpace.Auto;
                ReportError(overlay.SetOverlayTexture(handle, ref tex));
            }

            /**
             * Set an overlay's texture bounds
             */
            public void SetTextureBounds(float xMin, float yMin, float xMax, float yMax)
            {
                // @note u=horizontal/x, v=vertical/y
                // @note OpenVR's vMin is the top of the overlay, and vMax is the bottom
                var textureBounds = new VRTextureBounds_t();
                textureBounds.uMin = xMin;
                textureBounds.vMin = yMax;
                textureBounds.uMax = xMax;
                textureBounds.vMax = yMin;
                ReportError(overlay.SetOverlayTextureBounds(handle, ref textureBounds));
            }

            /**
             * Reset the texture bounds so the whole texture fills the overlay
             */
            public void FillTextureBounds()
            {
                SetTextureBounds(0, 0, 1, 1);
            }

            /**
             * Set the overlay so it is filled with a Unity texture
             * i.e. Set the texture and fill the texture bounds
             */
            public void SetFullTexture(Texture texture)
            {
                SetTexture(texture);
                FillTextureBounds();
            }

            /**
             * Set overlay's input method
             */
            public void SetInputMethod(VROverlayInputMethod inputMethod)
            {
                ReportError(overlay.SetOverlayInputMethod(handle, inputMethod));
            }

            /**
             * Set overlay's mouse scale
             */
            public void SetMouseScale(float v0, float v1)
            {
                var vecMouseScale = new HmdVector2_t();
                vecMouseScale.v0 = v0;
                vecMouseScale.v1 = v1;
                ReportError(overlay.SetOverlayMouseScale(handle, ref vecMouseScale));
            }

            /**
             * Set an absolute transform for the overlay, using an OpenVR HmdMatrix
             */
            public void SetTransformAbsolute(ETrackingUniverseOrigin trackingUniverseOrigin, ref HmdMatrix34_t t)
            {
                ReportError(overlay.SetOverlayTransformAbsolute(handle, trackingUniverseOrigin, ref t));
            }

            /**
             * Set an absolute transform for the overlay, using a RigidTransform
             */
            public void SetTransformAbsolute(ETrackingUniverseOrigin trackingUniverseOrigin, SteamVR_Utils.RigidTransform rigidTransform)
            {
                var t = rigidTransform.ToHmdMatrix34();
                SetTransformAbsolute(trackingUniverseOrigin, ref t);
            }

            /**
             * Set an absolute transform for the overlay, using a Unity Transform
             */
            public void SetTransformAbsolute(ETrackingUniverseOrigin trackingUniverseOrigin, Transform transform)
            {
                SetTransformAbsolute(trackingUniverseOrigin, new SteamVR_Utils.RigidTransform(transform));
            }
        }
    }
}
