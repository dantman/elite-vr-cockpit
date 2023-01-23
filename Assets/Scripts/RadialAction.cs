using EVRC;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;


namespace EVRC
{
    public class RadialAction : MonoBehaviour
    {
        public Texture icon = null;
        public string label = defaultLabel;
        public UnityEvent onPress = new UnityEvent();
        public HolographicOverlay iconObject = null;
        public HolographicText labelObject = null;

        [Header("Debug: make private")]
        //public RenderTexture renderTexture = null;
        public static string defaultLabel = "Radial Menu Item";
        public Color baseColor;
        public Color higlightColor;


        void Awake()
        {
            RadialMenuController.highlightedActionChanged.Listen(OnHighlightChange);

            HolographicOverlay iconChild = iconObject.GetComponent<HolographicOverlay>();
            iconChild.texture = icon;
            iconChild.color = baseColor;


            HolographicText labelChild = labelObject.GetComponentInChildren<HolographicText>();
            labelChild.text = label;
            labelChild.color = baseColor;
        }

        void OnDestroy()
        {
            RadialMenuController.highlightedActionChanged.Remove(OnHighlightChange);
        }

        public void OnHighlightChange(int highlightedId)
        {
            if (highlightedId == this.GetInstanceID())
            {
                Highlight();
            }
            else
            {
                Unhighlight();
            }
        }

        private void Highlight()
        {
            iconObject.color = higlightColor;
            labelObject.color = higlightColor;
        }

        private void Unhighlight()
        {
            iconObject.color = baseColor;
            labelObject.color = baseColor;
        }

        //public void createLabelTexture()
        //{
        //    renderTexture = new RenderTexture(128, 64, 0, RenderTextureFormat.ARGB32);
        //    renderTexture.wrapMode = TextureWrapMode.Clamp;
        //    renderTexture.antiAliasing = 4;
        //    renderTexture.filterMode = FilterMode.Trilinear;
        //    renderTexture.name = "Radial" + Regex.Replace(label, @"\s+", "") + "RenderTexture";

        //    renderTexture.Create();
        //    if (label != null && label != "")
        //    {
        //        RenderTextureTextCapture.RenderText(renderTexture, label, TMPro.TextAlignmentOptions.Top);

        //    }
        //    else
        //    {
        //        Debug.LogWarning($"Unable to render text into texture. Label is null: {this.label}");
        //    }
        //}
    }
}