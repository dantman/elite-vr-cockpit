using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    using ActionChange = ActionsController.ActionChange;
    using OutputAction = ActionsController.OutputAction;
    using BtnAction = ActionsController.BtnAction;
    using ButtonActionsPress = ActionsController.ButtonActionsPress;
    using DirectionAction = ActionsController.DirectionAction;
    using Direction = ActionsController.Direction;
    using HatDirection = vJoyInterface.HatDirection;

    /**
     * Outputs joystick buttons to vJoy when the associated joystick is grabbed
     */
    public class VirtualJoystickButtons : VirtualControlButtons
    {
        // Map of abstracted action presses to vJoy joystick button numbers
        private static Dictionary<OutputAction, uint> joyBtnMap = new Dictionary<OutputAction, uint>()
        {
            { OutputAction.ButtonPrimary, 1 },
            { OutputAction.ButtonSecondary, 2 },
            { OutputAction.ButtonAlt, 3 },
            //{ OutputAction.D1, 4 },
            //{ OutputAction.D2, 5 },
        };
        private static Dictionary<DirectionAction, uint> joyHatMap = new Dictionary<DirectionAction, uint>()
        {
            { DirectionAction.D1, 1 },
            { DirectionAction.D2, 2 },
        };
        private static Dictionary<Direction, HatDirection> directionMap = new Dictionary<Direction, HatDirection>()
        {
            { Direction.Up, HatDirection.Up },
            { Direction.Right, HatDirection.Right },
            { Direction.Down, HatDirection.Down },
            { Direction.Left, HatDirection.Left },
        };

        private ActionsControllerPressManager actionsPressManager;

        override protected void OnEnable()
        {
            base.OnEnable();
            actionsPressManager = new ActionsControllerPressManager(this)
                .ButtonPrimary(OnAction)
                .ButtonSecondary(OnAction);
            ActionsController.DirectionActionPress.Listen(OnDirectionPress);
            ActionsController.DirectionActionUnpress.Listen(OnDirectionUnpress);
        }

        override protected void OnDisable()
        {
            base.OnDisable();
            actionsPressManager.Clear();
            ActionsController.DirectionActionPress.Remove(OnDirectionPress);
            ActionsController.DirectionActionUnpress.Remove(OnDirectionUnpress);
        }

        private PressManager.UnpressHandlerDelegate<ActionChange> OnAction(ActionChange pEv)
        {
            if (IsValidHand(pEv.hand) && joyBtnMap.ContainsKey(pEv.action))
            {
                uint btnIndex = joyBtnMap[pEv.action];
                PressButton(btnIndex);

                return (uEv) => { UnpressButton(btnIndex); };
            }

            return (uEv) => { };
        }
        
        private void OnDirectionPress(ActionsController.DirectionActionsPress ev)
        {
            if (!IsValidHand(ev.hand)) return;

            if (joyHatMap.ContainsKey(ev.button))
            {
                uint hatNumber = joyHatMap[ev.button];
                SetHatDirection(hatNumber, directionMap[ev.direction]);
            }
        }

        private void OnDirectionUnpress(ActionsController.DirectionActionsPress ev)
        {
            if (!IsValidHand(ev.hand)) return;

            if (joyHatMap.ContainsKey(ev.button))
            {
                uint hatNumber = joyHatMap[ev.button];
                ReleaseHatDirection(hatNumber);
            }
        }
    }
}
