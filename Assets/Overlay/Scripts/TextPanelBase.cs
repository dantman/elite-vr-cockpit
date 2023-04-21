using UnityEngine;

namespace EVRC.Core.Overlay
{
    using Utils = OverlayUtils;
    public class TextPanelBase : MonoBehaviour
    {
        [Header("Text Mesh Objects")]
        [Tooltip("The TextMeshPro mesh to update with the Title Text")]
        public TMPro.TMP_Text TitleTextMesh;
        [Tooltip("The TextMeshPro mesh to update with help text 1 info (top half)")]
        public TMPro.TMP_Text TopTextMesh;
        [Tooltip("The TextMeshPro mesh to update with help text 2 info (bottom half)")]
        public TMPro.TMP_Text BottomTextMesh;

        [Header("Prefab References")]
        public HolographicImage overlay;
        public Camera captureCamera;
        public RenderTexture renderTexture;
        [Tooltip("Width for the text (in pixels). Hint: a Holographic button is 128 x 128")]
        public int renderWidthPx = 128;
        [Tooltip("Height for the text (in pixels). Hint: a Holographic button is 128 x 128")]
        public int renderHeightPx = 64;

        public string key
        {
            get
            {
                return Utils.GetKey("TextPanel", GetInstanceID().ToString());
            }
        }

        void OnValidate()
        {
            
        }

        public void OnEnable()
        {
            if (renderTexture == null)
            {
                // create renderTexture for Overlay
                CreateRenderTexture();

                // use same renderTexture for CaptureCamera
                captureCamera.targetTexture = renderTexture;
                overlay.SetTexture(renderTexture);
            }
        }

        public void CreateRenderTexture()
        {
            //TODO allow custom sizes in rendertexture creation
            renderTexture = new RenderTexture(renderWidthPx, renderHeightPx, 0, RenderTextureFormat.ARGB32);
            renderTexture.wrapMode = TextureWrapMode.Clamp;
            renderTexture.antiAliasing = 4;
            renderTexture.filterMode = FilterMode.Trilinear;
            renderTexture.name = key;

            renderTexture.Create();
        }

    }
}