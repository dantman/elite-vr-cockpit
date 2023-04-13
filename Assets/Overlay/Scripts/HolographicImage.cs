using UnityEngine;
using Valve.VR;

namespace EVRC.Core.Overlay
{
    using Utils = OverlayUtils;

    /**
     * Simple overlay like the holographic buttons but just outputs an image
     * @todo This is turning out to be 90% like the holo buttons, maybe I should just
     * combine them and add an option to switch backface handling type.
     */
    public class HolographicImage : MonoBehaviour, IRenderable
    {
        public enum RenderMode
        {
            Update,
            OnDemand,
        }

        public string id;
        public Texture texture;
        public Texture backface;
        public Color color = Color.white;
        public bool useHudColorMatrix = true;
        public RenderMode renderMode = RenderMode.Update;
        public Camera renderCamera;
        public float width = 1f;
        private ulong handle = OpenVR.k_ulOverlayHandleInvalid;
        private Texture lastTexture;
        private bool isFacingHmd = true;

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
                o.Show();

                o.SetColorWithAlpha(TransformColor(color));
                o.SetWidthInMeters(width);

                o.SetInputMethod(VROverlayInputMethod.None);
                o.SetMouseScale(1, 1);

                var isFacing = Utils.IsFacingHmd(transform);
                var wasFlipped = isFacingHmd != isFacing;
                isFacingHmd = isFacing;

                var offset = new SteamVR_Utils.RigidTransform(transform);
                if (renderMode == RenderMode.Update || wasFlipped)
                {
                    DoRenderTexture();
                }
                if (!isFacingHmd)
                {
                    offset.rot = offset.rot * Quaternion.AngleAxis(180, Vector3.up);
                }
                o.SetTransformAbsolute(ETrackingUniverseOrigin.TrackingUniverseStanding, offset);
            }
        }

        public void Render()
        {
            if (renderMode != RenderMode.OnDemand)
            {
                Debug.LogErrorFormat("IRenderable interface can only be used with OnDemand render mode, not {0}", renderMode.ToString());
                return;
            }

            DoRenderTexture();
        }

        private void DoRenderTexture()
        {
            var o = new Utils.OverlayHelper(handle, false);
            if (texture == null || !o.Valid) return;

            if (renderCamera)
            {
                renderCamera.Render();
            }

            if (isFacingHmd)
            {
                if (ShouldRenderTexture(texture))
                    o.SetTexture(texture);
                o.FillTextureBounds();
            }
            else
            {
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
            if (renderTexture != null && !renderTexture.IsCreated())
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
            lastTexture = null;
            isFacingHmd = true;
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

        private void OnDrawGizmosSelected()
        {
            if (texture == null) return;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.blue;

            var hw = width / 2f;
            var hh = (((float)texture.height / (float)texture.width) * width) / 2f;
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
