using System;
using UnityEngine;
using Valve.VR;

namespace EVRC.Core.Overlay
{
    public class HolographicBase : MonoBehaviour, IHolographic
    {
        public string id;
        public float size = 1f;
        [SerializeField] protected Texture texture;

        // Colors & Highlights. Stuff used for IHolographic
        protected Color color = Color.white;
        protected Color baseColor = Color.white;
        protected Color highlightColor = Color.white;
        protected ulong handle = OpenVR.k_ulOverlayHandleInvalid;

        public string key => OverlayUtils.GetKey("holographic", GetInstanceID().ToString());

        private void Init()
        {
            OverlayUtils.CreateOverlay(key, gameObject.name, ref handle);
        }

        protected virtual void OnDisable()
        {
            var o = new OverlayUtils.OverlayHelper(handle, false);
            if (o.Valid)
            {
                o.Destroy();
            }

            handle = OpenVR.k_ulOverlayHandleInvalid;
        }

        protected virtual void Update()
        {
            CVROverlay overlay = OpenVR.Overlay;
            if (overlay == null) return;

            if (handle == OpenVR.k_ulOverlayHandleInvalid)
            {
                Init();
            }

            var o = new OverlayUtils.OverlayHelper(handle);
            if (texture == null || !o.Valid) return;
            o.Show();

            o.SetColorWithAlpha(color);
            o.SetWidthInMeters(size);

            o.SetInputMethod(VROverlayInputMethod.None);
            o.SetMouseScale(1, 1);

            var offset = new SteamVR_Utils.RigidTransform(transform);
            if (!OverlayUtils.IsFacingHmd(transform))
            {
                offset.rot *= Quaternion.AngleAxis(180, Vector3.up);
            }
            o.SetTransformAbsolute(ETrackingUniverseOrigin.TrackingUniverseStanding, offset);
        }

        public virtual void SetColor(Color setColor)
        {
            this.baseColor = setColor;
        }

        public virtual void SetHighlightColor(Color setColor)
        {
            this.highlightColor = setColor;
        }

        public virtual void SetTexture(Texture newTexture)
        {
            this.texture = newTexture;
        }

        public virtual void Highlight()
        {
            color = highlightColor;
        }

        public virtual void UnHighlight()
        {
            color = baseColor;
        }

        

        



        private void OnDrawGizmosSelected()
        {
            if (texture == null) return;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.blue;

            var hw = size / 2f;
            var hh = (((float)texture.height / (float)texture.width) * size) / 2f;
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