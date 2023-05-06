using System;
using EVRC.Core.Actions;
using UnityEngine;

namespace EVRC.Core.Overlay
{
    /// <summary>
    /// Controls the game logic for all button types
    /// </summary>

    public abstract class BaseButton : MonoBehaviour, IActivateable
    {
        protected delegate void Unpress();
        protected static Unpress noop = () => { };
        private ControllerInteractionPoint currentPressingInteractionPoint;

        public IHighlightable buttonImage;
        private bool highlighted = false;


        protected virtual void OnEnable()
        {
            buttonImage = GetComponentInChildren<IHighlightable>();
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

        protected virtual void Refresh()
        {
            if (buttonImage == null) return;

            if (highlighted)
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
