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
        public RadialMenuOverlay iconObject = null;
        public RadialLabelRenderer labelObject = null;

        [Header("Debug: make private")]
        public RenderTexture renderTexture = null;
        public static string defaultLabel = "Radial Menu Item";

        void OnEnable()
        {
            RadialMenuController.highlightedActionChanged.Listen(OnHighlightChange);

            iconObject.GetComponent<RadialMenuOverlay>().texture = icon;
            labelObject.GetComponentInChildren<RadialLabelRenderer>().label = label;
        }

        void OnDisable()
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
            iconObject.color = Color.red;
            labelObject.color = Color.red;
        }

        private void Unhighlight()
        {
            iconObject.color = Color.white;
            labelObject.color = Color.white;
        }

        public void createLabelTexture()
        {
            renderTexture = new RenderTexture(128, 64, 0, RenderTextureFormat.ARGB32);
            renderTexture.wrapMode = TextureWrapMode.Clamp;
            renderTexture.antiAliasing = 4;
            renderTexture.filterMode = FilterMode.Trilinear;
            renderTexture.name = "Radial" + Regex.Replace(label, @"\s+", "") + "RenderTexture";

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
    }
}