using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    public class InteractiveUIPanelInteractionSource : MonoBehaviour, IActivateable, IHoverable
    {
        public Vector2 size;
        public InteractiveUIPanelInteractionDestination destination;
        public Dictionary<ControllerInteractionPoint, Coroutine> hoveringRoutines = new Dictionary<ControllerInteractionPoint, Coroutine>();

        public void Hover(ControllerInteractionPoint interactionPoint)
        {
            hoveringRoutines.Add(interactionPoint, StartCoroutine(Hovering(interactionPoint)));
        }

        public void Unhover(ControllerInteractionPoint interactionPoint)
        {
            if (hoveringRoutines.ContainsKey(interactionPoint))
            {
                StopCoroutine(hoveringRoutines[interactionPoint]);
                hoveringRoutines.Remove(interactionPoint);
            }

            if (destination)
            {
                destination.Hover();
            }
        }

        public IEnumerator Hovering(ControllerInteractionPoint interactionPoint)
        {
            Vector2 cursorPoint;
            while (enabled)
            {
                cursorPoint = CalculateCursorPoint(interactionPoint.transform.position);

                if (destination)
                {
                    destination.Hover(cursorPoint);
                }

                yield return null;
            }
        }

        public Action Activate(ControllerInteractionPoint interactionPoint)
        {
            Vector2 clickPoint = CalculateCursorPoint(interactionPoint.transform.position);

            if (destination)
            {
                destination.Click(clickPoint);
            }

            // Return a noop since all we care about for now is clicks
            // We could change this later if we want to implement draggable UI
            return () => { };
        }

        /**
         * Calculate a 0-1 viewpoint coordinate for the cursor based on a world space 3d point
         */
        protected Vector2 CalculateCursorPoint(Vector3 worldPoint)
        {
            var localPoint = transform.InverseTransformPoint(worldPoint);

            return localPoint / size + Vector2.one / 2f;
        }
    }
}
