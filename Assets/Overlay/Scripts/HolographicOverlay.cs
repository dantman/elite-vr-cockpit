using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


namespace EVRC
{
    using Utils = OverlayUtils;
    public class HolographicOverlay : MonoBehaviour, IHolographic
    {
        public enum RenderMode
        {
            Update,
            OnDemand
        }
        public RenderMode renderMode = RenderMode.Update;
        public Texture texture;
        private Texture lastTexture;
        public float size = .05f;
        public Color color = Color.white;
        public bool useHudColorMatrix = true;

        [Header("Debug - make private after testing")]
        public ulong handle = OpenVR.k_ulOverlayHandleInvalid;


        public string key
        {
            get
            {
                return Utils.GetKey("HoloOverlay", GetInstanceID().ToString());
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

            var o = new Utils.OverlayHelper(handle);
            if (texture != null && o.Valid)
            {
                o.Show();

                if (texture != lastTexture)
                {
                    //Holographic buttons do this in the "IsFacingHmd" section below
                    o.SetFullTexture(texture);
                    lastTexture = texture;
                }

                o.SetColorWithAlpha(color);
                o.SetWidthInMeters(size);

                o.SetInputMethod(VROverlayInputMethod.None);
                o.SetMouseScale(1, 1);

                var offset = new SteamVR_Utils.RigidTransform(transform);
                //if (Utils.IsFacingHmd(transform))
                //{
                //    o.SetFullTexture(texture);
                //} else {
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

        public void SetColor(Color color)
        {
            this.color = color;
        }

        public void SetTexture(Texture texture)
        {
            this.texture = texture;
        }

        /// <summary>
        /// Renders Gizmos only when the GameOjbect is selected, this will simplify the scene editor view when developing, but won't affect
        /// the compiled game.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (texture == null) return;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.yellow;

            var width = size;
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