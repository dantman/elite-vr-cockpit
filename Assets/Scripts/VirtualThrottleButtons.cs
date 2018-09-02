using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    using Hand = ActionsController.Hand;
    using BtnAction = ActionsController.BtnAction;
    using ButtonActionsPress = ActionsController.ButtonActionsPress;

    /**
     * Outputs joystick buttons to vJoy when the associated throttle is grabbed
     */
    public class VirtualThrottleButtons : MonoBehaviour
    {
        public vJoyInterface output;
        // Map of abstractes BtnAction presses to vJoy joystick button numbers
        private static Dictionary<BtnAction, uint> joyBtnMap = new Dictionary<BtnAction, uint>()
        {
            { BtnAction.Trigger, 8 },
            { BtnAction.Secondary, 7 },
        };
        // The hand of the controller grabbing the joystick, Unknown is considered "not grabbing"
        private Hand grabbedHand = Hand.Unknown;

        private void OnEnable()
        {
            ActionsController.ButtonActionPress.Listen(OnActionPress);
            ActionsController.ButtonActionUnpress.Listen(OnActionUnpress);
        }

        private void OnDisable()
        {

            ActionsController.ButtonActionPress.Remove(OnActionPress);
            ActionsController.ButtonActionUnpress.Remove(OnActionUnpress);
        }

        public void Grabbed(Hand hand)
        {
            grabbedHand = hand;
        }

        public void Ungrabbed()
        {
            grabbedHand = Hand.Unknown;
            // @todo Release all buttons when ungrabbed
        }

        private void OnActionPress(ButtonActionsPress ev)
        {
            if (grabbedHand == Hand.Unknown) return; // not grabbing
            if (grabbedHand != ev.hand) return; // wrong hand

            if (joyBtnMap.ContainsKey(ev.button))
            {
                uint btnIndex = joyBtnMap[ev.button];
                if (output)
                {
                    output.SetButton(btnIndex, true);
                }
            }
        }

        private void OnActionUnpress(ButtonActionsPress ev)
        {
            if (grabbedHand == Hand.Unknown) return; // not grabbing
            if (grabbedHand != ev.hand) return; // wrong hand

            if (joyBtnMap.ContainsKey(ev.button))
            {
                uint btnIndex = joyBtnMap[ev.button];
                if (output)
                {
                    output.SetButton(btnIndex, false);
                }
            }
        }
    }
}
