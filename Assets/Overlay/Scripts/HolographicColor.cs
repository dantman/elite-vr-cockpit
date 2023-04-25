using System.Collections;
using System.Collections.Generic;
using EVRC.Core.Overlay;
using UnityEngine;

namespace EVRC.Core.Overlay
{
    [RequireComponent(typeof(IHolographic))]
    public class HolographicColor : MonoBehaviour
    {
        public HudColor hudColor;
        private IHolographic[] holographicComponents;
        
        [Tooltip("use a custom configuration from the user's GraphicsConfigurationOverride.xml file (not recommended)")]
        public bool useHudColorMatrixOverride;

        public Color baseColor;
        public Color highlightColor;

        private void OnValidate()
        {
            
            GetColors();
        }

        private void OnEnable()
        {
            holographicComponents = GetComponents<IHolographic>();
            ApplyHudColor();
        }

        public void GetColors()
        {
            if (hudColor == null)
            {
                Debug.LogWarning($"HudColor is not set. Cannot apply colors: {gameObject.name}");
                return;
            }

            if (useHudColorMatrixOverride)
            {
                baseColor = hudColor.ApplyColorToMatrix(hudColor.baseColor);
                highlightColor = hudColor.ApplyColorToMatrix(hudColor.highlightColor);
                return;
            }

            baseColor = hudColor.baseColor;
            highlightColor = hudColor.highlightColor;
        }


        public void ApplyHudColor()
        {
            if (holographicComponents == null) return;

            foreach (IHolographic component in holographicComponents)
            {
                component.SetColor(baseColor);
                component.SetHighlightColor(highlightColor);
                component.UnHighlight(); // set the current color to base (once)
            }
        }


    }
}
