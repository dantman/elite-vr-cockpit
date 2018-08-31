using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    /**
     * A virtual 3-axis joystick that outputs to vJoy when grabbed
     */
    public class VirtualJoystick : MonoBehaviour, IGrabable, IHighlightable
    {
        public Color color;
        public Color highlightColor;
        public HolographicRect line;
        protected CockpitStateController controller;
        private bool highlighted = false;
        private ControllerInteractionPoint attachedInteractionPoint;

        void Start()
        {
            controller = CockpitStateController.instance;
            Refresh();
        }

        public bool Grabbed(ControllerInteractionPoint interactionPoint)
        {
            if (attachedInteractionPoint != null) return false;
            // Don't allow joystick use when editing is unlocked, so the movable surface can be used instead
            if (!controller.editLocked) return false;

            attachedInteractionPoint = interactionPoint;

            return false;
        }

        public void Ungrabbed(ControllerInteractionPoint interactionPoint)
        {
            if (interactionPoint == attachedInteractionPoint)
            {
                attachedInteractionPoint = null;
            }
        }

        public void OnHover()
        {
            highlighted = true;
            Refresh();
        }

        public void OnUnhover()
        {
            highlighted = false;
            Refresh();
        }

        void Refresh()
        {
            if (line)
            {
                if (highlighted)
                {
                    line.color = highlightColor;
                }
                else
                {
                    line.color = color;
                }
            }
        }
    }
}
