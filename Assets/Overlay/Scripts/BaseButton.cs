using System;
using EVRC.Core.Actions;
using UnityEngine;

namespace EVRC.Core.Overlay
{
    public abstract class BaseButton : MonoBehaviour, IHighlightable, IActivateable
    {
        public static Color invalidColor = Color.red;
        protected IHolographic buttonImage;
        protected bool highlighted = false;

        protected delegate void Unpress();
        protected static Unpress noop = () => { };
        private ControllerInteractionPoint currentPressingInteractionPoint;

        protected virtual void OnEnable()
        {
            buttonImage = GetComponentInChildren<IHolographic>();
            if (buttonImage == null)
            {
                Debug.LogWarningFormat("A button image is missing from {0}", name);
            }
            Refresh();
        }

        protected virtual void OnDisable()
        {
            if (currentPressingInteractionPoint)
            {
                currentPressingInteractionPoint.ForceUnpress(this);
            }
        }

        protected virtual void Update() { }

        public void Highlight()
        {
            highlighted = true;
            Refresh();
        }

        public void UnHighlight()
        {
            highlighted = false;
            Refresh();
        }

        protected virtual void Refresh()
        {
            if (!IsValid())
            {
                buttonImage.SetColor(invalidColor);
            }
            else if (highlighted)
            {
                buttonImage.Highlight();
            }
            else
            {
                buttonImage.UnHighlight();
            }
        }

        /**
         * Check that can be overridden to indicate if a button is invalid
         */
        public virtual bool IsValid()
        {
            return true;
        }

        public Action Activate(ControllerInteractionPoint interactionPoint)
        {
            if (currentPressingInteractionPoint) return () => { };

            currentPressingInteractionPoint = interactionPoint;
            var unpress = Activate();
            return () =>
            {
                currentPressingInteractionPoint = null;
                unpress();
            };
        }

        protected abstract Unpress Activate();
    }
}
