using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    [RequireComponent(typeof(HolographicButton))]
    abstract public class BaseButton : MonoBehaviour
    {
        public Color color; // @todo Automatically handle color
        public Color highlightColor;
        protected HolographicButton holoButton;
        protected HashSet<ControllerInteractionPoint> hoveringPoints = new HashSet<ControllerInteractionPoint>();

        virtual protected void Start()
        {
            holoButton = GetComponent<HolographicButton>();
            holoButton.color = color;
        }

        virtual protected void Update() { }

        private void OnTriggerEnter(Collider other)
        {
            var interactionPoint = other.GetComponent<ControllerInteractionPoint>();
            if (interactionPoint != null)
            {
                hoveringPoints.Add(interactionPoint);
            }

            holoButton.color = highlightColor;
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
                holoButton.color = color;
            }
        }

        abstract public void Activate();
    }
}
