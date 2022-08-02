﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    using ActionChange = ActionsController.ActionChange;
    using OutputAction = ActionsController.OutputAction;
    using Hand = ActionsController.Hand;
    using ActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.ActionChange>;

    public class ControllerInteractionPoint : MonoBehaviour
    {
        public float toggleGrabPressTiming = 0.35f;
        public TooltipDisplay tooltipDisplay;

        private ActionsControllerPressManager actionsPressManager;

        private TrackedHand trackedHand;
        private HashSet<IGrabable> intersectingGrababales = new HashSet<IGrabable>();
        private HashSet<IActivateable> intersectingActivatables = new HashSet<IActivateable>();
        private Dictionary<IActivateable, Action> pressedActivatableReleases = new Dictionary<IActivateable, Action>();
        private readonly HashSet<IGrabable> grabbing = new HashSet<IGrabable>();
        private readonly HashSet<IGrabable> toggleGrabbing = new HashSet<IGrabable>();
        private ITooltip tooltip;

        private float lastGrabPressTime;

        public Hand Hand
        {
            get
            {
                return trackedHand.hand;
            }
        }

        void Start()
        {
            trackedHand = GetComponentInParent<TrackedHand>();
        }

        void OnEnable()
        {
            actionsPressManager = new ActionsControllerPressManager(this)
                .InteractUI(OnInteractUI)
                .GrabHold(OnGrab)
                .GrabToggle(OnGrab);
                // .GrabPinch(OnGrabPinch)

            Tooltip.TooltipUpdated.Listen(OnTooltipUpdate);
        }

        void OnDisable()
        {
            actionsPressManager.Clear();

            Tooltip.TooltipUpdated.Remove(OnTooltipUpdate);
        }

        private void OnTooltipUpdate(ITooltip tooltip, string text)
        {
            if (this.tooltip == tooltip && tooltipDisplay.text != text)
            {
                tooltipDisplay.text = text;
                tooltipDisplay.Refresh();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var hoverable = other.GetComponent<IHoverable>();
            if (hoverable != null)
            {
                hoverable.Hover(this);
            }

            var grabables = other.GetComponents<IGrabable>();
            foreach (var grabable in grabables)
            {
                intersectingGrababales.Add(grabable);
            }

            var activatable = other.GetComponent<IActivateable>();
            if (activatable != null)
            {
                intersectingActivatables.Add(activatable);
            }

            var tt = other.GetComponent<ITooltip>();
            if (tt != null)
            {
                tooltip = tt;
                if (tooltipDisplay)
                {
                    tooltipDisplay.text = tooltip.GetTooltipText();
                    tooltipDisplay.enabled = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var hoverable = other.GetComponent<IHoverable>();
            if (hoverable != null)
            {
                hoverable.Unhover(this);
            }

            var grabables = other.GetComponents<IGrabable>();
            foreach (var grabable in grabables)
            {
                intersectingGrababales.Remove(grabable);
            }

            var activatable = other.GetComponent<IActivateable>();
            if (activatable != null)
            {
                intersectingActivatables.Remove(activatable);
                if (pressedActivatableReleases.ContainsKey(activatable))
                {
                    var unpress = pressedActivatableReleases[activatable];
                    unpress();
                    pressedActivatableReleases.Remove(activatable);
                }
            }

            var tt = other.GetComponent<ITooltip>();
            if (tt == tooltip)
            {
                tooltip = null;
                tooltipDisplay.enabled = false;
            }
        }

        private bool IsSameHand(Hand tHand, ActionsController.Hand bHand)
        {
            if (tHand == Hand.Left)
            {
                return bHand == ActionsController.Hand.Left;
            }
            else if (tHand == Hand.Right)
            {
                return bHand == ActionsController.Hand.Right;
            }
            return false;
        }

        private ActionChangeUnpressHandler OnInteractUI(ActionChange pEv)
        {
            if (!IsSameHand(trackedHand.hand, pEv.hand)) return (uEv) => { };

            foreach (IActivateable button in intersectingActivatables)
            {
                var unpress = button.Activate(this);
                pressedActivatableReleases.Add(button, unpress);
            }

            return (uEv) =>
            {
                foreach (var pressedActivatable in pressedActivatableReleases)
                {
                    var unpress = pressedActivatable.Value;
                    unpress();
                }

                pressedActivatableReleases.Clear();
            };
        }
        
        /**
         * Force an activatable to be unpressed even when the user has not released it.
         * Normally used when a button is about to be hidden.
         */
        public void ForceUnpress(IActivateable button)
        {
            if (pressedActivatableReleases.ContainsKey(button))
            {
                var unpress = pressedActivatableReleases[button];
                unpress();
                pressedActivatableReleases.Remove(button);
            }
        }

        private ActionChangeUnpressHandler OnGrab(ActionChange pEv)
        {
            if (!IsSameHand(trackedHand.hand, pEv.hand)) return (uEv) => { };

            foreach (IGrabable grabable in intersectingGrababales)
            {
                var canGrab = !grabbing.Contains(grabable) &&
                    grabable.GetGrabMode().HasFlag(GrabMode.Grabable);
                if (canGrab && grabable.Grabbed(this))
                {
                    grabbing.Add(grabable);
                }
            }

            lastGrabPressTime = Time.time;

            return OnUngrab;
        }

        private void OnUngrab(ActionChange uEv)
        {
            var delta = Time.time - lastGrabPressTime;
            var isUnderGrabToggleTiming = delta < toggleGrabPressTiming;

            foreach (IGrabable grabable in grabbing)
            {
                // If we are toggle grabbing the object we should also remove it from the list
                if (toggleGrabbing.Contains(grabable))
                {
                    toggleGrabbing.Remove(grabable);
                    grabable.Ungrabbed(this);
                }
                // if this might be a grab toggle and the surface allows it,
                // add it to the toggle-grabbing list. otherwise, whether it's
                // not a grab toggle or the surface won't be toggle-grabbed,
                // ungrab the surface
                else if (uEv.action == OutputAction.GrabToggle && isUnderGrabToggleTiming && grabable.GetGrabMode().HasFlag(GrabMode.ToggleGrabable))
                {
                    toggleGrabbing.Add(grabable);
                }
                else
                {
                    grabable.Ungrabbed(this);
                }
            }

            grabbing.Clear();
            foreach (var grabable in toggleGrabbing)
            {
                grabbing.Add(grabable);
            }
        }

        /**
         * Force a grabbable to be ungrabbed even when the user has not released it.
         * Normally used when a control is about to be hidden.
         */
        public void ForceUngrab(IGrabable grabable)
        {
            if (toggleGrabbing.Contains(grabable))
            {
                toggleGrabbing.Remove(grabable);
            }
            if (grabbing.Contains(grabable))
            {
                grabable.Ungrabbed(this);
                grabbing.Remove(grabable);
            }
        }
    }
}
