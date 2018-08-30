using System;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    [RequireComponent(typeof(HolographicButton))]
    abstract public class BaseButton : MonoBehaviour
    {
        public Color color;
        public Color highlightColor;
        public bool useHudColorMatrix = true;
        protected HolographicButton holoButton;
        protected HashSet<ControllerInteractionPoint> hoveringPoints = new HashSet<ControllerInteractionPoint>();

        virtual protected void OnEnable()
        {
            holoButton = GetComponent<HolographicButton>();
            EDStateManager.HudColorMatrixChanged.Listen(OnHudColorMatrixChange);
            Refresh();
        }

        virtual protected void OnDisable()
        {
            EDStateManager.HudColorMatrixChanged.Remove(OnHudColorMatrixChange);
        }

        virtual protected void Update() { }

        private void OnHudColorMatrixChange(HudColorMatrix arg0)
        {
            Refresh();
        }

        private void OnTriggerEnter(Collider other)
        {
            var interactionPoint = other.GetComponent<ControllerInteractionPoint>();
            if (interactionPoint != null)
            {
                hoveringPoints.Add(interactionPoint);
            }

            Refresh();
        }

        private void OnTriggerExit(Collider other)
        {
            var interactionPoint = other.GetComponent<ControllerInteractionPoint>();
            if (interactionPoint != null)
            {
                hoveringPoints.Remove(interactionPoint);
            }

            if (hoveringPoints.Count == 0)
            {
                Refresh();
                holoButton.color = TransformColor(color);
            }
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

        virtual protected void Refresh()
        {
            if (hoveringPoints.Count > 0)
            {
                holoButton.color = TransformColor(highlightColor);
            }
            else
            {
                holoButton.color = TransformColor(color);
            }
        }

        abstract public void Activate();
    }
}
