using UnityEngine;
using Valve.VR;

namespace EVRC.Core.Overlay
{
    using Utils = OverlayUtils;
    /// <summary>
    ///      Default implementation for anywhere you wish to display holographic/floating text. 
    /// </summary>
    public class HolographicText : MonoBehaviour
    {

        [Tooltip("Size of the object (in meters)")] public float size = .05f;
        public string text = "holotext";
        public TMPro.TextAlignmentOptions textAlignment = TMPro.TextAlignmentOptions.Center;       
        public Color color = Color.white;

        [Header("Rendered Text Size")]
        [Tooltip("Width for the text (in pixels). Hint: a Holographic button is 128 x 128")] 
        public int renderWidthPx = 128;
        [Tooltip("Height for the text (in pixels). Hint: a Holographic button is 128 x 128")] 
        public int renderHeightPx = 64;
        
        private string lastText = string.Empty;
        private RenderTexture renderTexture;
        private ulong handle = OpenVR.k_ulOverlayHandleInvalid;
        public string key
        {
            get
            {
                return Utils.GetKey("holotext", GetInstanceID().ToString());
            }
        }

        void Awake()
        {
            createRenderTexture();
        }

        void OnEnable()
        {
            Refresh();
        }

        private void OnValidate()
        {
            if (Application.isPlaying && enabled)
            {
                createRenderTexture();
            }
        }

        public void createRenderTexture()
        {
            //TODO allow custom sizes in rendertexture creation
            renderTexture = new RenderTexture(renderWidthPx, renderHeightPx, 0, RenderTextureFormat.ARGB32);
            renderTexture.wrapMode = TextureWrapMode.Clamp;
            renderTexture.antiAliasing = 4;
            renderTexture.filterMode = FilterMode.Trilinear;
            renderTexture.name = key;

            renderTexture.Create();
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
                o.SetWidthInMeters(size);

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

        public void Refresh()
        {
            if (text != null && text != "")
            {
                RenderTextureTextCapture.RenderText(renderTexture, text, TMPro.TextAlignmentOptions.Top);

            }
            else
            {
                Debug.LogWarning($"Unable to render text into texture. Label is null: {this.text}");
            }
        }
        /// <summary>
        ///     Re-render (after text has been updated)
        /// </summary>
        public void ReRender()
        {

            if (text == lastText) { 
                Debug.LogWarning($"Holographic Text render skipped: text has not changed. Previous text: {lastText}"); 
                return; 
            }

            lastText = text;
            Refresh();
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
