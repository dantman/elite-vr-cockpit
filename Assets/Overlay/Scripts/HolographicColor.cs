using System.Collections;
using System.Collections.Generic;
using EVRC.Core.Overlay;
using UnityEngine;

namespace EVRC.Core.Overlay
{
    [RequireComponent(typeof(IColorable))]
    public class HolographicColor : MonoBehaviour
    {
        public HudColor hudColor;
        private IColorable[] colorableComponents;
        private IHighlightable[] highlightableComponents;
        
        [Tooltip("use a custom configuration from the user's GraphicsConfigurationOverride.xml file (not recommended)")]
        public bool useHudColorMatrixOverride = false;

        public Color baseColor;
        public Color highlightColor;
        public Color invalidColor;

        private void OnValidate()
        {
            GetColors();
        }

        private void OnEnable()
        {
            colorableComponents = GetComponents<IColorable>();
            highlightableComponents = GetComponents<IHighlightable>();
            ApplyHudColors();
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
                invalidColor = hudColor.ApplyColorToMatrix(hudColor.invalidColor);
                return;
            }

            baseColor = hudColor.baseColor;
            highlightColor = hudColor.highlightColor;
            invalidColor = hudColor.invalidColor;
        }


        public void ApplyHudColors()
        {
            foreach (IColorable component in colorableComponents)
            {
                component.SetBaseColors(baseColor, invalidColor);
                // component.UnHighlight(); // set the current color to base (once)
            }
            foreach (IHighlightable component in highlightableComponents)
            {
                component.SetHighlightColor(highlightColor);
                // component.UnHighlight(); // set the current color to base (once)
            }

        }


    }
}
