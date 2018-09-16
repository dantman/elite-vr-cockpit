using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using Utils = OverlayUtils;

    /**
     * Simple overlay like the holographic buttons but just outputs an image
     * @todo This is turning out to be 90% like the holo buttons, maybe I should just
     * combine them and add an option to switch backface handling type.
     */
    public class HolographicImage : MonoBehaviour
    {
        public string id;
        public Texture texture;
        public Texture backface;
        public Color color = Color.white;
        public bool useHudColorMatrix = true;
        public Camera renderCamera;
        public float width = 1f;
        private ulong handle = OpenVR.k_ulOverlayHandleInvalid;
        private Texture lastTexture;

        public string key
        {
            get
            {
                return Utils.GetKey("image", id);
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
                if (renderCamera)
                {
                    renderCamera.Render();
                }

                o.Show();

                o.SetColorWithAlpha(TransformColor(color));
                o.SetWidthInMeters(width);

                o.SetInputMethod(VROverlayInputMethod.None);
                o.SetMouseScale(1, 1);

                var offset = new SteamVR_Utils.RigidTransform(transform);
                if (Utils.IsFacingHmd(transform))
                {
                    if (ShouldRenderTexture(texture))
                        o.SetTexture(texture);
                    o.FillTextureBounds();
                }
                else
                {
                    offset.rot = offset.rot * Quaternion.AngleAxis(180, Vector3.up);
                    if (backface == null)
                    {
                        if (ShouldRenderTexture(texture))
                            o.SetTexture(texture);
                        o.SetTextureBounds(1, 0, 0, 1);
                    }
                    else
                    {
                        if (ShouldRenderTexture(backface))
                            o.SetTexture(backface);
                        o.FillTextureBounds();
                    }
                }
                o.SetTransformAbsolute(ETrackingUniverseOrigin.TrackingUniverseStanding, offset);
            }
        }

        private bool ShouldRenderTexture(Texture texture)
        {
            var textureChanged = lastTexture != texture;
            lastTexture = texture;

            // Should render if the texture has changed, or is a dynamic render texture
            // @todo Support a "static"/"ondemand" fps flag for rarely changing render textures
            return textureChanged || texture is RenderTexture;
        }

        private void OnEnable()
        {
            var renderTexture = texture as RenderTexture;
            if (renderTexture != null)
            {
                renderTexture.Create();
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
            if (useHudColorMatrix)
            {
                return EDStateManager.ApplyHudColorMatrix(color);
            }

            return color;
        }
    }
}
