using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    using ButtonPress = ActionsController.ButtonPress;

    public class ControllerInteractionPoint : MonoBehaviour
    {
        public HashSet<BaseButton> intersectingButtons = new HashSet<BaseButton>();
        private TrackedHand trackedHand;

        void Start()
        {
            trackedHand = GetComponentInParent<TrackedHand>();
        }

        void OnEnable()
        {
            ActionsController.TriggerPress.Listen(OnTriggerPress);
            ActionsController.TriggerUnpress.Listen(OnTriggerUnpress);
        }

        void OnDisable()
        {
            ActionsController.TriggerPress.Remove(OnTriggerPress);
            ActionsController.TriggerUnpress.Remove(OnTriggerUnpress);
        }

        private void OnTriggerEnter(Collider other)
        {
            var button = other.GetComponent<BaseButton>();
            if (button != null)
            {
                intersectingButtons.Add(button);
            }
        }

        private void OnTriggerExit(Collider other)
        {
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
            if (!IsSameHand(trackedHand.hand, btn.hand))
            {
                return;
            }

            foreach (BaseButton button in intersectingButtons)
            {
                button.Activate();
            }
        }

        private void OnTriggerUnpress(ButtonPress btn)
        {
        }
    }
}
