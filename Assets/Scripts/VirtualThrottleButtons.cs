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
    public class VirtualThrottleButtons : VirtualControlButtons
    {
        // Map of abstracted BtnAction presses to vJoy joystick button numbers
        private static Dictionary<BtnAction, uint> joyBtnMap = new Dictionary<BtnAction, uint>()
        {
            { BtnAction.Trigger, 8 },
            { BtnAction.Secondary, 7 },
        };

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

        private void OnActionPress(ButtonActionsPress ev)
        {
            if (!IsValidHand(ev.hand)) return;

            if (joyBtnMap.ContainsKey(ev.button))
            {
                uint btnIndex = joyBtnMap[ev.button];
                PressButton(btnIndex);
            }
        }

        private void OnActionUnpress(ButtonActionsPress ev)
        {
            if (!IsValidHand(ev.hand)) return;

            if (joyBtnMap.ContainsKey(ev.button))
            {
                uint btnIndex = joyBtnMap[ev.button];
                UnpressButton(btnIndex);
            }
        }
    }
}
