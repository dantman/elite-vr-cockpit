﻿using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core.Overlay
{
    public class Hoverable : MonoBehaviour
    {
        protected IHighlightable[] highlightables;
        protected HashSet<ControllerInteractionPoint> hoveringPoints = new HashSet<ControllerInteractionPoint>();

        private void OnEnable()
        {
            highlightables = GetComponentsInChildren<IHighlightable>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var interactionPoint = other.GetComponent<ControllerInteractionPoint>();
            if (interactionPoint != null)
            {
                hoveringPoints.Add(interactionPoint);
            }

            if(hoveringPoints.Count == 1)
            {
                foreach (var hoverable in highlightables)
                {
                    hoverable.Highlight();
                }
            }
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
                foreach (var hoverable in highlightables)
                {
                    hoverable.UnHighlight();
                }
            }
        }

    }
}
