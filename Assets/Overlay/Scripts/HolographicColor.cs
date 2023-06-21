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

        private void OnEnable()
        {
            if (hudColor == null)
            {
                Debug.LogWarning($"HudColor is not set. Setting Defaults: {gameObject.name}");
                hudColor = ScriptableObject.CreateInstance<HudColor>();
            }

            colorableComponents = GetComponents<IColorable>();
            highlightableComponents = GetComponents<IHighlightable>();
            ApplyHudColors();
        }

        public void ApplyHudColors()
        {
            foreach (IColorable component in colorableComponents)
            {
                component.SetBaseColors(hudColor.baseColor, hudColor.invalidColor, hudColor.unavailableColor);
                // component.UnHighlight(); // set the current color to base (once)
            }
            foreach (IHighlightable component in highlightableComponents)
            {
                component.SetHighlightColor(hudColor.highlightColor);
                // component.UnHighlight(); // set the current color to base (once)
            }

        }


    }
}
