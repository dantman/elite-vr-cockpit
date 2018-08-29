using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    using System;
    using ButtonPress = ActionsController.ButtonPress;

    public class ControllerInteractionPoint : MonoBehaviour
    {
        public HashSet<MovableSurface> intersectingSurfaces = new HashSet<MovableSurface>();
        public HashSet<BaseButton> intersectingButtons = new HashSet<BaseButton>();
        public HashSet<MovableSurface> grabbingSurfaces = new HashSet<MovableSurface>();
        private TrackedHand trackedHand;

        void Start()
        {
            trackedHand = GetComponentInParent<TrackedHand>();
        }

        void OnEnable()
        {
            ActionsController.TriggerPress.Listen(OnTriggerPress);
            ActionsController.TriggerUnpress.Listen(OnTriggerUnpress);
            ActionsController.GrabPress.Listen(OnGrabPress);
            ActionsController.GrabUnpress.Listen(OnGrabUnpress);
        }

        void OnDisable()
        {
            ActionsController.TriggerPress.Remove(OnTriggerPress);
            ActionsController.TriggerUnpress.Remove(OnTriggerUnpress);
            ActionsController.GrabPress.Remove(OnGrabPress);
            ActionsController.GrabUnpress.Remove(OnGrabPress);
        }

        private void OnTriggerEnter(Collider other)
        {
            var surface = other.GetComponent<MovableSurface>();
            if (surface != null)
            {
                intersectingSurfaces.Add(surface);
            }

            var button = other.GetComponent<BaseButton>();
            if (button != null)
            {
                intersectingButtons.Add(button);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var surface = other.GetComponent<MovableSurface>();
            if (surface != null)
            {
                intersectingSurfaces.Remove(surface);
            }

            var button = other.GetComponent<BaseButton>();
            if (button != null)
            {
                intersectingButtons.Remove(button);
            }
        }

        private bool IsSameHand(TrackedHand.Hand tHand, ActionsController.Hand bHand)
        {
            if (tHand == TrackedHand.Hand.Left)
            {
                return bHand == ActionsController.Hand.Left;
            }
            else if (tHand == TrackedHand.Hand.Right)
            {
                return bHand == ActionsController.Hand.Right;
            }
            return false;
        }

        private void OnTriggerPress(ButtonPress btn)
        {
            if (!IsSameHand(trackedHand.hand, btn.hand)) return;

            foreach (BaseButton button in intersectingButtons)
            {
                button.Activate();
            }
        }

        private void OnTriggerUnpress(ButtonPress btn)
        {
        }

        private void OnGrabPress(ButtonPress btn)
        {
            if (!IsSameHand(trackedHand.hand, btn.hand)) return;

            foreach (MovableSurface surface in intersectingSurfaces)
            {
                if (surface.Grabbed(this))
                {
                    grabbingSurfaces.Add(surface);
                }
            }
        }

        private void OnGrabUnpress(ButtonPress btn)
        {
            if (!IsSameHand(trackedHand.hand, btn.hand)) return;

            foreach (MovableSurface surface in grabbingSurfaces)
            {
                surface.Ungrabbed(this);
            }

            grabbingSurfaces.Clear();
        }

    }
}
